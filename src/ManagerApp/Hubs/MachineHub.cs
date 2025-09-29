using Microsoft.AspNetCore.SignalR;
using MachineManagement.ManagerApp.Services;

namespace MachineManagement.ManagerApp.Hubs
{
    public class MachineHub : Hub
    {
        private readonly IMachineService _machineService;
        private readonly ILogger<MachineHub> _logger;

        public MachineHub(IMachineService machineService, ILogger<MachineHub> logger)
        {
            _machineService = machineService;
            _logger = logger;
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Client {ConnectionId} joined group {GroupName}", Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Client {ConnectionId} left group {GroupName}", Context.ConnectionId, groupName);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogWarning(exception, "Client disconnected with error: {ConnectionId}", Context.ConnectionId);
            }
            else
            {
                _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        // Methods to send updates to clients
        public async Task SendMachineStatusUpdate(int machineId, string status)
        {
            await Clients.All.SendAsync("MachineStatusUpdated", machineId, status);
        }

        public async Task SendMachineHeartbeat(int machineId, DateTime timestamp)
        {
            await Clients.All.SendAsync("MachineHeartbeatReceived", machineId, timestamp);
        }

        public async Task SendMachineMetricsUpdate(object metrics)
        {
            await Clients.All.SendAsync("MachineMetricsUpdated", metrics);
        }
    }
}