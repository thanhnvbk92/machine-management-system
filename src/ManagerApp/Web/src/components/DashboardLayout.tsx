'use client';

import { useAuth } from '@/contexts/AuthContext';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { ReactNode, useState, useEffect } from 'react';
import {
  LayoutDashboard,
  Factory,
  Users,
  Building2,
  Truck,
  Package,
  Cog,
  Wrench,
  Settings,
  FileText,
  Terminal,
  ChevronDown,
  ChevronRight,
  LogOut,
  Bell,
  Search,
  Monitor,
  Database,
  ShoppingCart,
  MapPin,
  Box,
  Play
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
  
  // Auto-expand groups that contain active routes
  const getActiveGroups = () => {
    const activeGroups: string[] = [];
    
    if (pathname.startsWith('/buyers') || pathname.startsWith('/lines') || 
        pathname.startsWith('/models') || pathname.startsWith('/group-models') ||
        pathname.startsWith('/model-processes') || pathname.startsWith('/stations')) {
      activeGroups.push('production-management');
    }
    
    if (pathname.startsWith('/machines') || pathname.startsWith('/machine-types')) {
      activeGroups.push('equipment-management');
    }
    
    return activeGroups;
  };
  
  const [expandedGroups, setExpandedGroups] = useState<string[]>(() => {
    const defaultExpanded = ['production-management', 'equipment-management'];
    return [...defaultExpanded, ...getActiveGroups()];
  });
  
  // Auto-expand groups when pathname changes
  useEffect(() => {
    const activeGroups = getActiveGroups();
    setExpandedGroups(prev => {
      const newExpanded = new Set([...prev, ...activeGroups]);
      return Array.from(newExpanded);
    });
  }, [pathname]);

  const navigation: NavigationItem[] = [
    {
      name: 'Dashboard',
      href: '/dashboard',
      icon: <LayoutDashboard size={18} />
    },
    {
      name: 'Production Management',
      icon: <Factory size={18} />,
      children: [
        { name: 'Buyers', href: '/buyers', icon: <ShoppingCart size={16} /> },
        { name: 'Lines', href: '/lines', icon: <Truck size={16} /> },
        { name: 'Models', href: '/models', icon: <Box size={16} /> },
        { name: 'Group Models', href: '/group-models', icon: <Package size={16} /> },
        { name: 'Model Processes', href: '/model-processes', icon: <Play size={16} /> },
        { name: 'Stations', href: '/stations', icon: <MapPin size={16} /> },
      ]
    },
    {
      name: 'Equipment Management',
      icon: <Cog size={18} />,
      children: [
        { name: 'Machines', href: '/machines', icon: <Monitor size={16} /> },
        { name: 'Machine Types', href: '/machine-types', icon: <Settings size={16} /> },
      ]
    },
    {
      name: 'User Management',
      href: '/users',
      icon: <Users size={18} />
    },
    {
      name: 'System Logs',
      href: '/logs',
      icon: <Database size={18} />
    },
    {
      name: 'Commands',
      href: '/commands',
      icon: <Terminal size={18} />
    },
  ];  const toggleGroup = (groupName: string) => {
    const groupKey = groupName.toLowerCase().replace(/\s+/g, '-');
    setExpandedGroups(prev =>
      prev.includes(groupKey)
        ? prev.filter(g => g !== groupKey)
        : [...prev, groupKey]
    );
  };  const isActiveRoute = (href?: string) => {
    if (!href) return false;
    if (href === '/dashboard') {
      return pathname === '/dashboard';
    }
    return pathname.startsWith(href);
  };

  const isGroupActive = (group: NavigationItem) => {
    if (group.href) return isActiveRoute(group.href);
    return group.children?.some(child => isActiveRoute(child.href)) || false;
  };

  const renderNavigationItem = (item: NavigationItem, level = 0) => {
    const hasChildren = item.children && item.children.length > 0;
    const groupKey = item.name.toLowerCase().replace(/\s+/g, '-');
    const isExpanded = expandedGroups.includes(groupKey);
    const isActive = hasChildren ? isGroupActive(item) : isActiveRoute(item.href);

    if (hasChildren) {
      return (
        <div key={item.name}>
          <button
            onClick={() => toggleGroup(item.name)}
            className={`${
              isActive
                ? 'bg-blue-50 border-blue-500 text-blue-700'
                : 'border-transparent text-gray-600 hover:text-gray-900 hover:bg-gray-50'
            } group flex items-center justify-between w-full px-3 py-2 text-sm font-medium border-l-4 transition-colors`}
          >
            <div className="flex items-center">
              <span className="mr-3">{item.icon}</span>
              {item.name}
            </div>
            {isExpanded ? <ChevronDown size={16} /> : <ChevronRight size={16} />}
          </button>
          {isExpanded && (
            <div className="ml-4 space-y-1">
              {item.children?.map(child => renderNavigationItem(child, level + 1))}
            </div>
          )}
        </div>
      );
    }

    return (
      <Link
        key={item.name}
        href={item.href!}
        className={`${
          isActive
            ? 'bg-blue-50 border-blue-500 text-blue-700'
            : 'border-transparent text-gray-600 hover:text-gray-900 hover:bg-gray-50'
        } group flex items-center px-3 py-2 text-sm font-medium border-l-4 transition-colors ${
          level > 0 ? 'ml-4' : ''
        }`}
      >
        <span className="mr-3">{item.icon}</span>
        {item.name}
      </Link>
    );
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Top Navigation */}
      <nav className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-3">
                <div className="w-10 h-10 bg-gradient-to-br from-blue-600 to-blue-700 rounded-xl flex items-center justify-center shadow-lg">
                  <Factory className="text-white" size={24} />
                </div>
                <div>
                  <h1 className="text-xl font-bold text-gray-900">Machine Management</h1>
                  <p className="text-xs text-gray-500">Industrial Operations System</p>
                </div>
              </div>
            </div>
            
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-3">
                <button className="p-2 text-gray-400 hover:text-gray-600 transition-colors">
                  <Search size={20} />
                </button>
                <button className="p-2 text-gray-400 hover:text-gray-600 transition-colors relative">
                  <Bell size={20} />
                  <span className="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full"></span>
                </button>
              </div>
              
              <div className="flex items-center space-x-2">
                <div className="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
                <span className="text-sm text-gray-600">Online</span>
              </div>
              
              <div className="flex items-center space-x-3 pl-4 border-l border-gray-200">
                <div className="text-sm text-right">
                  <div className="font-medium text-gray-900">{user?.username}</div>
                  <div className="text-gray-500">{user?.role}</div>
                </div>
                <button
                  onClick={logout}
                  className="flex items-center space-x-2 text-sm text-red-600 hover:text-red-800 font-medium transition-colors"
                >
                  <LogOut size={16} />
                  <span>Logout</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      </nav>

      <div className="flex">
        {/* Sidebar */}
        <div className="w-72 bg-white shadow-sm min-h-screen border-r border-gray-200">
          <nav className="mt-6 px-3">
            <div className="space-y-2">
              {navigation.map(item => renderNavigationItem(item))}
            </div>
          </nav>
        </div>

        {/* Main content */}
        <div className="flex-1">
          <main className="py-8">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
              {children}
            </div>
          </main>
        </div>
      </div>
    </div>
  );
}