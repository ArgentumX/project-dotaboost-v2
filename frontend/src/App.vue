<script setup lang="ts">
import { useAuth } from '@/composables/useAuth'
import { useRouter } from 'vue-router'

const { isAuthenticated, logout } = useAuth()
const router = useRouter()

function onLogout() {
  logout()
  router.push({ name: 'home' })
}
</script>

<template>
  <v-app>
    <v-app-bar app flat>
      <v-container class="d-flex align-center justify-space-between">
        <router-link :to="{ name: 'home' }" class="text-decoration-none">
          <span class="text-h6">DotaBoost Pro</span>
        </router-link>
        <div class="d-flex align-center gap-4">
          <template v-if="!isAuthenticated">
            <router-link :to="{ name: 'login' }" class="text-primary">Login</router-link>
            <router-link :to="{ name: 'register' }" class="text-primary">Register</router-link>
          </template>
          <template v-else>
            <router-link :to="{ name: 'dashboard' }" class="text-primary">Dashboard</router-link>
            <v-btn size="small" color="primary" variant="tonal" @click="onLogout">Logout</v-btn>
          </template>
        </div>
      </v-container>
    </v-app-bar>

    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<style scoped>
.gap-4 {
  gap: 1rem;
}
.text-decoration-none {
  text-decoration: none;
}
</style>



