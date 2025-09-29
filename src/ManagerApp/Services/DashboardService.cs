using MachineManagement.ManagerApp.Models;

namespace MachineManagement.ManagerApp.Services;

/// <summary>
/// Service for dashboard operations
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IMachineService _machineService;
    private readonly ILogService _logService;
    private readonly ICommandService _commandService;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        IMachineService machineService,
        ILogService logService,
        ICommandService commandService,
        ILogger<DashboardService> logger)
    {
        _machineService = machineService;
        _logService = logService;
        _commandService = commandService;
        _logger = logger;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching dashboard statistics");
            
            // Get all machines to calculate online/offline counts
            var machines = await _machineService.GetAllMachinesAsync();
            var onlineMachines = machines.Count(m => m.IsOnline);
            
            // Get today's logs
            var todayStart = DateTime.Today;
            var todayEnd = todayStart.AddDays(1).AddTicks(-1);
            
            var todayLogs = await _logService.GetLogsAsync(new LogQueryRequest
            {
                StartTime = todayStart,
                EndTime = todayEnd,
                PageSize = 1000
            });

            var errorLogs = await _logService.GetLogsAsync(new LogQueryRequest
            {
                Level = "Error",
                StartTime = todayStart,
                EndTime = todayEnd,
                PageSize = 1000
            });

            var warningLogs = await _logService.GetLogsAsync(new LogQueryRequest
            {
                Level = "Warning", 
                StartTime = todayStart,
                EndTime = todayEnd,
                PageSize = 1000
            });

            // Get commands
            var allCommands = await _commandService.GetCommandsAsync();
            var pendingCommands = allCommands.Count(c => c.Status == "Pending");
            var completedToday = allCommands.Count(c => 
                c.Status == "Completed" && c.ExecutedAt >= todayStart);

            return new DashboardStatsDto
            {
                TotalMachines = machines.Count,
                OnlineMachines = onlineMachines,
                OfflineMachines = machines.Count - onlineMachines,
                TotalLogsToday = todayLogs.TotalCount,
                TotalErrors = errorLogs.TotalCount,
                TotalWarnings = warningLogs.TotalCount,
                PendingCommands = pendingCommands,
                CompletedCommandsToday = completedToday,
                AverageResponseTime = CalculateAverageResponseTime(machines)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching dashboard stats");
            return new DashboardStatsDto();
        }
    }

    public async Task<IList<MachineDto>> GetRecentMachineUpdatesAsync(int count = 10)
    {
        try
        {
            _logger.LogInformation("Fetching recent machine updates");
            var machines = await _machineService.GetAllMachinesAsync();
            
            return machines
                .OrderByDescending(m => m.LastHeartbeat)
                .Take(count)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent machine updates");
            return new List<MachineDto>();
        }
    }

    public async Task<IList<LogEntryDto>> GetRecentErrorsAsync(int count = 10)
    {
        try
        {
            _logger.LogInformation("Fetching recent errors");
            var errorLogs = await _logService.GetLogsAsync(new LogQueryRequest
            {
                Level = "Error",
                PageSize = count
            });

            return errorLogs.Items;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent errors");
            return new List<LogEntryDto>();
        }
    }

    public async Task<IList<CommandDto>> GetRecentCommandsAsync(int count = 10)
    {
        try
        {
            _logger.LogInformation("Fetching recent commands");
            var commands = await _commandService.GetCommandsAsync();
            
            return commands
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent commands");
            return new List<CommandDto>();
        }
    }

    private static double CalculateAverageResponseTime(IList<MachineDto> machines)
    {
        if (machines.Count == 0) return 0;
        
        var responseTimes = machines
            .Where(m => m.IsOnline && m.Uptime.HasValue)
            .Select(m => m.Uptime!.Value.TotalSeconds);
        
        return responseTimes.Any() ? responseTimes.Average() : 0;
    }
}