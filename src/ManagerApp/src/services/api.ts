import { apiClient } from './apiClient';
import { 
  Buyer, 
  CreateBuyerDto, 
  UpdateBuyerDto,
  Line,
  CreateLineDto,
  UpdateLineDto,
  ModelGroup,
  CreateModelGroupDto,
  UpdateModelGroupDto,
  Model,
  CreateModelDto,
  UpdateModelDto,
  ModelProcess,
  CreateModelProcessDto,
  UpdateModelProcessDto,
  Station,
  CreateStationDto,
  UpdateStationDto,
  MachineType,
  CreateMachineTypeDto,
  UpdateMachineTypeDto
} from '../types';

// Buyers API
export const buyersApi = {
  getAll: () => apiClient.get<Buyer[]>('/buyers'),
  getById: (id: number) => apiClient.get<Buyer>(`/buyers/${id}`),
  create: (data: CreateBuyerDto) => apiClient.post<Buyer>('/buyers', data),
  update: (id: number, data: UpdateBuyerDto) => 
    apiClient.put<Buyer>(`/buyers/${id}`, data),
  delete: (id: number) => apiClient.delete(`/buyers/${id}`),
};

// Lines API
export const linesApi = {
  getAll: () => apiClient.get<Line[]>('/lines'),
  getById: (id: number) => apiClient.get<Line>(`/lines/${id}`),
  create: (data: CreateLineDto) => apiClient.post<Line>('/lines', data),
  update: (id: number, data: UpdateLineDto) => 
    apiClient.put<Line>(`/lines/${id}`, data),
  delete: (id: number) => apiClient.delete(`/lines/${id}`),
};

// Model Groups API
export const modelGroupsApi = {
  getAll: () => apiClient.get<ModelGroup[]>('/modelgroups'),
  getById: (id: number) => apiClient.get<ModelGroup>(`/modelgroups/${id}`),
  create: (data: CreateModelGroupDto) => 
    apiClient.post<ModelGroup>('/modelgroups', data),
  update: (id: number, data: UpdateModelGroupDto) => 
    apiClient.put<ModelGroup>(`/modelgroups/${id}`, data),
  delete: (id: number) => apiClient.delete(`/modelgroups/${id}`),
};

// Models API
export const modelsApi = {
  getAll: () => apiClient.get<Model[]>('/models'),
  getById: (id: number) => apiClient.get<Model>(`/models/${id}`),
  create: (data: CreateModelDto) => apiClient.post<Model>('/models', data),
  update: (id: number, data: UpdateModelDto) => 
    apiClient.put<Model>(`/models/${id}`, data),
  delete: (id: number) => apiClient.delete(`/models/${id}`),
};

// Stations API
export const stationsApi = {
  getAll: () => apiClient.get<Station[]>('/stations'),
  getById: (id: number) => apiClient.get<Station>(`/stations/${id}`),
  create: (data: CreateStationDto) => apiClient.post<Station>('/stations', data),
  update: (id: number, data: UpdateStationDto) => 
    apiClient.put<Station>(`/stations/${id}`, data),
  delete: (id: number) => apiClient.delete(`/stations/${id}`),
};

// Machine Types API
export const machineTypesApi = {
  getAll: () => apiClient.get<MachineType[]>('/machinetypes'),
  getById: (id: number) => apiClient.get<MachineType>(`/machinetypes/${id}`),
  create: (data: CreateMachineTypeDto) => 
    apiClient.post<MachineType>('/machinetypes', data),
  update: (id: number, data: UpdateMachineTypeDto) => 
    apiClient.put<MachineType>(`/machinetypes/${id}`, data),
  delete: (id: number) => apiClient.delete(`/machinetypes/${id}`),
};

// Model Processes API
export const modelProcessesApi = {
  getAll: () => apiClient.get<ModelProcess[]>('/modelprocesses'),
  getById: (id: number) => apiClient.get<ModelProcess>(`/modelprocesses/${id}`),
  create: (data: CreateModelProcessDto) => 
    apiClient.post<ModelProcess>('/modelprocesses', data),
  update: (id: number, data: UpdateModelProcessDto) => 
    apiClient.put<ModelProcess>(`/modelprocesses/${id}`, data),
  delete: (id: number) => apiClient.delete(`/modelprocesses/${id}`),
};