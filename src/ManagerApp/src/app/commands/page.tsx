'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';

interface Command {
  id: number;
  machineId: number;
  machineName: string;
  commandType: 'start' | 'stop' | 'pause' | 'resume' | 'reset' | 'maintenance' | 'custom';
  commandData: string;
  status: 'pending' | 'sent' | 'acknowledged' | 'completed' | 'failed';
  sentAt: string;
  executedAt?: string;
  response?: string;
  sentBy: string;
}

export default function CommandsPage() {
  const [commands, setCommands] = useState<Command[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [filterStatus, setFilterStatus] = useState('all');
  const [filterMachine, setFilterMachine] = useState('all');
  const [filterType, setFilterType] = useState('all');

  // Mock data - replace with actual API calls
  useEffect(() => {
    const mockCommands: Command[] = [
      {
        id: 1,
        machineId: 1,
        machineName: 'CNC-001',
        commandType: 'start',
        commandData: '{"program": "AUTO_CNC", "speed": 100}',
        status: 'completed',
        sentAt: '2024-10-02T14:30:00Z',
        executedAt: '2024-10-02T14:30:15Z',
        response: '{"status": "success", "message": "Program started"}',
        sentBy: 'admin'
      },
      {
        id: 2,
        machineId: 2,
        machineName: 'LASER-003',
        commandType: 'stop',
        commandData: '{"emergency": false}',
        status: 'failed',
        sentAt: '2024-10-02T14:25:00Z',
        response: '{"status": "error", "message": "Machine not responding"}',
        sentBy: 'operator01'
      },
      {
        id: 3,
        machineId: 3,
        machineName: 'DRILL-002',
        commandType: 'maintenance',
        commandData: '{"type": "calibration", "duration": 300}',
        status: 'pending',
        sentAt: '2024-10-02T14:20:00Z',
        sentBy: 'manager01'
      },
      {
        id: 4,
        machineId: 1,
        machineName: 'CNC-001',
        commandType: 'custom',
        commandData: '{"command": "UPDATE_PROGRAM", "program_id": "P001"}',
        status: 'sent',
        sentAt: '2024-10-02T14:15:00Z',
        sentBy: 'admin'
      }
    ];
    setCommands(mockCommands);
    setLoading(false);
  }, []);

  const filteredCommands = commands.filter(command => {
    const matchesSearch = command.machineName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         command.commandData.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         command.sentBy.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = filterStatus === 'all' || command.status === filterStatus;
    const matchesMachine = filterMachine === 'all' || command.machineId.toString() === filterMachine;
    const matchesType = filterType === 'all' || command.commandType === filterType;
    return matchesSearch && matchesStatus && matchesMachine && matchesType;
  });

  const handleSendCommand = (commandData: Partial<Command>) => {
    const newCommand: Command = {
      id: Date.now(),
      sentAt: new Date().toISOString(),
      status: 'pending',
      sentBy: 'admin', // Get from auth context
      machineId: 0,
      machineName: '',
      commandType: 'start',
      commandData: '',
      ...commandData
    };
    setCommands([newCommand, ...commands]);
    setShowModal(false);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'completed': return 'bg-green-100 text-green-800';
      case 'sent': return 'bg-blue-100 text-blue-800';
      case 'acknowledged': return 'bg-yellow-100 text-yellow-800';
      case 'pending': return 'bg-orange-100 text-orange-800';
      case 'failed': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'completed': return 'Hoàn thành';
      case 'sent': return 'Đã gửi';
      case 'acknowledged': return 'Đã nhận';
      case 'pending': return 'Chờ xử lý';
      case 'failed': return 'Thất bại';
      default: return 'Không xác định';
    }
  };

  const getCommandTypeColor = (type: string) => {
    switch (type) {
      case 'start': return 'bg-green-100 text-green-800';
      case 'stop': return 'bg-red-100 text-red-800';
      case 'pause': return 'bg-yellow-100 text-yellow-800';
      case 'resume': return 'bg-blue-100 text-blue-800';
      case 'reset': return 'bg-purple-100 text-purple-800';
      case 'maintenance': return 'bg-orange-100 text-orange-800';
      case 'custom': return 'bg-gray-100 text-gray-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getCommandTypeText = (type: string) => {
    switch (type) {
      case 'start': return 'Khởi động';
      case 'stop': return 'Dừng';
      case 'pause': return 'Tạm dừng';
      case 'resume': return 'Tiếp tục';
      case 'reset': return 'Khởi động lại';
      case 'maintenance': return 'Bảo trì';
      case 'custom': return 'Tùy chỉnh';
      default: return 'Khác';
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString('vi-VN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
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
            <h1 className="text-2xl font-bold text-gray-900">Lệnh điều khiển</h1>
            <p className="text-gray-600">Gửi lệnh và theo dõi trạng thái điều khiển máy móc</p>
          </div>
          <button
            onClick={() => setShowModal(true)}
            className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium"
          >
            ⚡ Gửi lệnh mới
          </button>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-5 gap-6">
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-orange-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">⏳</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Chờ xử lý</p>
                <p className="text-2xl font-bold text-gray-900">
                  {commands.filter(c => c.status === 'pending').length}
                </p>
              </div>
            </div>
          </div>
          
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-blue-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">📤</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Đã gửi</p>
                <p className="text-2xl font-bold text-gray-900">
                  {commands.filter(c => c.status === 'sent').length}
                </p>
              </div>
            </div>
          </div>
          
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-green-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">✅</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Hoàn thành</p>
                <p className="text-2xl font-bold text-gray-900">
                  {commands.filter(c => c.status === 'completed').length}
                </p>
              </div>
            </div>
          </div>
          
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-red-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">❌</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Thất bại</p>
                <p className="text-2xl font-bold text-gray-900">
                  {commands.filter(c => c.status === 'failed').length}
                </p>
              </div>
            </div>
          </div>
          
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-purple-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">📊</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Tổng số</p>
                <p className="text-2xl font-bold text-gray-900">{commands.length}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Filters */}
        <div className="bg-white rounded-lg shadow p-6">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Tìm kiếm
              </label>
              <input
                type="text"
                placeholder="Máy, lệnh, người gửi..."
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
                <option value="pending">Chờ xử lý</option>
                <option value="sent">Đã gửi</option>
                <option value="acknowledged">Đã nhận</option>
                <option value="completed">Hoàn thành</option>
                <option value="failed">Thất bại</option>
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Loại lệnh
              </label>
              <select
                value={filterType}
                onChange={(e) => setFilterType(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="all">Tất cả</option>
                <option value="start">Khởi động</option>
                <option value="stop">Dừng</option>
                <option value="pause">Tạm dừng</option>
                <option value="resume">Tiếp tục</option>
                <option value="reset">Khởi động lại</option>
                <option value="maintenance">Bảo trì</option>
                <option value="custom">Tùy chỉnh</option>
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Máy
              </label>
              <select
                value={filterMachine}
                onChange={(e) => setFilterMachine(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="all">Tất cả máy</option>
                <option value="1">CNC-001</option>
                <option value="2">LASER-003</option>
                <option value="3">DRILL-002</option>
              </select>
            </div>
          </div>
          <div className="mt-4 text-sm text-gray-600">
            Hiển thị {filteredCommands.length} / {commands.length} lệnh
          </div>
        </div>

        {/* Commands Table */}
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Thời gian
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Máy
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Loại lệnh
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Trạng thái
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Người gửi
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredCommands.map((command) => (
                  <tr key={command.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm text-gray-900">{formatDate(command.sentAt)}</div>
                      {command.executedAt && (
                        <div className="text-xs text-gray-500">
                          Thực hiện: {formatDate(command.executedAt)}
                        </div>
                      )}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm font-medium text-gray-900">{command.machineName}</div>
                      <div className="text-sm text-gray-500">ID: {command.machineId}</div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getCommandTypeColor(command.commandType)}`}>
                        {getCommandTypeText(command.commandType)}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(command.status)}`}>
                        {getStatusText(command.status)}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {command.sentBy}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <button className="text-blue-600 hover:text-blue-900">
                        👁️ Chi tiết
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        {filteredCommands.length === 0 && (
          <div className="text-center py-12 bg-white rounded-lg shadow">
            <div className="text-gray-400 text-lg mb-2">🔍</div>
            <div className="text-gray-500">Không tìm thấy lệnh nào phù hợp</div>
          </div>
        )}
      </div>

      {/* Modal for Send Command */}
      {showModal && (
        <SendCommandModal
          onSend={handleSendCommand}
          onClose={() => setShowModal(false)}
        />
      )}
    </DashboardLayout>
  );
}

// Send Command Modal Component
interface SendCommandModalProps {
  onSend: (command: Partial<Command>) => void;
  onClose: () => void;
}

function SendCommandModal({ onSend, onClose }: SendCommandModalProps) {
  const [formData, setFormData] = useState({
    machineId: '',
    machineName: '',
    commandType: 'start' as Command['commandType'],
    commandData: ''
  });

  const machines = [
    { id: 1, name: 'CNC-001' },
    { id: 2, name: 'LASER-003' },
    { id: 3, name: 'DRILL-002' }
  ];

  const handleMachineChange = (machineId: string) => {
    const machine = machines.find(m => m.id.toString() === machineId);
    setFormData({
      ...formData,
      machineId,
      machineName: machine?.name || ''
    });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.machineId || !formData.commandData) return;
    
    onSend({
      machineId: parseInt(formData.machineId),
      machineName: formData.machineName,
      commandType: formData.commandType,
      commandData: formData.commandData
    });
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md">
        <h3 className="text-lg font-semibold mb-4">Gửi lệnh điều khiển</h3>
        
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Máy đích *
            </label>
            <select
              required
              value={formData.machineId}
              onChange={(e) => handleMachineChange(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">Chọn máy</option>
              {machines.map(machine => (
                <option key={machine.id} value={machine.id}>
                  {machine.name}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Loại lệnh
            </label>
            <select
              value={formData.commandType}
              onChange={(e) => setFormData({...formData, commandType: e.target.value as Command['commandType']})}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="start">Khởi động</option>
              <option value="stop">Dừng</option>
              <option value="pause">Tạm dừng</option>
              <option value="resume">Tiếp tục</option>
              <option value="reset">Khởi động lại</option>
              <option value="maintenance">Bảo trì</option>
              <option value="custom">Tùy chỉnh</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Dữ liệu lệnh (JSON) *
            </label>
            <textarea
              required
              rows={4}
              value={formData.commandData}
              onChange={(e) => setFormData({...formData, commandData: e.target.value})}
              placeholder='{"program": "AUTO_MODE", "speed": 100}'
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500 font-mono text-sm"
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
              ⚡ Gửi lệnh
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}