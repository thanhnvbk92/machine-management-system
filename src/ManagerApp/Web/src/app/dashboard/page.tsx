'use client';

import DashboardLayout from '@/components/DashboardLayout';
import { 
  TrendingUp, 
  TrendingDown, 
  Activity, 
  Users, 
  Factory, 
  Monitor, 
  AlertTriangle, 
  CheckCircle,
  Clock,
  BarChart3,
  Settings,
  Calendar
} from 'lucide-react';

export default function DashboardPage() {
  return (
    <DashboardLayout>
      <div className="space-y-8">
        <div className="relative overflow-hidden bg-gradient-to-r from-blue-600 via-purple-600 to-indigo-600 rounded-3xl p-8 text-white">
          <div className="relative z-10">
            <h1 className="text-4xl font-bold mb-2">Welcome to Concept Dashboard</h1>
            <p className="text-blue-100 text-lg">
              Monitor your machine management system with real-time insights and analytics
            </p>
          </div>
          <div className="absolute top-0 right-0 -mt-4 -mr-4 opacity-20">
            <div className="w-32 h-32 bg-white rounded-full"></div>
          </div>
          <div className="absolute bottom-0 left-0 -mb-8 -ml-8 opacity-10">
            <div className="w-64 h-64 bg-white rounded-full"></div>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <div className="relative overflow-hidden bg-white rounded-2xl shadow-soft border border-slate-100 hover:shadow-medium transition-all duration-300">
            <div className="p-6">
              <div className="flex items-center justify-between mb-4">
                <div className="p-3 rounded-xl bg-gradient-to-r from-blue-500 to-blue-600 text-white shadow-medium">
                  <Factory className="w-6 h-6" />
                </div>
                <div className="flex items-center space-x-1 text-sm font-medium text-green-600">
                  <TrendingUp className="w-4 h-4" />
                  <span>+12%</span>
                </div>
              </div>
              <div>
                <h3 className="text-3xl font-bold text-slate-900 mb-1">248</h3>
                <p className="text-slate-500 font-medium">Total Machines</p>
              </div>
            </div>
            <div className="absolute bottom-0 left-0 right-0 h-1 bg-gradient-to-r from-slate-100 to-slate-200"></div>
          </div>

          <div className="relative overflow-hidden bg-white rounded-2xl shadow-soft border border-slate-100 hover:shadow-medium transition-all duration-300">
            <div className="p-6">
              <div className="flex items-center justify-between mb-4">
                <div className="p-3 rounded-xl bg-gradient-to-r from-green-500 to-green-600 text-white shadow-medium">
                  <Users className="w-6 h-6" />
                </div>
                <div className="flex items-center space-x-1 text-sm font-medium text-green-600">
                  <TrendingUp className="w-4 h-4" />
                  <span>+8%</span>
                </div>
              </div>
              <div>
                <h3 className="text-3xl font-bold text-slate-900 mb-1">1,429</h3>
                <p className="text-slate-500 font-medium">Active Users</p>
              </div>
            </div>
            <div className="absolute bottom-0 left-0 right-0 h-1 bg-gradient-to-r from-slate-100 to-slate-200"></div>
          </div>

          <div className="relative overflow-hidden bg-white rounded-2xl shadow-soft border border-slate-100 hover:shadow-medium transition-all duration-300">
            <div className="p-6">
              <div className="flex items-center justify-between mb-4">
                <div className="p-3 rounded-xl bg-gradient-to-r from-purple-500 to-purple-600 text-white shadow-medium">
                  <Activity className="w-6 h-6" />
                </div>
                <div className="flex items-center space-x-1 text-sm font-medium text-green-600">
                  <TrendingUp className="w-4 h-4" />
                  <span>+0.2%</span>
                </div>
              </div>
              <div>
                <h3 className="text-3xl font-bold text-slate-900 mb-1">99.8%</h3>
                <p className="text-slate-500 font-medium">System Uptime</p>
              </div>
            </div>
            <div className="absolute bottom-0 left-0 right-0 h-1 bg-gradient-to-r from-slate-100 to-slate-200"></div>
          </div>

          <div className="relative overflow-hidden bg-white rounded-2xl shadow-soft border border-slate-100 hover:shadow-medium transition-all duration-300">
            <div className="p-6">
              <div className="flex items-center justify-between mb-4">
                <div className="p-3 rounded-xl bg-gradient-to-r from-orange-500 to-orange-600 text-white shadow-medium">
                  <AlertTriangle className="w-6 h-6" />
                </div>
                <div className="flex items-center space-x-1 text-sm font-medium text-red-600">
                  <TrendingDown className="w-4 h-4" />
                  <span>-25%</span>
                </div>
              </div>
              <div>
                <h3 className="text-3xl font-bold text-slate-900 mb-1">12</h3>
                <p className="text-slate-500 font-medium">Alerts</p>
              </div>
            </div>
            <div className="absolute bottom-0 left-0 right-0 h-1 bg-gradient-to-r from-slate-100 to-slate-200"></div>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          <div className="bg-white rounded-2xl shadow-soft border border-slate-100 p-6">
            <div className="flex items-center justify-between mb-6">
              <div>
                <h3 className="text-xl font-bold text-slate-900">Machine Performance</h3>
                <p className="text-slate-500">Last 30 days overview</p>
              </div>
              <div className="flex items-center space-x-2">
                <button className="p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-100 rounded-lg transition-colors">
                  <BarChart3 className="w-5 h-5" />
                </button>
                <button className="p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-100 rounded-lg transition-colors">
                  <Settings className="w-5 h-5" />
                </button>
              </div>
            </div>
            <div className="h-64 flex items-center justify-center bg-gradient-to-br from-slate-50 to-slate-100 rounded-xl">
              <div className="text-center">
                <BarChart3 className="w-16 h-16 text-slate-300 mx-auto mb-4" />
                <p className="text-slate-500 font-medium">Chart visualization would go here</p>
                <p className="text-slate-400 text-sm">Integration with Chart.js or similar</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-2xl shadow-soft border border-slate-100 p-6">
            <div className="flex items-center justify-between mb-6">
              <div>
                <h3 className="text-xl font-bold text-slate-900">Recent Activity</h3>
                <p className="text-slate-500">Latest system events</p>
              </div>
              <button className="text-blue-600 hover:text-blue-700 font-medium text-sm transition-colors">
                View All
              </button>
            </div>
            <div className="space-y-4">
              <div className="flex items-start space-x-3 p-3 rounded-xl hover:bg-slate-50 transition-colors">
                <div className="mt-1">
                  <CheckCircle className="w-5 h-5 text-green-500" />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-slate-900">Machine M-001 maintenance completed</p>
                  <div className="flex items-center space-x-2 mt-1">
                    <Clock className="w-3 h-3 text-slate-400" />
                    <p className="text-xs text-slate-500">2 hours ago</p>
                  </div>
                </div>
              </div>
              <div className="flex items-start space-x-3 p-3 rounded-xl hover:bg-slate-50 transition-colors">
                <div className="mt-1">
                  <AlertTriangle className="w-5 h-5 text-orange-500" />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-slate-900">Temperature warning on Line 3</p>
                  <div className="flex items-center space-x-2 mt-1">
                    <Clock className="w-3 h-3 text-slate-400" />
                    <p className="text-xs text-slate-500">4 hours ago</p>
                  </div>
                </div>
              </div>
              <div className="flex items-start space-x-3 p-3 rounded-xl hover:bg-slate-50 transition-colors">
                <div className="mt-1">
                  <Monitor className="w-5 h-5 text-blue-500" />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-slate-900">New machine added to inventory</p>
                  <div className="flex items-center space-x-2 mt-1">
                    <Clock className="w-3 h-3 text-slate-400" />
                    <p className="text-xs text-slate-500">6 hours ago</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white rounded-2xl shadow-soft border border-slate-100 p-6">
          <h3 className="text-xl font-bold text-slate-900 mb-6">Quick Actions</h3>
          <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-6 gap-4">
            <button className="group flex flex-col items-center p-4 rounded-xl border border-slate-200 hover:border-slate-300 hover:shadow-soft transition-all duration-200">
              <div className="p-3 rounded-xl bg-gradient-to-r from-blue-500 to-blue-600 text-white mb-3 group-hover:scale-110 transition-transform">
                <Factory className="w-6 h-6" />
              </div>
              <span className="text-sm font-medium text-slate-700 group-hover:text-slate-900">Add Machine</span>
            </button>
            <button className="group flex flex-col items-center p-4 rounded-xl border border-slate-200 hover:border-slate-300 hover:shadow-soft transition-all duration-200">
              <div className="p-3 rounded-xl bg-gradient-to-r from-green-500 to-green-600 text-white mb-3 group-hover:scale-110 transition-transform">
                <Users className="w-6 h-6" />
              </div>
              <span className="text-sm font-medium text-slate-700 group-hover:text-slate-900">Manage Users</span>
            </button>
            <button className="group flex flex-col items-center p-4 rounded-xl border border-slate-200 hover:border-slate-300 hover:shadow-soft transition-all duration-200">
              <div className="p-3 rounded-xl bg-gradient-to-r from-purple-500 to-purple-600 text-white mb-3 group-hover:scale-110 transition-transform">
                <BarChart3 className="w-6 h-6" />
              </div>
              <span className="text-sm font-medium text-slate-700 group-hover:text-slate-900">View Reports</span>
            </button>
            <button className="group flex flex-col items-center p-4 rounded-xl border border-slate-200 hover:border-slate-300 hover:shadow-soft transition-all duration-200">
              <div className="p-3 rounded-xl bg-gradient-to-r from-orange-500 to-orange-600 text-white mb-3 group-hover:scale-110 transition-transform">
                <Settings className="w-6 h-6" />
              </div>
              <span className="text-sm font-medium text-slate-700 group-hover:text-slate-900">System Config</span>
            </button>
            <button className="group flex flex-col items-center p-4 rounded-xl border border-slate-200 hover:border-slate-300 hover:shadow-soft transition-all duration-200">
              <div className="p-3 rounded-xl bg-gradient-to-r from-teal-500 to-teal-600 text-white mb-3 group-hover:scale-110 transition-transform">
                <Calendar className="w-6 h-6" />
              </div>
              <span className="text-sm font-medium text-slate-700 group-hover:text-slate-900">Schedule</span>
            </button>
            <button className="group flex flex-col items-center p-4 rounded-xl border border-slate-200 hover:border-slate-300 hover:shadow-soft transition-all duration-200">
              <div className="p-3 rounded-xl bg-gradient-to-r from-red-500 to-red-600 text-white mb-3 group-hover:scale-110 transition-transform">
                <AlertTriangle className="w-6 h-6" />
              </div>
              <span className="text-sm font-medium text-slate-700 group-hover:text-slate-900">Alerts</span>
            </button>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
