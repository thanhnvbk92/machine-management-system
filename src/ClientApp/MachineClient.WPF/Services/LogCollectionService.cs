using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MachineClient.WPF.Models;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.Services
{
    public class LogCollectionService : ILogCollectionService
    {
        private readonly ILogger<LogCollectionService> _logger;

        public LogCollectionService(ILogger<LogCollectionService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<LogData>> CollectLogsAsync(string logFolderPath)
        {
            try
            {
                if (!Directory.Exists(logFolderPath))
                {
                    return Enumerable.Empty<LogData>();
                }

                var logFiles = Directory.GetFiles(logFolderPath, "*.log", SearchOption.TopDirectoryOnly)
                    .Take(10); // Limit to 10 files

                var logs = new List<LogData>();

                foreach (var file in logFiles)
                {
                    var lines = await File.ReadAllLinesAsync(file);
                    var fileLogs = lines.Take(100).Select((line, index) => new LogData
                    {
                        MachineID = 1,
                        LogLevel = "INFO",
                        Message = line.Length > 500 ? line.Substring(0, 500) : line,
                        Source = Path.GetFileName(file),
                        Timestamp = DateTime.Now.AddMinutes(-index)
                    });

                    logs.AddRange(fileLogs);
                }

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to collect logs from {LogFolderPath}", logFolderPath);
                return Enumerable.Empty<LogData>();
            }
        }

        public Task<int> GetQueueCountAsync()
        {
            // Simple implementation
            return Task.FromResult(0);
        }

        public Task<bool> IsValidLogFileAsync(string filePath)
        {
            try
            {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                return Task.FromResult(extension == ".log" || extension == ".txt");
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task ClearQueueAsync()
        {
            // Simple implementation
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetLogFilesAsync(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    return Task.FromResult(Enumerable.Empty<string>());
                }

                var files = Directory.GetFiles(folderPath, "*.log")
                    .Union(Directory.GetFiles(folderPath, "*.txt"))
                    .Take(20);

                return Task.FromResult(files.AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get log files from {FolderPath}", folderPath);
                return Task.FromResult(Enumerable.Empty<string>());
            }
        }
    }
}