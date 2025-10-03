'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { Machine, MachineType, Station, CreateMachineDto, UpdateMachineDto } from '@/types';
import { machinesApi, machineTypesApi, stationsApi } from '@/services/api';
import { Plus, Edit, Trash2, Search, Filter } from 'lucide-react';

export default function MachinesPage() {
  const [machines, setMachines] = useState<Machine[]>([]);
  const [machineTypes, setMachineTypes] = useState<MachineType[]>([]);
  const [stations, setStations] = useState<Station[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingMachine, setEditingMachine] = useState<Machine | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [filterStatus, setFilterStatus] = useState('all');

  // Load data from API
  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        const [machinesResponse, machineTypesResponse, stationsResponse] = await Promise.all([
          machinesApi.getAll(),
          machineTypesApi.getAll(),
          stationsApi.getAll()
        ]);
        
        setMachines(machinesResponse);
        setMachineTypes(machineTypesResponse);
        setStations(stationsResponse);
      } catch (error) {
        console.error('Error loading data:', error);
        // Fallback to mock data if API fails
        setMachines([
          {
            id: 1,
            name: 'CNC-001',
            status: 'online',
            ip: '192.168.1.101',
            gmesName: 'CNC_MACHINE_01',
            machineTypeId: 1,
            stationId: 1,
            programName: 'AUTO_CNC',
            macAddress: '00:1B:44:11:3A:B7'
          },
          {
            id: 2,
            name: 'LASER-003',
            status: 'offline',
            ip: '192.168.1.103',
            gmesName: 'LASER_CUTTER_03',
            machineTypeId: 1,
            stationId: 2,
            programName: 'LASER_AUTO',
            macAddress: '00:1B:44:11:3A:B8'
          }
        ]);
        setMachineTypes([
          { id: 1, name: 'CNC Machine' },
          { id: 2, name: 'Laser Cutter' }
        ]);
        setStations([
          { id: 1, name: 'Station A', modelProcessId: 1, lineId: 1 },
          { id: 2, name: 'Station B', modelProcessId: 1, lineId: 1 }
        ]);
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, []);

  // Helper function to get machine type name
  const getMachineTypeName = (machineTypeId?: number) => {
    if (!machineTypeId) return 'N/A';
    const type = machineTypes.find(t => t.id === machineTypeId);
    return type?.name || `Type ${machineTypeId}`;
  };

  // Helper function to get station name
  const getStationName = (stationId?: number) => {
    if (!stationId) return 'N/A';
    const station = stations.find(s => s.id === stationId);
    return station?.name || `Station ${stationId}`;
  };

  const filteredMachines = machines.filter(machine => {
    const matchesSearch = machine.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         machine.ip?.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         machine.gmesName?.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesFilter = filterStatus === 'all' || machine.status === filterStatus;
    return matchesSearch && matchesFilter;
  });

  const handleAddMachine = () => {
    setEditingMachine(null);
    setShowModal(true);
  };

  const handleEditMachine = (machine: Machine) => {
    setEditingMachine(machine);
    setShowModal(true);
  };

  const handleDeleteMachine = async (id: number) => {
    if (confirm('Bạn có chắc chắn muốn xóa máy này?')) {
      try {
        await machinesApi.delete(id);
        setMachines(machines.filter(m => m.id !== id));
      } catch (error) {
        console.error('Error deleting machine:', error);
        alert('Lỗi khi xóa máy');
      }
    }
  };

  const handleSaveMachine = async (formData: CreateMachineDto | UpdateMachineDto) => {
    try {
      if (editingMachine) {
        // Update existing machine
        const updatedMachine = await machinesApi.update(editingMachine.id, formData as UpdateMachineDto);
        setMachines(machines.map(m => m.id === editingMachine.id ? updatedMachine : m));
      } else {
        // Create new machine
        const newMachine = await machinesApi.create(formData as CreateMachineDto);
        setMachines([...machines, newMachine]);
      }
      setShowModal(false);
    } catch (error) {
      console.error('Error saving machine:', error);
      alert('Lỗi khi lưu máy');
    }
  };

  const getStatusBadgeClass = (status: string) => {
    switch (status) {
      case 'online':
        return 'bg-green-100 text-green-800';
      case 'offline':
        return 'bg-red-100 text-red-800';
      case 'maintenance':
        return 'bg-yellow-100 text-yellow-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  if (loading) {
    return (
      <DashboardLayout>
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-500"></div>
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout>
      <div className="p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold text-gray-900">Quản lý Máy móc</h1>
          <button
            onClick={handleAddMachine}
            className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg flex items-center gap-2 transition-colors"
          >
            <Plus size={20} />
            Thêm máy mới
          </button>
        </div>

        {/* Filters */}
        <div className="bg-white p-4 rounded-lg shadow mb-6">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1 relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={16} />
              <input
                type="text"
                placeholder="Tìm kiếm theo tên, IP, GMES..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
              />
            </div>
            <div className="sm:w-48 relative">
              <Filter className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={16} />
              <select
                value={filterStatus}
                onChange={(e) => setFilterStatus(e.target.value)}
                className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 appearance-none text-gray-900 bg-white"
              >
                <option value="all">Tất cả trạng thái</option>
                <option value="online">Online</option>
                <option value="offline">Offline</option>
                <option value="maintenance">Bảo trì</option>
              </select>
            </div>
          </div>
        </div>

        {/* Machines Table */}
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Tên máy
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Trạng thái
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Loại máy
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Trạm
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    IP Address
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    GMES Name
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Hành động
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredMachines.map((machine) => (
                  <tr key={machine.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="font-medium text-gray-900">{machine.name}</div>
                      {machine.programName && (
                        <div className="text-sm text-gray-500">{machine.programName}</div>
                      )}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusBadgeClass(machine.status)}`}>
                        {machine.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {getMachineTypeName(machine.machineTypeId)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {getStationName(machine.stationId)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {machine.ip || 'N/A'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {machine.gmesName || 'N/A'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                      <button
                        onClick={() => handleEditMachine(machine)}
                        className="text-indigo-600 hover:text-indigo-900 mr-3 inline-flex items-center gap-1"
                      >
                        <Edit size={16} />
                        Sửa
                      </button>
                      <button
                        onClick={() => handleDeleteMachine(machine.id)}
                        className="text-red-600 hover:text-red-900 inline-flex items-center gap-1"
                      >
                        <Trash2 size={16} />
                        Xóa
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          
          {filteredMachines.length === 0 && (
            <div className="text-center py-8">
              <p className="text-gray-500">Không tìm thấy máy nào.</p>
            </div>
          )}
        </div>

        {/* Machine Modal */}
        {showModal && (
          <MachineModal
            machine={editingMachine}
            machineTypes={machineTypes}
            stations={stations}
            onSave={handleSaveMachine}
            onClose={() => setShowModal(false)}
          />
        )}
      </div>
    </DashboardLayout>
  );
}

// Machine Modal Component
interface MachineModalProps {
  machine: Machine | null;
  machineTypes: MachineType[];
  stations: Station[];
  onSave: (data: CreateMachineDto | UpdateMachineDto) => void;
  onClose: () => void;
}

function MachineModal({ machine, machineTypes, stations, onSave, onClose }: MachineModalProps) {
  const [formData, setFormData] = useState({
    name: machine?.name || '',
    status: machine?.status || 'offline',
    machineTypeId: machine?.machineTypeId || undefined,
    stationId: machine?.stationId || undefined,
    ip: machine?.ip || '',
    gmesName: machine?.gmesName || '',
    programName: machine?.programName || '',
    macAddress: machine?.macAddress || ''
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave(formData);
  };

  return (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div className="mt-3">
          <h3 className="text-lg font-medium text-gray-900 mb-4">
            {machine ? 'Sửa máy' : 'Thêm máy mới'}
          </h3>
          
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700">Tên máy *</label>
              <input
                type="text"
                required
                value={formData.name}
                onChange={(e) => setFormData({...formData, name: e.target.value})}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">Trạng thái *</label>
              <select
                required
                value={formData.status}
                onChange={(e) => setFormData({...formData, status: e.target.value})}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 bg-white"
              >
                <option value="offline">Offline</option>
                <option value="online">Online</option>
                <option value="maintenance">Bảo trì</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">Loại máy</label>
              <select
                value={formData.machineTypeId || ''}
                onChange={(e) => setFormData({...formData, machineTypeId: e.target.value ? Number(e.target.value) : undefined})}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 bg-white"
              >
                <option value="">Chọn loại máy</option>
                {machineTypes.map(type => (
                  <option key={type.id} value={type.id}>
                    {type.name}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">Trạm</label>
              <select
                value={formData.stationId || ''}
                onChange={(e) => setFormData({...formData, stationId: e.target.value ? Number(e.target.value) : undefined})}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 bg-white"
              >
                <option value="">Chọn trạm</option>
                {stations.map(station => (
                  <option key={station.id} value={station.id}>
                    {station.name}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">IP Address</label>
              <input
                type="text"
                value={formData.ip}
                onChange={(e) => setFormData({...formData, ip: e.target.value})}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
                placeholder="192.168.1.100"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">GMES Name</label>
              <input
                type="text"
                value={formData.gmesName}
                onChange={(e) => setFormData({...formData, gmesName: e.target.value})}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">Program Name</label>
              <input
                type="text"
                value={formData.programName}
                onChange={(e) => setFormData({...formData, programName: e.target.value})}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">MAC Address</label>
              <input
                type="text"
                value={formData.macAddress}
                onChange={(e) => setFormData({...formData, macAddress: e.target.value})}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
                placeholder="00:1B:44:11:3A:B7"
              />
            </div>

            <div className="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                onClick={onClose}
                className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 rounded-md hover:bg-gray-300"
              >
                Hủy
              </button>
              <button
                type="submit"
                className="px-4 py-2 text-sm font-medium text-white bg-blue-500 rounded-md hover:bg-blue-600"
              >
                {machine ? 'Cập nhật' : 'Thêm mới'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}