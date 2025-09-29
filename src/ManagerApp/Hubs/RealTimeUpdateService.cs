using MachineManagement.ManagerApp.Models;
using MachineManagement.ManagerApp.Services;
using Microsoft.AspNetCore.SignalR;

namespace MachineManagement.ManagerApp.Hubs;

/// <summary>
/// Background service for sending real-time updates via SignalR
/// </summary>
public class RealTimeUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<MachineHub> _hubContext;
    private readonly ILogger<RealTimeUpdateService> _logger;
    private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(30);

    public RealTimeUpdateService(
        IServiceProvider serviceProvider,
        IHubContext<MachineHub> hubContext,
        ILogger<RealTimeUpdateService> logger)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Real-time update service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendDashboardUpdates();
                await SendMachineUpdates();
                
                await Task.Delay(_updateInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in real-time update service");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("Real-time update service stopped");
    }

    private async Task SendDashboardUpdates()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var stats = await dashboardService.GetStatsAsync();
            await _hubContext.Clients.Group("DashboardUpdates")
                .SendAsync("DashboardStats", stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending dashboard updates");
        }
    }

    private async Task SendMachineUpdates()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var machineService = scope.ServiceProvider.GetRequiredService<IMachineService>();

            var machines = await machineService.GetAllMachinesAsync();
            
            // Send to all machine subscribers
            await _hubContext.Clients.Group("MachineUpdates")
                .SendAsync("MachineStatusUpdate", machines);

            // Send to specific machine subscribers
            foreach (var machine in machines)
            {
                await _hubContext.Clients.Group($"Machine_{machine.MachineId}")
                    .SendAsync("MachineStatusUpdate", new[] { machine });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending machine updates");
        }
    }

    public async Task SendLogUpdate(LogEntryDto logEntry)
    {
        try
        {
            // Send to all log subscribers
            await _hubContext.Clients.Group("LogUpdates")
                .SendAsync("NewLogEntry", logEntry);

            // Send to level-specific subscribers
            await _hubContext.Clients.Group($"LogUpdates_{logEntry.Level}")
                .SendAsync("NewLogEntry", logEntry);

            // Send to machine-specific subscribers
            if (!string.IsNullOrEmpty(logEntry.MachineId))
            {
                await _hubContext.Clients.Group($"LogUpdates_{logEntry.MachineId}")
                    .SendAsync("NewLogEntry", logEntry);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending log update");
        }
    }

    public async Task SendCommandUpdate(CommandDto command)
    {
        try
        {
            await _hubContext.Clients.All
                .SendAsync("CommandStatusUpdate", command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending command update");
        }
    }
}