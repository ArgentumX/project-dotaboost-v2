<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'

const { register } = useAuth()
const router = useRouter()

const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const error = ref('')

const passwordRules = [
  (v: string) => !!v || 'Password is required',
  (v: string) => v.length >= 6 || 'Password must be at least 6 characters'
]

const confirmPasswordRules = [
  (v: string) => !!v || 'Password confirmation is required',
  (v: string) => v === password.value || 'Passwords do not match'
]

async function onSubmit() {
  if (password.value !== confirmPassword.value) {
    error.value = 'Passwords do not match'
    return
  }

  loading.value = true
  error.value = ''
  try {
    await register({ email: email.value, password: password.value })
    router.push({ name: 'dashboard' })
  } catch (e: any) {
    error.value = e.response?.data?.message || 'Registration failed'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <v-container class="d-flex justify-center align-center fill-height">
    <v-card class="pa-6" width="420">
      <v-card-title class="text-h5 mb-2 text-center">Register</v-card-title>

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
          :rules="passwordRules"
          required
        />

        <v-text-field
          v-model="confirmPassword"
          label="Confirm Password"
          type="password"
          prepend-inner-icon="mdi-lock-outline"
          :rules="confirmPasswordRules"
          required
        />

        <v-btn
          class="mt-4"
          color="primary"
          block
          type="submit"
          :loading="loading"
        >
          Register
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
          Already have an account? 
          <router-link :to="{ name: 'login' }" class="text-primary">
            Login here
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
