using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MachineClient.WPF.Services;
using MachineClient.WPF.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;

namespace MachineClient.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IApiService _apiService;
        private readonly ILogCollectionService _logService;
        private readonly IConfigurationService _configService;
        private readonly ILogger<MainViewModel> _logger;
        private readonly Timer _heartbeatTimer;
        private readonly Timer _logCollectionTimer;

        [ObservableProperty]
        private string _machineId = "MACHINE_001";

        [ObservableProperty]
        private string _stationName = "Station A";

        [ObservableProperty]
        private string _lineName = "Line 1";

        [ObservableProperty]
        private string _connectionStatus = "Disconnected";

        [ObservableProperty]
        private bool _isConnected;

        [ObservableProperty]
        private DateTime _lastHeartbeat = DateTime.MinValue;

        [ObservableProperty]
        private string _apiUrl = "https://localhost:5001";

        [ObservableProperty]
        private int _logCollectionInterval = 30; // seconds

        [ObservableProperty]
        private int _heartbeatInterval = 60; // seconds

        [ObservableProperty]
        private int _totalLogsSent;

        [ObservableProperty]
        private int _logsInQueue;

        [ObservableProperty]
        private string _lastError = string.Empty;

        [ObservableProperty]
        private bool _autoStart = true;

        [ObservableProperty]
        private string _logFolderPath = @"C:\MachineData\Logs";

        [ObservableProperty]
        private bool _isServiceRunning;

        public ObservableCollection<LogEntry> RecentLogs { get; } = new();
        public ObservableCollection<CommandEntry> PendingCommands { get; } = new();
        
        public ICollectionView RecentLogsView { get; }
        public ICollectionView PendingCommandsView { get; }

        public MainViewModel(
            IApiService apiService,
            ILogCollectionService logService,
            IConfigurationService configService,
            ILogger<MainViewModel> logger)
        {
            _apiService = apiService;
            _logService = logService;
            _configService = configService;
            _logger = logger;

            // Setup collection views for filtering/sorting
            RecentLogsView = CollectionViewSource.GetDefaultView(RecentLogs);
            PendingCommandsView = CollectionViewSource.GetDefaultView(PendingCommands);

            // Initialize timers
            _heartbeatTimer = new Timer(SendHeartbeat, null, Timeout.Infinite, Timeout.Infinite);
            _logCollectionTimer = new Timer(CollectLogs, null, Timeout.Infinite, Timeout.Infinite);

            // Load configuration
            LoadConfiguration();

            // Auto-start if enabled
            if (AutoStart)
            {
                _ = StartServicesAsync();
            }
        }

        [RelayCommand]
        private async Task StartServicesAsync()
        {
            try
            {
                _logger.LogInformation("Starting machine services...");
                
                // Test API connection
                var isApiReachable = await _apiService.TestConnectionAsync();
                if (!isApiReachable)
                {
                    LastError = "Cannot connect to API server";
                    return;
                }

                // Register machine
                var machine = new Machine
                {
                    MachineId = MachineId,
                    StationName = StationName,
                    LineName = LineName,
                    Status = "Online",
                    LastHeartbeat = DateTime.UtcNow
                };

                await _apiService.RegisterMachineAsync(machine);
                
                // Start timers
                _heartbeatTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(HeartbeatInterval));
                _logCollectionTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(LogCollectionInterval));
                
                IsServiceRunning = true;
                ConnectionStatus = "Connected";
                IsConnected = true;
                LastError = string.Empty;
                
                _logger.LogInformation("Machine services started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start machine services");
                LastError = ex.Message;
            }
        }

        [RelayCommand]
        private void StopServices()
        {
            try
            {
                _logger.LogInformation("Stopping machine services...");
                
                _heartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _logCollectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
                
                IsServiceRunning = false;
                ConnectionStatus = "Disconnected";
                IsConnected = false;
                
                _logger.LogInformation("Machine services stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping services");
                LastError = ex.Message;
            }
        }

        [RelayCommand]
        private async Task SaveConfigurationAsync()
        {
            try
            {
                var config = new ClientConfiguration
                {
                    MachineId = MachineId,
                    StationName = StationName,
                    LineName = LineName,
                    ApiUrl = ApiUrl,
                    LogCollectionInterval = LogCollectionInterval,
                    HeartbeatInterval = HeartbeatInterval,
                    AutoStart = AutoStart,
                    LogFolderPath = LogFolderPath
                };

                await _configService.SaveConfigurationAsync(config);
                
                _logger.LogInformation("Configuration saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save configuration");
                LastError = ex.Message;
            }
        }

        [RelayCommand]
        private void TestConnection()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var isReachable = await _apiService.TestConnectionAsync();
                    ConnectionStatus = isReachable ? "Connected" : "Failed";
                    IsConnected = isReachable;
                    LastError = isReachable ? string.Empty : "API server unreachable";
                }
                catch (Exception ex)
                {
                    ConnectionStatus = "Error";
                    IsConnected = false;
                    LastError = ex.Message;
                }
            });
        }

        [RelayCommand]
        private void ClearLogs()
        {
            RecentLogs.Clear();
            TotalLogsSent = 0;
            LogsInQueue = 0;
        }

        [RelayCommand]
        private void OpenLogFolder()
        {
            try
            {
                if (Directory.Exists(LogFolderPath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", LogFolderPath);
                }
                else
                {
                    LastError = "Log folder does not exist";
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }

        private async void SendHeartbeat(object? state)
        {
            try
            {
                await _apiService.SendHeartbeatAsync(MachineId);
                LastHeartbeat = DateTime.Now;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send heartbeat");
            }
        }

        private async void CollectLogs(object? state)
        {
            try
            {
                var logs = await _logService.CollectLogsAsync(LogFolderPath);
                if (logs.Any())
                {
                    await _apiService.SendLogsAsync(logs);
                    
                    TotalLogsSent += logs.Count();
                    
                    // Update recent logs for UI
                    foreach (var log in logs.TakeLast(10))
                    {
                        RecentLogs.Insert(0, new LogEntry
                        {
                            Timestamp = log.Timestamp,
                            Level = log.Level,
                            Message = log.Message,
                            MachineId = log.MachineId
                        });
                    }
                    
                    // Keep only last 100 entries
                    while (RecentLogs.Count > 100)
                    {
                        RecentLogs.RemoveAt(RecentLogs.Count - 1);
                    }
                }
                
                LogsInQueue = await _logService.GetQueueCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to collect logs");
            }
        }

        private async void LoadConfiguration()
        {
            try
            {
                var config = await _configService.LoadConfigurationAsync();
                if (config != null)
                {
                    MachineId = config.MachineId;
                    StationName = config.StationName;
                    LineName = config.LineName;
                    ApiUrl = config.ApiUrl;
                    LogCollectionInterval = config.LogCollectionInterval;
                    HeartbeatInterval = config.HeartbeatInterval;
                    AutoStart = config.AutoStart;
                    LogFolderPath = config.LogFolderPath;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load configuration");
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            
            // Auto-save configuration on key property changes
            if (e.PropertyName is nameof(MachineId) or nameof(StationName) or nameof(LineName) or 
                nameof(ApiUrl) or nameof(LogCollectionInterval) or nameof(HeartbeatInterval) or 
                nameof(AutoStart) or nameof(LogFolderPath))
            {
                _ = SaveConfigurationAsync();
            }
        }

        public void Dispose()
        {
            _heartbeatTimer?.Dispose();
            _logCollectionTimer?.Dispose();
        }
    }

    // Helper classes for UI binding
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string MachineId { get; set; } = string.Empty;
    }

    public class CommandEntry
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}