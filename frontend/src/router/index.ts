import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import Home from '@/views/Home.vue'
import Login from '@/views/Login.vue'
import Register from '@/views/Register.vue'

const AdminDashboard = () => import('@/views/AdminPanel.vue')
const ClientDashboard = () => import('@/views/BoostOrders.vue')
const BoosterDashboard = () => import('@/views/Boosters.vue')
const BoosterApplications = () => import('@/views/BoosterApplications.vue')
const CreateBoosterApplication = () => import('@/views/CreateBoosterApplication.vue')
const CreateOrder = () => import('@/views/CreateOrder.vue')
const OrderDetails = () => import('@/views/OrderDetails.vue')

const routes: RouteRecordRaw[] = [
  { path: '/', name: 'home', component: Home },
  { path: '/login', name: 'login', component: Login },
  { path: '/register', name: 'register', component: Register },
  // Generic dashboard route -> redirects based on role in a global guard
  { path: '/dashboard', name: 'dashboard', component: Home },
  // Role-specific dashboards
  { path: '/admin', name: 'admin-dashboard', component: AdminDashboard, meta: { requiresAuth: true, roles: ['Administrator'] } },
  // Client dashboard is default for any authenticated user (no role requirement)
  { path: '/client', name: 'client-dashboard', component: ClientDashboard, meta: { requiresAuth: true } },
  { path: '/booster', name: 'booster-dashboard', component: BoosterDashboard, meta: { requiresAuth: true, roles: ['Booster'] } },
  // Booster application routes
  { path: '/booster-applications', name: 'booster-applications', component: BoosterApplications, meta: { requiresAuth: true } },
  { path: '/booster-applications/create', name: 'create-booster-application', component: CreateBoosterApplication, meta: { requiresAuth: true } },
  // Order routes
  { path: '/orders/create', name: 'create-order', component: CreateOrder, meta: { requiresAuth: true } },
  { path: '/orders/:id', name: 'order-details', component: OrderDetails, meta: { requiresAuth: true } },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to) => {
  const { isAuthenticated, user } = useAuth()

  // Redirect generic dashboard to role-specific target
  if (to.name === 'dashboard') {
    if (!isAuthenticated.value) {
      return { name: 'login' }
    }
    const roles = user.value?.role || []
    if (roles.includes('Administrator')) return { name: 'admin-dashboard' }
    if (roles.includes('Booster')) return { name: 'booster-dashboard' }
    // default to client dashboard
    return { name: 'client-dashboard' }
  }

  // Auth guard with role checks
  if (to.meta?.requiresAuth) {
    if (!isAuthenticated.value) {
      return { name: 'login', query: { redirect: to.fullPath } }
    }
    const requiredRoles = (to.meta.roles as string[]) || []
    if (requiredRoles.length) {
      const roles = user.value?.role || []
      const hasRole = requiredRoles.some(r => roles.includes(r))
      if (!hasRole) {
        // No permission: send to home (avoids loops)
        return { name: 'home' }
      }
    }
  }
})

export default router


