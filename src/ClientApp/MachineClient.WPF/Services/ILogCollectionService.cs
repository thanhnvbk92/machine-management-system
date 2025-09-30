using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MachineClient.WPF.Models;

namespace MachineClient.WPF.Services
{
    public interface ILogCollectionService
    {
        Task<IEnumerable<LogData>> CollectLogsAsync(string logFolderPath);
        Task<int> GetQueueCountAsync();
        Task<bool> IsValidLogFileAsync(string filePath);
        Task ClearQueueAsync();
        Task<IEnumerable<string>> GetLogFilesAsync(string folderPath);
    }
}