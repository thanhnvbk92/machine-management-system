using Microsoft.Extensions.Logging;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.Core.Interfaces.Services;

namespace MachineManagement.Infrastructure.Services
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

        public async Task<LogData> AddLogAsync(LogData logData)
        {
            try
            {
                logData.LogTimestamp = logData.LogTimestamp == DateTime.MinValue ? DateTime.UtcNow : logData.LogTimestamp;
                logData.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.LogData.AddAsync(logData);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogDebug("Log added for machine {MachineId}: {Message}", logData.MachineId, logData.Message);
                return logData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding log for machine {MachineId}", logData.MachineId);
                throw;
            }
        }

        public async Task AddLogBatchAsync(IEnumerable<LogData> logDataList)
        {
            try
            {
                var logList = logDataList.ToList();
                if (!logList.Any())
                {
                    _logger.LogWarning("Empty log batch received");
                    return;
                }

                foreach (var log in logList)
                {
                    log.LogTimestamp = log.LogTimestamp == DateTime.MinValue ? DateTime.UtcNow : log.LogTimestamp;
                    log.CreatedAt = DateTime.UtcNow;
                }

                await _unitOfWork.LogData.AddRangeAsync(logList);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Batch of {Count} logs added successfully", logList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding log batch");
                throw;
            }
        }

        public async Task<IEnumerable<LogData>> GetLogsByMachineIdAsync(int machineId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var logs = await _unitOfWork.LogData.FindAsync(l => l.MachineId == machineId &&
                    (fromDate == null || l.LogTimestamp >= fromDate) &&
                    (toDate == null || l.LogTimestamp <= toDate));

                return logs.OrderByDescending(l => l.LogTimestamp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs for machine {MachineId}", machineId);
                throw;
            }
        }

        public async Task<IEnumerable<LogData>> GetLogsByFilterAsync(string? machineCode = null, string? logLevel = null, 
            DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = 1, int pageSize = 100)
        {
            try
            {
                // For now, we'll filter by basic criteria. In a real implementation,
                // you might want to use a more sophisticated query with joins
                var allLogs = await _unitOfWork.LogData.FindAsync(l => 
                    (string.IsNullOrEmpty(logLevel) || l.LogLevel == logLevel) &&
                    (fromDate == null || l.LogTimestamp >= fromDate) &&
                    (toDate == null || l.LogTimestamp <= toDate));

                var filteredLogs = allLogs.OrderByDescending(l => l.LogTimestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);

                return filteredLogs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs with filters");
                throw;
            }
        }

        public async Task<LogData?> GetLogByIdAsync(long id)
        {
            try
            {
                return await _unitOfWork.LogData.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting log by ID {LogId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteLogAsync(long id)
        {
            try
            {
                var log = await _unitOfWork.LogData.GetByIdAsync(id);
                if (log == null)
                {
                    return false;
                }

                _unitOfWork.LogData.Remove(log);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Log with ID {LogId} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting log with ID {LogId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteOldLogsAsync(DateTime olderThan)
        {
            try
            {
                var oldLogs = await _unitOfWork.LogData.FindAsync(l => l.LogTimestamp < olderThan);
                var logList = oldLogs.ToList();

                if (!logList.Any())
                {
                    _logger.LogInformation("No old logs found to delete");
                    return true;
                }

                _unitOfWork.LogData.RemoveRange(logList);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Deleted {Count} old logs older than {Date}", logList.Count, olderThan);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting old logs");
                throw;
            }
        }
    }
}