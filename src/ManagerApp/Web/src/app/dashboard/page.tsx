'use client';

import DashboardLayout from '@/components/DashboardLayout';

export default function DashboardPage() {
  return (
    <DashboardLayout>
      <div className="space-y-6">
        {/* Page Header */}
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Tổng quan hệ thống</h1>
          <p className="text-gray-600">Theo dõi trạng thái máy móc và hệ thống</p>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-blue-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">🏭</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Tổng số máy</p>
                <p className="text-2xl font-bold text-gray-900">25</p>
              </div>
            </div>
          </div>
          
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-green-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">✅</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Đang hoạt động</p>
                <p className="text-2xl font-bold text-gray-900">18</p>
              </div>
            </div>
          </div>
          
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-red-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">❌</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Offline</p>
                <p className="text-2xl font-bold text-gray-900">7</p>
              </div>
            </div>
          </div>
          
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center">
              <div className="w-12 h-12 bg-yellow-500 rounded-lg flex items-center justify-center">
                <span className="text-white text-xl">⚠️</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Cảnh báo</p>
                <p className="text-2xl font-bold text-gray-900">3</p>
              </div>
            </div>
          </div>
        </div>

        {/* Recent Activity */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div className="bg-white rounded-lg shadow">
            <div className="px-6 py-4 border-b border-gray-200">
              <h3 className="text-lg font-semibold text-gray-900">Hoạt động gần đây</h3>
            </div>
            <div className="p-6">
              <div className="space-y-4">
                <div className="flex items-center space-x-3">
                  <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                  <div className="flex-1">
                    <p className="text-sm text-gray-900">Máy CNC-001 đã bắt đầu hoạt động</p>
                    <p className="text-xs text-gray-500">5 phút trước</p>
                  </div>
                </div>
                <div className="flex items-center space-x-3">
                  <div className="w-2 h-2 bg-red-500 rounded-full"></div>
                  <div className="flex-1">
                    <p className="text-sm text-gray-900">Máy LASER-003 báo lỗi</p>
                    <p className="text-xs text-gray-500">12 phút trước</p>
                  </div>
                </div>
                <div className="flex items-center space-x-3">
                  <div className="w-2 h-2 bg-blue-500 rounded-full"></div>
                  <div className="flex-1">
                    <p className="text-sm text-gray-900">Lệnh bảo trì đã được gửi tới máy DRILL-002</p>
                    <p className="text-xs text-gray-500">20 phút trước</p>
                  </div>
                </div>
                <div className="flex items-center space-x-3">
                  <div className="w-2 h-2 bg-yellow-500 rounded-full"></div>
                  <div className="flex-1">
                    <p className="text-sm text-gray-900">Cập nhật firmware cho máy PRESS-005</p>
                    <p className="text-xs text-gray-500">1 giờ trước</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow">
            <div className="px-6 py-4 border-b border-gray-200">
              <h3 className="text-lg font-semibold text-gray-900">Trạng thái hệ thống</h3>
            </div>
            <div className="p-6">
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <span className="text-gray-600">API Server</span>
                  <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800">
                    Hoạt động
                  </span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-gray-600">Database</span>
                  <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800">
                    Kết nối
                  </span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-gray-600">Message Queue</span>
                  <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800">
                    Hoạt động
                  </span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-gray-600">Backup System</span>
                  <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800">
                    Đang xử lý
                  </span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-gray-600">Cập nhật cuối</span>
                  <span className="text-sm text-gray-500">2 phút trước</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Quick Actions */}
        <div className="bg-white rounded-lg shadow">
          <div className="px-6 py-4 border-b border-gray-200">
            <h3 className="text-lg font-semibold text-gray-900">Thao tác nhanh</h3>
          </div>
          <div className="p-6">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <button className="p-4 bg-blue-50 hover:bg-blue-100 rounded-lg transition-colors text-left">
                <div className="flex items-center space-x-3">
                  <div className="w-10 h-10 bg-blue-500 rounded-lg flex items-center justify-center">
                    <span className="text-white text-lg">🏭</span>
                  </div>
                  <div>
                    <div className="font-medium text-blue-900">Quản lý máy</div>
                    <div className="text-sm text-blue-700">Thêm, sửa, xóa máy móc</div>
                  </div>
                </div>
              </button>
              
              <button className="p-4 bg-green-50 hover:bg-green-100 rounded-lg transition-colors text-left">
                <div className="flex items-center space-x-3">
                  <div className="w-10 h-10 bg-green-500 rounded-lg flex items-center justify-center">
                    <span className="text-white text-lg">⚡</span>
                  </div>
                  <div>
                    <div className="font-medium text-green-900">Gửi lệnh</div>
                    <div className="text-sm text-green-700">Điều khiển máy móc</div>
                  </div>
                </div>
              </button>
              
              <button className="p-4 bg-purple-50 hover:bg-purple-100 rounded-lg transition-colors text-left">
                <div className="flex items-center space-x-3">
                  <div className="w-10 h-10 bg-purple-500 rounded-lg flex items-center justify-center">
                    <span className="text-white text-lg">📊</span>
                  </div>
                  <div>
                    <div className="font-medium text-purple-900">Báo cáo</div>
                    <div className="text-sm text-purple-700">Xem logs và thống kê</div>
                  </div>
                </div>
              </button>
            </div>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}