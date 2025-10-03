'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { ModelProcess, CreateModelProcessDto, UpdateModelProcessDto, ModelGroup } from '@/types';
import { modelProcessesApi, modelGroupsApi } from '@/services/api';
import { Plus, Edit, Trash2, Search, Play, X } from 'lucide-react';

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
      setError('Cannot load data');
      console.error('Error loading data:', err);
      // Fallback data
      setModelProcesses([
        { 
          id: 1, 
          name: 'Q-Fusing', 
          modelGroupId: 1, 
          stationCount: 5, 
          createdAt: new Date().toISOString(), 
          isActive: true 
        },
        { 
          id: 2, 
          name: 'Calibration', 
          modelGroupId: 1, 
          stationCount: 3, 
          createdAt: new Date().toISOString(), 
          isActive: true 
        }
      ]);
      setModelGroups([
        { id: 1, name: 'OCU2', buyerId: 1 },
        { id: 2, name: 'ECU3', buyerId: 1 }
      ]);
    } finally {
      setLoading(false);
    }
  };

  const getModelGroupName = (modelGroupId?: number) => {
    if (!modelGroupId) return 'N/A';
    const modelGroup = modelGroups.find(mg => mg.id === modelGroupId);
    return modelGroup?.name || `Group ${modelGroupId}`;
  };

  const filteredModelProcesses = modelProcesses.filter(modelProcess =>
    modelProcess.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleAdd = () => {
    setEditingModelProcess(null);
    setFormData({ 
      name: '', 
      modelGroupId: modelGroups.length > 0 ? modelGroups[0].id : 0
    });
    setError(null);
    setShowModal(true);
  };

  const handleEdit = (modelProcess: ModelProcess) => {
    setEditingModelProcess(modelProcess);
    setFormData({
      name: modelProcess.name,
      modelGroupId: modelProcess.modelGroupId || 0
    });
    setError(null);
    setShowModal(true);
  };

  const handleDelete = async (id: number) => {
    if (confirm('Are you sure you want to delete this model process?')) {
      try {
        setError(null);
        await modelProcessesApi.delete(id);
        setModelProcesses(modelProcesses.filter(mp => mp.id !== id));
      } catch (err) {
        setError('Cannot delete model process');
        console.error('Error deleting model process:', err);
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.name.trim()) {
      setError('Model process name cannot be empty');
      return;
    }

    if (!formData.modelGroupId) {
      setError('Please select a model group');
      return;
    }

    try {
      setError(null);

      if (editingModelProcess) {
        const updateData: UpdateModelProcessDto = {
          name: formData.name.trim(),
          modelGroupId: formData.modelGroupId
        };
        const updated = await modelProcessesApi.update(editingModelProcess.id, updateData);
        setModelProcesses(modelProcesses.map(mp => mp.id === editingModelProcess.id ? updated : mp));
      } else {
        const createData: CreateModelProcessDto = {
          name: formData.name.trim(),
          modelGroupId: formData.modelGroupId
        };
        const newModelProcess = await modelProcessesApi.create(createData);
        setModelProcesses([...modelProcesses, newModelProcess]);
      }

      setShowModal(false);
      setFormData({ name: '', modelGroupId: 0 });
    } catch (err) {
      setError(editingModelProcess ? 'Cannot update model process' : 'Cannot create new model process');
      console.error('Error saving model process:', err);
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
            <h1 className="text-3xl font-bold text-gray-900">Model Processes Management</h1>
            <p className="text-gray-600 mt-1">Manage model processes and their configurations</p>
          </div>
          <button
            onClick={handleAdd}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 flex items-center space-x-2 transition-colors"
          >
            <Plus size={18} />
            <span>Add Model Process</span>
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
              placeholder="Search by model process name..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 placeholder-gray-500 bg-white"
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Total Model Processes</p>
            <p className="text-2xl font-bold text-gray-900">{modelProcesses.length}</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Search Results</p>
            <p className="text-2xl font-bold text-gray-900">{filteredModelProcesses.length}</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <p className="text-sm font-medium text-gray-600">Active Processes</p>
            <p className="text-2xl font-bold text-gray-900">
              {modelProcesses.filter(mp => mp.isActive).length}
            </p>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Process Name</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Model Group</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Station Count</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredModelProcesses.map((modelProcess) => (
                <tr key={modelProcess.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{modelProcess.id}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{modelProcess.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                      {getModelGroupName(modelProcess.modelGroupId)}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{modelProcess.stationCount || 0}</td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                      modelProcess.isActive 
                        ? 'bg-green-100 text-green-800' 
                        : 'bg-red-100 text-red-800'
                    }`}>
                      {modelProcess.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(modelProcess)}
                      className="text-blue-600 hover:text-blue-900 mr-3 inline-flex items-center gap-1 transition-colors"
                    >
                      <Edit size={16} />
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(modelProcess.id)}
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
          
          {filteredModelProcesses.length === 0 && (
            <div className="text-center py-8">
              <p className="text-gray-500">No model processes found.</p>
            </div>
          )}
        </div>

        {showModal && (
          <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center z-50">
            <div className="bg-white p-6 rounded-lg shadow-xl w-96 max-w-md mx-4">
              <h3 className="text-lg font-semibold mb-4 flex items-center gap-2 text-gray-900">
                <Play size={20} className="text-blue-600" />
                {editingModelProcess ? 'Edit Model Process' : 'Add New Model Process'}
              </h3>
              
              {error && (
                <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
                  {error}
                </div>
              )}
              
              <form onSubmit={handleSubmit}>
                <div className="mb-4">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Process Name *
                  </label>
                  <input
                    type="text"
                    required
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 placeholder-gray-500 bg-white"
                    placeholder="Enter process name..."
                  />
                </div>
                <div className="mb-4">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Model Group *
                  </label>
                  <select
                    required
                    value={formData.modelGroupId}
                    onChange={(e) => setFormData({ ...formData, modelGroupId: parseInt(e.target.value) })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 bg-white"
                  >
                    <option value={0}>Select Model Group...</option>
                    {modelGroups.map((modelGroup) => (
                      <option key={modelGroup.id} value={modelGroup.id}>
                        {modelGroup.name}
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
                    {editingModelProcess ? 'Update' : 'Add'}
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