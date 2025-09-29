namespace MachineManagement.ManagerApp.Models;

/// <summary>
/// DTO for machine information display
/// </summary>
public class MachineDto
{
    public string MachineId { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public string LineName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime LastHeartbeat { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public bool IsOnline { get; set; }
    public TimeSpan? Uptime { get; set; }
}

/// <summary>
/// DTO for log entry display
/// </summary>
public class LogEntryDto
{
    public long Id { get; set; }
    public string MachineId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// DTO for command information
/// </summary>
public class CommandDto
{
    public long Id { get; set; }
    public string MachineId { get; set; } = string.Empty;
    public string CommandType { get; set; } = string.Empty;
    public string CommandData { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Dashboard statistics DTO
/// </summary>
public class DashboardStatsDto
{
    public int TotalMachines { get; set; }
    public int OnlineMachines { get; set; }
    public int OfflineMachines { get; set; }
    public long TotalLogsToday { get; set; }
    public long TotalErrors { get; set; }
    public long TotalWarnings { get; set; }
    public int PendingCommands { get; set; }
    public int CompletedCommandsToday { get; set; }
    public double AverageResponseTime { get; set; }
}

/// <summary>
/// Request model for creating commands
/// </summary>
public class CreateCommandRequest
{
    public string MachineId { get; set; } = string.Empty;
    public string CommandType { get; set; } = string.Empty;
    public string CommandData { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// Request model for log queries
/// </summary>
public class LogQueryRequest
{
    public string? MachineId { get; set; }
    public string? Level { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? SearchText { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

/// <summary>
/// Paginated response model
/// </summary>
public class PagedResponse<T>
{
    public IList<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}