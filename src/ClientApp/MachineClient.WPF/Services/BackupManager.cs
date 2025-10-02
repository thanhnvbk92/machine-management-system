using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.Services
{
    public class BackupManager : IBackupManager
    {
        private readonly ILogger<BackupManager> _logger;
        private readonly IApplicationSettingsService _settingsService;
        private bool _isBackupRunning;
        private CancellationTokenSource? _cancellationTokenSource;

        public event EventHandler<BackupProgressEventArgs>? BackupProgressChanged;
        public event EventHandler<BackupCompletedEventArgs>? BackupCompleted;

        public BackupManager(ILogger<BackupManager> logger, IApplicationSettingsService settingsService)
        {
            _logger = logger;
            _settingsService = settingsService;
        }

        public async Task<BackupResult> CreateBackupAsync(BackupOptions options)
        {
            if (_isBackupRunning)
            {
                return new BackupResult
                {
                    IsSuccess = false,
                    Message = "Another backup operation is already running"
                };
            }

            _isBackupRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                _logger.LogInformation("Starting backup operation");
                ReportProgress(0, "Initializing backup");

                var backupTime = DateTime.Now;
                var backupFolder = options.BackupPath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MachineClient_Backup");
                
                if (options.CreateTimestampFolder)
                {
                    backupFolder = Path.Combine(backupFolder, $"Backup_{backupTime:yyyyMMdd_HHmmss}");
                }

                Directory.CreateDirectory(backupFolder);
                _logger.LogInformation("Created backup folder: {BackupFolder}", backupFolder);

                var backupFileName = options.CustomFileName ?? $"MachineClient_Backup_{backupTime:yyyyMMdd_HHmmss}.zip";
                var backupFilePath = Path.Combine(backupFolder, backupFileName);

                ReportProgress(10, "Creating backup archive");

                using (var archive = ZipFile.Open(backupFilePath, ZipArchiveMode.Create))
                {
                    var totalSteps = 0;
                    var currentStep = 0;

                    // Count total steps
                    if (options.IncludeSettings) totalSteps++;
                    if (options.IncludeLogs) totalSteps++;
                    totalSteps++; // Configuration files

                    // Backup settings
                    if (options.IncludeSettings)
                    {
                        currentStep++;
                        ReportProgress(20 + (currentStep * 60 / totalSteps), "Backing up settings");
                        await BackupSettingsAsync(archive);
                    }

                    // Backup configuration files
                    currentStep++;
                    ReportProgress(20 + (currentStep * 60 / totalSteps), "Backing up configuration");
                    await BackupConfigurationAsync(archive);

                    // Backup logs
                    if (options.IncludeLogs)
                    {
                        currentStep++;
                        ReportProgress(20 + (currentStep * 60 / totalSteps), "Backing up logs");
                        await BackupLogsAsync(archive);
                    }
                }

                ReportProgress(90, "Verifying backup");
                var isValid = await VerifyBackupAsync(backupFilePath);

                if (!isValid)
                {
                    File.Delete(backupFilePath);
                    throw new InvalidOperationException("Backup verification failed");
                }

                var fileInfo = new FileInfo(backupFilePath);
                var result = new BackupResult
                {
                    IsSuccess = true,
                    Message = "Backup created successfully",
                    BackupFilePath = backupFilePath,
                    BackupTime = backupTime,
                    BackupSizeBytes = fileInfo.Length
                };

                ReportProgress(100, "Backup completed successfully");
                _logger.LogInformation("Backup completed successfully: {BackupPath}", backupFilePath);

                BackupCompleted?.Invoke(this, new BackupCompletedEventArgs { Result = result });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating backup");
                var errorResult = new BackupResult
                {
                    IsSuccess = false,
                    Message = $"Backup failed: {ex.Message}",
                    BackupTime = DateTime.Now
                };

                BackupCompleted?.Invoke(this, new BackupCompletedEventArgs { Result = errorResult });
                return errorResult;
            }
            finally
            {
                _isBackupRunning = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public async Task<bool> VerifyBackupAsync(string backupFilePath)
        {
            try
            {
                _logger.LogInformation("Verifying backup file: {BackupPath}", backupFilePath);

                if (!File.Exists(backupFilePath))
                {
                    _logger.LogError("Backup file does not exist: {BackupPath}", backupFilePath);
                    return false;
                }

                using var archive = ZipFile.OpenRead(backupFilePath);
                
                // Check if archive can be opened and has entries
                if (archive.Entries.Count == 0)
                {
                    _logger.LogError("Backup archive is empty");
                    return false;
                }

                // Verify each entry can be read
                foreach (var entry in archive.Entries)
                {
                    using var stream = entry.Open();
                    var buffer = new byte[1024];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                }

                _logger.LogInformation("Backup verification successful");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying backup: {BackupPath}", backupFilePath);
                return false;
            }
        }

        public async Task<BackupStatus> GetBackupStatusAsync()
        {
            return await Task.FromResult(new BackupStatus
            {
                IsRunning = _isBackupRunning,
                ProgressPercentage = _isBackupRunning ? 50 : 0, // Simplified progress
                CurrentOperation = _isBackupRunning ? "Backup in progress" : "Idle"
            });
        }

        private async Task BackupSettingsAsync(ZipArchive archive)
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var settingsPath = Path.Combine(appDataPath, "MachineClient");

                if (Directory.Exists(settingsPath))
                {
                    await AddDirectoryToArchiveAsync(archive, settingsPath, "Settings/");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to backup settings");
            }
        }

        private async Task BackupConfigurationAsync(ZipArchive archive)
        {
            try
            {
                var configFiles = new[] { "appsettings.json", "appsettings.Development.json" };
                var appDirectory = AppDomain.CurrentDomain.BaseDirectory;

                foreach (var configFile in configFiles)
                {
                    var configPath = Path.Combine(appDirectory, configFile);
                    if (File.Exists(configPath))
                    {
                        var entry = archive.CreateEntry($"Configuration/{configFile}");
                        using var entryStream = entry.Open();
                        using var fileStream = File.OpenRead(configPath);
                        await fileStream.CopyToAsync(entryStream);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to backup configuration");
            }
        }

        private async Task BackupLogsAsync(ZipArchive archive)
        {
            try
            {
                var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var logFiles = Directory.GetFiles(appDirectory, "*.log");

                foreach (var logFile in logFiles)
                {
                    var fileName = Path.GetFileName(logFile);
                    var entry = archive.CreateEntry($"Logs/{fileName}");
                    using var entryStream = entry.Open();
                    using var fileStream = File.OpenRead(logFile);
                    await fileStream.CopyToAsync(entryStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to backup logs");
            }
        }

        private async Task AddDirectoryToArchiveAsync(ZipArchive archive, string sourceDirectory, string entryPrefix)
        {
            var files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var relativePath = Path.GetRelativePath(sourceDirectory, file);
                var entryName = entryPrefix + relativePath.Replace('\\', '/');
                
                var entry = archive.CreateEntry(entryName);
                using var entryStream = entry.Open();
                using var fileStream = File.OpenRead(file);
                await fileStream.CopyToAsync(entryStream);
            }
        }

        private void ReportProgress(int percentage, string operation)
        {
            BackupProgressChanged?.Invoke(this, new BackupProgressEventArgs
            {
                ProgressPercentage = percentage,
                CurrentOperation = operation
            });
        }
    }
}