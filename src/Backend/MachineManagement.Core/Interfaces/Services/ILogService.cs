using MachineManagement.Core.Entities;

namespace MachineManagement.Core.Interfaces.Services
{
    public interface ILogService
    {
        Task<LogData> AddLogAsync(LogData logData);
        Task AddLogBatchAsync(IEnumerable<LogData> logDataList);
        Task<IEnumerable<LogData>> GetLogsByMachineIdAsync(int machineId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<LogData>> GetLogsByFilterAsync(string? machineCode = null, string? logLevel = null, DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = 1, int pageSize = 100);
        Task<LogData?> GetLogByIdAsync(long id);
        Task<bool> DeleteLogAsync(long id);
        Task<bool> DeleteOldLogsAsync(DateTime olderThan);
    }
}