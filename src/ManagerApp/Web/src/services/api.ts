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
  CreateGroupModelDto,
  UpdateGroupModelDto,
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
  UpdateMachineTypeDto,
  Machine,
  CreateMachineDto,
  UpdateMachineDto
} from '../types';

// Buyers API
export const buyersApi = {
  getAll: () => apiClient.get<Buyer[]>('/Buyers'),
  getById: (id: number) => apiClient.get<Buyer>(`/Buyers/${id}`),
  create: (data: CreateBuyerDto) => apiClient.post<Buyer>('/Buyers', data),
  update: (id: number, data: UpdateBuyerDto) => 
    apiClient.put<Buyer>(`/Buyers/${id}`, data),
  delete: (id: number) => apiClient.delete(`/Buyers/${id}`),
};

// Lines API
export const linesApi = {
  getAll: () => apiClient.get<Line[]>('/Lines'),
  getById: (id: number) => apiClient.get<Line>(`/Lines/${id}`),
  create: (data: CreateLineDto) => apiClient.post<Line>('/Lines', data),
  update: (id: number, data: UpdateLineDto) => 
    apiClient.put<Line>(`/Lines/${id}`, data),
  delete: (id: number) => apiClient.delete(`/Lines/${id}`),
};

// Model Groups API
export const modelGroupsApi = {
  getAll: () => apiClient.get<ModelGroup[]>('/ModelGroups'),
  getById: (id: number) => apiClient.get<ModelGroup>(`/ModelGroups/${id}`),
  create: (data: CreateModelGroupDto) => 
    apiClient.post<ModelGroup>('/ModelGroups', data),
  update: (id: number, data: UpdateModelGroupDto) => 
    apiClient.put<ModelGroup>(`/ModelGroups/${id}`, data),
  delete: (id: number) => apiClient.delete(`/ModelGroups/${id}`),
};

// Group Models API (alias for Model Groups for compatibility)
export const groupModelsApi = {
  getAll: () => apiClient.get<ModelGroup[]>('/ModelGroups'),
  getById: (id: number) => apiClient.get<ModelGroup>(`/ModelGroups/${id}`),
  create: (data: CreateGroupModelDto) => 
    apiClient.post<ModelGroup>('/ModelGroups', data),
  update: (id: number, data: UpdateGroupModelDto) => 
    apiClient.put<ModelGroup>(`/ModelGroups/${id}`, data),
  delete: (id: number) => apiClient.delete(`/ModelGroups/${id}`),
};

// Models API
export const modelsApi = {
  getAll: () => apiClient.get<Model[]>('/Models'),
  getById: (id: number) => apiClient.get<Model>(`/Models/${id}`),
  create: (data: CreateModelDto) => apiClient.post<Model>('/Models', data),
  update: (id: number, data: UpdateModelDto) => 
    apiClient.put<Model>(`/Models/${id}`, data),
  delete: (id: number) => apiClient.delete(`/Models/${id}`),
};

// Stations API
export const stationsApi = {
  getAll: () => apiClient.get<Station[]>('/Stations'),
  getById: (id: number) => apiClient.get<Station>(`/Stations/${id}`),
  create: (data: CreateStationDto) => apiClient.post<Station>('/Stations', data),
  update: (id: number, data: UpdateStationDto) => 
    apiClient.put<Station>(`/Stations/${id}`, data),
  delete: (id: number) => apiClient.delete(`/Stations/${id}`),
};

// Machine Types API
export const machineTypesApi = {
  getAll: () => apiClient.get<MachineType[]>('/MachineTypes'),
  getById: (id: number) => apiClient.get<MachineType>(`/MachineTypes/${id}`),
  create: (data: CreateMachineTypeDto) => 
    apiClient.post<MachineType>('/MachineTypes', data),
  update: (id: number, data: UpdateMachineTypeDto) => 
    apiClient.put<MachineType>(`/MachineTypes/${id}`, data),
  delete: (id: number) => apiClient.delete(`/MachineTypes/${id}`),
};

// Model Processes API
export const modelProcessesApi = {
  getAll: () => apiClient.get<ModelProcess[]>('/ModelProcesses'),
  getById: (id: number) => apiClient.get<ModelProcess>(`/ModelProcesses/${id}`),
  create: (data: CreateModelProcessDto) => 
    apiClient.post<ModelProcess>('/ModelProcesses', data),
  update: (id: number, data: UpdateModelProcessDto) => 
    apiClient.put<ModelProcess>(`/ModelProcesses/${id}`, data),
  delete: (id: number) => apiClient.delete(`/ModelProcesses/${id}`),
};

// Machines API
export const machinesApi = {
  getAll: () => apiClient.get<Machine[]>('/Machines'),
  getById: (id: number) => apiClient.get<Machine>(`/Machines/${id}`),
  create: (data: CreateMachineDto) => 
    apiClient.post<Machine>('/Machines', data),
  update: (id: number, data: UpdateMachineDto) => 
    apiClient.put<Machine>(`/Machines/${id}`, data),
  delete: (id: number) => apiClient.delete(`/Machines/${id}`),
};