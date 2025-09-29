using MachineManagement.ManagerApp.Services;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace MachineManagement.ManagerApp.Hubs;

/// <summary>
/// SignalR Hub for real-time machine monitoring
/// </summary>
public class MachineHub : Hub
{
    private readonly IMachineService _machineService;
    private readonly ILogService _logService;
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<MachineHub> _logger;

    public MachineHub(
        IMachineService machineService,
        ILogService logService,
        IDashboardService dashboardService,
        ILogger<MachineHub> logger)
    {
        _machineService = machineService;
        _logService = logService;
        _dashboardService = dashboardService;
        _logger = logger;
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} joined group {GroupName}", 
            Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} left group {GroupName}", 
            Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Subscribe to all machine updates
    /// </summary>
    public async Task SubscribeToMachineUpdates()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "MachineUpdates");
        _logger.LogInformation("Client {ConnectionId} subscribed to machine updates", Context.ConnectionId);
    }

    /// <summary>
    /// Subscribe to specific machine updates
    /// </summary>
    public async Task SubscribeToMachine(string machineId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Machine_{machineId}");
        _logger.LogInformation("Client {ConnectionId} subscribed to machine {MachineId}", 
            Context.ConnectionId, machineId);
    }

    /// <summary>
    /// Subscribe to log updates
    /// </summary>
    public async Task SubscribeToLogs(string? level = null, string? machineId = null)
    {
        var groupName = "LogUpdates";
        if (!string.IsNullOrEmpty(level))
            groupName += $"_{level}";
        if (!string.IsNullOrEmpty(machineId))
            groupName += $"_{machineId}";

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} subscribed to logs: {GroupName}", 
            Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Subscribe to dashboard updates
    /// </summary>
    public async Task SubscribeToDashboard()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "DashboardUpdates");
        _logger.LogInformation("Client {ConnectionId} subscribed to dashboard updates", Context.ConnectionId);
        
        // Send initial dashboard data
        var stats = await _dashboardService.GetStatsAsync();
        await Clients.Caller.SendAsync("DashboardStats", stats);
    }

    /// <summary>
    /// Request current machine status
    /// </summary>
    public async Task RequestMachineStatus(string? machineId = null)
    {
        try
        {
            if (string.IsNullOrEmpty(machineId))
            {
                var machines = await _machineService.GetAllMachinesAsync();
                await Clients.Caller.SendAsync("MachineStatusUpdate", machines);
            }
            else
            {
                var machine = await _machineService.GetMachineByIdAsync(machineId);
                if (machine != null)
                {
                    await Clients.Caller.SendAsync("MachineStatusUpdate", new[] { machine });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting machine status for {MachineId}", machineId);
        }
    }

    /// <summary>
    /// Send ping to check connection
    /// </summary>
    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}. Exception: {Exception}", 
            Context.ConnectionId, exception?.Message);
        await base.OnDisconnectedAsync(exception);
    }
}