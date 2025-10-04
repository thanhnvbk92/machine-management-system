'use client';

import { useAuth } from '@/contexts/AuthContext';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { ReactNode, useState, useEffect } from 'react';
import {
  LayoutDashboard,
  Factory,
  Users,
  Truck,
  Package,
  Cog,
  Settings,
  FileText,
  Terminal,
  ChevronDown,
  LogOut,
  Bell,
  Search,
  Monitor,
  Database,
  ShoppingCart,
  MapPin,
  Box,
  Play,
  Menu,
  X,
  BarChart3,
  PieChart,
  TrendingUp,
  Activity,
  Shield,
  MessageSquare,
  Calendar
} from 'lucide-react';

interface NavigationItem {
  name: string;
  href?: string;
  icon: ReactNode;
  children?: NavigationItem[];
}

interface DashboardLayoutProps {
  children: ReactNode;
}

export default function DashboardLayout({ children }: DashboardLayoutProps) {
  const { user, logout } = useAuth();
  const pathname = usePathname();
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const [expandedMenus, setExpandedMenus] = useState<string[]>([]);
  
  const navigation: NavigationItem[] = [
    {
      name: 'Dashboard',
      href: '/dashboard',
      icon: <LayoutDashboard size={20} />
    },
    {
      name: 'Production Management',
      icon: <Factory size={20} />,
      children: [
        { name: 'Buyers', href: '/buyers', icon: <ShoppingCart size={18} /> },
        { name: 'Lines', href: '/lines', icon: <Truck size={18} /> },
        { name: 'Models', href: '/models', icon: <Box size={18} /> },
        { name: 'Group Models', href: '/group-models', icon: <Package size={18} /> },
        { name: 'Model Processes', href: '/model-processes', icon: <Play size={18} /> },
        { name: 'Stations', href: '/stations', icon: <MapPin size={18} /> },
      ]
    },
    {
      name: 'Equipment Management',
      icon: <Cog size={20} />,
      children: [
        { name: 'Machines', href: '/machines', icon: <Monitor size={18} /> },
        { name: 'Machine Types', href: '/machine-types', icon: <Settings size={18} /> },
      ]
    },
    {
      name: 'User Management',
      href: '/users',
      icon: <Users size={20} />
    },
    {
      name: 'System',
      icon: <Database size={20} />,
      children: [
        { name: 'System Logs', href: '/logs', icon: <FileText size={18} /> },
        { name: 'Commands', href: '/commands', icon: <Terminal size={18} /> },
        { name: 'Security', href: '/security', icon: <Shield size={18} /> },
      ]
    },
  ];

  const isActiveRoute = (href?: string) => {
    if (!href) return false;
    if (href === '/dashboard') {
      return pathname === '/dashboard';
    }
    return pathname.startsWith(href);
  };

  const toggleMenu = (menuName: string) => {
    setExpandedMenus(prev => 
      prev.includes(menuName) 
        ? prev.filter(name => name !== menuName)
        : [...prev, menuName]
    );
  };

  const isMenuExpanded = (menuName: string) => {
    return expandedMenus.includes(menuName);
  };

  const hasActiveChild = (children?: NavigationItem[]) => {
    if (!children) return false;
    return children.some(child => child.href && isActiveRoute(child.href));
  };

  // Auto-expand menus that contain active routes
  useEffect(() => {
    navigation.forEach(item => {
      if (item.children && hasActiveChild(item.children)) {
        if (!expandedMenus.includes(item.name)) {
          setExpandedMenus(prev => [...prev, item.name]);
        }
      }
    });
  }, [pathname]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100">
      {/* Sidebar */}
      <div className="fixed inset-y-0 left-0 z-50 w-80 bg-gradient-to-b from-slate-800 to-slate-900 shadow-2xl">
        {/* Logo & Brand */}
        <div className="flex items-center h-20 px-6 border-b border-slate-700">
          <div className="flex items-center space-x-3">
            <div className="w-12 h-12 bg-gradient-to-r from-blue-600 to-purple-600 rounded-2xl flex items-center justify-center shadow-lg">
              <Factory className="text-white" size={24} />
            </div>
            <div>
              <h1 className="text-xl font-bold text-white">Concept</h1>
              <p className="text-xs text-slate-400">Machine Management</p>
            </div>
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 px-4 py-6 space-y-2 overflow-y-auto">
          {navigation.map(item => (
            <div key={item.name}>
              {item.children ? (
                // Menu with submenu
                <div>
                  <button
                    onClick={() => toggleMenu(item.name)}
                    className={`group w-full flex items-center justify-between px-4 py-3 text-sm font-medium rounded-xl transition-all duration-200 ${
                      hasActiveChild(item.children) || isMenuExpanded(item.name)
                        ? 'text-white bg-slate-700' 
                        : 'text-slate-300 hover:text-white hover:bg-slate-700'
                    }`}
                  >
                    <div className="flex items-center space-x-3">
                      <span>{item.icon}</span>
                      <span>{item.name}</span>
                    </div>
                    <ChevronDown 
                      size={16} 
                      className={`transition-transform duration-200 ${
                        isMenuExpanded(item.name) ? 'rotate-180' : ''
                      }`}
                    />
                  </button>
                  
                  {/* Submenu */}
                  <div className={`ml-4 mt-2 space-y-1 overflow-hidden transition-all duration-200 ${
                    isMenuExpanded(item.name) ? 'max-h-96 opacity-100' : 'max-h-0 opacity-0'
                  }`}>
                    {item.children.map(child => (
                      <Link
                        key={child.name}
                        href={child.href || '#'}
                        className={`flex items-center space-x-3 px-4 py-2.5 text-sm rounded-lg transition-all duration-200 ${
                          isActiveRoute(child.href)
                            ? 'text-white bg-gradient-to-r from-blue-600 to-purple-600 shadow-lg' 
                            : 'text-slate-400 hover:text-white hover:bg-slate-600'
                        }`}
                      >
                        <span>{child.icon}</span>
                        <span>{child.name}</span>
                      </Link>
                    ))}
                  </div>
                </div>
              ) : (
                // Simple menu item
                <Link
                  href={item.href || '#'}
                  className={`group flex items-center space-x-3 px-4 py-3 text-sm font-medium rounded-xl transition-all duration-200 ${
                    isActiveRoute(item.href)
                      ? 'text-white bg-gradient-to-r from-blue-600 to-purple-600 shadow-lg' 
                      : 'text-slate-300 hover:text-white hover:bg-slate-700'
                  }`}
                >
                  <span>{item.icon}</span>
                  <span>{item.name}</span>
                </Link>
              )}
            </div>
          ))}
        </nav>

        {/* User Profile */}
        <div className="p-4 border-t border-slate-700">
          <div className="flex items-center space-x-3 p-3 rounded-xl bg-slate-700">
            <div className="w-10 h-10 bg-gradient-to-r from-blue-600 to-purple-600 rounded-full flex items-center justify-center">
              <span className="text-white font-semibold text-sm">
                {user?.username?.charAt(0).toUpperCase()}
              </span>
            </div>
            <div className="flex-1">
              <p className="text-sm font-medium text-white">{user?.username}</p>
              <p className="text-xs text-slate-400">{user?.role}</p>
            </div>
            <button onClick={logout} className="p-2 text-slate-400 hover:text-white">
              <LogOut size={16} />
            </button>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="ml-80">
        {/* Top Header */}
        <header className="bg-white shadow-sm border-b border-slate-200">
          <div className="flex items-center justify-between h-20 px-6">
            <div>
              <h2 className="text-2xl font-bold text-slate-900">Dashboard</h2>
              <p className="text-sm text-slate-500">Welcome back, {user?.username}</p>
            </div>
            <div className="flex items-center space-x-6">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400" size={20} />
                <input
                  type="text"
                  placeholder="Search..."
                  className="w-80 pl-10 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm"
                />
              </div>
              <button className="relative p-3 text-slate-600 hover:bg-slate-100 rounded-xl">
                <Bell size={20} />
                <span className="absolute -top-1 -right-1 w-5 h-5 bg-red-500 text-white text-xs rounded-full flex items-center justify-center">3</span>
              </button>
              <div className="flex items-center space-x-2 px-3 py-2 bg-green-50 text-green-700 rounded-xl">
                <div className="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
                <span className="text-sm font-medium">System Online</span>
              </div>
            </div>
          </div>
        </header>

        {/* Page Content */}
        <main className="p-6">
          {children}
        </main>
      </div>
    </div>
  );
}
