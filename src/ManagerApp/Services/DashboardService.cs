using MachineManagement.Core.Interfaces;

namespace MachineManagement.ManagerApp.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardService> _logger;
        private readonly IMachineService _machineService;
        private readonly ILogService _logService;
        private readonly ICommandService _commandService;

        public DashboardService(
            IUnitOfWork unitOfWork, 
            ILogger<DashboardService> logger,
            IMachineService machineService,
            ILogService logService,
            ICommandService commandService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _machineService = machineService;
            _logService = logService;
            _commandService = commandService;
        }

        public async Task<DashboardMetrics> GetDashboardMetricsAsync()
        {
            try
            {
                var totalMachines = await _machineService.GetTotalMachineCountAsync();
                var onlineMachines = await _machineService.GetOnlineMachineCountAsync();
                var offlineMachines = await _machineService.GetOfflineMachineCountAsync();
                var totalLogs = await _logService.GetTotalLogCountAsync();
                var pendingCommands = (await _commandService.GetPendingCommandsAsync()).Count();
                var completedCommands = (await _commandService.GetCommandsByStatusAsync("Completed")).Count();

                // Calculate today's logs
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                var todayLogs = (await _logService.GetLogsByDateRangeAsync(today, tomorrow)).Count();

                return new DashboardMetrics
                {
                    TotalMachines = totalMachines,
                    OnlineMachines = onlineMachines,
                    OfflineMachines = offlineMachines,
                    ErrorMachines = Math.Max(0, totalMachines - onlineMachines - offlineMachines),
                    TotalLogs = totalLogs,
                    TodayLogs = todayLogs,
                    PendingCommands = pendingCommands,
                    CompletedCommands = completedCommands,
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard metrics");
                throw;
            }
        }

        public async Task<IEnumerable<AlertInfo>> GetActiveAlertsAsync()
        {
            try
            {
                var alerts = new List<AlertInfo>();

                // Check for offline machines (placeholder logic)
                var offlineMachines = await _machineService.GetOfflineMachineCountAsync();
                if (offlineMachines > 0)
                {
                    alerts.Add(new AlertInfo
                    {
                        Id = 1,
                        Type = "MachineOffline",
                        Message = $"{offlineMachines} machine(s) are offline",
                        Severity = "Warning",
                        CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                    });
                }

                // Check for failed commands
                var failedCommands = await _commandService.GetCommandsByStatusAsync("Failed");
                if (failedCommands.Any())
                {
                    alerts.Add(new AlertInfo
                    {
                        Id = 2,
                        Type = "CommandFailed",
                        Message = $"{failedCommands.Count()} command(s) failed execution",
                        Severity = "Error",
                        CreatedAt = DateTime.UtcNow.AddMinutes(-15)
                    });
                }

                // Check for high error log count (last hour)
                var errorLogs = await _logService.GetLogsByLevelAsync("Error", 100);
                var recentErrorLogs = errorLogs.Where(l => l.LogTimestamp > DateTime.UtcNow.AddHours(-1));
                if (recentErrorLogs.Count() > 10)
                {
                    alerts.Add(new AlertInfo
                    {
                        Id = 3,
                        Type = "HighErrorRate",
                        Message = $"{recentErrorLogs.Count()} error logs in the last hour",
                        Severity = "Warning",
                        CreatedAt = DateTime.UtcNow.AddMinutes(-5)
                    });
                }

                return alerts.OrderByDescending(a => a.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active alerts");
                return new List<AlertInfo>();
            }
        }

        public async Task<IEnumerable<RecentActivity>> GetRecentActivitiesAsync(int count = 10)
        {
            try
            {
                var activities = new List<RecentActivity>();

                // Get recent commands
                var recentCommands = await _commandService.GetRecentCommandsAsync(count / 2);
                foreach (var command in recentCommands)
                {
                    var machine = await _machineService.GetMachineByIdAsync(command.MachineId);
                    activities.Add(new RecentActivity
                    {
                        Id = command.CommandId,
                        Type = "Command",
                        Description = $"Command '{command.CommandType}' executed",
                        MachineId = command.MachineId.ToString(),
                        MachineName = machine?.MachineName ?? "Unknown",
                        Timestamp = command.ExecutedAt ?? command.CreatedAt,
                        Status = command.Status
                    });
                }

                // Get recent error logs
                var errorLogs = await _logService.GetLogsByLevelAsync("Error", count / 2);
                foreach (var log in errorLogs.Take(count / 2))
                {
                    var machine = await _machineService.GetMachineByIdAsync(log.MachineId);
                    activities.Add(new RecentActivity
                    {
                        Id = (int)log.LogId,
                        Type = "Error",
                        Description = log.Message.Length > 50 ? log.Message.Substring(0, 50) + "..." : log.Message,
                        MachineId = log.MachineId.ToString(),
                        MachineName = machine?.MachineName ?? "Unknown",
                        Timestamp = log.LogTimestamp,
                        Status = log.LogLevel
                    });
                }

                return activities.OrderByDescending(a => a.Timestamp).Take(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent activities");
                return new List<RecentActivity>();
            }
        }

        public async Task<SystemHealthStatus> GetSystemHealthAsync()
        {
            try
            {
                // Basic health checks - these would be more sophisticated in a real implementation
                var dbHealth = await CheckDatabaseHealthAsync();
                var apiHealth = true; // Placeholder
                var signalRHealth = true; // Placeholder

                return new SystemHealthStatus
                {
                    DatabaseHealth = dbHealth,
                    ApiHealth = apiHealth,
                    SignalRHealth = signalRHealth,
                    CpuUsage = GetRandomMetric(10, 80), // Placeholder
                    MemoryUsage = GetRandomMetric(30, 70), // Placeholder
                    LastChecked = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system health");
                return new SystemHealthStatus
                {
                    DatabaseHealth = false,
                    ApiHealth = false,
                    SignalRHealth = false,
                    LastChecked = DateTime.UtcNow
                };
            }
        }

        public async Task<Dictionary<string, int>> GetMachineStatusDistributionAsync()
        {
            try
            {
                // Placeholder implementation - would need actual status tracking
                var totalMachines = await _machineService.GetTotalMachineCountAsync();
                var onlineMachines = await _machineService.GetOnlineMachineCountAsync();
                var offlineMachines = await _machineService.GetOfflineMachineCountAsync();

                return new Dictionary<string, int>
                {
                    { "Online", onlineMachines },
                    { "Offline", offlineMachines },
                    { "Error", Math.Max(0, totalMachines - onlineMachines - offlineMachines) }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting machine status distribution");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetLogLevelDistributionAsync()
        {
            try
            {
                return await _logService.GetLogCountByLevelAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting log level distribution");
                throw;
            }
        }

        public async Task<Dictionary<DateTime, int>> GetLogCountByHourAsync(int hours = 24)
        {
            try
            {
                var from = DateTime.UtcNow.AddHours(-hours);
                var logs = await _logService.GetLogsByDateRangeAsync(from, DateTime.UtcNow);

                return logs.GroupBy(l => new DateTime(l.LogTimestamp.Year, l.LogTimestamp.Month, l.LogTimestamp.Day, l.LogTimestamp.Hour, 0, 0))
                          .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting log count by hour");
                throw;
            }
        }

        private async Task<bool> CheckDatabaseHealthAsync()
        {
            try
            {
                // Simple database health check
                await _unitOfWork.Machines.CountAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private double GetRandomMetric(double min, double max)
        {
            var random = new Random();
            return min + (random.NextDouble() * (max - min));
        }
    }
}