'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { Station, Line, ModelProcess, CreateStationDto, UpdateStationDto } from '@/types';
import { stationsApi, linesApi, modelProcessesApi } from '@/services/api';

export default function StationsPage() {
  const [stations, setStations] = useState<Station[]>([]);
  const [lines, setLines] = useState<Line[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingStation, setEditingStation] = useState<Station | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    lineId: 0,
    modelProcessId: 1
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);
      const [stationsData, linesData] = await Promise.all([
        stationsApi.getAll(),
        linesApi.getAll()
      ]);
      setStations(stationsData);
      setLines(linesData);
    } catch (err) {
      setError('Không thể tải dữ liệu');
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const filteredStations = stations.filter(station =>
    station.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleAdd = () => {
    setEditingStation(null);
    setFormData({ name: '', lineId: lines[0]?.id || 0, modelProcessId: 1 });
    setShowModal(true);
  };

  const handleEdit = (station: Station) => {
    setEditingStation(station);
    setFormData({ 
      name: station.name, 
      lineId: station.lineId || 0, 
      modelProcessId: station.modelProcessId || 1 
    });
    setShowModal(true);
  };

  const handleDelete = async (id: number) => {
    if (confirm('Bạn có chắc chắn muốn xóa station này?')) {
      try {
        setError(null);
        await stationsApi.delete(id);
        await loadData();
      } catch (err) {
        setError('Không thể xóa station');
        console.error('Error deleting station:', err);
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setError(null);
      
      if (editingStation) {
        const updateData: UpdateStationDto = {
          name: formData.name || undefined,
          lineId: formData.lineId || undefined,
          modelProcessId: formData.modelProcessId || undefined,
        };
        await stationsApi.update(editingStation.id, updateData);
      } else {
        const createData: CreateStationDto = {
          name: formData.name,
          lineId: formData.lineId,
          modelProcessId: formData.modelProcessId,
        };
        await stationsApi.create(createData);
      }
      
      await loadData();
      setShowModal(false);
      setFormData({ name: '', lineId: lines[0]?.id || 0, modelProcessId: 1 });
    } catch (err) {
      setError(editingStation ? 'Không thể cập nhật station' : 'Không thể tạo station mới');
      console.error('Error saving station:', err);
    }
  };

  const StationModal = () => (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center z-50">
      <div className="bg-white p-6 rounded-lg shadow-xl w-96">
        <h3 className="text-lg font-semibold mb-4">
          {editingStation ? 'Sửa Station' : 'Thêm Station Mới'}
        </h3>
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Tên Station
            </label>
            <input
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
              placeholder="VD: Station A"
            />
          </div>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Line
            </label>
            <select
              value={formData.lineId}
              onChange={(e) => setFormData({ ...formData, lineId: parseInt(e.target.value) })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
            >
              <option value="">Chọn Line</option>
              {lines.map(line => (
                <option key={line.id} value={line.id}>
                  {line.name}
                </option>
              ))}
            </select>
          </div>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Model Process ID
            </label>
            <input
              type="number"
              value={formData.modelProcessId}
              onChange={(e) => setFormData({ ...formData, modelProcessId: parseInt(e.target.value) })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
              min="1"
            />
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
              {editingStation ? 'Cập nhật' : 'Thêm'}
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
            <h1 className="text-2xl font-bold text-gray-900">Quản lý Stations</h1>
            <p className="text-gray-600">Quản lý các station trong line</p>
          </div>
          <button
            onClick={handleAdd}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 flex items-center space-x-2"
          >
            <span>+</span>
            <span>Thêm Station</span>
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
            placeholder="Tìm kiếm theo tên station..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Tổng số Stations</p>
            <p className="text-2xl font-bold text-gray-900">{stations.length}</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Kết quả tìm kiếm</p>
            <p className="text-2xl font-bold text-gray-900">{filteredStations.length}</p>
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
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tên Station</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Line ID</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Model Process ID</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Thao tác</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredStations.map((station) => (
                <tr key={station.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{station.id}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{station.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{station.lineId}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{station.modelProcessId}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(station)}
                      className="text-blue-600 hover:text-blue-900 mr-3"
                    >
                      Sửa
                    </button>
                    <button
                      onClick={() => handleDelete(station.id)}
                      className="text-red-600 hover:text-red-900"
                    >
                      Xóa
                    </button>
                  </td>
                </tr>
              ))}
              {filteredStations.length === 0 && (
                <tr>
                  <td colSpan={5} className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-center">
                    Không tìm thấy station nào
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {showModal && <StationModal />}
      </div>
    </DashboardLayout>
  );
}