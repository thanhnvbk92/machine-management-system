// Authentication Models and Types
export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
  expiresAt: string;
}

export interface User {
  id: number;
  username: string;
  email: string;
  role: UserRole;
  createdAt: string;
  isActive: boolean;
}

export enum UserRole {
  Admin = 'Admin',
  Manager = 'Manager', 
  Operator = 'Operator',
  Viewer = 'Viewer'
}

// Database Models for CRUD
export interface Machine {
  id: number;
  name: string;
  status: string;
  machineTypeId?: number;
  ip?: string;
  gmesName?: string;
  stationId?: number;
  programName?: string;
  macAddress?: string;
  machineType?: MachineType;
  station?: Station;
}

export interface MachineType {
  id: number;
  name: string;
}

export interface Station {
  id: number;
  name: string;
  modelProcessId: number;
  lineId: number;
  modelProcess?: ModelProcess;
  line?: Line;
}

export interface Line {
  id: number;
  name: string;
}

export interface ModelProcess {
  id: number;
  name: string;
  modelGroupId: number;
  modelGroup?: ModelGroup;
}

export interface ModelGroup {
  id: number;
  name: string;
  buyerId: number;
  buyer?: Buyer;
}

export interface Buyer {
  id: number;
  code: string;
  name: string;
}

export interface Model {
  id: number;
  name: string;
  modelGroupId: number;
  modelGroup?: ModelGroup;
}

// API Response wrapper
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

// Pagination
export interface PaginationParams {
  page: number;
  pageSize: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  search?: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// DTO Types for Create and Update operations
export interface CreateBuyerDto {
  code: string;
  name: string;
}

export interface UpdateBuyerDto {
  code?: string;
  name?: string;
}

export interface CreateLineDto {
  name: string;
}

export interface UpdateLineDto {
  name?: string;
}

export interface CreateModelGroupDto {
  name: string;
  buyerId: number;
}

export interface UpdateModelGroupDto {
  name?: string;
  buyerId?: number;
}

export interface CreateGroupModelDto {
  name: string;
  buyerId: number;
}

export interface UpdateGroupModelDto {
  name?: string;
  buyerId?: number;
}

// Type alias để tương thích với trang group-models
export type GroupModel = ModelGroup;

export interface CreateModelDto {
  name: string;
  modelGroupId: number;
}

export interface UpdateModelDto {
  name?: string;
  modelGroupId?: number;
}

export interface CreateStationDto {
  name: string;
  modelProcessId: number;
  lineId: number;
}

export interface UpdateStationDto {
  name?: string;
  modelProcessId?: number;
  lineId?: number;
}

export interface CreateMachineTypeDto {
  name: string;
}

export interface UpdateMachineTypeDto {
  name?: string;
}

export interface CreateMachineDto {
  name: string;
  status: string;
  machineTypeId?: number;
  ip?: string;
  gmesName?: string;
  stationId?: number;
  programName?: string;
  macAddress?: string;
}

export interface UpdateMachineDto {
  name?: string;
  status?: string;
  machineTypeId?: number;
  ip?: string;
  gmesName?: string;
  stationId?: number;
  programName?: string;
  macAddress?: string;
}

// ModelProcess interfaces
export interface ModelProcess {
  id: number;
  name: string;
  modelGroupId: number;
  modelGroupName?: string;
  buyerName?: string;
  stationCount: number;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  modelGroup?: ModelGroup;
  stations?: Station[];
}

export interface CreateModelProcessDto {
  name: string;
  modelGroupId: number;
}

export interface UpdateModelProcessDto {
  name?: string;
  modelGroupId?: number;
}