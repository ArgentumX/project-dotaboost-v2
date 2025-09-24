<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiService } from '@/api'
import type { BoosterApplication, BoosterApplicationFilter } from '@/types'
import { ApplicationStatus } from '@/types'

const applications = ref<BoosterApplication[]>([])
const loading = ref(true)
const totalCount = ref(0)
const page = ref(1)
const perPage = ref(10)
const detailsDialog = ref(false)
const selectedApplication = ref<BoosterApplication | null>(null)

const headers = [
  { title: 'Status', key: 'status', sortable: true },
  { title: 'Motivation', key: 'motivation', sortable: false },
  { title: 'Contact', key: 'contact', sortable: false },
  { title: 'Steam Profile', key: 'steamAccountLink', sortable: false },
  { title: 'Applied Date', key: 'createdAt', sortable: true },
  { title: 'Actions', key: 'actions', sortable: false }
]

const statusFilter = ref<ApplicationStatus | null>(null)

onMounted(() => {
  loadApplications()
})

async function loadApplications() {
  loading.value = true
  try {
    const filter: BoosterApplicationFilter = {
      page: page.value,
      perPage: perPage.value,
      ...(statusFilter.value !== null && { status: statusFilter.value })
    }
    
    const response = await apiService.getBoosterApplications(filter)
    applications.value = response.items
    totalCount.value = response.totalCount
  } catch (error) {
    console.error('Failed to load applications:', error)
  } finally {
    loading.value = false
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
    case ApplicationStatus.Approved: return 'Approved'
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
  loadApplications()
}

function clearFilter() {
  statusFilter.value = null
  page.value = 1
  loadApplications()
}

function viewApplication(application: BoosterApplication) {
  selectedApplication.value = application
  detailsDialog.value = true
}
</script>

<template>
  <v-container>
    <v-row class="mb-6">
      <v-col cols="12" class="d-flex align-center justify-space-between">
        <div>
          <h1 class="text-h4 font-weight-bold">Booster Applications</h1>
          <p class="text-body-1 text-grey-darken-1">
            Manage your booster applications and track their status
          </p>
        </div>
        
        <v-btn
          color="primary"
          :to="{ name: 'create-booster-application' }"
          prepend-icon="mdi-plus"
        >
          Apply to be a Booster
        </v-btn>
      </v-col>
    </v-row>

    <!-- Filter -->
    <v-row class="mb-4">
      <v-col cols="12" md="4">
        <v-select
          v-model="statusFilter"
          label="Filter by Status"
          :items="[
            { title: 'All Applications', value: null },
            { title: 'Pending', value: ApplicationStatus.Pending },
            { title: 'Approved', value: ApplicationStatus.Approved },
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

    <!-- Applications Table -->
    <v-card>
      <v-data-table
        :headers="headers"
        :items="applications"
        :loading="loading"
        :items-per-page="perPage"
        :page="page"
        @update:page="page = $event; loadApplications()"
        @update:items-per-page="perPage = $event; loadApplications()"
      >
        <template #item.status="{ item }">
          <v-chip
            :color="getStatusColor(item.status)"
            size="small"
          >
            {{ getStatusText(item.status) }}
          </v-chip>
        </template>
        
        <template #item.motivation="{ item }">
          <div class="text-truncate" style="max-width: 300px;">
            {{ item.motivation }}
          </div>
        </template>
        
        <template #item.contact="{ item }">
          <div class="text-truncate" style="max-width: 200px;">
            {{ item.contact }}
          </div>
        </template>
        
        <template #item.steamAccountLink="{ item }">
          <v-btn
            v-if="item.steamAccountLink"
            variant="text"
            size="small"
            prepend-icon="mdi-steam"
            :href="item.steamAccountLink"
            target="_blank"
          >
            View Profile
          </v-btn>
          <span v-else class="text-grey-darken-1">No link</span>
        </template>
        
        <template #item.createdAt="{ item }">
          {{ formatDate(item.createdAt) }}
        </template>
        
        <template #item.actions="{ item }">
          <v-btn
            icon="mdi-eye"
            variant="text"
            size="small"
            @click="viewApplication(item)"
          />
        </template>
        
        <template #no-data>
          <div class="text-center py-12">
            <v-icon icon="mdi-account-plus-outline" size="64" color="grey" />
            <p class="text-h6 mt-4">No applications found</p>
            <p class="text-body-2 text-grey-darken-1 mb-4">
              Apply to become a booster and start earning money
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

    <!-- Application Details Dialog -->
    <v-dialog v-model="detailsDialog" max-width="800">
      <v-card v-if="selectedApplication">
        <v-card-title class="d-flex align-center justify-space-between">
          <span>Application Details</span>
          <v-chip
            :color="getStatusColor(selectedApplication.status)"
            size="small"
          >
            {{ getStatusText(selectedApplication.status) }}
          </v-chip>
        </v-card-title>
        
        <v-card-text>
          <v-row>
            <v-col cols="12">
              <h3 class="text-h6 mb-2">Motivation</h3>
              <p class="text-body-2 mb-4">{{ selectedApplication.motivation }}</p>
            </v-col>
            
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">Contact Information</h3>
              <p class="text-body-2">{{ selectedApplication.contact }}</p>
            </v-col>
            
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">Steam Profile</h3>
              <v-btn
                v-if="selectedApplication.steamAccountLink"
                variant="outlined"
                size="small"
                prepend-icon="mdi-steam"
                :href="selectedApplication.steamAccountLink"
                target="_blank"
              >
                View Steam Profile
              </v-btn>
              <span v-else class="text-grey-darken-1">No Steam profile provided</span>
            </v-col>
            
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">Application Date</h3>
              <p class="text-body-2">{{ formatDate(selectedApplication.createdAt) }}</p>
            </v-col>
            
            <v-col cols="12" md="6">
              <h3 class="text-h6 mb-2">Last Updated</h3>
              <p class="text-body-2">{{ formatDate(selectedApplication.updatedAt) }}</p>
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
/* No additional styles needed */
</style>