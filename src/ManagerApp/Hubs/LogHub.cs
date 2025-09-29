using Microsoft.AspNetCore.SignalR;

namespace MachineManagement.ManagerApp.Hubs
{
    public class LogHub : Hub
    {
        private readonly ILogger<LogHub> _logger;

        public LogHub(ILogger<LogHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinMachineLogGroup(int machineId)
        {
            var groupName = $"Machine_{machineId}_Logs";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Client {ConnectionId} joined log group for machine {MachineId}", Context.ConnectionId, machineId);
        }

        public async Task LeaveMachineLogGroup(int machineId)
        {
            var groupName = $"Machine_{machineId}_Logs";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Client {ConnectionId} left log group for machine {MachineId}", Context.ConnectionId, machineId);
        }

        public async Task JoinLogLevelGroup(string logLevel)
        {
            var groupName = $"LogLevel_{logLevel}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Client {ConnectionId} joined log level group {LogLevel}", Context.ConnectionId, logLevel);
        }

        public async Task LeaveLogLevelGroup(string logLevel)
        {
            var groupName = $"LogLevel_{logLevel}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Client {ConnectionId} left log level group {LogLevel}", Context.ConnectionId, logLevel);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected to LogHub: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogWarning(exception, "Client disconnected from LogHub with error: {ConnectionId}", Context.ConnectionId);
            }
            else
            {
                _logger.LogInformation("Client disconnected from LogHub: {ConnectionId}", Context.ConnectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        // Methods to send log updates to clients
        public async Task SendNewLogEntry(object logEntry)
        {
            await Clients.All.SendAsync("NewLogEntry", logEntry);
        }

        public async Task SendLogToMachineGroup(int machineId, object logEntry)
        {
            var groupName = $"Machine_{machineId}_Logs";
            await Clients.Group(groupName).SendAsync("NewLogEntry", logEntry);
        }

        public async Task SendLogToLevelGroup(string logLevel, object logEntry)
        {
            var groupName = $"LogLevel_{logLevel}";
            await Clients.Group(groupName).SendAsync("NewLogEntry", logEntry);
        }
    }
}