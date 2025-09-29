// Machine Management System JavaScript utilities

window.machineManagement = {
    // SignalR connection helpers
    signalR: {
        connections: {},
        
        // Initialize SignalR connection
        initializeConnection: function(hubUrl, hubName) {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl(hubUrl)
                .withAutomaticReconnect([0, 2000, 10000, 30000])
                .build();

            // Handle connection state changes
            connection.onreconnecting((error) => {
                console.log(`Connection lost due to error "${error}". Reconnecting.`);
                this.showConnectionStatus('Reconnecting...', 'warning');
            });

            connection.onreconnected((connectionId) => {
                console.log(`Connection reestablished. Connected with connectionId "${connectionId}".`);
                this.showConnectionStatus('Connected', 'success');
            });

            connection.onclose((error) => {
                console.log(`Connection closed due to error "${error}". Try refreshing this page to restart the connection.`);
                this.showConnectionStatus('Disconnected', 'error');
            });

            this.connections[hubName] = connection;
            return connection;
        },

        // Show connection status
        showConnectionStatus: function(message, type) {
            // Integration with MudBlazor snackbar would go here
            console.log(`SignalR Status: ${message} (${type})`);
        }
    },

    // Dashboard utilities
    dashboard: {
        // Auto-refresh dashboard data
        startAutoRefresh: function(intervalMs, refreshCallback) {
            if (this.refreshInterval) {
                clearInterval(this.refreshInterval);
            }
            
            this.refreshInterval = setInterval(() => {
                if (typeof refreshCallback === 'function') {
                    refreshCallback();
                }
            }, intervalMs);
        },

        stopAutoRefresh: function() {
            if (this.refreshInterval) {
                clearInterval(this.refreshInterval);
                this.refreshInterval = null;
            }
        },

        // Chart utilities (placeholder for Chart.js integration)
        createChart: function(canvasId, chartConfig) {
            // Chart.js implementation would go here
            console.log(`Creating chart for ${canvasId}`, chartConfig);
        }
    },

    // Notification helpers
    notifications: {
        // Show toast notification
        showToast: function(message, type = 'info', duration = 5000) {
            // This would integrate with MudBlazor's snackbar
            console.log(`Toast: ${message} (${type})`);
        },

        // Show browser notification (if permitted)
        showBrowserNotification: function(title, message, icon = null) {
            if ('Notification' in window && Notification.permission === 'granted') {
                new Notification(title, {
                    body: message,
                    icon: icon || '/favicon.ico',
                    badge: '/favicon.ico',
                    timestamp: Date.now()
                });
            }
        },

        // Request notification permission
        requestNotificationPermission: function() {
            if ('Notification' in window && Notification.permission === 'default') {
                return Notification.requestPermission();
            }
            return Promise.resolve(Notification.permission);
        }
    },

    // Utility functions
    utils: {
        // Format date/time
        formatDateTime: function(dateString, format = 'short') {
            const date = new Date(dateString);
            if (format === 'short') {
                return date.toLocaleString();
            } else if (format === 'time') {
                return date.toLocaleTimeString();
            } else if (format === 'date') {
                return date.toLocaleDateString();
            }
            return date.toString();
        },

        // Format file size
        formatFileSize: function(bytes) {
            const sizes = ['Bytes', 'KB', 'MB', 'GB'];
            if (bytes === 0) return '0 Bytes';
            const i = Math.floor(Math.log(bytes) / Math.log(1024));
            return Math.round(bytes / Math.pow(1024, i) * 100) / 100 + ' ' + sizes[i];
        },

        // Debounce function
        debounce: function(func, wait) {
            let timeout;
            return function executedFunction(...args) {
                const later = () => {
                    clearTimeout(timeout);
                    func(...args);
                };
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
            };
        },

        // Copy to clipboard
        copyToClipboard: function(text) {
            if (navigator.clipboard && window.isSecureContext) {
                return navigator.clipboard.writeText(text);
            } else {
                // Fallback for older browsers
                const textArea = document.createElement('textarea');
                textArea.value = text;
                textArea.style.position = 'absolute';
                textArea.style.left = '-999999px';
                document.body.appendChild(textArea);
                textArea.select();
                document.execCommand('copy');
                document.body.removeChild(textArea);
                return Promise.resolve();
            }
        },

        // Export data as CSV
        exportToCSV: function(data, filename = 'export.csv') {
            const csv = this.convertToCSV(data);
            const blob = new Blob([csv], { type: 'text/csv' });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = filename;
            link.click();
            window.URL.revokeObjectURL(url);
        },

        // Convert array of objects to CSV
        convertToCSV: function(data) {
            if (!data || !data.length) return '';
            
            const headers = Object.keys(data[0]);
            const csvContent = [
                headers.join(','),
                ...data.map(row => 
                    headers.map(header => {
                        const value = row[header];
                        return typeof value === 'string' && value.includes(',') 
                            ? `"${value}"` 
                            : value;
                    }).join(',')
                )
            ].join('\n');
            
            return csvContent;
        }
    },

    // Local storage helpers
    storage: {
        // Set item with expiration
        setWithExpiry: function(key, value, ttl) {
            const now = new Date();
            const item = {
                value: value,
                expiry: now.getTime() + ttl,
            };
            localStorage.setItem(key, JSON.stringify(item));
        },

        // Get item with expiration check
        getWithExpiry: function(key) {
            const itemStr = localStorage.getItem(key);
            if (!itemStr) {
                return null;
            }
            const item = JSON.parse(itemStr);
            const now = new Date();
            if (now.getTime() > item.expiry) {
                localStorage.removeItem(key);
                return null;
            }
            return item.value;
        }
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('Machine Management System initialized');
    
    // Request notification permission
    machineManagement.notifications.requestNotificationPermission();
});

// Handle Blazor errors
window.Blazor?.addEventListener('error', event => {
    console.error('Blazor error:', event.detail.error);
    machineManagement.notifications.showToast('An error occurred. Please try again.', 'error');
});