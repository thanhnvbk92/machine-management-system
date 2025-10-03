'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { Machine, MachineType, Station } from '@/types';

export default function MachinesPage() {
  const [machines, setMachines] = useState<Machine[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingMachine, setEditingMachine] = useState<Machine | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [filterStatus, setFilterStatus] = useState('all');

  // Mock data - replace with actual API calls
  useEffect(() => {
    const mockMachines: Machine[] = [
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
      },
      {
        id: 3,
        name: 'DRILL-002',
        status: 'maintenance',
        ip: '192.168.1.102',
        gmesName: 'DRILL_MACHINE_02',
        machineTypeId: 1,
        stationId: 3,
        programName: 'DRILL_CYCLE',
        macAddress: '00:1B:44:11:3A:B9'
      }
    ];
    setMachines(mockMachines);
    setLoading(false);
  }, []);

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

  const handleDeleteMachine = (id: number) => {
    if (confirm('Bạn có chắc chắn muốn xóa máy này?')) {
      setMachines(machines.filter(m => m.id !== id));
    }
  };

  const handleSaveMachine = (machineData: Partial<Machine>) => {
    if (editingMachine) {
      // Update existing machine
      setMachines(machines.map(m => 
        m.id === editingMachine.id ? { ...m, ...machineData } : m
      ));
    } else {
      // Add new machine
      const newMachine: Machine = {
        id: Date.now(), // Use timestamp as ID for demo
        name: '',
        status: 'offline',
        ...machineData
      };
      setMachines([...machines, newMachine]);
    }
    setShowModal(false);
    setEditingMachine(null);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'online': return 'bg-green-100 text-green-800';
      case 'offline': return 'bg-red-100 text-red-800';
      case 'maintenance': return 'bg-yellow-100 text-yellow-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'online': return 'Hoạt động';
      case 'offline': return 'Offline';
      case 'maintenance': return 'Bảo trì';
      default: return 'Không xác định';
    }
  };

  if (loading) {
    return (
      <DashboardLayout>
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout>
      <div className="space-y-6">
        {/* Page Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Quản lý máy móc</h1>
            <p className="text-gray-600">Thêm, sửa, xóa thông tin máy móc trong hệ thống</p>
          </div>
          <button
            onClick={handleAddMachine}
            className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium"
          >
            ➕ Thêm máy mới
          </button>
        </div>

        {/* Filters and Search */}
        <div className="bg-white rounded-lg shadow p-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Tìm kiếm
              </label>
              <input
                type="text"
                placeholder="Tên máy, IP, GMES..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Trạng thái
              </label>
              <select
                value={filterStatus}
                onChange={(e) => setFilterStatus(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="all">Tất cả</option>
                <option value="online">Hoạt động</option>
                <option value="offline">Offline</option>
                <option value="maintenance">Bảo trì</option>
              </select>
            </div>
            <div className="flex items-end">
              <div className="text-sm text-gray-600">
                Hiển thị {filteredMachines.length} / {machines.length} máy
              </div>
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
                    IP Address
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    GMES Name
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Trạng thái
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Program
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredMachines.map((machine) => (
                  <tr key={machine.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm font-medium text-gray-900">{machine.name}</div>
                      <div className="text-sm text-gray-500">{machine.macAddress}</div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {machine.ip}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {machine.gmesName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(machine.status)}`}>
                        {getStatusText(machine.status)}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {machine.programName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <button
                        onClick={() => handleEditMachine(machine)}
                        className="text-blue-600 hover:text-blue-900 mr-3"
                      >
                        ✏️ Sửa
                      </button>
                      <button
                        onClick={() => handleDeleteMachine(machine.id)}
                        className="text-red-600 hover:text-red-900"
                      >
                        🗑️ Xóa
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        {filteredMachines.length === 0 && (
          <div className="text-center py-12 bg-white rounded-lg shadow">
            <div className="text-gray-400 text-lg mb-2">🔍</div>
            <div className="text-gray-500">Không tìm thấy máy nào phù hợp</div>
          </div>
        )}
      </div>

      {/* Modal for Add/Edit Machine */}
      {showModal && (
        <MachineModal
          machine={editingMachine}
          onSave={handleSaveMachine}
          onClose={() => {
            setShowModal(false);
            setEditingMachine(null);
          }}
        />
      )}
    </DashboardLayout>
  );
}

// Machine Modal Component
interface MachineModalProps {
  machine: Machine | null;
  onSave: (machine: Partial<Machine>) => void;
  onClose: () => void;
}

function MachineModal({ machine, onSave, onClose }: MachineModalProps) {
  const [formData, setFormData] = useState({
    name: machine?.name || '',
    ip: machine?.ip || '',
    gmesName: machine?.gmesName || '',
    status: machine?.status || 'offline',
    programName: machine?.programName || '',
    macAddress: machine?.macAddress || ''
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave(formData);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md">
        <h3 className="text-lg font-semibold mb-4">
          {machine ? 'Chỉnh sửa máy' : 'Thêm máy mới'}
        </h3>
        
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Tên máy *
            </label>
            <input
              type="text"
              required
              value={formData.name}
              onChange={(e) => setFormData({...formData, name: e.target.value})}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              IP Address
            </label>
            <input
              type="text"
              value={formData.ip}
              onChange={(e) => setFormData({...formData, ip: e.target.value})}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              GMES Name
            </label>
            <input
              type="text"
              value={formData.gmesName}
              onChange={(e) => setFormData({...formData, gmesName: e.target.value})}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Trạng thái
            </label>
            <select
              value={formData.status}
              onChange={(e) => setFormData({...formData, status: e.target.value})}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="online">Hoạt động</option>
              <option value="offline">Offline</option>
              <option value="maintenance">Bảo trì</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Program Name
            </label>
            <input
              type="text"
              value={formData.programName}
              onChange={(e) => setFormData({...formData, programName: e.target.value})}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              MAC Address
            </label>
            <input
              type="text"
              value={formData.macAddress}
              onChange={(e) => setFormData({...formData, macAddress: e.target.value})}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <div className="flex justify-end space-x-3 pt-4">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50"
            >
              Hủy
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
            >
              {machine ? 'Cập nhật' : 'Thêm mới'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}