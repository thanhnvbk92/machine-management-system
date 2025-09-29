using MachineManagement.ManagerApp.Models;

namespace MachineManagement.ManagerApp.Services;

/// <summary>
/// Interface for machine management operations
/// </summary>
public interface IMachineService
{
    Task<IList<MachineDto>> GetAllMachinesAsync();
    Task<MachineDto?> GetMachineByIdAsync(string machineId);
    Task<bool> UpdateMachineAsync(string machineId, MachineDto machine);
    Task<bool> DeleteMachineAsync(string machineId);
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}

/// <summary>
/// Interface for log management operations
/// </summary>
public interface ILogService
{
    Task<PagedResponse<LogEntryDto>> GetLogsAsync(LogQueryRequest request);
    Task<LogEntryDto?> GetLogByIdAsync(long id);
    Task<bool> DeleteLogsAsync(DateTime beforeDate);
    Task<byte[]> ExportLogsAsync(LogQueryRequest request, string format);
}

/// <summary>
/// Interface for command management operations
/// </summary>
public interface ICommandService
{
    Task<IList<CommandDto>> GetCommandsAsync(string? machineId = null);
    Task<CommandDto?> GetCommandByIdAsync(long id);
    Task<CommandDto> CreateCommandAsync(CreateCommandRequest request);
    Task<bool> UpdateCommandStatusAsync(long id, string status, string? result = null);
    Task<bool> DeleteCommandAsync(long id);
}

/// <summary>
/// Interface for dashboard operations
/// </summary>
public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
    Task<IList<MachineDto>> GetRecentMachineUpdatesAsync(int count = 10);
    Task<IList<LogEntryDto>> GetRecentErrorsAsync(int count = 10);
    Task<IList<CommandDto>> GetRecentCommandsAsync(int count = 10);
}