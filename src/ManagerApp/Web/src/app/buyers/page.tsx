'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { Buyer, CreateBuyerDto, UpdateBuyerDto } from '@/types';
import { buyersApi } from '@/services/api';
import { Plus, Edit, Trash2, Search, ShoppingCart, X } from 'lucide-react';

export default function BuyersPage() {
  const [buyers, setBuyers] = useState<Buyer[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingBuyer, setEditingBuyer] = useState<Buyer | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    code: '',
    name: ''
  });

  useEffect(() => {
    loadBuyers();
  }, []);

  const loadBuyers = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await buyersApi.getAll();
      setBuyers(data);
    } catch (err) {
      setError('Không thể tải danh sách buyers');
      console.error('Error loading buyers:', err);
    } finally {
      setLoading(false);
    }
  };

  const filteredBuyers = buyers.filter(buyer =>
    buyer.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    buyer.code.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleAdd = () => {
    setEditingBuyer(null);
    setFormData({ code: '', name: '' });
    setShowModal(true);
  };

  const handleEdit = (buyer: Buyer) => {
    setEditingBuyer(buyer);
    setFormData({ code: buyer.code, name: buyer.name });
    setShowModal(true);
  };

  const handleDelete = async (id: number) => {
    if (confirm('Bạn có chắc chắn muốn xóa buyer này?')) {
      try {
        setError(null);
        await buyersApi.delete(id);
        await loadBuyers();
      } catch (err) {
        setError('Không thể xóa buyer');
        console.error('Error deleting buyer:', err);
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setError(null);
      
      if (editingBuyer) {
        const updateData: UpdateBuyerDto = {
          code: formData.code || undefined,
          name: formData.name || undefined,
        };
        await buyersApi.update(editingBuyer.id, updateData);
      } else {
        const createData: CreateBuyerDto = {
          code: formData.code,
          name: formData.name,
        };
        await buyersApi.create(createData);
      }
      
      await loadBuyers();
      setShowModal(false);
      setFormData({ code: '', name: '' });
    } catch (err) {
      setError(editingBuyer ? 'Không thể cập nhật buyer' : 'Không thể tạo buyer mới');
      console.error('Error saving buyer:', err);
    }
  };

  const BuyerModal = () => (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center z-50">
      <div className="bg-white p-6 rounded-lg shadow-xl w-96">
        <h3 className="text-lg font-semibold mb-4 flex items-center gap-2">
          <ShoppingCart size={20} />
          {editingBuyer ? 'Sửa Buyer' : 'Thêm Buyer Mới'}
        </h3>
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Mã Buyer
            </label>
            <input
              type="text"
              value={formData.code}
              onChange={(e) => setFormData({ ...formData, code: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
              required
              placeholder="VD: BUY001"
            />
          </div>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Tên Buyer
            </label>
            <input
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
              required
              placeholder="VD: Samsung Electronics"
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
              {editingBuyer ? 'Cập nhật' : 'Thêm'}
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
            <h1 className="text-2xl font-bold text-gray-900">Quản lý Buyers</h1>
            <p className="text-gray-600">Quản lý danh sách các khách hàng</p>
          </div>
          <button
            onClick={handleAdd}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 flex items-center space-x-2 transition-colors"
          >
            <Plus size={18} />
            <span>Thêm Buyer</span>
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
            placeholder="Tìm kiếm theo tên hoặc mã buyer..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
          />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Tổng số Buyers</p>
            <p className="text-2xl font-bold text-gray-900">{buyers.length}</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Kết quả tìm kiếm</p>
            <p className="text-2xl font-bold text-gray-900">{filteredBuyers.length}</p>
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
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">ID</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Mã</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tên Buyer</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Thao tác</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredBuyers.map((buyer) => (
                <tr key={buyer.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{buyer.id}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{buyer.code}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{buyer.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(buyer)}
                      className="text-blue-600 hover:text-blue-900 mr-3 inline-flex items-center gap-1 transition-colors"
                    >
                      <Edit size={16} />
                      Sửa
                    </button>
                    <button
                      onClick={() => handleDelete(buyer.id)}
                      className="text-red-600 hover:text-red-900 inline-flex items-center gap-1 transition-colors"
                    >
                      <Trash2 size={16} />
                      Xóa
                    </button>
                  </td>
                </tr>
              ))}
              {filteredBuyers.length === 0 && (
                <tr>
                  <td colSpan={4} className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-center">
                    Không tìm thấy buyer nào
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {showModal && <BuyerModal />}
      </div>
    </DashboardLayout>
  );
}