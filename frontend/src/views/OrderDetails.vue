<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { apiService } from '@/api'
import type { BoostOrder, Batch, UpdateBoostOrderRequest } from '@/types'

const route = useRoute()
const router = useRouter()
const orderId = route.params.id as string

const order = ref<BoostOrder | null>(null)
const batches = ref<Batch[]>([])
const loading = ref(true)
const updating = ref(false)
const error = ref('')

const editDialog = ref(false)
const editForm = ref({
  description: '',
  id: orderId
})

onMounted(async () => {
  await Promise.all([
    loadOrder(),
    loadBatches()
  ])
})

async function loadOrder() {
  try {
    order.value = await apiService.getBoostOrder(orderId)
    editForm.value.description = order.value.description
  } catch (error) {
    console.error('Failed to load order:', error)
    router.push({ name: 'orders' })
  }
}

async function loadBatches() {
  try {
    const response = await apiService.getBatches({ orderId })
    batches.value = response.items
  } catch (error) {
    console.error('Failed to load batches:', error)
  } finally {
    loading.value = false
  }
}

async function updateOrder() {
  if (!order.value) return
  
  updating.value = true
  error.value = ''
  
  try {
    await apiService.updateBoostOrder(orderId, editForm.value)
    await loadOrder()
    editDialog.value = false
  } catch (e: any) {
    error.value = e.response?.data?.message || 'Failed to update order'
  } finally {
    updating.value = false
  }
}

async function closeOrder() {
  if (!order.value) return
  
  try {
    await apiService.closeBoostOrder(orderId)
    await loadOrder()
  } catch (error) {
    console.error('Failed to close order:', error)
  }
}

function calculateProgress(): number {
  if (!order.value) return 0
  const total = order.value.requiredRating - order.value.startRating
  const current = order.value.currentRating - order.value.startRating
  return Math.round((current / total) * 100)
}

function getStatusColor(): string {
  if (!order.value) return 'grey'
  if (order.value.isClosed) return 'success'
  if (!order.value.isPaid) return 'error'
  return 'warning'
}

function getStatusText(): string {
  if (!order.value) return 'Unknown'
  if (order.value.isClosed) return 'Completed'
  if (!order.value.isPaid) return 'Unpaid'
  return 'In Progress'
}

function formatDate(dateString?: string): string {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString()
}

function getTotalMmrGained(): number {
  return batches.value.reduce((total, batch) => total + batch.receivedMmr, 0)
}

function getWinRate(): number {
  if (batches.value.length === 0) return 0
  const wins = batches.value.filter(batch => batch.isWin).length
  return Math.round((wins / batches.value.length) * 100)
}

function openScreenshot(url: string) {
  window.open(url, '_blank')
}
</script>

<template>
  <v-container>
    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <v-progress-circular indeterminate size="64" />
    </div>

    <template v-else-if="order">
      <!-- Header -->
      <v-row class="mb-6">
        <v-col cols="12" class="d-flex align-center justify-space-between">
          <div>
            <h1 class="text-h4 font-weight-bold">Order Details</h1>
            <p class="text-body-1 text-grey-darken-1">
              {{ order.startRating }} → {{ order.requiredRating }} MMR
            </p>
          </div>
          
          <div class="d-flex gap-2">
            <v-btn
              variant="outlined"
              prepend-icon="mdi-pencil"
              @click="editDialog = true"
            >
              Edit
            </v-btn>
            
            <v-btn
              v-if="!order.isClosed"
              color="success"
              prepend-icon="mdi-check"
              @click="closeOrder"
            >
              Mark Complete
            </v-btn>
            
            <v-btn
              variant="outlined"
              prepend-icon="mdi-arrow-left"
              :to="{ name: 'orders' }"
            >
              Back to Orders
            </v-btn>
          </div>
        </v-col>
      </v-row>

      <!-- Order Overview -->
      <v-row class="mb-8">
        <v-col cols="12" md="8">
          <v-card class="pa-6">
            <v-card-title class="text-h5 mb-4">Order Information</v-card-title>
            
            <!-- Progress Bar -->
            <div class="mb-6">
              <div class="d-flex align-center justify-space-between mb-2">
                <span class="text-body-2">Progress</span>
                <span class="text-body-2">{{ calculateProgress() }}%</span>
              </div>
              <v-progress-linear
                :model-value="calculateProgress()"
                height="12"
                rounded
                :color="order.isClosed ? 'success' : 'primary'"
              />
              <div class="d-flex justify-space-between mt-2 text-caption text-grey-darken-1">
                <span>{{ order.startRating }} MMR</span>
                <span>{{ order.currentRating }} MMR</span>
                <span>{{ order.requiredRating }} MMR</span>
              </div>
            </div>

            <!-- Description -->
            <div class="mb-4">
              <h3 class="text-h6 mb-2">Description</h3>
              <p class="text-body-2">{{ order.description || 'No description provided' }}</p>
            </div>

            <!-- Account Info -->
            <div class="mb-4">
              <h3 class="text-h6 mb-2">Steam Account</h3>
              <div class="d-flex align-center">
                <v-icon icon="mdi-steam" class="mr-2" />
                <span>{{ order.steamUsername }}</span>
              </div>
            </div>

            <!-- Order Options -->
            <div class="d-flex gap-4 mb-4">
              <v-chip
                :color="order.isParty ? 'blue' : 'grey'"
                variant="outlined"
              >
                <v-icon :icon="order.isParty ? 'mdi-account-group' : 'mdi-account'" class="mr-2" />
                {{ order.isParty ? 'Party Boost' : 'Solo Boost' }}
              </v-chip>
              
              <v-chip
                :color="order.isPriority ? 'amber' : 'grey'"
                variant="outlined"
              >
                <v-icon :icon="order.isPriority ? 'mdi-star' : 'mdi-star-outline'" class="mr-2" />
                {{ order.isPriority ? 'Priority' : 'Standard' }}
              </v-chip>
            </div>
          </v-card>
        </v-col>

        <v-col cols="12" md="4">
          <v-card class="pa-6">
            <v-card-title class="text-h5 mb-4">Status</v-card-title>
            
            <div class="mb-4">
              <v-chip
                :color="getStatusColor()"
                size="large"
                class="mb-2"
              >
                {{ getStatusText() }}
              </v-chip>
            </div>

            <v-divider class="my-4" />

            <!-- Stats -->
            <div class="mb-3">
              <div class="d-flex justify-space-between">
                <span class="text-body-2">Created:</span>
                <span class="text-body-2">{{ formatDate(order.createdAt) }}</span>
              </div>
            </div>
            
            <div class="mb-3">
              <div class="d-flex justify-space-between">
                <span class="text-body-2">Payment Status:</span>
                <v-chip
                  :color="order.isPaid ? 'success' : 'error'"
                  size="small"
                >
                  {{ order.isPaid ? 'Paid' : 'Unpaid' }}
                </v-chip>
              </div>
            </div>
            
            <div class="mb-3">
              <div class="d-flex justify-space-between">
                <span class="text-body-2">MMR Gained:</span>
                <span class="text-body-2 font-weight-bold">+{{ getTotalMmrGained() }}</span>
              </div>
            </div>
            
            <div class="mb-3">
              <div class="d-flex justify-space-between">
                <span class="text-body-2">Win Rate:</span>
                <span class="text-body-2 font-weight-bold">{{ getWinRate() }}%</span>
              </div>
            </div>
          </v-card>
        </v-col>
      </v-row>

      <!-- Batches/Games History -->
      <v-row>
        <v-col cols="12">
          <v-card>
            <v-card-title class="d-flex align-center justify-space-between">
              <span>Game History</span>
              <v-chip>{{ batches.length }} games</v-chip>
            </v-card-title>
            
            <v-card-text>
              <v-data-table
                :headers="[
                  { title: 'Date', key: 'createdAt' },
                  { title: 'Result', key: 'isWin' },
                  { title: 'MMR Change', key: 'receivedMmr' },
                  { title: 'Screenshot', key: 'screen' }
                ]"
                :items="batches"
                :items-per-page="10"
              >
                <template #item.createdAt="{ item }">
                  {{ formatDate(item.createdAt) }}
                </template>
                
                <template #item.isWin="{ item }">
                  <v-chip
                    :color="item.isWin ? 'success' : 'error'"
                    size="small"
                  >
                    <v-icon
                      :icon="item.isWin ? 'mdi-check' : 'mdi-close'"
                      class="mr-1"
                    />
                    {{ item.isWin ? 'Win' : 'Loss' }}
                  </v-chip>
                </template>
                
                <template #item.receivedMmr="{ item }">
                  <span
                    :class="item.receivedMmr >= 0 ? 'text-success' : 'text-error'"
                    class="font-weight-bold"
                  >
                    {{ item.receivedMmr >= 0 ? '+' : '' }}{{ item.receivedMmr }}
                  </span>
                </template>
                
                <template #item.screen="{ item }">
                  <v-btn
                    v-if="item.screen"
                    icon="mdi-image"
                    variant="text"
                    size="small"
                    @click="openScreenshot(item.screen)"
                  />
                  <span v-else class="text-grey-darken-1">No screenshot</span>
                </template>
                
                <template #no-data>
                  <div class="text-center py-8">
                    <v-icon icon="mdi-gamepad-variant-outline" size="64" color="grey" />
                    <p class="text-h6 mt-4">No games played yet</p>
                    <p class="text-body-2 text-grey-darken-1">
                      Game history will appear here once your boost starts
                    </p>
                  </div>
                </template>
              </v-data-table>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Edit Dialog -->
      <v-dialog v-model="editDialog" max-width="600">
        <v-card>
          <v-card-title>Edit Order</v-card-title>
          
          <v-card-text>
            <v-form @submit.prevent="updateOrder">
              <v-textarea
                v-model="editForm.description"
                label="Description"
                variant="outlined"
                rows="4"
              />
              
              <v-alert
                v-if="error"
                type="error"
                variant="tonal"
                class="mt-4"
              >
                {{ error }}
              </v-alert>
            </v-form>
          </v-card-text>
          
          <v-card-actions>
            <v-spacer />
            <v-btn
              variant="text"
              @click="editDialog = false"
            >
              Cancel
            </v-btn>
            <v-btn
              color="primary"
              :loading="updating"
              @click="updateOrder"
            >
              Save Changes
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </template>
  </v-container>
</template>

<style scoped>
.gap-2 {
  gap: 8px;
}
</style>