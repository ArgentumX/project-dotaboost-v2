<script setup lang="ts">
import { useAuth } from '@/composables/useAuth';
import router from "@/router";
import {nextTick, reactive} from "vue";

const auth = reactive(useAuth())


async function onLogout() {
  auth.logout()
  await nextTick()
  await router.push('/')
}

</script>


<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-toolbar-title class="font-weight-bold">
        Project Dotaboost V2
      </v-toolbar-title>

      <v-spacer />

      <!-- Если пользователь не авторизован -->
      <template v-if="!auth.isAuthenticated">
        <RouterLink :to="{ name: 'user.login' }" custom v-slot="{ navigate }">
          <v-btn text @click="navigate">Login</v-btn>
        </RouterLink>

        <RouterLink :to="{ name: 'user.register' }" custom v-slot="{ navigate }">
          <v-btn outlined @click="navigate">Register</v-btn>
        </RouterLink>
      </template>

      <template v-else>
        <v-btn outlined @click="onLogout">Logout</v-btn>
      </template>
    </v-app-bar>

    <v-main class="pa-6">
      <router-view />
    </v-main>
  </v-app>
</template>


<style scoped>

</style>