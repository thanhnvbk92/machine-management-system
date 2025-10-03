'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { Station, Line, ModelProcess, CreateStationDto, UpdateStationDto } from '@/types';
import { stationsApi, linesApi, modelProcessesApi } from '@/services/api';
import { Plus, Edit, Trash2, Search, MapPin, X } from 'lucide-react';

export default function StationsPage() {
  const [stations, setStations] = useState<Station[]>([]);
  const [lines, setLines] = useState<Line[]>([]);
  const [modelProcesses, setModelProcesses] = useState<ModelProcess[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingStation, setEditingStation] = useState<Station | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    lineId: 0,
    modelProcessId: 0
  });

  const filteredStations = stations.filter(station =>
    station.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);
      const [stationsData, linesData, modelProcessesData] = await Promise.all([
        stationsApi.getAll(),
        linesApi.getAll(),
        modelProcessesApi.getAll()
      ]);
      setStations(stationsData);
      setLines(linesData);
      setModelProcesses(modelProcessesData);
    } catch (err) {
      setError('Cannot load data');
      console.error('Error loading data:', err);
      setStations([
        { id: 1, name: 'Station A', modelProcessId: 1, lineId: 1 },
        { id: 2, name: 'Station B', modelProcessId: 1, lineId: 1 }
      ]);
      setLines([
        { id: 1, name: 'Production Line 1' },
        { id: 2, name: 'Production Line 2' }
      ]);
      setModelProcesses([
        { id: 1, name: 'Process 1', modelGroupId: 1, stationCount: 2, createdAt: new Date().toISOString(), isActive: true },
        { id: 2, name: 'Process 2', modelGroupId: 1, stationCount: 1, createdAt: new Date().toISOString(), isActive: true }
      ]);
    } finally {
      setLoading(false);
    }
  };

  const getLineName = (lineId?: number) => {
    if (!lineId) return 'N/A';
    const line = lines.find(l => l.id === lineId);
    return line?.name || `Line ${lineId}`;
  };

  const getModelProcessName = (modelProcessId?: number) => {
    if (!modelProcessId) return 'N/A';
    const process = modelProcesses.find(mp => mp.id === modelProcessId);
    return process?.name || `Process ${modelProcessId}`;
  };

  const handleAdd = () => {
    setEditingStation(null);
    setFormData({ 
      name: '', 
      lineId: lines.length > 0 ? lines[0].id : 0, 
      modelProcessId: modelProcesses.length > 0 ? modelProcesses[0].id : 0 
    });
    setError(null);
    setShowModal(true);
  };

  const handleEdit = (station: Station) => {
    setEditingStation(station);
    setFormData({
      name: station.name,
      lineId: station.lineId || 0,
      modelProcessId: station.modelProcessId || 0
    });
    setError(null);
    setShowModal(true);
  };

  const handleDelete = async (id: number) => {
    if (confirm('Are you sure you want to delete this station?')) {
      try {
        setError(null);
        await stationsApi.delete(id);
        setStations(stations.filter(s => s.id !== id));
      } catch (err) {
        setError('Cannot delete station');
        console.error('Error deleting station:', err);
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.name.trim()) {
      setError('Station name cannot be empty');
      return;
    }

    if (!formData.lineId) {
      setError('Please select a line');
      return;
    }

    if (!formData.modelProcessId) {
      setError('Please select a model process');
      return;
    }

    try {
      setError(null);

      if (editingStation) {
        const updateData: UpdateStationDto = {
          name: formData.name.trim(),
          lineId: formData.lineId,
          modelProcessId: formData.modelProcessId,
        };
        const updated = await stationsApi.update(editingStation.id, updateData);
        setStations(stations.map(s => s.id === editingStation.id ? updated : s));
      } else {
        const createData: CreateStationDto = {
          name: formData.name.trim(),
          lineId: formData.lineId,
          modelProcessId: formData.modelProcessId,
        };
        const newStation = await stationsApi.create(createData);
        setStations([...stations, newStation]);
      }

      setShowModal(false);
      setFormData({ name: '', lineId: 0, modelProcessId: 0 });
    } catch (err) {
      setError(editingStation ? 'Cannot update station' : 'Cannot create new station');
      console.error('Error saving station:', err);
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
            <h1 className="text-3xl font-bold text-gray-900">Stations Management</h1>
            <p className="text-gray-600 mt-1">Manage stations in production lines</p>
          </div>
          <button
            onClick={handleAdd}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 flex items-center space-x-2 transition-colors"
          >
            <Plus size={18} />
            <span>Add Station</span>
          </button>
        </div>

        {error && (
          <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
            {error}
          </div>
        )}

        <div className="bg-white p-4 rounded-lg shadow mb-6">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={16} />
            <input
              type="text"
              placeholder="Search by station name..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 placeholder-gray-500 bg-white"
            />
          </div>
        </div>

        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Station Name</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Line</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Model Process</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredStations.map((station) => (
                <tr key={station.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{station.id}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{station.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{getLineName(station.lineId)}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{getModelProcessName(station.modelProcessId)}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(station)}
                      className="text-blue-600 hover:text-blue-900 mr-3 inline-flex items-center gap-1 transition-colors"
                    >
                      <Edit size={16} />
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(station.id)}
                      className="text-red-600 hover:text-red-900 inline-flex items-center gap-1 transition-colors"
                    >
                      <Trash2 size={16} />
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          
          {filteredStations.length === 0 && (
            <div className="text-center py-8">
              <p className="text-gray-500">No stations found.</p>
            </div>
          )}
        </div>

        {showModal && (
          <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center z-50">
            <div className="bg-white p-6 rounded-lg shadow-xl w-96 max-w-md mx-4">
              <h3 className="text-lg font-semibold mb-4 flex items-center gap-2 text-gray-900">
                <MapPin size={20} className="text-blue-600" />
                {editingStation ? 'Edit Station' : 'Add New Station'}
              </h3>
              
              {error && (
                <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
                  {error}
                </div>
              )}
              
              <form onSubmit={handleSubmit}>
                <div className="mb-4">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Station Name *
                  </label>
                  <input
                    type="text"
                    required
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 placeholder-gray-500 bg-white"
                    placeholder="Enter station name..."
                  />
                </div>
                <div className="mb-4">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Line *
                  </label>
                  <select
                    required
                    value={formData.lineId}
                    onChange={(e) => setFormData({ ...formData, lineId: parseInt(e.target.value) })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 bg-white"
                  >
                    <option value={0}>Select Line...</option>
                    {lines.map((line) => (
                      <option key={line.id} value={line.id}>
                        {line.name}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="mb-4">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Model Process *
                  </label>
                  <select
                    required
                    value={formData.modelProcessId}
                    onChange={(e) => setFormData({ ...formData, modelProcessId: parseInt(e.target.value) })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 bg-white"
                  >
                    <option value={0}>Select Model Process...</option>
                    {modelProcesses.map((process) => (
                      <option key={process.id} value={process.id}>
                        {process.name}
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
                    Cancel
                  </button>
                  <button
                    type="submit"
                    className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors inline-flex items-center gap-1"
                  >
                    <Plus size={16} />
                    {editingStation ? 'Update' : 'Add'}
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