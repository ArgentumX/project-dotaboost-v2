<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { apiService } from '@/api'
import type { BoostOrder, BoostOrderFilter } from '@/types'

const orders = ref<BoostOrder[]>([])
const loading = ref(true)
const totalCount = ref(0)
const page = ref(1)
const perPage = ref(10)
const search = ref('')

const headers = [
  { title: 'Description', key: 'description', sortable: false },
  { title: 'Start Rating', key: 'startRating', sortable: true },
  { title: 'Current Rating', key: 'currentRating', sortable: true },
  { title: 'Target Rating', key: 'requiredRating', sortable: true },
  { title: 'Progress', key: 'progress', sortable: false },
  { title: 'Status', key: 'status', sortable: false },
  { title: 'Priority', key: 'isPriority', sortable: true },
  { title: 'Party', key: 'isParty', sortable: true },
  { title: 'Actions', key: 'actions', sortable: false }
]

const filters = ref({
  isParty: null as boolean | null,
  isPriority: null as boolean | null,
  isPaid: null as boolean | null,
  isClosed: null as boolean | null
})

const showFilters = ref(false)

const filteredOrders = computed(() => {
  if (!search.value) return orders.value
  
  return orders.value.filter(order => 
    order.description?.toLowerCase().includes(search.value.toLowerCase()) ||
    order.steamUsername?.toLowerCase().includes(search.value.toLowerCase())
  )
})

onMounted(() => {
  loadOrders()
})

async function loadOrders() {
  loading.value = true
  try {
    const filter: BoostOrderFilter = {
      page: page.value,
      perPage: perPage.value,
      ...Object.fromEntries(
        Object.entries(filters.value).filter(([_, value]) => value !== null)
      )
    }
    
    const response = await apiService.getBoostOrders(filter)
    orders.value = response.items
    totalCount.value = response.totalCount
  } catch (error) {
    console.error('Failed to load orders:', error)
  } finally {
    loading.value = false
  }
}

function calculateProgress(order: BoostOrder): number {
  const total = order.requiredRating - order.startRating
  const current = order.currentRating - order.startRating
  return Math.round((current / total) * 100)
}

function getStatusColor(order: BoostOrder): string {
  if (order.isClosed) return 'success'
  if (!order.isPaid) return 'error'
  return 'warning'
}

function getStatusText(order: BoostOrder): string {
  if (order.isClosed) return 'Completed'
  if (!order.isPaid) return 'Unpaid'
  return 'In Progress'
}

async function closeOrder(orderId: string) {
  try {
    await apiService.closeBoostOrder(orderId)
    await loadOrders()
  } catch (error) {
    console.error('Failed to close order:', error)
  }
}

function clearFilters() {
  filters.value = {
    isParty: null,
    isPriority: null,
    isPaid: null,
    isClosed: null
  }
  loadOrders()
}

function applyFilters() {
  page.value = 1
  loadOrders()
}
</script>

<template>
  <v-container>
    <v-row class="mb-6">
      <v-col cols="12" class="d-flex align-center justify-space-between">
        <div>
          <h1 class="text-h4 font-weight-bold">My Boost Orders</h1>
          <p class="text-body-1 text-grey-darken-1">
            Manage and track your boost orders
          </p>
        </div>
        
        <v-btn
          color="primary"
          :to="{ name: 'create-order' }"
          prepend-icon="mdi-plus"
        >
          Create Order
        </v-btn>
        
        <v-btn
          variant="outlined"
          color="primary"
          :to="{ name: 'create-booster-application' }"
          prepend-icon="mdi-account-plus"
        >
          Become a Booster
        </v-btn>

        <v-btn
          variant="text"
          color="primary"
          :to="{ name: 'booster-applications' }"
          prepend-icon="mdi-file-document"
        >
          Application Status
        </v-btn>
      </v-col>
    </v-row>

    <!-- Search and Filters -->
    <v-row class="mb-4">
      <v-col cols="12" md="6">
        <v-text-field
          v-model="search"
          label="Search orders..."
          prepend-inner-icon="mdi-magnify"
          clearable
          variant="outlined"
          density="compact"
        />
      </v-col>
      
      <v-col cols="12" md="6" class="d-flex align-center justify-end">
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
                v-model="filters.isParty"
                label="Party Boost"
                :items="[
                  { title: 'All', value: null },
                  { title: 'Solo', value: false },
                  { title: 'Party', value: true }
                ]"
                variant="outlined"
                density="compact"
              />
            </v-col>
            
            <v-col cols="12" sm="6" md="3">
              <v-select
                v-model="filters.isPriority"
                label="Priority"
                :items="[
                  { title: 'All', value: null },
                  { title: 'Normal', value: false },
                  { title: 'Priority', value: true }
                ]"
                variant="outlined"
                density="compact"
              />
            </v-col>
            
            <v-col cols="12" sm="6" md="3">
              <v-select
                v-model="filters.isPaid"
                label="Payment Status"
                :items="[
                  { title: 'All', value: null },
                  { title: 'Unpaid', value: false },
                  { title: 'Paid', value: true }
                ]"
                variant="outlined"
                density="compact"
              />
            </v-col>
            
            <v-col cols="12" sm="6" md="3">
              <v-select
                v-model="filters.isClosed"
                label="Status"
                :items="[
                  { title: 'All', value: null },
                  { title: 'Active', value: false },
                  { title: 'Completed', value: true }
                ]"
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

    <!-- Orders Table -->
    <v-card>
      <v-data-table
        :headers="headers"
        :items="filteredOrders"
        :loading="loading"
        :items-per-page="perPage"
        :page="page"
        @update:page="page = $event; loadOrders()"
        @update:items-per-page="perPage = $event; loadOrders()"
      >
        <template #item.description="{ item }">
          <div class="text-truncate" style="max-width: 200px;">
            {{ item.description || 'No description' }}
          </div>
        </template>
        
        <template #item.progress="{ item }">
          <div class="d-flex align-center">
            <v-progress-linear
              :model-value="calculateProgress(item)"
              height="8"
              rounded
              :color="item.isClosed ? 'success' : 'primary'"
              class="mr-2"
              style="min-width: 100px;"
            />
            <span class="text-caption">{{ calculateProgress(item) }}%</span>
          </div>
        </template>
        
        <template #item.status="{ item }">
          <v-chip
            :color="getStatusColor(item)"
            size="small"
          >
            {{ getStatusText(item) }}
          </v-chip>
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
          <div class="d-flex gap-2">
            <v-btn
              icon="mdi-eye"
              variant="text"
              size="small"
              :to="{ name: 'order-details', params: { id: item.id } }"
            />
            
            <v-btn
              v-if="!item.isClosed"
              icon="mdi-check"
              variant="text"
              size="small"
              color="success"
              @click="closeOrder(item.id)"
            />
          </div>
        </template>
        
        <template #no-data>
          <div class="text-center py-12">
            <v-icon icon="mdi-clipboard-outline" size="64" color="grey" />
            <p class="text-h6 mt-4">No orders found</p>
            <p class="text-body-2 text-grey-darken-1 mb-4">
              Create your first boost order to get started
            </p>
            <v-btn
              color="primary"
              :to="{ name: 'create-order' }"
            >
              Create Order
            </v-btn>
          </div>
        </template>
      </v-data-table>
    </v-card>
  </v-container>
</template>

<style scoped>
.gap-2 {
  gap: 8px;
}
</style>