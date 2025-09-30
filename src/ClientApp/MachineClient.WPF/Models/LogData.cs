using System;

namespace MachineClient.WPF.Models;

/// <summary>
/// Represents log data collected from machines
/// </summary>
public class LogData
{
    public int ID { get; set; }
    public int MachineID { get; set; }
    public string LogLevel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Source { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? Category { get; set; }
    public string? AdditionalData { get; set; }
}