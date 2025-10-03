'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { Model, ModelGroup, CreateModelDto, UpdateModelDto } from '@/types';
import { modelsApi, modelGroupsApi } from '@/services/api';

export default function ModelsPage() {
  const [models, setModels] = useState<Model[]>([]);
  const [modelGroups, setModelGroups] = useState<ModelGroup[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingModel, setEditingModel] = useState<Model | null>(null);
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
      const [modelsData, modelGroupsData] = await Promise.all([
        modelsApi.getAll(),
        modelGroupsApi.getAll()
      ]);
      setModels(modelsData);
      setModelGroups(modelGroupsData);
    } catch (err) {
      setError('Không thể tải dữ liệu');
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const filteredModels = models.filter(model =>
    model.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getModelGroupName = (modelGroupId?: number) => {
    if (!modelGroupId) return 'N/A';
    const modelGroup = modelGroups.find(mg => mg.id === modelGroupId);
    return modelGroup?.name || `Group ${modelGroupId}`;
  };

  const handleAdd = () => {
    setEditingModel(null);
    setFormData({ name: '', modelGroupId: modelGroups[0]?.id || 0 });
    setShowModal(true);
  };

  const handleEdit = (model: Model) => {
    setEditingModel(model);
    setFormData({ name: model.name, modelGroupId: model.modelGroupId });
    setShowModal(true);
  };

  const handleDelete = async (id: number) => {
    if (confirm('Bạn có chắc chắn muốn xóa model này?')) {
      try {
        setError(null);
        await modelsApi.delete(id);
        await loadData();
      } catch (err) {
        setError('Không thể xóa model');
        console.error('Error deleting model:', err);
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setError(null);
      
      if (editingModel) {
        const updateData: UpdateModelDto = {
          name: formData.name || undefined,
          modelGroupId: formData.modelGroupId || undefined,
        };
        await modelsApi.update(editingModel.id, updateData);
      } else {
        const createData: CreateModelDto = {
          name: formData.name,
          modelGroupId: formData.modelGroupId,
        };
        await modelsApi.create(createData);
      }
      
      await loadData();
      setShowModal(false);
      setFormData({ name: '', modelGroupId: modelGroups[0]?.id || 0 });
    } catch (err) {
      setError(editingModel ? 'Không thể cập nhật model' : 'Không thể tạo model mới');
      console.error('Error saving model:', err);
    }
  };

  const ModelModal = () => (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center z-50">
      <div className="bg-white p-6 rounded-lg shadow-xl w-96">
        <h3 className="text-lg font-semibold mb-4">
          {editingModel ? 'Sửa Model' : 'Thêm Model Mới'}
        </h3>
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Tên Model
            </label>
            <input
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
              placeholder="VD: Model A1"
            />
          </div>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Model Group
            </label>
            <select
              value={formData.modelGroupId}
              onChange={(e) => setFormData({ ...formData, modelGroupId: parseInt(e.target.value) })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
            >
              <option value="">Chọn Model Group</option>
              {modelGroups.map(group => (
                <option key={group.id} value={group.id}>
                  {group.name}
                </option>
              ))}
            </select>
          </div>
          <div className="flex justify-end space-x-2">
            <button
              type="button"
              onClick={() => setShowModal(false)}
              className="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50"
            >
              Hủy
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
            >
              {editingModel ? 'Cập nhật' : 'Thêm'}
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
            <h1 className="text-2xl font-bold text-gray-900">Quản lý Models</h1>
            <p className="text-gray-600">Quản lý các model sản phẩm</p>
          </div>
          <button
            onClick={handleAdd}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 flex items-center space-x-2"
          >
            <span>+</span>
            <span>Thêm Model</span>
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
            placeholder="Tìm kiếm theo tên model..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
          />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Tổng số Models</p>
            <p className="text-2xl font-bold text-gray-900">{models.length}</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Kết quả tìm kiếm</p>
            <p className="text-2xl font-bold text-gray-900">{filteredModels.length}</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Model Groups</p>
            <p className="text-2xl font-bold text-gray-900">2</p>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">ID</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tên Model</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Model Group</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Thao tác</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredModels.map((model) => (
                <tr key={model.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {model.id}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {model.name}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                      {getModelGroupName(model.modelGroupId)}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(model)}
                      className="text-blue-600 hover:text-blue-900 mr-3"
                    >
                      Sửa
                    </button>
                    <button
                      onClick={() => handleDelete(model.id)}
                      className="text-red-600 hover:text-red-900"
                    >
                      Xóa
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {showModal && <ModelModal />}
    </DashboardLayout>
  );
}