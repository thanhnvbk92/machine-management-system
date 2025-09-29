using Microsoft.AspNetCore.SignalR;

namespace MachineManagement.ManagerApp.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected to NotificationHub: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogWarning(exception, "Client disconnected from NotificationHub with error: {ConnectionId}", Context.ConnectionId);
            }
            else
            {
                _logger.LogInformation("Client disconnected from NotificationHub: {ConnectionId}", Context.ConnectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        // Methods to send notifications to clients
        public async Task SendAlert(string type, string message, string severity = "Info", int? machineId = null)
        {
            var alert = new
            {
                Type = type,
                Message = message,
                Severity = severity,
                MachineId = machineId,
                Timestamp = DateTime.UtcNow
            };
            
            await Clients.All.SendAsync("AlertReceived", alert);
        }

        public async Task SendSystemNotification(string title, string message, string type = "Info")
        {
            var notification = new
            {
                Title = title,
                Message = message,
                Type = type,
                Timestamp = DateTime.UtcNow
            };
            
            await Clients.All.SendAsync("SystemNotification", notification);
        }

        public async Task SendDashboardUpdate(object dashboardData)
        {
            await Clients.All.SendAsync("DashboardUpdated", dashboardData);
        }

        public async Task SendMachineHealthAlert(int machineId, string machineName, string status, string message)
        {
            var healthAlert = new
            {
                MachineId = machineId,
                MachineName = machineName,
                Status = status,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            
            await Clients.All.SendAsync("MachineHealthAlert", healthAlert);
        }

        public async Task SendPerformanceAlert(string metric, double value, double threshold, string severity)
        {
            var performanceAlert = new
            {
                Metric = metric,
                Value = value,
                Threshold = threshold,
                Severity = severity,
                Timestamp = DateTime.UtcNow
            };
            
            await Clients.All.SendAsync("PerformanceAlert", performanceAlert);
        }

        public async Task SendMaintenanceReminder(int machineId, string machineName, string maintenanceType, DateTime dueDate)
        {
            var reminder = new
            {
                MachineId = machineId,
                MachineName = machineName,
                MaintenanceType = maintenanceType,
                DueDate = dueDate,
                Timestamp = DateTime.UtcNow
            };
            
            await Clients.All.SendAsync("MaintenanceReminder", reminder);
        }
    }
}