<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiService } from '@/api'
import type { Booster, BoosterFilter, BoostOrder } from '@/types'
import { ApplicationStatus } from '@/types'

const boosters = ref<Booster[]>([])
const availableOrders = ref<BoostOrder[]>([])
const loading = ref(true)
const ordersLoading = ref(false)
const totalCount = ref(0)
const page = ref(1)
const perPage = ref(10)
const detailsDialog = ref(false)
const selectedBooster = ref<Booster | null>(null)

const headers = [
  { title: 'Booster ID', key: 'id', sortable: false },
  { title: 'Status', key: 'status', sortable: true },
  { title: 'User ID', key: 'userId', sortable: false },
  { title: 'Join Date', key: 'createdAt', sortable: true },
  { title: 'Actions', key: 'actions', sortable: false }
]

const orderHeaders = [
  { title: 'Description', key: 'description', sortable: false },
  { title: 'MMR Range', key: 'mmrRange', sortable: false },
  { title: 'Priority', key: 'isPriority', sortable: true },
  { title: 'Party', key: 'isParty', sortable: true },
  { title: 'Actions', key: 'actions', sortable: false }
]

const statusFilter = ref<ApplicationStatus | null>(null)
const showOrders = ref(false)
const selectedOrderId = ref<string>('')
const taking = ref(false)

onMounted(() => {
  loadBoosters()
})

async function loadBoosters() {
  loading.value = true
  try {
    const filter: BoosterFilter = {
      page: page.value,
      perPage: perPage.value,
      ...(statusFilter.value !== null && { status: statusFilter.value })
    }
    
    const response = await apiService.getBoosters(filter)
    boosters.value = response.items
    totalCount.value = response.totalCount
  } catch (error) {
    console.error('Failed to load boosters:', error)
  } finally {
    loading.value = false
  }
}

async function loadAvailableOrders() {
  ordersLoading.value = true
  try {
    const response = await apiService.getBoostOrders({ 
      isClosed: false, 
      isPaid: true,
      perPage: 20
    })
    availableOrders.value = response.items
  } catch (error) {
    console.error('Failed to load available orders:', error)
  } finally {
    ordersLoading.value = false
  }
}

async function takeOrder() {
  if (!selectedOrderId.value) return
  
  taking.value = true
  try {
    await apiService.takeOrder({ orderId: selectedOrderId.value })
    await loadAvailableOrders()
    selectedOrderId.value = ''
    showOrders.value = false
  } catch (error) {
    console.error('Failed to take order:', error)
  } finally {
    taking.value = false
  }
}

async function refuseOrder() {
  try {
    await apiService.refuseOrder()
    await loadAvailableOrders()
  } catch (error) {
    console.error('Failed to refuse order:', error)
  }
}

function getStatusColor(status: ApplicationStatus): string {
  switch (status) {
    case ApplicationStatus.Pending: return 'warning'
    case ApplicationStatus.Approved: return 'success'
    case ApplicationStatus.Rejected: return 'error'
    default: return 'grey'
  }
}

function getStatusText(status: ApplicationStatus): string {
  switch (status) {
    case ApplicationStatus.Pending: return 'Pending'
    case ApplicationStatus.Approved: return 'Active'
    case ApplicationStatus.Rejected: return 'Rejected'
    default: return 'Unknown'
  }
}

function formatDate(dateString?: string): string {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString()
}

function onFilterChange() {
  page.value = 1
  loadBoosters()
}

function clearFilter() {
  statusFilter.value = null
  page.value = 1
  loadBoosters()
}

function openOrdersDialog() {
  showOrders.value = true
  loadAvailableOrders()
}

function formatMmrRange(order: BoostOrder): string {
  return `${order.startRating} → ${order.requiredRating}`
}

function calculateEstimatedEarnings(order: BoostOrder): number {
  const ratingDiff = order.requiredRating - order.startRating
  const baseRate = 1.5 // $1.5 per MMR point for boosters
  const priorityMultiplier = order.isPriority ? 1.5 : 1
  const partyMultiplier = order.isParty ? 1.2 : 1
  
  return Math.round(ratingDiff * baseRate * priorityMultiplier * partyMultiplier)
}

function viewBooster(booster: Booster) {
  selectedBooster.value = booster
  detailsDialog.value = true
}
</script>

<template>
  <v-container>
    <v-row class="mb-6">
      <v-col cols="12" class="d-flex align-center justify-space-between">
        <div>
          <h1 class="text-h4 font-weight-bold">Boosters</h1>
          <p class="text-body-1 text-grey-darken-1">
            Browse active boosters and available orders
          </p>
        </div>
        
        <div class="d-flex gap-2">
          <v-btn
            color="primary"
            prepend-icon="mdi-clipboard-list"
            @click="openOrdersDialog"
          >
            Available Orders
          </v-btn>
          
          <v-btn
            variant="outlined"
            :to="{ name: 'create-booster-application' }"
            prepend-icon="mdi-plus"
          >
            Become a Booster
          </v-btn>
        </div>
      </v-col>
    </v-row>

    <!-- Filter -->
    <v-row class="mb-4">
      <v-col cols="12" md="4">
        <v-select
          v-model="statusFilter"
          label="Filter by Status"
          :items="[
            { title: 'All Boosters', value: null },
            { title: 'Pending', value: ApplicationStatus.Pending },
            { title: 'Active', value: ApplicationStatus.Approved },
            { title: 'Rejected', value: ApplicationStatus.Rejected }
          ]"
          variant="outlined"
          density="compact"
          @update:model-value="onFilterChange"
        />
      </v-col>
      
      <v-col cols="12" md="2">
        <v-btn
          variant="outlined"
          @click="clearFilter"
        >
          Clear Filter
        </v-btn>
      </v-col>
    </v-row>

    <!-- Boosters Table -->
    <v-card>
      <v-data-table
        :headers="headers"
        :items="boosters"
        :loading="loading"
        :items-per-page="perPage"
        :page="page"
        @update:page="page = $event; loadBoosters()"
        @update:items-per-page="perPage = $event; loadBoosters()"
      >
        <template #item.id="{ item }">
          <code class="text-caption">{{ item.id.substring(0, 8) }}...</code>
        </template>
        
        <template #item.status="{ item }">
          <v-chip
            :color="getStatusColor(item.status)"
            size="small"
          >
            {{ getStatusText(item.status) }}
          </v-chip>
        </template>
        
        <template #item.userId="{ item }">
          <code class="text-caption">{{ item.userId.substring(0, 8) }}...</code>
        </template>
        
        <template #item.createdAt="{ item }">
          {{ formatDate(item.createdAt) }}
        </template>
        
        <template #item.actions="{ item }">
          <v-btn
            icon="mdi-eye"
            variant="text"
            size="small"
            @click="viewBooster(item)"
          />
        </template>
        
        <template #no-data>
          <div class="text-center py-12">
            <v-icon icon="mdi-account-group-outline" size="64" color="grey" />
            <p class="text-h6 mt-4">No boosters found</p>
            <p class="text-body-2 text-grey-darken-1 mb-4">
              Be the first to join our booster team
            </p>
            <v-btn
              color="primary"
              :to="{ name: 'create-booster-application' }"
            >
              Apply Now
            </v-btn>
          </div>
        </template>
      </v-data-table>
    </v-card>

    <!-- Available Orders Dialog -->
    <v-dialog v-model="showOrders" max-width="1200">
      <v-card>
        <v-card-title class="d-flex align-center justify-space-between">
          <span>Available Orders</span>
          <div class="d-flex gap-2">
            <v-btn
              variant="outlined"
              size="small"
              prepend-icon="mdi-refresh"
              @click="loadAvailableOrders"
            >
              Refresh
            </v-btn>
            <v-btn
              variant="outlined"
              size="small"
              prepend-icon="mdi-close-circle"
              @click="refuseOrder"
            >
              Refuse Current
            </v-btn>
          </div>
        </v-card-title>
        
        <v-card-text>
          <v-data-table
            :headers="orderHeaders"
            :items="availableOrders"
            :loading="ordersLoading"
            :items-per-page="10"
          >
            <template #item.description="{ item }">
              <div class="text-truncate" style="max-width: 200px;">
                {{ item.description || 'No description' }}
              </div>
            </template>
            
            <template #item.mmrRange="{ item }">
              <div class="font-weight-bold">
                {{ formatMmrRange(item) }}
              </div>
              <div class="text-caption text-success">
                ~${{ calculateEstimatedEarnings(item) }} earnings
              </div>
            </template>
            
            <template #item.isPriority="{ item }">
              <v-icon
                :icon="item.isPriority ? 'mdi-star' : 'mdi-star-outline'"
                :color="item.isPriority ? 'amber' : 'grey'"
              />
            </template>
            
            <template #item.isParty="{ item }">
              <v-chip
                :color="item.isParty ? 'blue' : 'grey'"
                size="small"
                variant="outlined"
              >
                {{ item.isParty ? 'Party' : 'Solo' }}
              </v-chip>
            </template>
            
            <template #item.actions="{ item }">
              <v-btn
                color="primary"
                size="small"
                @click="selectedOrderId = item.id; takeOrder()"
                :loading="taking && selectedOrderId === item.id"
              >
                Take Order
              </v-btn>
            </template>
            
            <template #no-data>
              <div class="text-center py-8">
                <v-icon icon="mdi-clipboard-list-outline" size="64" color="grey" />
                <p class="text-h6 mt-4">No available orders</p>
                <p class="text-body-2 text-grey-darken-1">
                  Check back later for new boost orders
                </p>
              </div>
            </template>
          </v-data-table>
        </v-card-text>
        
        <v-card-actions>
          <v-spacer />
          <v-btn
            variant="text"
            @click="showOrders = false"
          >
            Close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Booster Details Dialog -->
    <v-dialog v-model="detailsDialog" max-width="600">
      <v-card v-if="selectedBooster">
        <v-card-title class="d-flex align-center justify-space-between">
          <span>Booster Details</span>
          <v-chip
            :color="getStatusColor(selectedBooster.status)"
            size="small"
          >
            {{ getStatusText(selectedBooster.status) }}
          </v-chip>
        </v-card-title>
        
        <v-card-text>
          <v-row>
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">Booster ID</h3>
              <code class="text-body-2">{{ selectedBooster.id }}</code>
            </v-col>
            
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">User ID</h3>
              <code class="text-body-2">{{ selectedBooster.userId }}</code>
            </v-col>
            
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">Join Date</h3>
              <p class="text-body-2">{{ formatDate(selectedBooster.createdAt) }}</p>
            </v-col>
            
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">Last Updated</h3>
              <p class="text-body-2">{{ formatDate(selectedBooster.updatedAt) }}</p>
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