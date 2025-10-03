'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { Line, CreateLineDto, UpdateLineDto } from '@/types';
import { linesApi } from '@/services/api';
import { Plus, Edit, Trash2, Search, Truck, X } from 'lucide-react';

export default function LinesPage() {
  const [lines, setLines] = useState<Line[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingLine, setEditingLine] = useState<Line | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    name: ''
  });

  useEffect(() => {
    loadLines();
  }, []);

  const loadLines = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await linesApi.getAll();
      setLines(data);
    } catch (err) {
      setError('Không thể tải danh sách dây chuyền');
      console.error('Error loading lines:', err);
    } finally {
      setLoading(false);
    }
  };

  const filteredLines = lines.filter(line =>
    line.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleAdd = () => {
    setEditingLine(null);
    setFormData({ name: '' });
    setShowModal(true);
  };

  const handleEdit = (line: Line) => {
    setEditingLine(line);
    setFormData({ name: line.name });
    setShowModal(true);
  };

  const handleDelete = async (id: number) => {
    if (confirm('Bạn có chắc chắn muốn xóa line này?')) {
      try {
        setError(null);
        await linesApi.delete(id);
        await loadLines();
      } catch (err) {
        setError('Không thể xóa dây chuyền');
        console.error('Error deleting line:', err);
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setError(null);
      
      if (editingLine) {
        const updateData: UpdateLineDto = {
          name: formData.name || undefined,
        };
        await linesApi.update(editingLine.id, updateData);
      } else {
        const createData: CreateLineDto = {
          name: formData.name,
        };
        await linesApi.create(createData);
      }
      
      await loadLines();
      setShowModal(false);
      setFormData({ name: '' });
    } catch (err) {
      setError(editingLine ? 'Không thể cập nhật dây chuyền' : 'Không thể tạo dây chuyền mới');
      console.error('Error saving line:', err);
    }
  };

  const LineModal = () => (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center z-50">
      <div className="bg-white p-6 rounded-lg shadow-xl w-96">
        <h3 className="text-lg font-semibold mb-4 flex items-center gap-2">
          <Truck size={20} />
          {editingLine ? 'Sửa Line' : 'Thêm Line Mới'}
        </h3>
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Tên Line
            </label>
            <input
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
              required
              placeholder="VD: Line 1 - Assembly"
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
              {editingLine ? 'Cập nhật' : 'Thêm'}
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
            <h1 className="text-2xl font-bold text-gray-900">Quản lý Lines</h1>
            <p className="text-gray-600">Quản lý các line sản xuất</p>
          </div>
          <button
            onClick={handleAdd}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 flex items-center space-x-2 transition-colors"
          >
            <Plus size={18} />
            <span>Thêm Line</span>
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
            placeholder="Tìm kiếm theo tên line..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-gray-900 placeholder-gray-500 bg-white"
          />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Tổng số Lines</p>
            <p className="text-2xl font-bold text-gray-900">{lines.length}</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Kết quả tìm kiếm</p>
            <p className="text-2xl font-bold text-gray-900">{filteredLines.length}</p>
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
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tên Line</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Thao tác</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredLines.map((line) => (
                <tr key={line.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {line.id}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {line.name}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(line)}
                      className="text-blue-600 hover:text-blue-900 mr-3 inline-flex items-center gap-1 transition-colors"
                    >
                      <Edit size={16} />
                      Sửa
                    </button>
                    <button
                      onClick={() => handleDelete(line.id)}
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
        </div>
      </div>

      {showModal && <LineModal />}
    </DashboardLayout>
  );
}