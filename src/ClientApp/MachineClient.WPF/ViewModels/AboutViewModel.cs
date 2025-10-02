using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.ViewModels
{
    /// <summary>
    /// ViewModel for About page - handles system information, version details, and update checking
    /// </summary>
    public partial class AboutViewModel : ObservableObject
    {
        private readonly ILogger<AboutViewModel> _logger;

        #region System Information Properties

        [ObservableProperty]
        private string _appVersion = "1.0.0";

        [ObservableProperty]
        private DateTime _buildDate = DateTime.Now;

        [ObservableProperty]
        private string _operatingSystem = RuntimeInformation.OSDescription;

        [ObservableProperty]
        private string _runtimeVersion = RuntimeInformation.FrameworkDescription;

        [ObservableProperty]
        private string _machineName = Environment.MachineName;

        [ObservableProperty]
        private string _userName = Environment.UserName;

        [ObservableProperty]
        private string _workingDirectory = Environment.CurrentDirectory;

        [ObservableProperty]
        private int _processorCount = Environment.ProcessorCount;

        [ObservableProperty]
        private string _systemArchitecture = RuntimeInformation.OSArchitecture.ToString();

        [ObservableProperty]
        private string _processArchitecture = RuntimeInformation.ProcessArchitecture.ToString();

        #endregion

        #region Update Information Properties

        [ObservableProperty]
        private string _updateStatus = "Ready to check for updates";

        [ObservableProperty]
        private bool _isCheckingUpdates = false;

        [ObservableProperty]
        private string _latestVersion = "";

        [ObservableProperty]
        private bool _hasUpdates = false;

        [ObservableProperty]
        private string _updateUrl = "";

        [ObservableProperty]
        private DateTime? _lastUpdateCheck;

        #endregion

        #region Application Information Properties

        [ObservableProperty]
        private string _applicationName = "Machine Management Client";

        [ObservableProperty]
        private string _description = "Desktop client application for machine monitoring and management";

        [ObservableProperty]
        private string _copyright = $"© {DateTime.Now.Year} Machine Management System";

        [ObservableProperty]
        private string _company = "Your Company Name";

        [ObservableProperty]
        private string _supportEmail = "support@yourcompany.com";

        [ObservableProperty]
        private string _websiteUrl = "https://yourcompany.com";

        #endregion

        #region Memory and Performance Properties

        [ObservableProperty]
        private long _workingSetMemory = 0;

        [ObservableProperty]
        private long _totalMemory = 0;

        [ObservableProperty]
        private string _memoryUsage = "";

        [ObservableProperty]
        private TimeSpan _uptime = TimeSpan.Zero;

        [ObservableProperty]
        private DateTime _startTime = DateTime.Now;

        #endregion

        public AboutViewModel(ILogger<AboutViewModel> logger)
        {
            _logger = logger;
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            try
            {
                // Set build date (in a real app, this would come from assembly info)
                BuildDate = GetBuildDate();
                
                // Get application version (in a real app, this would come from assembly info)
                AppVersion = GetApplicationVersion();
                
                // Initialize memory information
                UpdateMemoryInformation();
                
                // Set application start time
                StartTime = DateTime.Now;
                
                _logger.LogInformation("AboutViewModel initialized");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize AboutViewModel");
            }
        }

        #region Update Commands

        [RelayCommand]
        private async Task CheckForUpdatesAsync()
        {
            try
            {
                IsCheckingUpdates = true;
                UpdateStatus = "Checking for updates...";
                
                _logger.LogInformation("Checking for updates");
                
                // Simulate update check (in real app, this would call an update service)
                await Task.Delay(2000);
                
                // Simulate update check result
                var hasNewVersion = await SimulateUpdateCheck();
                
                LastUpdateCheck = DateTime.Now;
                
                if (hasNewVersion)
                {
                    HasUpdates = true;
                    LatestVersion = "1.0.1";
                    UpdateUrl = "https://yourcompany.com/downloads/machine-client-v1.0.1";
                    UpdateStatus = $"Update available: v{LatestVersion}";
                }
                else
                {
                    HasUpdates = false;
                    UpdateStatus = "You have the latest version";
                }
                
                _logger.LogInformation("Update check completed - HasUpdates: {HasUpdates}", HasUpdates);
            }
            catch (Exception ex)
            {
                UpdateStatus = $"Update check failed: {ex.Message}";
                _logger.LogError(ex, "Update check failed");
            }
            finally
            {
                IsCheckingUpdates = false;
            }
        }

        [RelayCommand]
        private async Task DownloadUpdateAsync()
        {
            if (!HasUpdates || string.IsNullOrEmpty(UpdateUrl))
            {
                return;
            }

            try
            {
                _logger.LogInformation("Opening update URL: {UpdateUrl}", UpdateUrl);
                
                // Open update URL in default browser
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = UpdateUrl,
                    UseShellExecute = true
                });
                
                UpdateStatus = "Update download started in browser";
            }
            catch (Exception ex)
            {
                UpdateStatus = $"Failed to open update URL: {ex.Message}";
                _logger.LogError(ex, "Failed to open update URL");
            }
        }

        #endregion

        #region System Information Commands

        [RelayCommand]
        private void RefreshSystemInfo()
        {
            try
            {
                _logger.LogInformation("Refreshing system information");
                
                // Refresh system information
                OperatingSystem = RuntimeInformation.OSDescription;
                RuntimeVersion = RuntimeInformation.FrameworkDescription;
                MachineName = Environment.MachineName;
                UserName = Environment.UserName;
                WorkingDirectory = Environment.CurrentDirectory;
                ProcessorCount = Environment.ProcessorCount;
                SystemArchitecture = RuntimeInformation.OSArchitecture.ToString();
                ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString();
                
                // Update memory information
                UpdateMemoryInformation();
                
                // Update uptime
                Uptime = DateTime.Now - StartTime;
                
                _logger.LogInformation("System information refreshed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh system information");
            }
        }

        [RelayCommand]
        private void CopySystemInfo()
        {
            try
            {
                var systemInfo = GetSystemInfoText();
                
                // Copy to clipboard
                System.Windows.Clipboard.SetText(systemInfo);
                
                _logger.LogInformation("System information copied to clipboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to copy system information to clipboard");
            }
        }

        #endregion

        #region Support Commands

        [RelayCommand]
        private void OpenSupportEmail()
        {
            try
            {
                var mailto = $"mailto:{SupportEmail}?subject=Machine Management Client Support&body=Please describe your issue:";
                
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = mailto,
                    UseShellExecute = true
                });
                
                _logger.LogInformation("Support email opened");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open support email");
            }
        }

        [RelayCommand]
        private void OpenWebsite()
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = WebsiteUrl,
                    UseShellExecute = true
                });
                
                _logger.LogInformation("Website opened: {WebsiteUrl}", WebsiteUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open website");
            }
        }

        #endregion

        #region Private Methods

        private DateTime GetBuildDate()
        {
            // In a real application, you would get this from assembly metadata
            // For now, return a static date or current date
            return DateTime.Now.Date;
        }

        private string GetApplicationVersion()
        {
            try
            {
                // In a real application, you would get this from assembly version
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                return version?.ToString() ?? "1.0.0";
            }
            catch
            {
                return "1.0.0";
            }
        }

        private void UpdateMemoryInformation()
        {
            try
            {
                var process = System.Diagnostics.Process.GetCurrentProcess();
                WorkingSetMemory = process.WorkingSet64;
                
                // Get total system memory (this is an approximation)
                var gcMemoryInfo = GC.GetGCMemoryInfo();
                TotalMemory = gcMemoryInfo.TotalAvailableMemoryBytes;
                
                // Format memory usage
                var workingSetMB = WorkingSetMemory / (1024.0 * 1024.0);
                var totalMB = TotalMemory / (1024.0 * 1024.0);
                MemoryUsage = $"{workingSetMB:F1} MB / {totalMB:F1} MB";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update memory information");
                MemoryUsage = "Unable to retrieve memory information";
            }
        }

        private async Task<bool> SimulateUpdateCheck()
        {
            // Simulate network call to update server
            await Task.Delay(1500);
            
            // Randomly return true/false for demonstration
            // In real app, this would check against actual update server
            return new Random().Next(0, 100) < 20; // 20% chance of having update
        }

        private string GetSystemInfoText()
        {
            return $@"Machine Management Client - System Information

Application Information:
- Name: {ApplicationName}
- Version: {AppVersion}
- Build Date: {BuildDate:yyyy-MM-dd}
- Description: {Description}

System Information:
- Machine Name: {MachineName}
- User Name: {UserName}
- Operating System: {OperatingSystem}
- System Architecture: {SystemArchitecture}
- Process Architecture: {ProcessArchitecture}
- Runtime Version: {RuntimeVersion}
- Processor Count: {ProcessorCount}
- Working Directory: {WorkingDirectory}

Performance Information:
- Memory Usage: {MemoryUsage}
- Uptime: {Uptime:dd\\.hh\\:mm\\:ss}
- Start Time: {StartTime:yyyy-MM-dd HH:mm:ss}

Support Information:
- Company: {Company}
- Support Email: {SupportEmail}
- Website: {WebsiteUrl}
- Copyright: {Copyright}
";
        }

        #endregion
    }
}