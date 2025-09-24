export interface User {
  id: string
  email: string
  role?: string[]
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
}

export interface AuthResponse {
  token: string
  user: User
}

export enum ApplicationStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2
}

export enum Sorting {
  Asc = 0,
  Desc = 1
}

export enum CombineType {
  And = 0,
  Or = 1
}

export interface PaginationParams {
  page?: number
  perPage?: number
  sortBy?: Sorting
  sort?: string
  combineWith?: CombineType
}

export interface BoostOrder {
  id: string
  description: string
  isParty: boolean
  isPriority: boolean
  steamUsername: string
  steamPassword?: string
  startRating: number
  currentRating: number
  requiredRating: number
  userId: string
  isPaid: boolean
  isClosed: boolean
  createdAt?: string
  updatedAt?: string
}

export interface CreateBoostOrderRequest {
  description: string
  isParty: boolean
  isPriority: boolean
  steamUsername: string
  steamPassword: string
  startRating: number
  requiredRating: number
}

export interface UpdateBoostOrderRequest {
  description: string
  id: string
}

export interface BoosterApplication {
  id: string
  motivation: string
  contact: string
  steamAccountLink: string
  status: ApplicationStatus
  userId: string
  createdAt?: string
  updatedAt?: string
}

export interface CreateBoosterApplicationRequest {
  motivation: string
  contact: string
  steamAccountLink: string
}

export interface Booster {
  id: string
  userId: string
  status: ApplicationStatus
  createdAt?: string
  updatedAt?: string
}

export interface TakeOrderRequest {
  orderId: string
}

export interface Batch {
  id: string
  screen: string
  receivedMmr: number
  isWin: boolean
  orderId: string
  boosterId: string
  createdAt?: string
}

export interface CreateBatchRequest {
  screen: string
  receivedMmr: number
  isWin: boolean
  orderId: string
}

export interface BoostOrderFilter extends PaginationParams {
  isParty?: boolean
  isPriority?: boolean
  steamUsername?: string
  isPaid?: boolean
  isClosed?: boolean
  startRating?: number
  currentRating?: number
  requiredRating?: number
  userId?: string
}

export interface BoosterApplicationFilter extends PaginationParams {
  userId?: string
  status?: ApplicationStatus
}

export interface BoosterFilter extends PaginationParams {
  userId?: string
  status?: ApplicationStatus
}

export interface BatchFilter extends PaginationParams {
  receivedMmr?: number
  isWin?: boolean
  orderId?: string
  boosterId?: string
}

export interface PaginatedResponse<T> {
  items: T[]
  totalCount: number
  page: number
  perPage: number
}