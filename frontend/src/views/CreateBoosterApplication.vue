<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { apiService } from '@/api'
import type { CreateBoosterApplicationRequest } from '@/types'

const router = useRouter()
const loading = ref(false)
const error = ref('')

const form = ref<CreateBoosterApplicationRequest>({
  motivation: '',
  contact: '',
  steamAccountLink: ''
})

const formValid = ref(false)

const rules = {
  required: (v: any) => !!v || 'This field is required',
  minLength: (min: number) => (v: string) => 
    (v && v.length >= min) || `Must be at least ${min} characters`,
  maxLength: (max: number) => (v: string) => 
    !v || v.length <= max || `Must be no more than ${max} characters`,
  url: (v: string) => {
    if (!v) return true
    try {
      new URL(v)
      return true
    } catch {
      return 'Must be a valid URL'
    }
  },
  steamUrl: (v: string) => {
    if (!v) return true
    const steamUrlPattern = /^https?:\/\/(www\.)?steamcommunity\.com\/(id|profiles)\/[a-zA-Z0-9_-]+\/?$/
    return steamUrlPattern.test(v) || 'Must be a valid Steam profile URL'
  }
}

async function submitApplication() {
  if (!formValid.value) return
  
  loading.value = true
  error.value = ''
  
  try {
    await apiService.createBoosterApplication(form.value)
    router.push({ name: 'booster-applications' })
  } catch (e: any) {
    error.value = e.response?.data?.message || 'Failed to submit application'
    console.error('Failed to submit application:', e)
  } finally {
    loading.value = false
  }
}

const benefits = [
  {
    icon: 'mdi-cash',
    title: 'Earn Money',
    description: 'Get paid for playing the game you love. Competitive rates based on your skill level.'
  },
  {
    icon: 'mdi-clock-outline',
    title: 'Flexible Schedule',
    description: 'Work whenever you want. Choose orders that fit your availability.'
  },
  {
    icon: 'mdi-trophy-award',
    title: 'Showcase Skills',
    description: 'Demonstrate your Dota 2 expertise and help others improve their gameplay.'
  },
  {
    icon: 'mdi-account-group',
    title: 'Join Elite Team',
    description: 'Become part of our professional booster network and connect with top players.'
  }
]

const requirements = [
  'Minimum 5500 MMR (Divine rank or higher)',
  'Stable internet connection and dedicated playing time',
  'Professional attitude and communication skills',
  'Clean account history with no recent bans',
  'Ability to maintain high win rates consistently'
]
</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="12" lg="8" class="mx-auto">
        <!-- Header -->
        <div class="text-center mb-8">
          <h1 class="text-h3 font-weight-bold mb-4">Apply to be a Booster</h1>
          <p class="text-h6 text-grey-darken-1 mb-6">
            Turn your Dota 2 skills into earnings. Join our team of professional boosters.
          </p>
        </div>

        <!-- Benefits Section -->
        <v-card class="mb-8">
          <v-card-title class="text-h5 mb-4">Why Become a Booster?</v-card-title>
          <v-card-text>
            <v-row>
              <v-col
                v-for="benefit in benefits"
                :key="benefit.title"
                cols="12"
                sm="6"
              >
                <div class="d-flex">
                  <v-icon
                    :icon="benefit.icon"
                    size="32"
                    color="primary"
                    class="mr-4 mt-1"
                  />
                  <div>
                    <h3 class="text-h6 mb-1">{{ benefit.title }}</h3>
                    <p class="text-body-2">{{ benefit.description }}</p>
                  </div>
                </div>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>

        <!-- Requirements Section -->
        <v-card class="mb-8">
          <v-card-title class="text-h5 mb-4">Requirements</v-card-title>
          <v-card-text>
            <v-list>
              <v-list-item
                v-for="requirement in requirements"
                :key="requirement"
                prepend-icon="mdi-check-circle"
              >
                <v-list-item-title>{{ requirement }}</v-list-item-title>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>

        <!-- Application Form -->
        <v-card class="pa-6">
          <v-card-title class="text-h5 mb-4">Application Form</v-card-title>
          
          <v-form v-model="formValid" @submit.prevent="submitApplication">
            <!-- Motivation -->
            <v-row>
              <v-col cols="12">
                <v-textarea
                  v-model="form.motivation"
                  label="Why do you want to become a booster?"
                  placeholder="Tell us about your Dota 2 experience, achievements, and why you'd be a great booster..."
                  :rules="[rules.required, rules.minLength(50), rules.maxLength(1000)]"
                  variant="outlined"
                  rows="5"
                  counter="1000"
                />
                <div class="text-caption text-grey-darken-1 mt-1">
                  Include your current MMR, rank, years of experience, and any notable achievements.
                </div>
              </v-col>
            </v-row>

            <!-- Contact Information -->
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="form.contact"
                  label="Contact Information"
                  placeholder="Discord: username#1234, Telegram: @username, or email"
                  :rules="[rules.required, rules.minLength(5), rules.maxLength(100)]"
                  variant="outlined"
                  prepend-inner-icon="mdi-message-text"
                />
                <div class="text-caption text-grey-darken-1 mt-1">
                  Provide your preferred contact method (Discord, Telegram, email, etc.)
                </div>
              </v-col>
            </v-row>

            <!-- Steam Profile -->
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="form.steamAccountLink"
                  label="Steam Profile URL"
                  placeholder="https://steamcommunity.com/id/yourprofile"
                  :rules="[rules.required, rules.url, rules.steamUrl]"
                  variant="outlined"
                  prepend-inner-icon="mdi-steam"
                />
                <div class="text-caption text-grey-darken-1 mt-1">
                  Link to your Steam profile. Make sure your profile is public so we can verify your MMR and match history.
                </div>
              </v-col>
            </v-row>

            <!-- Important Notes -->
            <v-row>
              <v-col cols="12">
                <v-alert
                  type="info"
                  variant="tonal"
                  class="mb-4"
                >
                  <v-alert-title>Important Notes</v-alert-title>
                  <ul class="mt-2">
                    <li>Applications are reviewed within 2-3 business days</li>
                    <li>We may contact you for additional verification</li>
                    <li>Your Steam profile must be public for verification</li>
                    <li>Only high-skill players (Divine+) will be considered</li>
                  </ul>
                </v-alert>
              </v-col>
            </v-row>

            <!-- Error Alert -->
            <v-row v-if="error">
              <v-col cols="12">
                <v-alert
                  type="error"
                  variant="tonal"
                >
                  {{ error }}
                </v-alert>
              </v-col>
            </v-row>

            <!-- Submit Button -->
            <v-row>
              <v-col cols="12" class="d-flex gap-4">
                <v-btn
                  type="submit"
                  color="primary"
                  size="large"
                  :loading="loading"
                  :disabled="!formValid"
                  class="px-8"
                >
                  Submit Application
                </v-btn>
                
                <v-btn
                  variant="outlined"
                  size="large"
                  :to="{ name: 'booster-applications' }"
                  class="px-8"
                >
                  Cancel
                </v-btn>
              </v-col>
            </v-row>
          </v-form>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>
.gap-4 {
  gap: 16px;
}
</style>