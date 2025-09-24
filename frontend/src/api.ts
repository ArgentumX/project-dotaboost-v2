import axios from 'axios'
import type { AxiosInstance, AxiosResponse } from 'axios'
import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  BoostOrder,
  CreateBoostOrderRequest,
  UpdateBoostOrderRequest,
  BoostOrderFilter,
  BoosterApplication,
  CreateBoosterApplicationRequest,
  BoosterApplicationFilter,
  Booster,
  BoosterFilter,
  TakeOrderRequest,
  Batch,
  CreateBatchRequest,
  BatchFilter,
  PaginatedResponse
} from './types'

class ApiService {
  private api: AxiosInstance

  constructor() {
    this.api = axios.create({
      baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5083/api/v1/',
      headers: {
        'Content-Type': 'application/json'
      }
    })

    this.api.interceptors.request.use((config) => {
      const token = localStorage.getItem('authToken')
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
      return config
    })

    this.api.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          localStorage.removeItem('authToken')
          window.location.href = '/login'
        }
        return Promise.reject(error)
      }
    )
  }

  // Auth endpoints
  async register(data: RegisterRequest): Promise<AuthResponse> {
    const response = await this.api.post<AuthResponse>('/Account/register', data)
    return response.data
  }

  async login(data: LoginRequest): Promise<AuthResponse> {
    const response = await this.api.post<AuthResponse>('/Account/login', data)
    return response.data
  }

  // BoostOrder endpoints
  async getBoostOrders(filter?: BoostOrderFilter): Promise<PaginatedResponse<BoostOrder>> {
    const response = await this.api.get<PaginatedResponse<BoostOrder>>('/BoostOrder', {
      params: filter
    })
    return response.data
  }

  async getBoostOrder(id: string): Promise<BoostOrder> {
    const response = await this.api.get<BoostOrder>(`/BoostOrder/${id}`)
    return response.data
  }

  async createBoostOrder(data: CreateBoostOrderRequest): Promise<BoostOrder> {
    const response = await this.api.post<BoostOrder>('/BoostOrder', data)
    return response.data
  }

  async updateBoostOrder(id: string, data: UpdateBoostOrderRequest): Promise<BoostOrder> {
    const response = await this.api.patch<BoostOrder>(`/BoostOrder/${id}`, data)
    return response.data
  }

  async closeBoostOrder(id: string): Promise<void> {
    await this.api.post(`/BoostOrder/${id}/close`)
  }

  // BoosterApplication endpoints
  async getBoosterApplications(filter?: BoosterApplicationFilter): Promise<PaginatedResponse<BoosterApplication>> {
    const response = await this.api.get<PaginatedResponse<BoosterApplication>>('/BoosterApplication', {
      params: filter
    })
    return response.data
  }

  async getBoosterApplication(id: string): Promise<BoosterApplication> {
    const response = await this.api.get<BoosterApplication>(`/BoosterApplication/${id}`)
    return response.data
  }

  async createBoosterApplication(data: CreateBoosterApplicationRequest): Promise<BoosterApplication> {
    const response = await this.api.post<BoosterApplication>('/BoosterApplication', data)
    return response.data
  }

  // Admin BoosterApplication endpoints
  async approveBoosterApplication(id: string): Promise<void> {
    await this.api.post(`/AdminBoosterApplication/${id}/approve`)
  }

  async rejectBoosterApplication(id: string): Promise<void> {
    await this.api.post(`/AdminBoosterApplication/${id}/reject`)
  }

  // Booster endpoints
  async getBoosters(filter?: BoosterFilter): Promise<PaginatedResponse<Booster>> {
    const response = await this.api.get<PaginatedResponse<Booster>>('/Booster', {
      params: filter
    })
    return response.data
  }

  async getBooster(id: string): Promise<Booster> {
    const response = await this.api.get<Booster>(`/Booster/${id}`)
    return response.data
  }

  async takeOrder(data: TakeOrderRequest): Promise<void> {
    await this.api.post('/Booster/take', data)
  }

  async refuseOrder(): Promise<void> {
    await this.api.post('/Booster/refuse')
  }

  // Batch endpoints
  async getBatches(filter?: BatchFilter): Promise<PaginatedResponse<Batch>> {
    const response = await this.api.get<PaginatedResponse<Batch>>('/Batch', {
      params: filter
    })
    return response.data
  }

  async getBatch(id: string): Promise<Batch> {
    const response = await this.api.get<Batch>(`/Batch/${id}`)
    return response.data
  }

  async createBatch(data: CreateBatchRequest): Promise<Batch> {
    const response = await this.api.post<Batch>('/Batch', data)
    return response.data
  }
}

export const apiService = new ApiService()
export default apiService