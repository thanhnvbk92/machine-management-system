// JavaScript functions for Machine Management System

// Chart.js configuration and updates
window.updateMachineStatusChart = (onlineCount, offlineCount) => {
    const ctx = document.getElementById('machineStatusChart');
    if (!ctx) return;

    // Destroy existing chart if exists
    if (window.machineStatusChart) {
        window.machineStatusChart.destroy();
    }

    window.machineStatusChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Online', 'Offline'],
            datasets: [{
                data: [onlineCount, offlineCount],
                backgroundColor: [
                    '#4caf50',  // Green for online
                    '#f44336'   // Red for offline
                ],
                borderWidth: 0,
                cutout: '60%'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 20,
                        font: {
                            size: 14
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const label = context.label || '';
                            const value = context.parsed;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((value / total) * 100).toFixed(1);
                            return `${label}: ${value} (${percentage}%)`;
                        }
                    }
                }
            },
            animation: {
                animateRotate: true,
                duration: 1000
            }
        }
    });
};

// Real-time data updates
window.updateRealTimeData = (elementId, value) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.textContent = value;
        element.classList.add('pulse');
        setTimeout(() => {
            element.classList.remove('pulse');
        }, 1000);
    }
};

// Notification system
window.showNotification = (title, message, type = 'info') => {
    // This would integrate with browser notifications if permitted
    if ('Notification' in window && Notification.permission === 'granted') {
        const notification = new Notification(title, {
            body: message,
            icon: '/favicon.ico',
            badge: '/favicon.ico'
        });
        
        setTimeout(() => {
            notification.close();
        }, 5000);
    }
};

// Request notification permission
window.requestNotificationPermission = () => {
    if ('Notification' in window && Notification.permission === 'default') {
        Notification.requestPermission();
    }
};

// Local storage helpers
window.localStorage = {
    get: (key) => {
        return localStorage.getItem(key);
    },
    set: (key, value) => {
        localStorage.setItem(key, value);
    },
    remove: (key) => {
        localStorage.removeItem(key);
    },
    clear: () => {
        localStorage.clear();
    }
};

// Auto-refresh functionality
window.autoRefresh = {
    interval: null,
    start: (callback, intervalMs = 30000) => {
        window.autoRefresh.stop();
        window.autoRefresh.interval = setInterval(callback, intervalMs);
    },
    stop: () => {
        if (window.autoRefresh.interval) {
            clearInterval(window.autoRefresh.interval);
            window.autoRefresh.interval = null;
        }
    }
};

// Utility functions
window.utils = {
    formatDateTime: (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleString();
    },
    
    formatDuration: (seconds) => {
        const hours = Math.floor(seconds / 3600);
        const minutes = Math.floor((seconds % 3600) / 60);
        const secs = Math.floor(seconds % 60);
        
        if (hours > 0) {
            return `${hours}h ${minutes}m ${secs}s`;
        } else if (minutes > 0) {
            return `${minutes}m ${secs}s`;
        } else {
            return `${secs}s`;
        }
    },
    
    copyToClipboard: (text) => {
        navigator.clipboard.writeText(text).then(() => {
            console.log('Text copied to clipboard');
        }).catch(err => {
            console.error('Failed to copy text: ', err);
        });
    },
    
    downloadJson: (data, filename) => {
        const dataStr = JSON.stringify(data, null, 2);
        const dataBlob = new Blob([dataStr], { type: 'application/json' });
        
        const link = document.createElement('a');
        link.href = URL.createObjectURL(dataBlob);
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
};

// Initialize app when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    console.log('Machine Management System initialized');
    
    // Request notification permission
    window.requestNotificationPermission();
    
    // Add any global event listeners here
    window.addEventListener('beforeunload', () => {
        // Stop any running intervals
        window.autoRefresh.stop();
    });
});

// Error handling
window.addEventListener('error', (event) => {
    console.error('Global error:', event.error);
});

window.addEventListener('unhandledrejection', (event) => {
    console.error('Unhandled promise rejection:', event.reason);
});