'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { MachineType, CreateMachineTypeDto, UpdateMachineTypeDto } from '@/types';
import { machineTypesApi } from '@/services/api';
import { Plus, Edit, Trash2, Search, Settings, X } from 'lucide-react';

export default function MachineTypesPage() {
  const [machineTypes, setMachineTypes] = useState<MachineType[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingMachineType, setEditingMachineType] = useState<MachineType | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    name: ''
  });

  useEffect(() => {
    loadMachineTypes();
  }, []);

  const loadMachineTypes = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await machineTypesApi.getAll();
      setMachineTypes(data);
    } catch (err) {
      setError('Không thể tải danh sách machine types');
      console.error('Error loading machine types:', err);
      setMachineTypes([
        { id: 1, name: 'CNC Machine' },
        { id: 2, name: 'Laser Cutter' }
      ]);
    } finally {
      setLoading(false);
    }
  };

  const filteredMachineTypes = machineTypes.filter(machineType =>
    machineType.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleAdd = () => {
    setEditingMachineType(null);
    setFormData({ name: '' });
    setShowModal(true);
  };

  const handleEdit = (machineType: MachineType) => {
    setEditingMachineType(machineType);
    setFormData({ name: machineType.name });
    setShowModal(true);
  };

  const handleDelete = async (id: number) => {
    if (confirm('Bạn có chắc chắn muốn xóa machine type này?')) {
      try {
        setError(null);
        await machineTypesApi.delete(id);
        await loadMachineTypes();
      } catch (err) {
        setError('Không thể xóa machine type');
        console.error('Error deleting machine type:', err);
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setError(null);
      
      if (editingMachineType) {
        const updateData: UpdateMachineTypeDto = {
          name: formData.name || undefined,
        };
        await machineTypesApi.update(editingMachineType.id, updateData);
      } else {
        const createData: CreateMachineTypeDto = {
          name: formData.name,
        };
        await machineTypesApi.create(createData);
      }
      
      await loadMachineTypes();
      setShowModal(false);
      setFormData({ name: '' });
    } catch (err) {
      setError(editingMachineType ? 'Không thể cập nhật machine type' : 'Không thể tạo machine type mới');
      console.error('Error saving machine type:', err);
    }
  };

  const BuyerModal = () => (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center z-50">
      <div className="bg-white p-6 rounded-lg shadow-xl w-96">
        <h3 className="text-lg font-semibold mb-4 flex items-center gap-2 text-gray-900">
          <Settings size={20} className="text-blue-600" />
          {editingMachineType ? 'Sửa Machine Type' : 'Thêm Machine Type Mới'}
        </h3>
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Tên Machine Type *
            </label>
            <input
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 placeholder-gray-500 bg-white"
              required
              placeholder="Nhập tên machine type..."
            />
          </div>
          <div className="flex justify-end space-x-2">
            <button
              type="button"
              onClick={() => setShowModal(false)}
              className="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50 inline-flex items-center gap-1 transition-colors"
            >
              <X size={16} />
              Hủy
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 inline-flex items-center gap-1 transition-colors"
            >
              <Plus size={16} />
              {editingMachineType ? 'Cập nhật' : 'Thêm'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );

  if (loading) {
    return (
      <DashboardLayout>
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Quản lý Machine Types</h1>
            <p className="text-gray-600 mt-1">Quản lý các loại máy móc</p>
          </div>
          <button
            onClick={handleAdd}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 flex items-center space-x-2 transition-colors"
          >
            <Plus size={18} />
            <span>Thêm Machine Type</span>
          </button>
        </div>

        {error && (
          <div className="bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded-lg">
            {error}
          </div>
        )}

        <div className="bg-white p-4 rounded-lg shadow">
          <input
            type="text"
            placeholder="Tìm kiếm theo tên machine type..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900"
          />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Tổng số Machine Types</p>
            <p className="text-2xl font-bold text-gray-900">{machineTypes.length}</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Kết quả tìm kiếm</p>
            <p className="text-2xl font-bold text-gray-900">{filteredMachineTypes.length}</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Hoạt động</p>
            <p className="text-2xl font-bold text-gray-900">100%</p>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Tên</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Thao tác</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredMachineTypes.map((machineType) => (
                <tr key={machineType.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{machineType.id}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{machineType.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(machineType)}
                      className="text-blue-600 hover:text-blue-900 mr-3 inline-flex items-center gap-1 transition-colors"
                    >
                      <Edit size={16} />
                      Sửa
                    </button>
                    <button
                      onClick={() => handleDelete(machineType.id)}
                      className="text-red-600 hover:text-red-900 inline-flex items-center gap-1 transition-colors"
                    >
                      <Trash2 size={16} />
                      Xóa
                    </button>
                  </td>
                </tr>
              ))}
              {filteredMachineTypes.length === 0 && (
                <tr>
                  <td colSpan={3} className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-center">
                    Không tìm thấy machine type nào
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {showModal && (
          <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center z-50">
            <div className="bg-white p-6 rounded-lg shadow-xl w-96 max-w-md mx-4">
              <h3 className="text-lg font-semibold mb-4 flex items-center gap-2 text-gray-900">
                <Settings size={20} className="text-blue-600" />
                {editingMachineType ? 'Sửa Machine Type' : 'Thêm Machine Type Mới'}
              </h3>
              
              {error && (
                <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
                  {error}
                </div>
              )}
              
              <form onSubmit={handleSubmit}>
                <div className="mb-4">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Tên Machine Type *
                  </label>
                  <input
                    type="text"
                    required
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 placeholder-gray-500 bg-white"
                    placeholder="Nhập tên machine type..."
                  />
                </div>
                <div className="flex justify-end space-x-2">
                  <button
                    type="button"
                    onClick={() => setShowModal(false)}
                    className="px-4 py-2 text-gray-700 bg-gray-100 border border-gray-300 rounded-md hover:bg-gray-200 transition-colors inline-flex items-center gap-1"
                  >
                    <X size={16} />
                    Hủy
                  </button>
                  <button
                    type="submit"
                    className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors inline-flex items-center gap-1"
                  >
                    <Plus size={16} />
                    {editingMachineType ? 'Cập nhật' : 'Thêm'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </DashboardLayout>
  );
}