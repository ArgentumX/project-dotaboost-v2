<script setup lang="ts">
import { useAuth } from '@/composables/useAuth'
import { reactive } from 'vue'

const auth = reactive(useAuth())

const features = [
  {
    icon: 'mdi-trophy',
    title: 'Professional Boosting',
    description: 'Get boosted by experienced players to reach your desired MMR quickly and efficiently.'
  },
  {
    icon: 'mdi-shield-check',
    title: 'Safe & Secure',
    description: 'Your account security is our top priority. All boosters are vetted professionals.'
  },
  {
    icon: 'mdi-clock-fast',
    title: 'Fast Delivery',
    description: 'Quick turnaround times with real-time progress tracking and updates.'
  },
  {
    icon: 'mdi-account-star',
    title: 'Become a Booster',
    description: 'High-skilled player? Join our team and earn money by helping others climb the ranks.'
  }
]
</script>

<template>
  <v-container>
    <!-- Hero Section -->
    <v-row class="mb-12">
      <v-col cols="12" class="text-center">
        <h1 class="text-h2 font-weight-bold mb-4 text-primary">
          DotaBoost Pro
        </h1>
        <p class="text-h5 mb-6 text-grey-darken-1">
          Professional Dota 2 Boosting Service
        </p>
        <p class="text-body-1 mb-8 mx-auto" style="max-width: 600px;">
          Climb the ranks with confidence. Our professional boosters will help you reach your desired MMR
          while you learn from the best players in the game.
        </p>
        
        <div v-if="!auth.isAuthenticated" class="d-flex justify-center gap-4">
          <v-btn
            size="large"
            color="primary"
            :to="{ name: 'register' }"
            class="px-8"
          >
            Get Started
          </v-btn>
          <v-btn
            size="large"
            variant="outlined"
            :to="{ name: 'login' }"
            class="px-8"
          >
            Login
          </v-btn>
        </div>
        
        <div v-else>
          <v-btn
            size="large"
            color="primary"
            :to="{ name: 'dashboard' }"
            class="px-8"
          >
            Go to Dashboard
          </v-btn>
        </div>
      </v-col>
    </v-row>

    <!-- Features Section -->
    <v-row class="mb-12">
      <v-col cols="12">
        <h2 class="text-h4 text-center mb-8">Why Choose DotaBoost Pro?</h2>
      </v-col>
      
      <v-col
        v-for="feature in features"
        :key="feature.title"
        cols="12"
        sm="6"
        md="3"
      >
        <v-card class="pa-4 h-100" variant="outlined">
          <div class="text-center">
            <v-icon
              :icon="feature.icon"
              size="48"
              color="primary"
              class="mb-4"
            />
            <h3 class="text-h6 mb-3">{{ feature.title }}</h3>
            <p class="text-body-2">{{ feature.description }}</p>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <!-- CTA Section -->
    <v-row v-if="!auth.isAuthenticated">
      <v-col cols="12">
        <v-card class="pa-8 text-center" color="primary" variant="flat">
          <h3 class="text-h4 mb-4 text-white">Ready to Rank Up?</h3>
          <p class="text-body-1 mb-6 text-white opacity-90">
            Join thousands of satisfied customers who have achieved their Dota 2 goals with our help.
          </p>
          <v-btn
            size="large"
            color="white"
            :to="{ name: 'register' }"
            class="px-8"
          >
            Start Your Journey
          </v-btn>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>
.gap-4 {
  gap: 1rem;
}
</style>