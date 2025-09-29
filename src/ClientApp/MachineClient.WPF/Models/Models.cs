namespace MachineClient.WPF.Models
{
    public class ClientConfiguration
    {
        public string MachineId { get; set; } = Environment.MachineName;
        public string StationName { get; set; } = "Station_A";
        public string LineName { get; set; } = "Line_1";
        public string ApiUrl { get; set; } = "https://localhost:5001";
        public int LogCollectionInterval { get; set; } = 30; // seconds
        public int HeartbeatInterval { get; set; } = 60; // seconds
        public bool AutoStart { get; set; } = true;
        public string LogFolderPath { get; set; } = @"C:\MachineData\Logs";
        
        // Advanced settings
        public int MaxLogFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
        public int MaxLogFiles { get; set; } = 100;
        public int LogRetentionDays { get; set; } = 30;
        public bool EnableDebugLogging { get; set; } = false;
        public int ConnectionTimeout { get; set; } = 30; // seconds
        public int RetryAttempts { get; set; } = 3;
        public int RetryDelay { get; set; } = 5000; // milliseconds
    }

    public class Machine
    {
        public string MachineId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string LineName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime LastHeartbeat { get; set; }
        public string? IpAddress { get; set; }
        public string? Version { get; set; }
        public Dictionary<string, object>? Properties { get; set; }
    }

    public class LogData
    {
        public string MachineId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public string RawData { get; set; } = string.Empty;
        public Dictionary<string, object>? Properties { get; set; }
    }

    public class Command
    {
        public int Id { get; set; }
        public string MachineId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExecutedAt { get; set; }
        public string? Result { get; set; }
        public string? ErrorMessage { get; set; }
    }
}