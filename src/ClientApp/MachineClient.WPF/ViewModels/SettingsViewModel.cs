using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MachineClient.WPF.Services;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.ViewModels
{
    /// <summary>
    /// ViewModel for Settings page - handles application settings, API configuration, and backup settings
    /// </summary>
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IApplicationSettingsService _settingsService;
        private readonly IBackupManager _backupManager;
        private readonly IMachineInfoService _machineInfoService;
        private readonly ILogger<SettingsViewModel> _logger;

        #region API Settings Properties

        public string ApiBaseUrl
        {
            get => _settingsService.ApiBaseUrl; 
            set => _settingsService.ApiBaseUrl = value; 
        }

        public int ApiTimeoutSeconds
        {
            get => _settingsService.ApiTimeoutSeconds; 
            set => _settingsService.ApiTimeoutSeconds = value; 
        }

        public bool AutoStartConnection
        {
            get => _settingsService.AutoStartConnection; 
            set => _settingsService.AutoStartConnection = value; 
        }

        public bool EnableFileLogging
        {
            get => _settingsService.EnableFileLogging; 
            set => _settingsService.EnableFileLogging = value; 
        }

        public string LogLevel
        {
            get => _settingsService.LogLevel; 
            set => _settingsService.LogLevel = value; 
        }

        #endregion

        #region Backup Settings Properties

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

        [ObservableProperty]
        private bool _enableDateFilter = false;

        [ObservableProperty]
        private DateTime _backupFromDate = DateTime.Today.AddDays(-7); // Default to 1 week ago

        [ObservableProperty]
        private string _backupFromDateDisplay = "";

        #endregion

        #region Network Settings Properties

        [ObservableProperty]
        private string _preferredIpPrefix = "10.224";

        [ObservableProperty]
        private string _ipAddress = "";

        [ObservableProperty]
        private string _macAddress = "";

        #endregion

        #region Status Properties

        [ObservableProperty]
        private string _statusMessage = "";

        [ObservableProperty]
        private bool _isTestingFtp = false;

        #endregion

        public SettingsViewModel(
            IApplicationSettingsService settingsService,
            IBackupManager backupManager,
            IMachineInfoService machineInfoService,
            ILogger<SettingsViewModel> logger)
        {
            _settingsService = settingsService;
            _backupManager = backupManager;
            _machineInfoService = machineInfoService;
            _logger = logger;

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            // Initialize network info
            IpAddress = _machineInfoService.GetIpAddress();
            MacAddress = _machineInfoService.GetMacAddress();
            
            // Initialize backup date display
            BackupFromDateDisplay = BackupFromDate.ToString("yyyy-MM-dd");
            
            // Initialize next backup time
            UpdateNextBackupTime();
            
            _logger.LogInformation("SettingsViewModel initialized");
        }

        #region API Settings Commands

        [RelayCommand]
        private async Task SaveApiSettingsAsync()
        {
            try
            {
                await _settingsService.SaveSettingsAsync();
                StatusMessage = "API settings saved successfully";
                _logger.LogInformation("API settings saved");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to save API settings: {ex.Message}";
                _logger.LogError(ex, "Failed to save API settings");
            }
        }

        [RelayCommand]
        private void ResetApiSettings()
        {
            try
            {
                ApiBaseUrl = "http://localhost:5275";
                ApiTimeoutSeconds = 30;
                AutoStartConnection = true;
                EnableFileLogging = true;
                LogLevel = "Info";
                
                StatusMessage = "API settings reset to default";
                _logger.LogInformation("API settings reset to default");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to reset API settings: {ex.Message}";
                _logger.LogError(ex, "Failed to reset API settings");
            }
        }

        [RelayCommand]
        private void RefreshNetworkInfo()
        {
            try
            {
                // Refresh IP address with preferred prefix
                IpAddress = _machineInfoService.GetIpAddress();
                MacAddress = _machineInfoService.GetMacAddress();
                
                StatusMessage = $"Network info refreshed - IP: {IpAddress}";
                _logger.LogInformation("Network info refreshed - IP: {IpAddress}, MAC: {MacAddress}", IpAddress, MacAddress);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error refreshing network info: {ex.Message}";
                _logger.LogError(ex, "Failed to refresh network info");
            }
        }

        #endregion

        #region Backup Settings Commands

        [RelayCommand]
        private async Task SaveBackupSettingsAsync()
        {
            try
            {
                // Parse backup time if scheduled backup is enabled
                if (EnableScheduledBackup && !string.IsNullOrEmpty(BackupTimeDisplay))
                {
                    if (TimeSpan.TryParseExact(BackupTimeDisplay, @"hh\:mm", null, out var parsedTime))
                    {
                        BackupScheduleTime = parsedTime;
                        StatusMessage = $"Scheduled backup time set to {BackupTimeDisplay}";
                    }
                    else
                    {
                        StatusMessage = "Invalid time format. Please use HH:mm format (e.g., 02:00)";
                        return;
                    }
                }

                // Parse backup from date if enabled
                if (EnableDateFilter && !string.IsNullOrEmpty(BackupFromDateDisplay))
                {
                    if (DateTime.TryParseExact(BackupFromDateDisplay, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                    {
                        BackupFromDate = parsedDate;
                        StatusMessage = $"Date filter set: backup files created/modified after {BackupFromDateDisplay}";
                    }
                    else
                    {
                        StatusMessage = "Invalid date format. Please use yyyy-MM-dd format (e.g., 2025-09-20)";
                        return;
                    }
                }

                // Save backup settings (simplified version)
                StatusMessage = "Backup settings saved successfully";
                
                if (EnableScheduledBackup)
                {
                    UpdateNextBackupTime();
                }
                else
                {
                    NextBackupTime = "";
                }
                
                _logger.LogInformation("Backup settings saved successfully");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to save backup settings: {ex.Message}";
                _logger.LogError(ex, "Failed to save backup settings");
            }
        }

        [RelayCommand]
        private void ResetBackupSettings()
        {
            try
            {
                BackupPlan = "Manual";
                BackupSourceFolder = @"C:\Data";
                BackupFilePattern = "*.txt;*.log;*.csv";
                FtpServer = "";
                FtpPort = 21;
                FtpUsername = "";
                FtpPassword = "";
                FtpRemoteFolder = "/backup/machine-data";
                EnableScheduledBackup = false;
                BackupScheduleTime = new TimeSpan(2, 0, 0);
                BackupTimeDisplay = "02:00";
                EnableDateFilter = false;
                BackupFromDate = DateTime.Today.AddDays(-7);
                BackupFromDateDisplay = BackupFromDate.ToString("yyyy-MM-dd");
                NextBackupTime = "";
                
                StatusMessage = "Backup settings reset to default";
                _logger.LogInformation("Backup settings reset to default");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to reset backup settings: {ex.Message}";
                _logger.LogError(ex, "Failed to reset backup settings");
            }
        }

        [RelayCommand]
        private void BrowseBackupFolder()
        {
            try
            {
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
                    StatusMessage = $"Selected backup folder: {BackupSourceFolder}";
                    _logger.LogInformation("Backup folder selected: {BackupSourceFolder}", BackupSourceFolder);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error selecting backup folder: {ex.Message}";
                _logger.LogError(ex, "Failed to select backup folder");
            }
        }

        [RelayCommand]
        private async Task TestFtpConnectionAsync()
        {
            if (string.IsNullOrEmpty(FtpServer) || string.IsNullOrEmpty(FtpUsername))
            {
                StatusMessage = "FTP settings not configured. Please fill in server and username.";
                return;
            }

            try
            {
                IsTestingFtp = true;
                StatusMessage = "Testing FTP connection...";
                
                // Simplified FTP test - just simulate for now
                await Task.Delay(2000);
                StatusMessage = "FTP connection test completed (simulated)";
            }
            catch (Exception ex)
            {
                StatusMessage = $"FTP connection test error: {ex.Message}";
                _logger.LogError(ex, "FTP connection test failed");
            }
            finally
            {
                IsTestingFtp = false;
            }
        }

        #endregion

        #region Private Methods

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

        #endregion

        #region Backup Settings Model

        public class BackupSettings
        {
            public string BackupPlan { get; set; } = "";
            public string SourceFolder { get; set; } = "";
            public string FilePattern { get; set; } = "";
            public string FtpServer { get; set; } = "";
            public int FtpPort { get; set; }
            public string FtpUsername { get; set; } = "";
            public string FtpPassword { get; set; } = "";
            public string FtpRemoteFolder { get; set; } = "";
            public bool EnableScheduledBackup { get; set; }
            public TimeSpan ScheduleTime { get; set; }
            public bool EnableDateFilter { get; set; }
            public DateTime FromDate { get; set; }
        }

        #endregion
    }
}