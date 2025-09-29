using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;

namespace MachineManagement.ManagerApp.Services
{
    public class LogService : ILogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LogService> _logger;

        public LogService(IUnitOfWork unitOfWork, ILogger<LogService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<LogData>> GetRecentLogsAsync(int count = 100)
        {
            try
            {
                var logs = await _unitOfWork.LogData.GetAllAsync();
                return logs.OrderByDescending(l => l.LogTimestamp).Take(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent logs");
                throw;
            }
        }

        public async Task<IEnumerable<LogData>> GetLogsByMachineIdAsync(int machineId, int count = 100)
        {
            try
            {
                var logs = await _unitOfWork.LogData.FindAsync(l => l.MachineId == machineId);
                return logs.OrderByDescending(l => l.LogTimestamp).Take(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs for machine {MachineId}", machineId);
                throw;
            }
        }

        public async Task<IEnumerable<LogData>> GetLogsByDateRangeAsync(DateTime from, DateTime to)
        {
            try
            {
                return await _unitOfWork.LogData.FindAsync(l => 
                    l.LogTimestamp >= from && l.LogTimestamp <= to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs by date range: {From} - {To}", from, to);
                throw;
            }
        }

        public async Task<IEnumerable<LogData>> GetLogsByLevelAsync(string logLevel, int count = 100)
        {
            try
            {
                var logs = await _unitOfWork.LogData.FindAsync(l => l.LogLevel == logLevel);
                return logs.OrderByDescending(l => l.LogTimestamp).Take(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs by level: {LogLevel}", logLevel);
                throw;
            }
        }

        public async Task<IEnumerable<LogData>> SearchLogsAsync(string searchTerm, int page = 1, int size = 100)
        {
            try
            {
                var allLogs = await _unitOfWork.LogData.FindAsync(l => 
                    l.Message.Contains(searchTerm) || 
                    (l.Details != null && l.Details.Contains(searchTerm)) ||
                    (l.Source != null && l.Source.Contains(searchTerm)));

                return allLogs.OrderByDescending(l => l.LogTimestamp)
                             .Skip((page - 1) * size)
                             .Take(size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching logs: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<long> GetTotalLogCountAsync()
        {
            try
            {
                return await _unitOfWork.LogData.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total log count");
                throw;
            }
        }

        public async Task<long> GetLogCountByMachineAsync(int machineId)
        {
            try
            {
                return await _unitOfWork.LogData.CountAsync(l => l.MachineId == machineId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting log count for machine {MachineId}", machineId);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetLogCountByLevelAsync()
        {
            try
            {
                var logs = await _unitOfWork.LogData.GetAllAsync();
                return logs.GroupBy(l => l.LogLevel)
                          .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting log count by level");
                throw;
            }
        }

        public async Task<IEnumerable<LogData>> GetPagedLogsAsync(int page, int size, string? level = null, int? machineId = null)
        {
            try
            {
                IEnumerable<LogData> logs;

                if (!string.IsNullOrEmpty(level) && machineId.HasValue)
                {
                    logs = await _unitOfWork.LogData.FindAsync(l => l.LogLevel == level && l.MachineId == machineId);
                }
                else if (!string.IsNullOrEmpty(level))
                {
                    logs = await _unitOfWork.LogData.FindAsync(l => l.LogLevel == level);
                }
                else if (machineId.HasValue)
                {
                    logs = await _unitOfWork.LogData.FindAsync(l => l.MachineId == machineId);
                }
                else
                {
                    logs = await _unitOfWork.LogData.GetAllAsync();
                }

                return logs.OrderByDescending(l => l.LogTimestamp)
                          .Skip((page - 1) * size)
                          .Take(size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged logs");
                throw;
            }
        }
    }
}