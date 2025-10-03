'use client';

import { useAuth } from '@/contexts/AuthContextSimple';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { ReactNode } from 'react';

interface DashboardLayoutProps {
  children: ReactNode;
}

export default function DashboardLayout({ children }: DashboardLayoutProps) {
  const { user, logout } = useAuth();
  const pathname = usePathname();

  const navigation = [
    { name: 'Tổng quan', href: '/dashboard', icon: '📊' },
    { name: 'Quản lý máy', href: '/machines', icon: '🏭' },
    { name: 'Người dùng', href: '/users', icon: '👥' },
    { name: 'Buyers', href: '/buyers', icon: '🏢' },
    { name: 'Lines', href: '/lines', icon: '🏭' },
    { name: 'Models', href: '/models', icon: '📦' },
    { name: 'Quy trình Model', href: '/model-processes', icon: '🔄' },
    { name: 'Stations', href: '/stations', icon: '🔧' },
    { name: 'Machine Types', href: '/machine-types', icon: '⚙️' },
    { name: 'Logs hệ thống', href: '/logs', icon: '📝' },
    { name: 'Lệnh điều khiển', href: '/commands', icon: '⚡' },
    { name: 'Cấu hình', href: '/settings', icon: '🛠️' },
  ];

  const isActiveRoute = (href: string) => {
    if (href === '/dashboard') {
      return pathname === '/dashboard';
    }
    return pathname.startsWith(href);
  };

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Top Navigation */}
      <nav className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-3">
                <div className="w-8 h-8 bg-blue-600 rounded-lg flex items-center justify-center">
                  <span className="text-white font-bold text-sm">MM</span>
                </div>
                <h1 className="text-xl font-bold text-gray-900">Machine Management</h1>
              </div>
            </div>
            
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-2">
                <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                <span className="text-sm text-gray-600">System Online</span>
              </div>
              
              <div className="flex items-center space-x-3">
                <div className="text-sm">
                  <div className="font-medium text-gray-900">{user?.username}</div>
                  <div className="text-gray-500">{user?.role}</div>
                </div>
                <button
                  onClick={logout}
                  className="text-sm text-red-600 hover:text-red-800 font-medium"
                >
                  Đăng xuất
                </button>
              </div>
            </div>
          </div>
        </div>
      </nav>

      <div className="flex">
        {/* Sidebar */}
        <div className="w-64 bg-white shadow-sm min-h-screen">
          <nav className="mt-5 px-2">
            <div className="space-y-1">
              {navigation.map((item) => (
                <Link
                  key={item.name}
                  href={item.href}
                  className={`${
                    isActiveRoute(item.href)
                      ? 'bg-blue-100 border-blue-500 text-blue-700'
                      : 'border-transparent text-gray-600 hover:text-gray-900 hover:bg-gray-50'
                  } group flex items-center px-3 py-2 text-sm font-medium border-l-4 transition-colors`}
                >
                  <span className="mr-3 text-lg">{item.icon}</span>
                  {item.name}
                </Link>
              ))}
            </div>
          </nav>
        </div>

        {/* Main content */}
        <div className="flex-1">
          <main className="py-6">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
              {children}
            </div>
          </main>
        </div>
      </div>
    </div>
  );
}