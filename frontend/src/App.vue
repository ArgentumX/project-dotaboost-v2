<script setup lang="ts">

</script>

<template>
  <v-app>
    <!-- Верхняя панель -->
    <v-app-bar app color="primary" dark>
      <v-toolbar-title class="font-weight-bold">
        Project Dotaboost V2
      </v-toolbar-title>

      <v-spacer />

      <!-- Кнопки для навигации -->
      <RouterLink :to="{ name: 'user.login' }" custom v-slot="{ navigate }">
        <v-btn text @click="navigate">Login</v-btn>
      </RouterLink>

      <RouterLink :to="{ name: 'user.register' }" custom v-slot="{ navigate }">
        <v-btn outlined @click="navigate">Register</v-btn>
      </RouterLink>
    </v-app-bar>

    <!-- Основной контент -->
    <v-main class="pa-6">
      <router-view />
    </v-main>
  </v-app>
</template>


<style scoped>

</style>