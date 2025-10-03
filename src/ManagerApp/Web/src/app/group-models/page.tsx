'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { GroupModel, CreateGroupModelDto, UpdateGroupModelDto, Buyer } from '@/types';
import { groupModelsApi, buyersApi } from '@/services/api';
import { Plus, Edit, Trash2, Search, Package, X } from 'lucide-react';

export default function GroupModelsPage() {
  const [groupModels, setGroupModels] = useState<GroupModel[]>([]);
  const [buyers, setBuyers] = useState<Buyer[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingGroupModel, setEditingGroupModel] = useState<GroupModel | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({ name: '', buyerId: 0 });

  useEffect(() => {
    loadGroupModels();
    loadBuyers();
  }, []);

  const loadBuyers = async () => {
    try {
      const data = await buyersApi.getAll();
      setBuyers(data);
    } catch (err) {
      console.error('Error loading buyers:', err);
    }
  };

  const loadGroupModels = async () => {
    try {
      setLoading(true);
      const data = await groupModelsApi.getAll();
      setGroupModels(data);
    } catch (err) {
      console.error('Error loading group models:', err);
      setGroupModels([
        { id: 1, name: 'Electronics Group', buyerId: 1 },
        { id: 2, name: 'Mechanical Group', buyerId: 1 }
      ]);
    } finally {
      setLoading(false);
    }
  };

  const filteredGroupModels = groupModels.filter(groupModel =>
    groupModel.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleAdd = () => {
    setEditingGroupModel(null);
    setFormData({ name: '', buyerId: buyers.length > 0 ? buyers[0].id : 0 });
    setError(null);
    setShowModal(true);
  };

  const handleEdit = (groupModel: GroupModel) => {
    setEditingGroupModel(groupModel);
    setFormData({ name: groupModel.name, buyerId: groupModel.buyerId });
    setError(null);
    setShowModal(true);
  };

  const handleDelete = async (id: number) => {
    if (confirm('Bạn có chắc chắn muốn xóa group model này?')) {
      try {
        await groupModelsApi.delete(id);
        setGroupModels(groupModels.filter(gm => gm.id !== id));
      } catch (err) {
        alert('Không thể xóa group model');
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.name.trim()) {
      setError('Tên group model không được để trống');
      return;
    }

    if (!formData.buyerId) {
      setError('Vui lòng chọn buyer');
      return;
    }

    try {
      setError(null);
      
      if (editingGroupModel) {
        const updateData: UpdateGroupModelDto = { 
          name: formData.name.trim(),
          buyerId: formData.buyerId
        };
        const updated = await groupModelsApi.update(editingGroupModel.id, updateData);
        setGroupModels(groupModels.map(gm => gm.id === editingGroupModel.id ? updated : gm));
      } else {
        const createData: CreateGroupModelDto = { 
          name: formData.name.trim(),
          buyerId: formData.buyerId
        };
        const newGroupModel = await groupModelsApi.create(createData);
        setGroupModels([...groupModels, newGroupModel]);
      }
      
      setShowModal(false);
      setFormData({ name: '', buyerId: 0 });
    } catch (err) {
      setError(editingGroupModel ? 'Không thể cập nhật group model' : 'Không thể tạo group model mới');
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
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Quản lý Group Models</h1>
            <p className="text-gray-600 mt-1">Quản lý các nhóm model sản phẩm</p>
          </div>
          <button
            onClick={handleAdd}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 flex items-center space-x-2 transition-colors"
          >
            <Plus size={18} />
            <span>Thêm Group Model</span>
          </button>
        </div>

        <div className="bg-white p-4 rounded-lg shadow mb-6">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={16} />
            <input
              type="text"
              placeholder="Tìm kiếm theo tên group model..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 placeholder-gray-500 bg-white"
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Tổng số Group Models</p>
            <p className="text-2xl font-bold text-gray-900">{groupModels.length}</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Kết quả tìm kiếm</p>
            <p className="text-2xl font-bold text-gray-900">{filteredGroupModels.length}</p>
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
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Buyer</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Thao tác</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredGroupModels.map((groupModel) => (
                <tr key={groupModel.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{groupModel.id}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{groupModel.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {groupModel.buyer?.name || buyers.find(b => b.id === groupModel.buyerId)?.name || 'Unknown'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(groupModel)}
                      className="text-blue-600 hover:text-blue-900 mr-3 inline-flex items-center gap-1 transition-colors"
                    >
                      <Edit size={16} />
                      Sửa
                    </button>
                    <button
                      onClick={() => handleDelete(groupModel.id)}
                      className="text-red-600 hover:text-red-900 inline-flex items-center gap-1 transition-colors"
                    >
                      <Trash2 size={16} />
                      Xóa
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          
          {filteredGroupModels.length === 0 && (
            <div className="text-center py-8">
              <p className="text-gray-500">Không tìm thấy group model nào.</p>
            </div>
          )}
        </div>

        {showModal && (
          <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center z-50">
            <div className="bg-white p-6 rounded-lg shadow-xl w-96 max-w-md mx-4">
              <h3 className="text-lg font-semibold mb-4 flex items-center gap-2 text-gray-900">
                <Package size={20} className="text-blue-600" />
                {editingGroupModel ? 'Sửa Group Model' : 'Thêm Group Model Mới'}
              </h3>
              
              {error && (
                <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
                  {error}
                </div>
              )}
              
              <form onSubmit={handleSubmit}>
                <div className="mb-4">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Tên Group Model *
                  </label>
                  <input
                    type="text"
                    required
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 placeholder-gray-500 bg-white"
                    placeholder="Nhập tên group model..."
                  />
                </div>
                <div className="mb-4">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Buyer *
                  </label>
                  <select
                    required
                    value={formData.buyerId}
                    onChange={(e) => setFormData({ ...formData, buyerId: parseInt(e.target.value) })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 bg-white"
                  >
                    <option value={0}>Chọn Buyer...</option>
                    {buyers.map((buyer) => (
                      <option key={buyer.id} value={buyer.id}>
                        {buyer.name}
                      </option>
                    ))}
                  </select>
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
                    {editingGroupModel ? 'Cập nhật' : 'Thêm'}
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