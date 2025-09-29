using MachineClient.WPF.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace MachineClient.WPF.Services
{
    public class LogCollectionService : ILogCollectionService
    {
        private readonly ILogger<LogCollectionService> _logger;
        private readonly ConcurrentQueue<LogData> _logQueue = new();
        private readonly HashSet<string> _processedFiles = new();
        private readonly string[] _supportedExtensions = { ".log", ".txt", ".csv" };
        private readonly Regex _logLinePattern = new(@"^(?<timestamp>\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}[,.:]\d{3})\s*\|?\s*(?<level>\w+)\s*\|?\s*(?<message>.*)", RegexOptions.Compiled);

        public LogCollectionService(ILogger<LogCollectionService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<LogData>> CollectLogsAsync(string logFolderPath)
        {
            var collectedLogs = new List<LogData>();

            try
            {
                if (!Directory.Exists(logFolderPath))
                {
                    _logger.LogWarning("Log folder does not exist: {LogFolderPath}", logFolderPath);
                    return collectedLogs;
                }

                var logFiles = await GetLogFilesAsync(logFolderPath);
                
                foreach (var filePath in logFiles)
                {
                    try
                    {
                        var logs = await ReadLogFileAsync(filePath);
                        collectedLogs.AddRange(logs);
                        
                        // Mark file as processed
                        _processedFiles.Add(filePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to read log file: {FilePath}", filePath);
                    }
                }

                _logger.LogInformation("Collected {Count} log entries from {FileCount} files", 
                    collectedLogs.Count, logFiles.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to collect logs from folder: {LogFolderPath}", logFolderPath);
            }

            return collectedLogs;
        }

        public async Task<IEnumerable<string>> GetLogFilesAsync(string folderPath)
        {
            var logFiles = new List<string>();

            try
            {
                var allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => _supportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .Where(f => !_processedFiles.Contains(f) || HasNewContent(f))
                    .OrderBy(f => new FileInfo(f).LastWriteTime);

                logFiles.AddRange(allFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get log files from folder: {FolderPath}", folderPath);
            }

            return logFiles;
        }

        private async Task<IEnumerable<LogData>> ReadLogFileAsync(string filePath)
        {
            var logs = new List<LogData>();
            var machineId = Environment.MachineName; // Default to machine name

            try
            {
                using var reader = new StreamReader(filePath);
                string? line;
                int lineNumber = 0;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var logData = ParseLogLine(line, filePath, lineNumber, machineId);
                    if (logData != null)
                    {
                        logs.Add(logData);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read log file: {FilePath}", filePath);
            }

            return logs;
        }

        private LogData? ParseLogLine(string line, string filePath, int lineNumber, string machineId)
        {
            try
            {
                var match = _logLinePattern.Match(line);
                
                if (match.Success)
                {
                    // Structured log format
                    var timestampStr = match.Groups["timestamp"].Value;
                    var level = match.Groups["level"].Value;
                    var message = match.Groups["message"].Value;

                    if (DateTime.TryParse(timestampStr, out var timestamp))
                    {
                        return new LogData
                        {
                            MachineId = machineId,
                            Timestamp = timestamp,
                            Level = level,
                            Message = message,
                            Source = Path.GetFileName(filePath),
                            LineNumber = lineNumber,
                            RawData = line
                        };
                    }
                }
                else
                {
                    // Unstructured log - treat as info level
                    return new LogData
                    {
                        MachineId = machineId,
                        Timestamp = DateTime.UtcNow,
                        Level = "INFO",
                        Message = line,
                        Source = Path.GetFileName(filePath),
                        LineNumber = lineNumber,
                        RawData = line
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse log line {LineNumber} from {FilePath}: {Line}", 
                    lineNumber, filePath, line);
            }

            return null;
        }

        private bool HasNewContent(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                return fileInfo.LastWriteTime > DateTime.Now.AddMinutes(-5); // Modified in last 5 minutes
            }
            catch
            {
                return true; // Assume new content if we can't check
            }
        }

        public Task<int> GetQueueCountAsync()
        {
            return Task.FromResult(_logQueue.Count);
        }

        public Task<bool> IsValidLogFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return Task.FromResult(false);

                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                var isValidExtension = _supportedExtensions.Contains(extension);
                
                var fileInfo = new FileInfo(filePath);
                var isNotEmpty = fileInfo.Length > 0;
                var isNotTooOld = fileInfo.LastWriteTime > DateTime.Now.AddDays(-30);

                return Task.FromResult(isValidExtension && isNotEmpty && isNotTooOld);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate log file: {FilePath}", filePath);
                return Task.FromResult(false);
            }
        }

        public Task ClearQueueAsync()
        {
            while (_logQueue.TryDequeue(out _))
            {
                // Empty the queue
            }
            
            _processedFiles.Clear();
            return Task.CompletedTask;
        }
    }
}