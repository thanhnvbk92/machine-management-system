using System;

namespace MachineClient.WPF.Models;

/// <summary>
/// Represents a command sent to machines
/// </summary>
public class Command
{
    public int ID { get; set; }
    public int MachineID { get; set; }
    public string CommandType { get; set; } = string.Empty;
    public string Parameters { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public DateTime? ExecutedTime { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
}