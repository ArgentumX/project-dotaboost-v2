import { ref, readonly, computed } from 'vue'
import { apiService } from '@/api'
import type { User, LoginRequest, RegisterRequest } from '@/types'

const user = ref<User | null>(null)
const isAuthenticated = ref<boolean>(!!localStorage.getItem('authToken'))

function decodeJwt(token: string): any | null {
  try {
    const payload = token.split('.')[1]
    const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
    return JSON.parse(decodeURIComponent(escape(json)))
  } catch {
    return null
  }
}

function extractRoles(payload: any): string[] {
  if (!payload) return []
  const candidates = [
    payload.role,
    payload.roles,
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
  ].flat().filter(Boolean)
  const roles = Array.isArray(candidates) ? candidates : [candidates]
  return roles.map((r: unknown) => String(r))
}

function extractUserId(payload: any): string | undefined {
  if (!payload) return undefined
  return (
    payload.sub ||
    payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
    payload["nameid"]
  ) as string | undefined
}

function setToken(token?: string | null) {
  if (token) {
    localStorage.setItem('authToken', token)
    isAuthenticated.value = true
    const payload = decodeJwt(token)
    const roles = extractRoles(payload)
    const id = extractUserId(payload)
    user.value = {
      id: id || user.value?.id || '',
      email: user.value?.email || '',
      role: roles,
    }
  } else {
    localStorage.removeItem('authToken')
    isAuthenticated.value = false
    user.value = null
  }
}

export async function register(data: RegisterRequest) {
  const response = await apiService.register(data)
  if (response?.token) {
    setToken(response.token)
    // merge backend user info if provided
    if (response.user) {
      user.value = { ...user.value, ...response.user }
    }
  }
  return response
}

export async function login(data: LoginRequest) {
  const response = await apiService.login(data)
  if (response?.token) {
    setToken(response.token)
    if (response.user) {
      user.value = { ...user.value, ...response.user }
    }
  }
  return response
}

export function logout() {
  setToken(null)
  user.value = null
}

export function useAuth() {
  return {
    user: readonly(user),
    isAuthenticated: readonly(isAuthenticated),
    isAdmin: computed(() => {
      const roles = user.value?.role || []
      return roles.some(r => /admin/i.test(r))
    }),
    isBooster: computed(() => {
      const roles = user.value?.role || []
      return roles.some(r => /booster/i.test(r))
    }),
    login,
    register,
    logout,
  }
}

// Initialize from existing token on load
;(function initializeFromStorage() {
  const token = localStorage.getItem('authToken')
  if (token) {
    setToken(token)
  }
})()