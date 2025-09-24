<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiService } from '@/api'
import type { BoosterApplication, BoostOrder, Batch } from '@/types'
import { ApplicationStatus } from '@/types'

const applications = ref<BoosterApplication[]>([])
const orders = ref<BoostOrder[]>([])
const batches = ref<Batch[]>([])
const loading = ref(true)
const acting = ref(false)

const stats = ref({
  totalApplications: 0,
  pendingApplications: 0,
  approvedApplications: 0,
  rejectedApplications: 0,
  totalOrders: 0,
  activeOrders: 0,
  completedOrders: 0,
  totalBatches: 0
})

const selectedTab = ref('applications')

onMounted(async () => {
  await loadData()
})

async function loadData() {
  loading.value = true
  try {
    const [applicationsResponse, ordersResponse, batchesResponse] = await Promise.all([
      apiService.getBoosterApplications({ perPage: 50 }),
      apiService.getBoostOrders({ perPage: 50 }),
      apiService.getBatches({ perPage: 50 })
    ])
    
    applications.value = applicationsResponse.items
    orders.value = ordersResponse.items
    batches.value = batchesResponse.items
    
    calculateStats()
  } catch (error) {
    console.error('Failed to load admin data:', error)
  } finally {
    loading.value = false
  }
}

function calculateStats() {
  stats.value = {
    totalApplications: applications.value.length,
    pendingApplications: applications.value.filter(a => a.status === ApplicationStatus.Pending).length,
    approvedApplications: applications.value.filter(a => a.status === ApplicationStatus.Approved).length,
    rejectedApplications: applications.value.filter(a => a.status === ApplicationStatus.Rejected).length,
    totalOrders: orders.value.length,
    activeOrders: orders.value.filter(o => !o.isClosed).length,
    completedOrders: orders.value.filter(o => o.isClosed).length,
    totalBatches: batches.value.length
  }
}

async function approveApplication(applicationId: string) {
  acting.value = true
  try {
    await apiService.approveBoosterApplication(applicationId)
    await loadData()
  } catch (error) {
    console.error('Failed to approve application:', error)
  } finally {
    acting.value = false
  }
}

async function rejectApplication(applicationId: string) {
  acting.value = true
  try {
    await apiService.rejectBoosterApplication(applicationId)
    await loadData()
  } catch (error) {
    console.error('Failed to reject application:', error)
  } finally {
    acting.value = false
  }
}

function getApplicationStatusColor(status: ApplicationStatus): string {
  switch (status) {
    case ApplicationStatus.Pending: return 'warning'
    case ApplicationStatus.Approved: return 'success'
    case ApplicationStatus.Rejected: return 'error'
    default: return 'grey'
  }
}

function getApplicationStatusText(status: ApplicationStatus): string {
  switch (status) {
    case ApplicationStatus.Pending: return 'Pending'
    case ApplicationStatus.Approved: return 'Approved'
    case ApplicationStatus.Rejected: return 'Rejected'
    default: return 'Unknown'
  }
}

function formatDate(dateString?: string): string {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString()
}

function formatOrderStatus(order: BoostOrder): string {
  if (order.isClosed) return 'Completed'
  if (!order.isPaid) return 'Unpaid'
  return 'Active'
}

function getOrderStatusColor(order: BoostOrder): string {
  if (order.isClosed) return 'success'
  if (!order.isPaid) return 'error'
  return 'warning'
}

const applicationHeaders = [
  { title: 'User ID', key: 'userId', sortable: false },
  { title: 'Status', key: 'status', sortable: true },
  { title: 'Motivation', key: 'motivation', sortable: false },
  { title: 'Contact', key: 'contact', sortable: false },
  { title: 'Steam Profile', key: 'steamAccountLink', sortable: false },
  { title: 'Applied', key: 'createdAt', sortable: true },
  { title: 'Actions', key: 'actions', sortable: false }
]

const orderHeaders = [
  { title: 'Description', key: 'description', sortable: false },
  { title: 'MMR Range', key: 'mmrRange', sortable: false },
  { title: 'Progress', key: 'progress', sortable: false },
  { title: 'Status', key: 'status', sortable: false },
  { title: 'User', key: 'userId', sortable: false },
  { title: 'Created', key: 'createdAt', sortable: true }
]

const batchHeaders = [
  { title: 'Order', key: 'orderId', sortable: false },
  { title: 'Booster', key: 'boosterId', sortable: false },
  { title: 'Result', key: 'isWin', sortable: true },
  { title: 'MMR Change', key: 'receivedMmr', sortable: true },
  { title: 'Date', key: 'createdAt', sortable: true }
]

function calculateProgress(order: BoostOrder): number {
  const total = order.requiredRating - order.startRating
  const current = order.currentRating - order.startRating
  return Math.round((current / total) * 100)
}
</script>

<template>
  <v-container>
    <v-row class="mb-6">
      <v-col cols="12">
        <h1 class="text-h4 font-weight-bold">Admin Panel</h1>
        <p class="text-body-1 text-grey-darken-1">
          Manage booster applications, monitor orders, and oversee platform operations
        </p>
      </v-col>
    </v-row>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <v-progress-circular indeterminate size="64" />
    </div>

    <template v-else>
      <!-- Stats Overview -->
      <v-row class="mb-8">
        <v-col cols="12" sm="6" md="3">
          <v-card class="pa-4">
            <div class="d-flex align-center">
              <v-icon icon="mdi-account-plus" size="40" color="orange" class="mr-4" />
              <div>
                <div class="text-h5 font-weight-bold">{{ stats.pendingApplications }}</div>
                <div class="text-body-2 text-grey-darken-1">Pending Apps</div>
                <div class="text-caption">{{ stats.totalApplications }} total</div>
              </div>
            </div>
          </v-card>
        </v-col>
        
        <v-col cols="12" sm="6" md="3">
          <v-card class="pa-4">
            <div class="d-flex align-center">
              <v-icon icon="mdi-clipboard-clock" size="40" color="warning" class="mr-4" />
              <div>
                <div class="text-h5 font-weight-bold">{{ stats.activeOrders }}</div>
                <div class="text-body-2 text-grey-darken-1">Active Orders</div>
                <div class="text-caption">{{ stats.totalOrders }} total</div>
              </div>
            </div>
          </v-card>
        </v-col>
        
        <v-col cols="12" sm="6" md="3">
          <v-card class="pa-4">
            <div class="d-flex align-center">
              <v-icon icon="mdi-check-circle" size="40" color="success" class="mr-4" />
              <div>
                <div class="text-h5 font-weight-bold">{{ stats.completedOrders }}</div>
                <div class="text-body-2 text-grey-darken-1">Completed</div>
                <div class="text-caption">{{ Math.round((stats.completedOrders / stats.totalOrders) * 100) || 0 }}% completion rate</div>
              </div>
            </div>
          </v-card>
        </v-col>
        
        <v-col cols="12" sm="6" md="3">
          <v-card class="pa-4">
            <div class="d-flex align-center">
              <v-icon icon="mdi-gamepad-variant" size="40" color="primary" class="mr-4" />
              <div>
                <div class="text-h5 font-weight-bold">{{ stats.totalBatches }}</div>
                <div class="text-body-2 text-grey-darken-1">Games Played</div>
                <div class="text-caption">{{ Math.round(batches.filter(b => b.isWin).length / stats.totalBatches * 100) || 0 }}% win rate</div>
              </div>
            </div>
          </v-card>
        </v-col>
      </v-row>

      <!-- Tabs for Different Sections -->
      <v-card>
        <v-tabs v-model="selectedTab" bg-color="primary">
          <v-tab value="applications">
            <v-icon icon="mdi-account-plus" class="mr-2" />
            Booster Applications
            <v-chip
              v-if="stats.pendingApplications > 0"
              size="small"
              color="warning"
              class="ml-2"
            >
              {{ stats.pendingApplications }}
            </v-chip>
          </v-tab>
          
          <v-tab value="orders">
            <v-icon icon="mdi-clipboard-list" class="mr-2" />
            Orders Overview
          </v-tab>
          
          <v-tab value="batches">
            <v-icon icon="mdi-gamepad-variant" class="mr-2" />
            Game History
          </v-tab>
        </v-tabs>

        <v-card-text>
          <v-tabs-window v-model="selectedTab">
            <!-- Booster Applications Tab -->
            <v-tabs-window-item value="applications">
              <v-data-table
                :headers="applicationHeaders"
                :items="applications"
                :items-per-page="10"
              >
                <template #item.userId="{ item }">
                  <code class="text-caption">{{ item.userId.substring(0, 8) }}...</code>
                </template>
                
                <template #item.status="{ item }">
                  <v-chip
                    :color="getApplicationStatusColor(item.status)"
                    size="small"
                  >
                    {{ getApplicationStatusText(item.status) }}
                  </v-chip>
                </template>
                
                <template #item.motivation="{ item }">
                  <div class="text-truncate" style="max-width: 200px;">
                    {{ item.motivation }}
                  </div>
                </template>
                
                <template #item.contact="{ item }">
                  <div class="text-truncate" style="max-width: 150px;">
                    {{ item.contact }}
                  </div>
                </template>
                
                <template #item.steamAccountLink="{ item }">
                  <v-btn
                    v-if="item.steamAccountLink"
                    icon="mdi-steam"
                    variant="text"
                    size="small"
                    :href="item.steamAccountLink"
                    target="_blank"
                  />
                  <span v-else class="text-grey-darken-1">-</span>
                </template>
                
                <template #item.createdAt="{ item }">
                  {{ formatDate(item.createdAt) }}
                </template>
                
                <template #item.actions="{ item }">
                  <div v-if="item.status === ApplicationStatus.Pending" class="d-flex gap-2">
                    <v-btn
                      icon="mdi-check"
                      variant="text"
                      size="small"
                      color="success"
                      :loading="acting"
                      @click="approveApplication(item.id)"
                    />
                    <v-btn
                      icon="mdi-close"
                      variant="text"
                      size="small"
                      color="error"
                      :loading="acting"
                      @click="rejectApplication(item.id)"
                    />
                  </div>
                  <span v-else class="text-grey-darken-1">Processed</span>
                </template>
              </v-data-table>
            </v-tabs-window-item>

            <!-- Orders Overview Tab -->
            <v-tabs-window-item value="orders">
              <v-data-table
                :headers="orderHeaders"
                :items="orders"
                :items-per-page="10"
              >
                <template #item.description="{ item }">
                  <div class="text-truncate" style="max-width: 200px;">
                    {{ item.description || 'No description' }}
                  </div>
                </template>
                
                <template #item.mmrRange="{ item }">
                  {{ item.startRating }} → {{ item.requiredRating }}
                </template>
                
                <template #item.progress="{ item }">
                  <div class="d-flex align-center">
                    <v-progress-linear
                      :model-value="calculateProgress(item)"
                      height="6"
                      rounded
                      color="primary"
                      class="mr-2"
                      style="min-width: 80px;"
                    />
                    <span class="text-caption">{{ calculateProgress(item) }}%</span>
                  </div>
                </template>
                
                <template #item.status="{ item }">
                  <v-chip
                    :color="getOrderStatusColor(item)"
                    size="small"
                  >
                    {{ formatOrderStatus(item) }}
                  </v-chip>
                </template>
                
                <template #item.userId="{ item }">
                  <code class="text-caption">{{ item.userId.substring(0, 8) }}...</code>
                </template>
                
                <template #item.createdAt="{ item }">
                  {{ formatDate(item.createdAt) }}
                </template>
              </v-data-table>
            </v-tabs-window-item>

            <!-- Game History Tab -->
            <v-tabs-window-item value="batches">
              <v-data-table
                :headers="batchHeaders"
                :items="batches"
                :items-per-page="10"
              >
                <template #item.orderId="{ item }">
                  <code class="text-caption">{{ item.orderId.substring(0, 8) }}...</code>
                </template>
                
                <template #item.boosterId="{ item }">
                  <code class="text-caption">{{ item.boosterId.substring(0, 8) }}...</code>
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
                
                <template #item.createdAt="{ item }">
                  {{ formatDate(item.createdAt) }}
                </template>
              </v-data-table>
            </v-tabs-window-item>
          </v-tabs-window>
        </v-card-text>
      </v-card>
    </template>
  </v-container>
</template>

<style scoped>
.gap-2 {
  gap: 8px;
}
</style>