using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.Services
{
    public interface IBackupManager
    {
        Task<BackupResult> CreateBackupAsync(BackupOptions options);
        Task<bool> VerifyBackupAsync(string backupFilePath);
        Task<BackupStatus> GetBackupStatusAsync();
        event EventHandler<BackupProgressEventArgs>? BackupProgressChanged;
        event EventHandler<BackupCompletedEventArgs>? BackupCompleted;
    }

    public class BackupOptions
    {
        public string? BackupPath { get; set; }
        public bool IncludeSettings { get; set; } = true;
        public bool IncludeLogs { get; set; } = false;
        public bool CreateTimestampFolder { get; set; } = true;
        public string? CustomFileName { get; set; }
    }

    public class BackupResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? BackupFilePath { get; set; }
        public DateTime BackupTime { get; set; }
        public long BackupSizeBytes { get; set; }
    }

    public class BackupStatus
    {
        public bool IsRunning { get; set; }
        public int ProgressPercentage { get; set; }
        public string CurrentOperation { get; set; } = string.Empty;
        public DateTime? LastBackupTime { get; set; }
        public string? LastBackupPath { get; set; }
    }

    public class BackupProgressEventArgs : EventArgs
    {
        public int ProgressPercentage { get; set; }
        public string CurrentOperation { get; set; } = string.Empty;
        public string? CurrentFile { get; set; }
    }

    public class BackupCompletedEventArgs : EventArgs
    {
        public BackupResult Result { get; set; } = new();
    }
}