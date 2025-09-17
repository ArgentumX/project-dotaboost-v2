import { createRouter, createWebHistory } from 'vue-router'

import Login from '../views/Login.vue';
import Register from '../views/Register.vue';
import App from "@/App.vue";
import Home from "@/views/Home.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', component: Home, name: "home" },
    { path: '/login', component: Login, name: "user.login"},
    { path: '/register', component: Register, name: "user.register" },
  ],
})

router.beforeEach((to, from, next) => {
    const token = localStorage.getItem('token');
    if (to.meta.requiresAuth && !token) {
        return next({
            path: '/login',
            query: { redirect: to.fullPath },
        });
    }
    if (token && (to.path === '/login' || to.path === '/register')) {
        return next("/");
    }
    next();
});


export default router
