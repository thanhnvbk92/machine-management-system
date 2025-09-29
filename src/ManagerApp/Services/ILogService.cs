using MachineManagement.Core.Entities;

namespace MachineManagement.ManagerApp.Services
{
    public interface ILogService
    {
        Task<IEnumerable<LogData>> GetRecentLogsAsync(int count = 100);
        Task<IEnumerable<LogData>> GetLogsByMachineIdAsync(int machineId, int count = 100);
        Task<IEnumerable<LogData>> GetLogsByDateRangeAsync(DateTime from, DateTime to);
        Task<IEnumerable<LogData>> GetLogsByLevelAsync(string logLevel, int count = 100);
        Task<IEnumerable<LogData>> SearchLogsAsync(string searchTerm, int page = 1, int size = 100);
        Task<long> GetTotalLogCountAsync();
        Task<long> GetLogCountByMachineAsync(int machineId);
        Task<Dictionary<string, int>> GetLogCountByLevelAsync();
        Task<IEnumerable<LogData>> GetPagedLogsAsync(int page, int size, string? level = null, int? machineId = null);
    }
}