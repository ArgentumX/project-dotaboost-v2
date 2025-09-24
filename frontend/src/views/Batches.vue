<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiService } from '@/api'
import type { Batch, BatchFilter, CreateBatchRequest } from '@/types'

const batches = ref<Batch[]>([])
const loading = ref(true)
const totalCount = ref(0)
const page = ref(1)
const perPage = ref(10)
const detailsDialog = ref(false)
const selectedBatch = ref<Batch | null>(null)

const headers = [
  { title: 'Order ID', key: 'orderId', sortable: false },
  { title: 'Booster ID', key: 'boosterId', sortable: false },
  { title: 'Result', key: 'isWin', sortable: true },
  { title: 'MMR Change', key: 'receivedMmr', sortable: true },
  { title: 'Screenshot', key: 'screen', sortable: false },
  { title: 'Date', key: 'createdAt', sortable: true },
  { title: 'Actions', key: 'actions', sortable: false }
]

const filters = ref({
  isWin: null as boolean | null,
  orderId: '',
  boosterId: ''
})

const showFilters = ref(false)
const createDialog = ref(false)
const createForm = ref<CreateBatchRequest>({
  screen: '',
  receivedMmr: 0,
  isWin: true,
  orderId: ''
})
const creating = ref(false)
const error = ref('')

onMounted(() => {
  loadBatches()
})

async function loadBatches() {
  loading.value = true
  try {
    const filter: BatchFilter = {
      page: page.value,
      perPage: perPage.value,
      ...Object.fromEntries(
        Object.entries(filters.value).filter(([_, value]) => value !== null && value !== '')
      )
    }
    
    const response = await apiService.getBatches(filter)
    batches.value = response.items
    totalCount.value = response.totalCount
  } catch (error) {
    console.error('Failed to load batches:', error)
  } finally {
    loading.value = false
  }
}

async function createBatch() {
  creating.value = true
  error.value = ''
  
  try {
    await apiService.createBatch(createForm.value)
    createDialog.value = false
    createForm.value = {
      screen: '',
      receivedMmr: 0,
      isWin: true,
      orderId: ''
    }
    await loadBatches()
  } catch (e: any) {
    error.value = e.response?.data?.message || 'Failed to create batch'
  } finally {
    creating.value = false
  }
}

function formatDate(dateString?: string): string {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString()
}

function clearFilters() {
  filters.value = {
    isWin: null,
    orderId: '',
    boosterId: ''
  }
  loadBatches()
}

function applyFilters() {
  page.value = 1
  loadBatches()
}

function formatOrderId(orderId: string): string {
  return orderId.substring(0, 8) + '...'
}

function formatBoosterId(boosterId: string): string {
  return boosterId.substring(0, 8) + '...'
}

function viewBatch(batch: Batch) {
  selectedBatch.value = batch
  detailsDialog.value = true
}

function openScreenshot(url: string) {
  window.open(url, '_blank')
}
</script>

<template>
  <v-container>
    <v-row class="mb-6">
      <v-col cols="12" class="d-flex align-center justify-space-between">
        <div>
          <h1 class="text-h4 font-weight-bold">Game Batches</h1>
          <p class="text-body-1 text-grey-darken-1">
            Track game results and MMR changes for boost orders
          </p>
        </div>
        
        <v-btn
          color="primary"
          prepend-icon="mdi-plus"
          @click="createDialog = true"
        >
          Add Batch
        </v-btn>
      </v-col>
    </v-row>

    <!-- Filters -->
    <v-row class="mb-4">
      <v-col cols="12" class="d-flex align-center justify-space-between">
        <v-btn
          variant="outlined"
          prepend-icon="mdi-filter"
          @click="showFilters = !showFilters"
        >
          Filters
        </v-btn>
      </v-col>
    </v-row>

    <!-- Filter Panel -->
    <v-expand-transition>
      <v-card v-show="showFilters" class="mb-4">
        <v-card-title>Filters</v-card-title>
        <v-card-text>
          <v-row>
            <v-col cols="12" sm="6" md="3">
              <v-select
                v-model="filters.isWin"
                label="Game Result"
                :items="[
                  { title: 'All', value: null },
                  { title: 'Wins Only', value: true },
                  { title: 'Losses Only', value: false }
                ]"
                variant="outlined"
                density="compact"
              />
            </v-col>
            
            <v-col cols="12" sm="6" md="3">
              <v-text-field
                v-model="filters.orderId"
                label="Order ID"
                placeholder="Enter order ID..."
                variant="outlined"
                density="compact"
              />
            </v-col>
            
            <v-col cols="12" sm="6" md="3">
              <v-text-field
                v-model="filters.boosterId"
                label="Booster ID"
                placeholder="Enter booster ID..."
                variant="outlined"
                density="compact"
              />
            </v-col>
          </v-row>
          
          <v-row>
            <v-col cols="12" class="d-flex gap-2">
              <v-btn
                color="primary"
                @click="applyFilters"
              >
                Apply Filters
              </v-btn>
              <v-btn
                variant="outlined"
                @click="clearFilters"
              >
                Clear
              </v-btn>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
    </v-expand-transition>

    <!-- Batches Table -->
    <v-card>
      <v-data-table
        :headers="headers"
        :items="batches"
        :loading="loading"
        :items-per-page="perPage"
        :page="page"
        @update:page="page = $event; loadBatches()"
        @update:items-per-page="perPage = $event; loadBatches()"
      >
        <template #item.orderId="{ item }">
          <code class="text-caption">{{ formatOrderId(item.orderId) }}</code>
        </template>
        
        <template #item.boosterId="{ item }">
          <code class="text-caption">{{ formatBoosterId(item.boosterId) }}</code>
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
        
        <template #item.createdAt="{ item }">
          {{ formatDate(item.createdAt) }}
        </template>
        
        <template #item.actions="{ item }">
          <v-btn
            icon="mdi-eye"
            variant="text"
            size="small"
            @click="viewBatch(item)"
          />
        </template>
        
        <template #no-data>
          <div class="text-center py-12">
            <v-icon icon="mdi-gamepad-variant-outline" size="64" color="grey" />
            <p class="text-h6 mt-4">No game batches found</p>
            <p class="text-body-2 text-grey-darken-1 mb-4">
              Game results will appear here as orders are completed
            </p>
            <v-btn
              color="primary"
              @click="createDialog = true"
            >
              Add First Batch
            </v-btn>
          </div>
        </template>
      </v-data-table>
    </v-card>

    <!-- Create Batch Dialog -->
    <v-dialog v-model="createDialog" max-width="600">
      <v-card>
        <v-card-title>Add Game Batch</v-card-title>
        
        <v-card-text>
          <v-form @submit.prevent="createBatch">
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="createForm.orderId"
                  label="Order ID"
                  placeholder="Enter the order ID this batch belongs to"
                  variant="outlined"
                  required
                />
              </v-col>
              
              <v-col cols="12" md="6">
                <v-text-field
                  v-model.number="createForm.receivedMmr"
                  label="MMR Change"
                  type="number"
                  placeholder="Enter MMR gained/lost"
                  variant="outlined"
                  required
                />
              </v-col>
              
              <v-col cols="12" md="6">
                <v-switch
                  v-model="createForm.isWin"
                  label="Game Result"
                  :true-value="true"
                  :false-value="false"
                  color="success"
                  inset
                />
                <div class="text-caption text-grey-darken-1">
                  {{ createForm.isWin ? 'Victory' : 'Defeat' }}
                </div>
              </v-col>
              
              <v-col cols="12">
                <v-text-field
                  v-model="createForm.screen"
                  label="Screenshot URL"
                  placeholder="Enter URL to game result screenshot"
                  variant="outlined"
                  prepend-inner-icon="mdi-image"
                />
              </v-col>
            </v-row>
            
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
            @click="createDialog = false"
          >
            Cancel
          </v-btn>
          <v-btn
            color="primary"
            :loading="creating"
            @click="createBatch"
          >
            Add Batch
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Batch Details Dialog -->
    <v-dialog v-model="detailsDialog" max-width="600">
      <v-card v-if="selectedBatch">
        <v-card-title class="d-flex align-center justify-space-between">
          <span>Batch Details</span>
          <v-chip
            :color="selectedBatch.isWin ? 'success' : 'error'"
            size="small"
          >
            {{ selectedBatch.isWin ? 'Victory' : 'Defeat' }}
          </v-chip>
        </v-card-title>
        
        <v-card-text>
          <v-row>
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">Order ID</h3>
              <code class="text-body-2">{{ selectedBatch.orderId }}</code>
            </v-col>
            
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">Booster ID</h3>
              <code class="text-body-2">{{ selectedBatch.boosterId }}</code>
            </v-col>
            
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">MMR Change</h3>
              <span
                :class="selectedBatch.receivedMmr >= 0 ? 'text-success' : 'text-error'"
                class="font-weight-bold text-h6"
              >
                {{ selectedBatch.receivedMmr >= 0 ? '+' : '' }}{{ selectedBatch.receivedMmr }}
              </span>
            </v-col>
            
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">Date</h3>
              <p class="text-body-2">{{ formatDate(selectedBatch.createdAt) }}</p>
            </v-col>
            
            <v-col cols="12" v-if="selectedBatch.screen">
              <h3 class="text-h6 mb-2">Screenshot</h3>
              <v-btn
                variant="outlined"
                prepend-icon="mdi-open-in-new"
                :href="selectedBatch.screen"
                target="_blank"
              >
                View Screenshot
              </v-btn>
            </v-col>
          </v-row>
        </v-card-text>
        
        <v-card-actions>
          <v-spacer />
          <v-btn
            variant="text"
            @click="detailsDialog = false"
          >
            Close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>



<style scoped>
.gap-2 {
  gap: 8px;
}
</style>