using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MachineClient.WPF.Models;
using MachineClient.WPF.Services;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.ViewModels
{
    /// <summary>
    /// ViewModel for Home page - handles machine connection, monitoring, backup and pin count functionality
    /// </summary>
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IMachineConnectionService _connectionService;
        private readonly IBackupManager _backupManager;
        private readonly IUIStateManager _uiStateManager;
        private readonly IMachineInfoService _machineInfoService;
        private readonly ILogger<HomeViewModel> _logger;

        #region Machine Information Properties

        [ObservableProperty]
        private string _machineId = "";

        [ObservableProperty]
        private string _macAddress = "";

        [ObservableProperty]
        private string _ipAddress = "";

        [ObservableProperty]
        private string _buyerName = "";

        [ObservableProperty]
        private string _lineName = "";

        [ObservableProperty]
        private string _stationName = "";

        [ObservableProperty]
        private string _modelName = "";

        [ObservableProperty]
        private string _programName = "";

        [ObservableProperty]
        private string _logText = "";

        #endregion

        #region Connection Status Properties

        [ObservableProperty]
        private string _status = "Disconnected";

        [ObservableProperty]
        private bool _isConnected = false;

        [ObservableProperty]
        private string _connectionStatusColor = "Red";

        [ObservableProperty]
        private string _connectionStatusIcon = "CloudOff";

        [ObservableProperty]
        private bool _isMonitoring = false;

        [ObservableProperty]
        private DateTime _lastUpdateTime = DateTime.Now;

        [ObservableProperty]
        private bool _isConnectionInProgress = false;

        // Add property to control button availability
        public bool CanTestConnection => !IsConnectionInProgress && !IsBackupInProgress && !IsMonitoring;

        #endregion

        #region Backup Properties

        [ObservableProperty]
        private bool _isBackupInProgress = false;

        [ObservableProperty]
        private double _backupProgress = 0;

        [ObservableProperty]
        private string _backupProgressText = "";

        [ObservableProperty]
        private string _backupStatusText = "Ready";

        #endregion

        // Pin Count Collection
        public ObservableCollection<PinCountModel> PinCounts { get; } = new();

        public HomeViewModel(
            IMachineConnectionService connectionService,
            IBackupManager backupManager,
            IUIStateManager uiStateManager,
            IMachineInfoService machineInfoService,
            ILogger<HomeViewModel> logger)
        {
            _connectionService = connectionService;
            _backupManager = backupManager;
            _uiStateManager = uiStateManager;
            _machineInfoService = machineInfoService;
            _logger = logger;

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            // Initialize machine info
            MacAddress = _machineInfoService.GetMacAddress();
            IpAddress = _machineInfoService.GetIpAddress();
            
            // Don't set MachineId here - it should come from database via API
            var computerName = _machineInfoService.GetMachineName();
            
            LogText = $"Machine Client Started\nMAC: {MacAddress}\nIP: {IpAddress}\nComputer: {computerName}\n";
            
            // Subscribe to connection service events
            _connectionService.MachineInfoUpdated += OnMachineInfoUpdated;
            _connectionService.ConnectionStatusChanged += OnConnectionStatusChanged;
            
            // Initialize sample pin count data
            InitializePinCountData();
            
            _logger.LogInformation("HomeViewModel initialized");
            
            // Auto connect on startup to get machine info from database
            LogWithTimestamp("Auto-connecting to get machine information...");
            
            // Use Task.Run to avoid blocking UI initialization
            Task.Run(async () =>
            {
                await Task.Delay(2000); // Wait for UI to be ready
                await TestConnectionAsync();
            });
        }

        #region Connection Commands

        [RelayCommand(CanExecute = nameof(CanTestConnection))]
        private async Task TestConnectionAsync()
        {
            // Force reset flag first to debug stuck issue
            if (IsConnectionInProgress)
            {
                _logger.LogWarning("Force resetting stuck IsConnectionInProgress flag");
                IsConnectionInProgress = false;
            }
            
            // Prevent concurrent execution
            if (IsConnectionInProgress)
            {
                _logger.LogWarning("Connection already in progress, ignoring duplicate request");
                return;
            }

            try
            {
                IsConnectionInProgress = true;
                OnPropertyChanged(nameof(CanTestConnection)); // Update button state
                
                _logger.LogInformation("Starting connection test - IP: {IP}, MAC: {MAC}", IpAddress, MacAddress);
                
                // Safe logging to prevent crashes
                try
                {
                    LogWithTimestamp("Testing connection and registering machine...");
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Error writing to UI log");
                }
                
                // Add detailed logging for debugging
                _logger.LogInformation("Connection service: {ServiceType}", _connectionService?.GetType().Name ?? "NULL");
                
                if (_connectionService == null)
                {
                    _logger.LogError("Connection service is NULL - DI issue!");
                    await Application.Current.Dispatcher.InvokeAsync(() => 
                    {
                        Status = "Service Error";
                        IsConnected = false;
                        ConnectionStatusColor = "Red";
                        ConnectionStatusIcon = "CloudOff";
                        try { LogWithTimestamp("Connection service not available"); } catch { }
                    });
                    return;
                }
                
                // Add timeout for the entire operation (15 seconds)
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                
                // Use existing connection service logic
                // Create machine info for connection
                var machineInfo = new MachineInfo
                {
                    Name = _machineInfoService.GetMachineName(), // Use computer name for registration
                    MacAddress = MacAddress,
                    IpAddress = IpAddress,
                    MachineName = _machineInfoService.GetMachineName(),
                    AppVersion = "1.0.0"
                };
                
                _logger.LogInformation("Calling ConnectAndRegisterAsync with: Name={Name}, IP={IP}, MAC={MAC}", 
                    machineInfo.Name, machineInfo.IpAddress, machineInfo.MacAddress);
                
                var result = await _connectionService.ConnectAndRegisterAsync(machineInfo);
                
                _logger.LogInformation("Connection result: Success={Success}, Message={Message}", 
                    result.IsSuccess, result.Message);
                
                // Update UI on UI thread
                await Application.Current.Dispatcher.InvokeAsync(() => 
                {
                    if (result.IsSuccess)
                    {
                        Status = "Connected";
                        IsConnected = true;
                        try { LogWithTimestamp("Connection successful!"); } catch { }
                        
                        // Update connection status using CommunityToolkit properties
                        ConnectionStatusColor = "Green";
                        ConnectionStatusIcon = "CloudCheck";
                    }
                    else
                    {
                        Status = "Disconnected";
                        IsConnected = false;
                        try { LogWithTimestamp($"Connection failed: {result.Message}"); } catch { }
                        
                        // Update connection status using CommunityToolkit properties
                        ConnectionStatusColor = "Red";
                        ConnectionStatusIcon = "CloudOff";
                    }
                });
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Connection test timed out after 15 seconds");
                await Application.Current.Dispatcher.InvokeAsync(() => 
                {
                    Status = "Connection Timeout";
                    IsConnected = false;
                    ConnectionStatusColor = "Orange";
                    ConnectionStatusIcon = "CloudAlert";
                    try { LogWithTimestamp("Connection timed out after 15 seconds"); } catch { }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => 
                {
                    Status = "Connection Error";
                    IsConnected = false;
                    try { LogWithTimestamp($"Connection error: {ex.Message}"); } catch { }
                    
                    // Update connection status using CommunityToolkit properties
                    ConnectionStatusColor = "Red";
                    ConnectionStatusIcon = "CloudOff";
                });
                _logger.LogError(ex, "Connection test failed");
            }
            finally
            {
                IsConnectionInProgress = false;
                OnPropertyChanged(nameof(CanTestConnection)); // Update button state
                _logger.LogInformation("Connection test completed, IsConnectionInProgress reset to false");
            }
        }

        [RelayCommand]
        private async Task UpdateMachineInfoAsync()
        {
            // Prevent concurrent execution
            if (IsConnectionInProgress)
            {
                _logger.LogWarning("Connection already in progress, ignoring duplicate request");
                return;
            }

            try
            {
                IsConnectionInProgress = true;
                await TestConnectionAsync().ConfigureAwait(false);
                
                // Update LastUpdateTime on UI thread
                await Application.Current.Dispatcher.InvokeAsync(() => 
                {
                    LastUpdateTime = DateTime.Now;
                });
            }
            finally
            {
                IsConnectionInProgress = false;
            }
        }

        #endregion

        #region Monitoring Commands

        [RelayCommand]
        private async Task StartMonitoringAsync()
        {
            try
            {
                IsMonitoring = true;
                LogText += "Monitoring started...\n";
                
                // Simulate monitoring activity
                await Task.Delay(1000);
                LogText += "Monitoring machine status...\n";
                
                Status = "Monitoring";
            }
            catch (Exception ex)
            {
                LogText += $"Monitoring error: {ex.Message}\n";
                _logger.LogError(ex, "Monitoring failed");
            }
        }

        [RelayCommand]
        private void StopMonitoring()
        {
            IsMonitoring = false;
            Status = IsConnected ? "Connected" : "Disconnected";
            LogText += "Monitoring stopped.\n";
        }

        #endregion

        #region Backup Commands

        [RelayCommand]
        private async Task StartBackupAsync()
        {
            try
            {
                IsBackupInProgress = true;
                BackupProgress = 0;
                BackupStatusText = "Starting backup...";
                
                LogText += "Starting file backup...\n";
                
                // Use existing backup service
                // Use existing backup service with default options
                var backupOptions = new BackupOptions
                {
                    BackupPath = @"C:\Backup",
                    IncludeSettings = true,
                    IncludeLogs = true
                };
                
                var backupResult = await _backupManager.CreateBackupAsync(backupOptions);
                
                if (backupResult.IsSuccess)
                {
                    BackupStatusText = "Backup completed successfully";
                    LogText += $"Backup completed successfully at {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";
                    LogText += $"Backup file: {backupResult.BackupFilePath}\n";
                }
                else
                {
                    BackupStatusText = "Backup failed";
                    LogText += $"Backup operation failed: {backupResult.Message}\n";
                }
            }
            catch (Exception ex)
            {
                BackupStatusText = "Backup failed";
                LogText += $"Backup failed: {ex.Message}\n";
                _logger.LogError(ex, "Backup operation failed");
            }
            finally
            {
                IsBackupInProgress = false;
                BackupProgress = 0;
            }
        }

        [RelayCommand]
        private void StopBackup()
        {
            // TODO: Implement backup cancellation when interface supports it
            LogText += "Backup stop requested (not implemented yet)...\n";
        }

        #endregion

        #region Pin Count Commands

        [RelayCommand]
        private void ResetPin(PinCountModel pin)
        {
            if (pin != null)
            {
                pin.Usage = 0;
                LogText += $"Reset pin: {pin.PinName}\n";
            }
        }

        #endregion

        #region Utility Commands

        [RelayCommand]
        private void ClearLogs()
        {
            LogText = "Machine Client Started\n";
        }

        #endregion

        #region Private Methods

        private void InitializePinCountData()
        {
            PinCounts.Clear();
            
            // Sample pin data with different status levels
            PinCounts.Add(new PinCountModel { PinName = "PIN_001", Usage = 850, Lifetime = 1000 });
            PinCounts.Add(new PinCountModel { PinName = "PIN_002", Usage = 950, Lifetime = 1000 });
            PinCounts.Add(new PinCountModel { PinName = "PIN_003", Usage = 1200, Lifetime = 1000 });
            PinCounts.Add(new PinCountModel { PinName = "PIN_004", Usage = 450, Lifetime = 1000 });
            PinCounts.Add(new PinCountModel { PinName = "PIN_005", Usage = 970, Lifetime = 1000 });
        }

        private void LogWithTimestamp(string message)
        {
            LogText += LogMessageFormatter.FormatUILogMessage(message) + "\n";
        }

        #endregion

        #region Event Handlers

        private async void OnMachineInfoUpdated(object? sender, MachineInfoUpdatedEventArgs e)
        {
            try
            {
                _logger.LogInformation("Received machine info update: {MachineName}, Buyer: {Buyer}, Line: {Line}, Station: {Station}, Model: {Model}", 
                    e.MachineInfo.Name, e.MachineInfo.BuyerName, e.MachineInfo.LineName, e.MachineInfo.StationName, e.MachineInfo.ModelName);

                // Update UI on UI thread
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // Update machine details from DB - MachineId should come from database
                    BuyerName = e.MachineInfo.BuyerName ?? "";
                    LineName = e.MachineInfo.LineName ?? "";
                    StationName = e.MachineInfo.StationName ?? "";
                    ModelName = e.MachineInfo.ModelName ?? "";
                    ProgramName = e.MachineInfo.ProgramName ?? "";
                    MachineId = e.MachineInfo.Name ?? ""; // This is the actual machine name from database

                    try { LogWithTimestamp($"Machine info updated: {e.MachineInfo.BuyerName} - {e.MachineInfo.LineName} - {e.MachineInfo.StationName} - {e.MachineInfo.ModelName} - {e.MachineInfo.ProgramName} - {e.MachineInfo.Name}"); } catch { }
                    
                    _logger.LogInformation("UI updated with machine info from database");
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating machine info in UI");
            }
        }

        private async void OnConnectionStatusChanged(object? sender, ConnectionStatusChangedEventArgs e)
        {
            try
            {
                _logger.LogInformation("Connection status changed: {IsConnected}, Status: {Status}, Message: {Message}", 
                    e.IsConnected, e.Status, e.Message);

                // Update UI on UI thread
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    IsConnected = e.IsConnected;
                    Status = e.Status;
                    
                    // Update visual indicators
                    ConnectionStatusColor = e.IsConnected ? "Green" : "Red";
                    ConnectionStatusIcon = e.IsConnected ? "CloudCheck" : "CloudOff";
                    
                    try { LogWithTimestamp($"Status: {e.Status} - {e.Message}"); } catch { }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating connection status in UI");
            }
        }

        #endregion
    }
}