<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'

const { login } = useAuth()
const router = useRouter()

const email = ref('')
const password = ref('')
const loading = ref(false)
const error = ref('')

async function onSubmit() {
  loading.value = true
  error.value = ''
  try {
    await login({ email: email.value, password: password.value })
    router.push({ name: 'dashboard' })
  } catch (e: any) {
    error.value = e.response?.data?.message || 'Login failed'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <v-container class="d-flex justify-center align-center fill-height">
    <v-card class="pa-6" width="420">
      <v-card-title class="text-h5 mb-2 text-center">Login</v-card-title>

      <v-form @submit.prevent="onSubmit">
        <v-text-field
          v-model="email"
          label="Email"
          type="email"
          prepend-inner-icon="mdi-email-outline"
          required
        />

        <v-text-field
          v-model="password"
          label="Password"
          type="password"
          prepend-inner-icon="mdi-lock-outline"
          required
        />

        <v-btn
          class="mt-4"
          color="primary"
          block
          type="submit"
          :loading="loading"
        >
          Login
        </v-btn>

        <v-alert
          v-if="error"
          type="error"
          variant="tonal"
          class="mt-4"
        >
          {{ error }}
        </v-alert>

        <div class="text-center mt-4">
          Don't have an account? 
          <router-link :to="{ name: 'register' }" class="text-primary">
            Register here
          </router-link>
        </div>
      </v-form>
    </v-card>
  </v-container>
</template>

<style scoped>
.fill-height {
  height: 100vh;
}
</style>
