<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'

const { register } = useAuth()
const router = useRouter()

const email = ref('')
const pass = ref('')
const loading = ref(false)
const error = ref('')

async function onSubmit() {
  loading.value = true
  error.value = ''
  try {
    await register(email.value, pass.value)
    router.push('/')
  } catch (e: any) {
    error.value = e.response?.data?.message || 'Ошибка регистрации'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <v-container class="d-flex justify-center align-center fill-height">
    <v-card class="pa-6" width="420">
      <v-card-title class="text-h5 mb-2">Регистрация</v-card-title>

      <v-form @submit.prevent="onSubmit">
        <v-text-field
            v-model="email"
            label="Email"
            type="email"
            prepend-inner-icon="mdi-email-outline"
            required
        />

        <v-text-field
            v-model="pass"
            label="Пароль"
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
          Зарегистрироваться
        </v-btn>

        <v-alert
            v-if="error"
            type="error"
            variant="tonal"
            class="mt-4"
        >
          {{ error }}
        </v-alert>
      </v-form>
    </v-card>
  </v-container>
</template>

<style scoped>
.fill-height {
  height: 100vh;
}
</style>
