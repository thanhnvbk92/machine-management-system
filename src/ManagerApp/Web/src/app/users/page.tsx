'use client';

import { useState, useEffect } from 'react';
import DashboardLayout from '@/components/DashboardLayout';
import { User, UserRole } from '@/types';

export default function UsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [filterRole, setFilterRole] = useState('all');

  // Mock data - replace with actual API calls
  useEffect(() => {
    const mockUsers: User[] = [
      {
        id: 1,
        username: 'admin',
        email: 'admin@company.com',
        role: UserRole.Admin,
        createdAt: '2024-01-15T08:00:00Z',
        isActive: true
      },
      {
        id: 2,
        username: 'manager01',
        email: 'manager01@company.com',
        role: UserRole.Manager,
        createdAt: '2024-02-01T09:00:00Z',
        isActive: true
      },
      {
        id: 3,
        username: 'operator01',
        email: 'operator01@company.com',
        role: UserRole.Operator,
        createdAt: '2024-02-15T10:00:00Z',
        isActive: true
      },
      {
        id: 4,
        username: 'viewer01',
        email: 'viewer01@company.com',
        role: UserRole.Viewer,
        createdAt: '2024-03-01T11:00:00Z',
        isActive: false
      }
    ];
    setUsers(mockUsers);
    setLoading(false);
  }, []);

  const filteredUsers = users.filter(user => {
    const matchesSearch = user.username.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         user.email.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesFilter = filterRole === 'all' || user.role === filterRole;
    return matchesSearch && matchesFilter;
  });

  const handleAddUser = () => {
    setEditingUser(null);
    setShowModal(true);
  };

  const handleEditUser = (user: User) => {
    setEditingUser(user);
    setShowModal(true);
  };

  const handleDeleteUser = (id: number) => {
    if (confirm('Bạn có chắc chắn muốn xóa người dùng này?')) {
      setUsers(users.filter(u => u.id !== id));
    }
  };

  const handleToggleActive = (id: number) => {
    setUsers(users.map(u => 
      u.id === id ? { ...u, isActive: !u.isActive } : u
    ));
  };

  const handleSaveUser = (userData: Partial<User>) => {
    if (editingUser) {
      // Update existing user
      setUsers(users.map(u => 
        u.id === editingUser.id ? { ...u, ...userData } : u
      ));
    } else {
      // Add new user
      const newUser: User = {
        id: Date.now(),
        createdAt: new Date().toISOString(),
        isActive: true,
        username: '',
        email: '',
        role: UserRole.Viewer,
        ...userData
      };
      setUsers([...users, newUser]);
    }
    setShowModal(false);
    setEditingUser(null);
  };

  const getRoleColor = (role: UserRole) => {
    switch (role) {
      case UserRole.Admin: return 'bg-purple-100 text-purple-800';
      case UserRole.Manager: return 'bg-blue-100 text-blue-800';
      case UserRole.Operator: return 'bg-green-100 text-green-800';
      case UserRole.Viewer: return 'bg-gray-100 text-gray-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getRoleText = (role: UserRole) => {
    switch (role) {
      case UserRole.Admin: return 'Quản trị viên';
      case UserRole.Manager: return 'Quản lý';
      case UserRole.Operator: return 'Điều hành';
      case UserRole.Viewer: return 'Xem';
      default: return 'Không xác định';
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('vi-VN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
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
            <h1 className="text-2xl font-bold text-gray-900">Quản lý người dùng</h1>
            <p className="text-gray-600">Thêm, sửa, xóa tài khoản người dùng hệ thống</p>
          </div>
          <button
            onClick={handleAddUser}
            className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium"
          >
            ➕ Thêm người dùng
          </button>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-purple-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">👑</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Quản trị viên</p>
                <p className="text-2xl font-bold text-gray-900">
                  {users.filter(u => u.role === UserRole.Admin).length}
                </p>
              </div>
            </div>
          </div>
          
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-blue-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">👨‍💼</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Quản lý</p>
                <p className="text-2xl font-bold text-gray-900">
                  {users.filter(u => u.role === UserRole.Manager).length}
                </p>
              </div>
            </div>
          </div>
          
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-green-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">👷</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Điều hành</p>
                <p className="text-2xl font-bold text-gray-900">
                  {users.filter(u => u.role === UserRole.Operator).length}
                </p>
              </div>
            </div>
          </div>
          
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-gray-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">👥</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Tổng số</p>
                <p className="text-2xl font-bold text-gray-900">{users.length}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Filters and Search */}
        <div className="bg-white rounded-lg shadow p-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Tìm kiếm
              </label>
              <input
                type="text"
                placeholder="Tên đăng nhập, email..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Phân quyền
              </label>
              <select
                value={filterRole}
                onChange={(e) => setFilterRole(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="all">Tất cả</option>
                <option value={UserRole.Admin}>Quản trị viên</option>
                <option value={UserRole.Manager}>Quản lý</option>
                <option value={UserRole.Operator}>Điều hành</option>
                <option value={UserRole.Viewer}>Xem</option>
              </select>
            </div>
            <div className="flex items-end">
              <div className="text-sm text-gray-600">
                Hiển thị {filteredUsers.length} / {users.length} người dùng
              </div>
            </div>
          </div>
        </div>

        {/* Users Table */}
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Người dùng
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Email
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Phân quyền
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Trạng thái
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Ngày tạo
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredUsers.map((user) => (
                  <tr key={user.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <div className="w-10 h-10 bg-gray-300 rounded-full flex items-center justify-center">
                          <span className="text-sm font-medium text-gray-600">
                            {user.username.charAt(0).toUpperCase()}
                          </span>
                        </div>
                        <div className="ml-4">
                          <div className="text-sm font-medium text-gray-900">{user.username}</div>
                          <div className="text-sm text-gray-500">ID: {user.id}</div>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {user.email}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getRoleColor(user.role)}`}>
                        {getRoleText(user.role)}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <button
                        onClick={() => handleToggleActive(user.id)}
                        className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full cursor-pointer ${
                          user.isActive 
                            ? 'bg-green-100 text-green-800 hover:bg-green-200' 
                            : 'bg-red-100 text-red-800 hover:bg-red-200'
                        }`}
                      >
                        {user.isActive ? 'Hoạt động' : 'Vô hiệu hóa'}
                      </button>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {formatDate(user.createdAt)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <button
                        onClick={() => handleEditUser(user)}
                        className="text-blue-600 hover:text-blue-900 mr-3"
                      >
                        ✏️ Sửa
                      </button>
                      <button
                        onClick={() => handleDeleteUser(user.id)}
                        className="text-red-600 hover:text-red-900"
                        disabled={user.role === UserRole.Admin && users.filter(u => u.role === UserRole.Admin).length === 1}
                      >
                        🗑️ Xóa
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        {filteredUsers.length === 0 && (
          <div className="text-center py-12 bg-white rounded-lg shadow">
            <div className="text-gray-400 text-lg mb-2">🔍</div>
            <div className="text-gray-500">Không tìm thấy người dùng nào phù hợp</div>
          </div>
        )}
      </div>

      {/* Modal for Add/Edit User */}
      {showModal && (
        <UserModal
          user={editingUser}
          onSave={handleSaveUser}
          onClose={() => {
            setShowModal(false);
            setEditingUser(null);
          }}
        />
      )}
    </DashboardLayout>
  );
}

// User Modal Component
interface UserModalProps {
  user: User | null;
  onSave: (user: Partial<User>) => void;
  onClose: () => void;
}

function UserModal({ user, onSave, onClose }: UserModalProps) {
  const [formData, setFormData] = useState({
    username: user?.username || '',
    email: user?.email || '',
    role: user?.role || UserRole.Viewer,
    isActive: user?.isActive ?? true
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave(formData);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md">
        <h3 className="text-lg font-semibold mb-4">
          {user ? 'Chỉnh sửa người dùng' : 'Thêm người dùng mới'}
        </h3>
        
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Tên đăng nhập *
            </label>
            <input
              type="text"
              required
              value={formData.username}
              onChange={(e) => setFormData({...formData, username: e.target.value})}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Email *
            </label>
            <input
              type="email"
              required
              value={formData.email}
              onChange={(e) => setFormData({...formData, email: e.target.value})}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Phân quyền
            </label>
            <select
              value={formData.role}
              onChange={(e) => setFormData({...formData, role: e.target.value as UserRole})}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            >
              <option value={UserRole.Viewer}>Xem</option>
              <option value={UserRole.Operator}>Điều hành</option>
              <option value={UserRole.Manager}>Quản lý</option>
              <option value={UserRole.Admin}>Quản trị viên</option>
            </select>
          </div>

          <div className="flex items-center">
            <input
              type="checkbox"
              id="isActive"
              checked={formData.isActive}
              onChange={(e) => setFormData({...formData, isActive: e.target.checked})}
              className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
            />
            <label htmlFor="isActive" className="ml-2 block text-sm text-gray-900">
              Tài khoản hoạt động
            </label>
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
              {user ? 'Cập nhật' : 'Thêm mới'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}