using System;
using System.Threading;
using System.Threading.Tasks;

namespace MachineClient.WPF.Services
{
    public interface IBackupService
    {
        Task<bool> BackupFilesAsync(
            string sourceFolder,
            string filePattern,
            string ftpServer,
            int ftpPort,
            string ftpUsername,
            string ftpPassword,
            string remoteFolder,
            IProgress<BackupProgress> progress,
            CancellationToken cancellationToken,
            DateTime? fromDate = null);

        Task<bool> TestFtpConnectionAsync(string server, int port, string username, string password);
    }

    public class BackupProgress
    {
        public int PercentComplete { get; set; }
        public string StatusText { get; set; } = "";
        public int FilesProcessed { get; set; }
        public int TotalFiles { get; set; }
        public long BytesProcessed { get; set; }
        public long TotalBytes { get; set; }
        public string CurrentFile { get; set; } = "";
    }
}