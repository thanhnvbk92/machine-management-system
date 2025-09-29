using Microsoft.AspNetCore.SignalR;

namespace MachineManagement.ManagerApp.Hubs
{
    public class CommandHub : Hub
    {
        private readonly ILogger<CommandHub> _logger;

        public CommandHub(ILogger<CommandHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinMachineCommandGroup(int machineId)
        {
            var groupName = $"Machine_{machineId}_Commands";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Client {ConnectionId} joined command group for machine {MachineId}", Context.ConnectionId, machineId);
        }

        public async Task LeaveMachineCommandGroup(int machineId)
        {
            var groupName = $"Machine_{machineId}_Commands";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Client {ConnectionId} left command group for machine {MachineId}", Context.ConnectionId, machineId);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected to CommandHub: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogWarning(exception, "Client disconnected from CommandHub with error: {ConnectionId}", Context.ConnectionId);
            }
            else
            {
                _logger.LogInformation("Client disconnected from CommandHub: {ConnectionId}", Context.ConnectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        // Methods to send command updates to clients
        public async Task SendCommandStatusUpdate(int commandId, string status, object? result = null)
        {
            await Clients.All.SendAsync("CommandStatusUpdated", commandId, status, result);
        }

        public async Task SendCommandToMachineGroup(int machineId, object command)
        {
            var groupName = $"Machine_{machineId}_Commands";
            await Clients.Group(groupName).SendAsync("NewCommand", command);
        }

        public async Task SendCommandExecutionResult(int machineId, int commandId, string status, object? result = null, string? errorMessage = null)
        {
            var groupName = $"Machine_{machineId}_Commands";
            await Clients.Group(groupName).SendAsync("CommandExecutionResult", commandId, status, result, errorMessage);
        }

        public async Task NotifyCommandCreated(object command)
        {
            await Clients.All.SendAsync("CommandCreated", command);
        }

        public async Task NotifyBatchCommandProgress(string batchId, int completed, int total)
        {
            await Clients.All.SendAsync("BatchCommandProgress", batchId, completed, total);
        }
    }
}