using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.Services
{
    public class BackupService : IBackupService
    {
        private readonly ILogger<BackupService> _logger;

        public BackupService(ILogger<BackupService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> BackupFilesAsync(
            string sourceFolder,
            string filePattern,
            string ftpServer,
            int ftpPort,
            string ftpUsername,
            string ftpPassword,
            string remoteFolder,
            IProgress<BackupProgress> progress,
            CancellationToken cancellationToken,
            DateTime? fromDate = null)
        {
            return await Task.Run(() => BackupFilesSync(
                sourceFolder, filePattern, ftpServer, ftpPort, 
                ftpUsername, ftpPassword, remoteFolder, progress, cancellationToken, fromDate), cancellationToken);
        }

        private bool BackupFilesSync(
            string sourceFolder,
            string filePattern,
            string ftpServer,
            int ftpPort,
            string ftpUsername,
            string ftpPassword,
            string remoteFolder,
            IProgress<BackupProgress> progress,
            CancellationToken cancellationToken,
            DateTime? fromDate = null)
        {
            try
            {
                if (!Directory.Exists(sourceFolder))
                {
                    _logger.LogError("Source folder does not exist: {SourceFolder}", sourceFolder);
                    return false;
                }

                // Parse file patterns
                var patterns = filePattern.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToArray();

                // Get files to backup
                var filesToBackup = new List<FileInfo>();
                var dirInfo = new DirectoryInfo(sourceFolder);

                foreach (var pattern in patterns)
                {
                    var files = dirInfo.GetFiles(pattern, SearchOption.AllDirectories);
                    
                    // Filter by date if specified
                    if (fromDate.HasValue)
                    {
                        files = files.Where(f => f.CreationTime >= fromDate.Value || f.LastWriteTime >= fromDate.Value).ToArray();
                    }
                    
                    filesToBackup.AddRange(files);
                }

                if (filesToBackup.Count == 0)
                {
                    if (fromDate.HasValue)
                    {
                        _logger.LogWarning("No files found matching pattern: {Pattern} after date: {FromDate}", filePattern, fromDate.Value.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        _logger.LogWarning("No files found matching pattern: {Pattern}", filePattern);
                    }
                    return false;
                }

                // Calculate total size
                long totalBytes = filesToBackup.Sum(f => f.Length);
                int totalFiles = filesToBackup.Count;

                if (fromDate.HasValue)
                {
                    _logger.LogInformation("Found {FileCount} files (after {FromDate}), total size: {TotalSize} bytes", totalFiles, fromDate.Value.ToString("yyyy-MM-dd"), totalBytes);
                }
                else
                {
                    _logger.LogInformation("Found {FileCount} files, total size: {TotalSize} bytes", totalFiles, totalBytes);
                }

                // Connect to FTP
                using var client = new FtpClient(ftpServer, ftpUsername, ftpPassword, ftpPort);
                
                progress?.Report(new BackupProgress
                {
                    PercentComplete = 0,
                    StatusText = "Connecting to FTP server...",
                    TotalFiles = totalFiles,
                    TotalBytes = totalBytes
                });

                client.Connect();

                // Ensure remote directory exists
                if (!client.DirectoryExists(remoteFolder))
                {
                    client.CreateDirectory(remoteFolder);
                }

                // Upload files
                long bytesProcessed = 0;
                int filesProcessed = 0;

                foreach (var file in filesToBackup)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    // Get relative path from source folder
                    var relativePath = Path.GetRelativePath(sourceFolder, file.FullName);
                    var remotePath = Path.Combine(remoteFolder, relativePath).Replace('\\', '/');

                    // Create remote directory if needed
                    var remoteDir = Path.GetDirectoryName(remotePath)?.Replace('\\', '/');
                    if (!string.IsNullOrEmpty(remoteDir) && !client.DirectoryExists(remoteDir))
                    {
                        client.CreateDirectory(remoteDir);
                    }

                    // Upload file
                    var uploadResult = client.UploadFile(file.FullName, remotePath, FtpRemoteExists.Overwrite);

                    if (uploadResult == FtpStatus.Success)
                    {
                        bytesProcessed += file.Length;
                        filesProcessed++;

                        var percentComplete = (int)((double)bytesProcessed / totalBytes * 100);

                        progress?.Report(new BackupProgress
                        {
                            PercentComplete = percentComplete,
                            StatusText = $"Uploaded {file.Name}",
                            CurrentFile = file.Name,
                            FilesProcessed = filesProcessed,
                            TotalFiles = totalFiles,
                            BytesProcessed = bytesProcessed,
                            TotalBytes = totalBytes
                        });

                        _logger.LogDebug("Uploaded file: {FilePath} -> {RemotePath}", file.FullName, remotePath);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to upload file: {FilePath}", file.FullName);
                    }
                }

                client.Disconnect();

                _logger.LogInformation("Backup completed successfully. Files: {FilesProcessed}/{TotalFiles}, Bytes: {BytesProcessed}/{TotalBytes}",
                    filesProcessed, totalFiles, bytesProcessed, totalBytes);

                return !cancellationToken.IsCancellationRequested && filesProcessed > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Backup operation failed");
                return false;
            }
        }

        public async Task<bool> TestFtpConnectionAsync(string ftpServer, int ftpPort, string ftpUsername, string ftpPassword)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var client = new FtpClient(ftpServer, ftpUsername, ftpPassword, ftpPort);
                    client.Connect();
                    client.Disconnect();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "FTP connection test failed");
                    return false;
                }
            });
        }
    }
}