import { createRouter, createWebHistory } from 'vue-router'

import Login from '../views/Login.vue';
import Register from '../views/Register.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/login', component: Login, name: "user.login"}, 
    { path: '/register', component: Register, name: "user.register" },
  ],
})

router.beforeEach((to, from, next) => {
    const token = !!localStorage.getItem('token'); 
    if (to.meta.requiresAuth && !token) {
        return next({ path: '/login', query: { redirect: to.fullPath } });
    }
    if ((to.path === '/login' || to.path === '/register') && token) {
        return next('/');
    }
    next();
});


export default router
