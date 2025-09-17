// src/composables/useAuth.ts
import { ref, readonly } from 'vue';
import api from '@/api';

interface User {
    id: string;
    email: string;
}

interface AuthResponse {
    token?: string;
    user?: User;
}

const user = ref<User | null>(null);
const isAuthenticated = ref<boolean>(!!localStorage.getItem('token'));

function setToken(token?: string | null) {
    if (token) {
        localStorage.setItem('token', token);
        isAuthenticated.value = true;
    } else {
        localStorage.removeItem('token');
        isAuthenticated.value = false;
    }
}

export async function register(email: string, password: string) {
    const r = await api.post<AuthResponse>('/Account/register', { email, password });
    if (r.data?.token) {
        setToken(r.data.token);
        user.value = r.data.user ?? null;
    }
    return r.data;
}

export async function login(email: string, password: string) {
    const r = await api.post<AuthResponse>('/Account/login', { email, password });
    if (r.data?.token) {
        setToken(r.data.token);
        user.value = r.data.user ?? null;
    }
    return r.data;
}

export function logout() {
    setToken(null);
    user.value = null;
    // Если есть роут /logout на бэке, можно вызвать его:
    // await api.post('/logout');
}

export function useAuth() {
    return {
        user: readonly(user),
        isAuthenticated: readonly(isAuthenticated),
        login,
        register,
        logout,
    };
}