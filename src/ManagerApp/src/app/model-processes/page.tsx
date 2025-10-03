'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { ModelProcess, ModelGroup, CreateModelProcessDto, UpdateModelProcessDto } from '@/types';
import { modelProcessesApi, modelGroupsApi } from '@/services/api';

export default function ModelProcessesPage() {
  const [modelProcesses, setModelProcesses] = useState<ModelProcess[]>([]);
  const [modelGroups, setModelGroups] = useState<ModelGroup[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingModelProcess, setEditingModelProcess] = useState<ModelProcess | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    modelGroupId: 0
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);
      const [modelProcessesData, modelGroupsData] = await Promise.all([
        modelProcessesApi.getAll(),
        modelGroupsApi.getAll()
      ]);
      setModelProcesses(modelProcessesData);
      setModelGroups(modelGroupsData);
    } catch (err) {
      setError('Không thể tải dữ liệu');
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const filteredModelProcesses = modelProcesses.filter(modelProcess =>
    modelProcess.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    modelProcess.modelGroup?.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleAdd = () => {
    setEditingModelProcess(null);
    setFormData({ name: '', modelGroupId: modelGroups[0]?.id || 0 });
    setShowModal(true);
  };

  const handleEdit = (modelProcess: ModelProcess) => {
    setEditingModelProcess(modelProcess);
    setFormData({
      name: modelProcess.name,
      modelGroupId: modelProcess.modelGroupId
    });
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      setError(null);
      
      if (editingModelProcess) {
        const updateData: UpdateModelProcessDto = {
          name: formData.name,
          modelGroupId: formData.modelGroupId
        };
        await modelProcessesApi.update(editingModelProcess.id, updateData);
      } else {
        const createData: CreateModelProcessDto = {
          name: formData.name,
          modelGroupId: formData.modelGroupId
        };
        await modelProcessesApi.create(createData);
      }
      
      setShowModal(false);
      await loadData();
    } catch (err) {
      setError(editingModelProcess ? 'Không thể cập nhật quy trình' : 'Không thể tạo quy trình');
      console.error('Error saving model process:', err);
    }
  };

  const handleDelete = async (id: number, name: string) => {
    if (window.confirm(`Bạn có chắc chắn muốn xóa quy trình "${name}"?`)) {
      try {
        setError(null);
        await modelProcessesApi.delete(id);
        await loadData();
      } catch (err) {
        setError('Không thể xóa quy trình');
        console.error('Error deleting model process:', err);
      }
    }
  };

  const resetForm = () => {
    setFormData({ name: '', modelGroupId: modelGroups[0]?.id || 0 });
    setEditingModelProcess(null);
    setError(null);
  };

  const handleModalClose = () => {
    setShowModal(false);
    resetForm();
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
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <h1 className="text-3xl font-bold text-gray-900">Quản lý Quy trình Model</h1>
          <button
            onClick={handleAdd}
            className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
          >
            Thêm Quy trình
          </button>
        </div>

        {/* Error Alert */}
        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
            {error}
          </div>
        )}

        {/* Search */}
        <div className="mb-4">
          <input
            type="text"
            placeholder="Tìm kiếm theo tên quy trình hoặc tên model group..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        {/* Table */}
        <div className="bg-white shadow overflow-hidden sm:rounded-md">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  ID
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Tên Quy trình
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Model Group
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Buyer
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Thao tác
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredModelProcesses.map((modelProcess) => (
                <tr key={modelProcess.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {modelProcess.id}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {modelProcess.name}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {modelProcess.modelGroup?.name || 'N/A'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {modelProcess.modelGroup?.buyer?.name || 'N/A'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <button
                      onClick={() => handleEdit(modelProcess)}
                      className="text-indigo-600 hover:text-indigo-900 mr-4"
                    >
                      Sửa
                    </button>
                    <button
                      onClick={() => handleDelete(modelProcess.id, modelProcess.name)}
                      className="text-red-600 hover:text-red-900"
                    >
                      Xóa
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {filteredModelProcesses.length === 0 && (
            <div className="text-center py-4 text-gray-500">
              Không tìm thấy quy trình nào
            </div>
          )}
        </div>

        {/* Modal */}
        {showModal && (
          <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
            <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
              <div className="mt-3">
                <h3 className="text-lg font-medium text-gray-900 mb-4">
                  {editingModelProcess ? 'Cập nhật Quy trình' : 'Thêm Quy trình Mới'}
                </h3>
                
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Tên Quy trình *
                    </label>
                    <input
                      type="text"
                      required
                      value={formData.name}
                      onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="Nhập tên quy trình"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Model Group *
                    </label>
                    <select
                      required
                      value={formData.modelGroupId}
                      onChange={(e) => setFormData({ ...formData, modelGroupId: parseInt(e.target.value) })}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                      <option value={0}>Chọn Model Group</option>
                      {modelGroups.map((group) => (
                        <option key={group.id} value={group.id}>
                          {group.name} ({group.buyer?.name})
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="flex justify-end space-x-2 pt-4">
                    <button
                      type="button"
                      onClick={handleModalClose}
                      className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 border border-gray-300 rounded-md hover:bg-gray-200 focus:outline-none focus:ring-2 focus:ring-gray-500"
                    >
                      Hủy
                    </button>
                    <button
                      type="submit"
                      className="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                      {editingModelProcess ? 'Cập nhật' : 'Tạo'}
                    </button>
                  </div>
                </form>
              </div>
            </div>
          </div>
        )}
      </div>
    </DashboardLayout>
  );
}