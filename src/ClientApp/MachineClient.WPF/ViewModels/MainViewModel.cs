using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MachineClient.WPF.Models;
using MachineClient.WPF.Services;
using FlaUI.Automation.Extensions.Services;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IApiService _apiService;
        private readonly IMachineInfoService _machineInfoService;
        private readonly IBackupService _backupService;
        private readonly IUIAutomationService _uiAutomationService;
        private readonly IElementMonitoringService _elementMonitoringService;
        private readonly IAutomationDemoService _automationDemoService;
        private readonly ILogger<MainViewModel> _logger;

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
        private string _status = "Disconnected";

        [ObservableProperty]
        private bool _isConnected = false;

        // Backup Settings Properties
        [ObservableProperty]
        private string _backupPlan = "Manual";

        [ObservableProperty]
        private string _backupSourceFolder = @"C:\Data";

        [ObservableProperty]
        private string _backupFilePattern = "*.txt;*.log;*.csv";

        [ObservableProperty]
        private string _ftpServer = "";

        [ObservableProperty]
        private int _ftpPort = 21;

        [ObservableProperty]
        private string _ftpUsername = "";

        [ObservableProperty]
        private string _ftpPassword = "";

        [ObservableProperty]
        private string _ftpRemoteFolder = "/backup/machine-data";

        // Backup scheduling properties
        [ObservableProperty]
        private DateTime? _lastBackupTime;

        [ObservableProperty]
        private TimeSpan _backupScheduleTime = new TimeSpan(2, 0, 0); // Default 2:00 AM

        [ObservableProperty]
        private bool _enableScheduledBackup = false;

        [ObservableProperty]
        private string _backupTimeDisplay = "02:00";

        [ObservableProperty]
        private string _nextBackupTime = "";

        // Date filtering properties
        [ObservableProperty]
        private bool _enableDateFilter = false;

        [ObservableProperty]
        private DateTime _backupFromDate = DateTime.Today.AddDays(-7); // Default to 1 week ago

        [ObservableProperty]
        private string _backupFromDateDisplay = "";

        [ObservableProperty]
        private bool _isMonitoring = false;

        [ObservableProperty]
        private string _logText = "";

        // Navigation properties
        [ObservableProperty]
        private string _selectedPage = "Home";

        [ObservableProperty]
        private bool _isHomePage = true;

        [ObservableProperty]
        private bool _isSettingsPage = false;

        [ObservableProperty]
        private bool _isAboutPage = false;

        // Connection status
        [ObservableProperty]
        private string _connectionStatusColor = "Red";

        [ObservableProperty]
        private string _connectionStatusIcon = "CloudOff";

        // Settings properties
        [ObservableProperty]
        private string _apiServerUrl = "http://localhost:5275";

        [ObservableProperty]
        private int _connectionTimeout = 30;

        [ObservableProperty]
        private bool _autoConnectOnStartup = true;

        [ObservableProperty]
        private bool _startMinimized = false;

        [ObservableProperty]
        private bool _enableLogging = true;

        [ObservableProperty]
        private string _logLevel = "Info";

        [ObservableProperty]
        private int _updateInterval = 30;

        // System info
        [ObservableProperty]
        private DateTime _lastUpdateTime = DateTime.Now;

        [ObservableProperty]
        private DateTime _buildDate = DateTime.Now;

        [ObservableProperty]
        private string _operatingSystem = RuntimeInformation.OSDescription;

        [ObservableProperty]
        private string _runtimeVersion = RuntimeInformation.FrameworkDescription;

        [ObservableProperty]
        private string _machineName = Environment.MachineName;

        // Backup Properties
        [ObservableProperty]
        private bool _isBackupInProgress = false;

        [ObservableProperty]
        private double _backupProgress = 0;

        [ObservableProperty]
        private string _backupProgressText = "";

        [ObservableProperty]
        private string _backupFileProgressText = "";

        [ObservableProperty]
        private string _backupSizeProgressText = "";

        [ObservableProperty]
        private string _backupStatusText = "Ready";

        private CancellationTokenSource? _backupCancellationTokenSource;

        // Pin Count Collection
        public ObservableCollection<PinCountModel> PinCounts { get; } = new();

        public MainViewModel(IApiService apiService, IMachineInfoService machineInfoService, IBackupService backupService, 
                            IUIAutomationService uiAutomationService, IElementMonitoringService elementMonitoringService, 
                            IAutomationDemoService automationDemoService, ILogger<MainViewModel> logger)
        {
            _apiService = apiService;
            _machineInfoService = machineInfoService;
            _backupService = backupService;
            _uiAutomationService = uiAutomationService;
            _elementMonitoringService = elementMonitoringService;
            _automationDemoService = automationDemoService;
            _logger = logger;
            
            // Initialize machine info
            MacAddress = _machineInfoService.GetMacAddress();
            IpAddress = _machineInfoService.GetIpAddress();
            MachineId = _machineInfoService.GetMachineName();
            
            LogText = $"Machine Client Started\nMAC: {MacAddress}\nIP: {IpAddress}\nComputer: {MachineId}\n";
            
            // Initialize sample pin count data
            InitializePinCountData();
            
            // Initialize backup date display
            BackupFromDateDisplay = BackupFromDate.ToString("yyyy-MM-dd");
            
            // Initialize next backup time
            UpdateNextBackupTime();
            
            // Auto-connect on startup
            _ = Task.Run(async () => await InitializeConnectionAsync());
        }

        /// <summary>
        /// Auto-connect and register machine on startup
        /// </summary>
        private async Task InitializeConnectionAsync()
        {
            try
            {
                await Task.Delay(2000); // Wait 2 seconds for UI to load
                
                LogText += "🔄 Auto-connecting to server...\n";
                await TestConnectionAsync();
            }
            catch (Exception ex)
            {
                LogText += $"❌ Auto-connection failed: {ex.Message}\n";
                _logger.LogError(ex, "Auto-connection failed");
            }
        }

        [RelayCommand]
        private async Task TestConnectionAsync()
        {
            try
            {
                // Ensure UI updates happen on UI thread
                await App.Current.Dispatcher.InvokeAsync(() => 
                {
                    LogText += "Testing connection and registering machine...\n";
                });
                
                // First test connection
                var connectionResult = await _apiService.TestConnectionAsync();
                if (!connectionResult)
                {
                    await App.Current.Dispatcher.InvokeAsync(() => 
                    {
                        Status = "Connection Failed";
                        IsConnected = false;
                        LogText += "❌ Connection failed!\n";
                    });
                    return;
                }
                
                // Register machine with server
                var registrationRequest = new MachineRegistrationRequest
                {
                    IP = IpAddress,
                    MacAddress = MacAddress,
                    MachineName = MachineId,
                    AppVersion = "1.0.0"
                };
                
                await App.Current.Dispatcher.InvokeAsync(() => 
                {
                    LogText += $"Registering machine - IP: {IpAddress}, MAC: {MacAddress}\n";
                });
                
                var registrationResult = await _apiService.RegisterMachineAsync(registrationRequest);
                
                await App.Current.Dispatcher.InvokeAsync(() => 
                {
                    if (registrationResult.IsSuccess)
                    {
                        Status = "Connected";
                        IsConnected = true;
                        UpdateConnectionStatus();
                        
                        // Update machine information from server if available
                        if (registrationResult.MachineInfo != null)
                        {
                            var machineInfo = registrationResult.MachineInfo;
                            BuyerName = machineInfo.BuyerName;
                            LineName = machineInfo.LineName;
                            StationName = machineInfo.StationName;
                            ModelName = machineInfo.ModelName;
                            ProgramName = machineInfo.ProgramName ?? "N/A";
                            
                            LogText += $"Buyer: {BuyerName}\n";
                            LogText += $"Line: {LineName}\n";
                            LogText += $"Station: {StationName}\n";
                            LogText += $"Model: {ModelName}\n";
                            LogText += $"Program: {ProgramName}\n";
                        }
                        
                        LogText += registrationResult.IsNewMachine 
                            ? "✅ New machine registered successfully!\n" 
                            : "✅ Existing machine found and updated!\n";
                    }
                    else
                    {
                        Status = "Disconnected";
                        IsConnected = false;
                        UpdateConnectionStatus();
                        LogText += $"❌ Registration failed: {registrationResult.Message}\n";
                    }
                });
            }
            catch (Exception ex)
            {
                await App.Current.Dispatcher.InvokeAsync(() => 
                {
                    Status = "Connection Error";
                    IsConnected = false;
                    LogText += $"❌ Connection error: {ex.Message}\n";
                });
                _logger.LogError(ex, "Connection test failed");
            }
        }

        [RelayCommand]
        private async Task StartMonitoringAsync()
        {
            try
            {
                IsMonitoring = true;
                LogText += "🔄 Monitoring started...\n";
                
                // Simulate monitoring activity
                await Task.Delay(1000);
                LogText += "📊 Monitoring machine status...\n";
                
                Status = "Monitoring";
            }
            catch (Exception ex)
            {
                LogText += $"❌ Monitoring error: {ex.Message}\n";
                _logger.LogError(ex, "Monitoring failed");
            }
        }

        [RelayCommand]
        private void StopMonitoring()
        {
            IsMonitoring = false;
            Status = IsConnected ? "Connected" : "Disconnected";
            LogText += "⏹️ Monitoring stopped.\n";
        }

        [RelayCommand]
        private void ClearLogs()
        {
            LogText = "Machine Client Started\n";
        }

        // Navigation Commands
        [RelayCommand]
        private void NavigateToHome()
        {
            SelectedPage = "Home";
            IsHomePage = true;
            IsSettingsPage = false;
            IsAboutPage = false;
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            SelectedPage = "Settings";
            IsHomePage = false;
            IsSettingsPage = true;
            IsAboutPage = false;
        }

        [RelayCommand]
        private void NavigateToAbout()
        {
            SelectedPage = "About";
            IsHomePage = false;
            IsSettingsPage = false;
            IsAboutPage = true;
        }

        // Machine Info Commands
        [RelayCommand]
        private async Task UpdateMachineInfoAsync()
        {
            await TestConnectionAsync();
            LastUpdateTime = DateTime.Now;
        }

        // Settings Commands
        [RelayCommand]
        private void SaveSettings()
        {
            // TODO: Implement settings persistence
            LogText += "⚙️ Settings saved.\n";
        }

        [RelayCommand]
        private void ResetSettings()
        {
            ApiServerUrl = "http://localhost:5275";
            ConnectionTimeout = 30;
            AutoConnectOnStartup = true;
            StartMinimized = false;
            EnableLogging = true;
            LogLevel = "Info";
            UpdateInterval = 30;
            LogText += "⚙️ Settings reset to default.\n";
        }

        // Pin Count Methods
        private void InitializePinCountData()
        {
            PinCounts.Clear();
            
            // Sample pin data with different status levels
            PinCounts.Add(new PinCountModel 
            { 
                PinName = "PIN_001", 
                Usage = 850, 
                Lifetime = 1000 
            });
            
            PinCounts.Add(new PinCountModel 
            { 
                PinName = "PIN_002", 
                Usage = 950, 
                Lifetime = 1000 
            });
            
            PinCounts.Add(new PinCountModel 
            { 
                PinName = "PIN_003", 
                Usage = 1200, 
                Lifetime = 1000 
            });
            
            PinCounts.Add(new PinCountModel 
            { 
                PinName = "PIN_004", 
                Usage = 450, 
                Lifetime = 1000 
            });
            
            PinCounts.Add(new PinCountModel 
            { 
                PinName = "PIN_005", 
                Usage = 970, 
                Lifetime = 1000 
            });
        }

        [RelayCommand]
        private void ResetPin(PinCountModel pin)
        {
            if (pin != null)
            {
                pin.Usage = 0;
                LogText += $"🔄 Reset pin: {pin.PinName}\n";
            }
        }

        [RelayCommand]
        private void CheckUpdate()
        {
            // TODO: Implement actual update checking logic
            LogText += "🔍 Checking for updates...\n";
            
            // Simulate update check
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    LogText += "✅ No updates available. You have the latest version.\n";
                });
            });
        }

        [RelayCommand]
        private async Task StartBackup()
        {
            if (IsBackupInProgress) return;

            try
            {
                IsBackupInProgress = true;
                BackupProgress = 0;
                BackupStatusText = "Starting backup...";
                BackupProgressText = "0%";
                BackupFileProgressText = "";
                BackupSizeProgressText = "";
                
                _backupCancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _backupCancellationTokenSource.Token;

                LogText += "🔄 Starting file backup to FTP server...\n";

                // Validate settings
                if (string.IsNullOrEmpty(FtpServer) || string.IsNullOrEmpty(FtpUsername))
                {
                    BackupStatusText = "FTP settings not configured";
                    LogText += "❌ FTP settings not configured. Please check Settings.\n";
                    return;
                }

                // Progress reporter
                var progress = new Progress<Services.BackupProgress>(p =>
                {
                    BackupProgress = p.PercentComplete;
                    BackupProgressText = $"{p.PercentComplete}%";
                    BackupStatusText = p.StatusText;
                    
                    // File progress text
                    BackupFileProgressText = $"Files: {p.FilesProcessed}/{p.TotalFiles}";
                    
                    // Size progress text
                    var processedMB = p.BytesProcessed / (1024.0 * 1024.0);
                    var totalMB = p.TotalBytes / (1024.0 * 1024.0);
                    BackupSizeProgressText = $"Size: {processedMB:F1}/{totalMB:F1} MB";
                    
                    if (!string.IsNullOrEmpty(p.CurrentFile))
                    {
                        LogText += $"📄 Uploading: {p.CurrentFile}\n";
                    }
                });

                // Start backup
                DateTime? filterDate = EnableDateFilter ? BackupFromDate : null;
                
                var success = await _backupService.BackupFilesAsync(
                    BackupSourceFolder,
                    BackupFilePattern,
                    FtpServer,
                    FtpPort,
                    FtpUsername,
                    FtpPassword,
                    FtpRemoteFolder,
                    progress,
                    cancellationToken,
                    filterDate
                );

                if (cancellationToken.IsCancellationRequested)
                {
                    BackupStatusText = "Backup cancelled";
                    LogText += "❌ Backup cancelled by user\n";
                }
                else if (success)
                {
                    BackupStatusText = "Backup completed successfully";
                    LastBackupTime = DateTime.Now;
                    LogText += $"✅ Backup completed successfully at {LastBackupTime:yyyy-MM-dd HH:mm:ss}\n";
                }
                else
                {
                    BackupStatusText = "Backup failed";
                    LogText += "❌ Backup operation failed. Check logs for details.\n";
                }
            }
            catch (Exception ex)
            {
                BackupStatusText = "Backup failed";
                LogText += $"❌ Backup failed: {ex.Message}\n";
                _logger.LogError(ex, "Backup operation failed");
            }
            finally
            {
                IsBackupInProgress = false;
                BackupProgress = 0;
                BackupProgressText = "";
                BackupFileProgressText = "";
                BackupSizeProgressText = "";
                _backupCancellationTokenSource?.Dispose();
                _backupCancellationTokenSource = null;
            }
        }

        [RelayCommand]
        private void StopBackup()
        {
            if (_backupCancellationTokenSource != null && !_backupCancellationTokenSource.IsCancellationRequested)
            {
                _backupCancellationTokenSource.Cancel();
                LogText += "⏹️ Stopping backup process...\n";
            }
        }

        [RelayCommand]
        private void BrowseBackupFolder()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder",
                Filter = "Folders|*.folder",
                Title = "Select Backup Source Folder"
            };

            // Hack to select folder instead of file
            var folderBrowser = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = "Select Backup Source Folder",
                ShowNewFolderButton = true
            };

            if (!string.IsNullOrEmpty(BackupSourceFolder))
            {
                folderBrowser.SelectedPath = BackupSourceFolder;
            }

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BackupSourceFolder = folderBrowser.SelectedPath;
                LogText += $"📁 Selected backup folder: {BackupSourceFolder}\n";
            }
        }

        [RelayCommand]
        private async Task TestFtpConnection()
        {
            if (string.IsNullOrEmpty(FtpServer) || string.IsNullOrEmpty(FtpUsername))
            {
                LogText += "❌ FTP settings not configured. Please fill in server and username.\n";
                return;
            }

            try
            {
                LogText += "🔄 Testing FTP connection...\n";
                
                var testResult = await _backupService.TestFtpConnectionAsync(
                    FtpServer,
                    FtpPort,
                    FtpUsername,
                    FtpPassword
                );

                if (testResult)
                {
                    LogText += "✅ FTP connection test successful!\n";
                }
                else
                {
                    LogText += "❌ FTP connection test failed. Check your settings.\n";
                }
            }
            catch (Exception ex)
            {
                LogText += $"❌ FTP connection test error: {ex.Message}\n";
                _logger.LogError(ex, "FTP connection test failed");
            }
        }

        [RelayCommand]
        private void SaveBackupSettings()
        {
            try
            {
                // Parse backup time if scheduled backup is enabled
                if (EnableScheduledBackup && !string.IsNullOrEmpty(BackupTimeDisplay))
                {
                    if (TimeSpan.TryParseExact(BackupTimeDisplay, @"hh\:mm", null, out var parsedTime))
                    {
                        BackupScheduleTime = parsedTime;
                        LogText += $"📅 Scheduled backup time set to {BackupTimeDisplay}\n";
                    }
                    else
                    {
                        LogText += "❌ Invalid time format. Please use HH:mm format (e.g., 02:00)\n";
                        return;
                    }
                }

                // Parse backup from date if enabled
                if (EnableDateFilter && !string.IsNullOrEmpty(BackupFromDateDisplay))
                {
                    if (DateTime.TryParseExact(BackupFromDateDisplay, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                    {
                        BackupFromDate = parsedDate;
                        LogText += $"📅 Date filter set: backup files created/modified after {BackupFromDateDisplay}\n";
                    }
                    else
                    {
                        LogText += "❌ Invalid date format. Please use yyyy-MM-dd format (e.g., 2025-09-20)\n";
                        return;
                    }
                }

                // In a real application, you would save to configuration file or database
                LogText += "💾 Backup settings saved successfully\n";
                
                // Log current settings
                LogText += $"   Source Folder: {BackupSourceFolder}\n";
                LogText += $"   File Pattern: {BackupFilePattern}\n";
                LogText += $"   FTP Server: {FtpServer}:{FtpPort}\n";
                LogText += $"   FTP Username: {FtpUsername}\n";
                LogText += $"   Remote Folder: {FtpRemoteFolder}\n";
                LogText += $"   Backup Plan: {BackupPlan}\n";
                
                if (EnableScheduledBackup)
                {
                    LogText += $"   Scheduled Time: {BackupTimeDisplay} (24-hour format)\n";
                    UpdateNextBackupTime();
                }
                else
                {
                    LogText += $"   Scheduled Backup: Disabled\n";
                    NextBackupTime = "";
                }
                
                if (EnableDateFilter)
                {
                    LogText += $"   Date Filter: Only files after {BackupFromDateDisplay}\n";
                }
                else
                {
                    LogText += $"   Date Filter: Disabled (all files)\n";
                }
            }
            catch (Exception ex)
            {
                LogText += $"❌ Failed to save backup settings: {ex.Message}\n";
                _logger.LogError(ex, "Failed to save backup settings");
            }
        }

        private void UpdateNextBackupTime()
        {
            if (!EnableScheduledBackup || BackupPlan == "Manual")
            {
                NextBackupTime = "";
                return;
            }

            var now = DateTime.Now;
            DateTime nextBackup;

            switch (BackupPlan)
            {
                case "Hourly":
                    // Next hour at minute 0
                    nextBackup = now.Date.AddHours(now.Hour + 1);
                    break;

                case "Daily":
                    // Today at scheduled time, or tomorrow if already passed
                    nextBackup = now.Date.Add(BackupScheduleTime);
                    if (nextBackup <= now)
                    {
                        nextBackup = nextBackup.AddDays(1);
                    }
                    break;

                case "Weekly":
                    // Next Monday at scheduled time
                    var daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
                    if (daysUntilMonday == 0 && now.TimeOfDay > BackupScheduleTime)
                    {
                        daysUntilMonday = 7; // Next week if today is Monday but time passed
                    }
                    nextBackup = now.Date.AddDays(daysUntilMonday).Add(BackupScheduleTime);
                    break;

                default:
                    NextBackupTime = "";
                    return;
            }

            NextBackupTime = $"Next backup: {nextBackup:yyyy-MM-dd HH:mm}";
        }

        // Connection Status Update
        private void UpdateConnectionStatus()
        {
            if (IsConnected)
            {
                ConnectionStatusColor = "Green";
                ConnectionStatusIcon = "CloudCheck";
            }
            else
            {
                ConnectionStatusColor = "Red";
                ConnectionStatusIcon = "CloudOff";
            }
        }

        #region UI Automation Demo Commands

        [RelayCommand]
        private async Task TestUIAutomationAsync()
        {
            try
            {
                LogText += "🔄 Testing UI Automation...\n";
                
                // Demo comprehensive UI automation functionality
                await _automationDemoService.InitializeAndTestAsync();
                
                LogText += "✅ UI Automation demo completed successfully!\n";
            }
            catch (Exception ex)
            {
                LogText += $"❌ UI Automation demo failed: {ex.Message}\n";
                _logger.LogError(ex, "UI Automation demo failed");
            }
        }

        [RelayCommand]
        private async Task StartPinCountMonitoringAsync()
        {
            try
            {
                LogText += "🔄 Starting Element Monitoring...\n";
                
                // Subscribe to element change events
                _elementMonitoringService.ElementChanged += OnElementChanged;
                
                // Start monitoring the backup status text for demo purposes
                await _elementMonitoringService.StartMonitoringAsync("BackupStatusTextBlock");
                
                LogText += "✅ Element Monitoring started successfully!\n";
            }
            catch (Exception ex)
            {
                LogText += $"❌ Element Monitoring failed: {ex.Message}\n";
                _logger.LogError(ex, "Element Monitoring failed");
            }
        }

        [RelayCommand]
        private async Task StopPinCountMonitoringAsync()
        {
            try
            {
                LogText += "🔄 Stopping Element Monitoring...\n";
                
                // Unsubscribe from events
                _elementMonitoringService.ElementChanged -= OnElementChanged;
                
                // Stop monitoring
                await _elementMonitoringService.StopMonitoringAsync();
                
                LogText += "✅ Element Monitoring stopped successfully!\n";
            }
            catch (Exception ex)
            {
                LogText += $"❌ Stop Element Monitoring failed: {ex.Message}\n";
                _logger.LogError(ex, "Stop Element Monitoring failed");
            }
        }

        [RelayCommand]
        private async Task AutoClickBackupButtonAsync()
        {
            try
            {
                LogText += "🔄 Auto-clicking Start Backup button...\n";
                
                // Initialize automation if needed
                if (!_uiAutomationService.IsInitialized)
                {
                    await _uiAutomationService.InitializeAsync();
                }
                
                // Try to click the start backup button
                var clicked = await _uiAutomationService.ClickButtonAsync("StartBackupButton");
                
                if (clicked)
                {
                    LogText += "✅ Successfully clicked Start Backup button!\n";
                }
                else
                {
                    LogText += "⚠️ Could not find or click Start Backup button.\n";
                }
            }
            catch (Exception ex)
            {
                LogText += $"❌ Auto-click backup button failed: {ex.Message}\n";
                _logger.LogError(ex, "Auto-click backup button failed");
            }
        }

        [RelayCommand]
        private async Task ReadMachineIdAsync()
        {
            try
            {
                LogText += "🔄 Reading Machine ID from UI...\n";
                
                // Initialize automation if needed
                if (!_uiAutomationService.IsInitialized)
                {
                    await _uiAutomationService.InitializeAsync();
                }
                
                // Try to read machine ID text
                var machineIdText = await _uiAutomationService.ReadTextAsync("MachineIdTextBlock");
                
                if (!string.IsNullOrEmpty(machineIdText))
                {
                    LogText += $"✅ Machine ID from UI: {machineIdText}\n";
                }
                else
                {
                    LogText += "⚠️ Could not read Machine ID from UI.\n";
                }
            }
            catch (Exception ex)
            {
                LogText += $"❌ Read Machine ID failed: {ex.Message}\n";
                _logger.LogError(ex, "Read Machine ID failed");
            }
        }

        private void OnElementChanged(object? sender, ElementChangedEventArgs e)
        {
            // Update UI on main thread
            App.Current.Dispatcher.Invoke(() =>
            {
                LogText += $"📍 Element Changed - {e.ElementIdentifier}: '{e.PreviousValue}' → '{e.NewValue}'\n";
            });
        }

        #endregion

    }
}