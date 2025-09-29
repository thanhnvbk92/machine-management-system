namespace MachineManagement.ManagerApp.Services
{
    public interface IDashboardService
    {
        Task<DashboardMetrics> GetDashboardMetricsAsync();
        Task<IEnumerable<AlertInfo>> GetActiveAlertsAsync();
        Task<IEnumerable<RecentActivity>> GetRecentActivitiesAsync(int count = 10);
        Task<SystemHealthStatus> GetSystemHealthAsync();
        Task<Dictionary<string, int>> GetMachineStatusDistributionAsync();
        Task<Dictionary<string, int>> GetLogLevelDistributionAsync();
        Task<Dictionary<DateTime, int>> GetLogCountByHourAsync(int hours = 24);
    }

    public class DashboardMetrics
    {
        public int TotalMachines { get; set; }
        public int OnlineMachines { get; set; }
        public int OfflineMachines { get; set; }
        public int ErrorMachines { get; set; }
        public long TotalLogs { get; set; }
        public long TodayLogs { get; set; }
        public int PendingCommands { get; set; }
        public int CompletedCommands { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class AlertInfo
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = "Info";
        public DateTime CreatedAt { get; set; }
        public string? MachineId { get; set; }
    }

    public class RecentActivity
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? MachineId { get; set; }
        public string? MachineName { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Status { get; set; }
    }

    public class SystemHealthStatus
    {
        public bool DatabaseHealth { get; set; }
        public bool ApiHealth { get; set; }
        public bool SignalRHealth { get; set; }
        public double CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
        public DateTime LastChecked { get; set; } = DateTime.UtcNow;
    }
}