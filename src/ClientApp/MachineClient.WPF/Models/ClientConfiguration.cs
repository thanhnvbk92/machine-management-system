using System;

namespace MachineClient.WPF.Models;

/// <summary>
/// Represents client configuration settings
/// </summary>
public class ClientConfiguration
{
    public int ID { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public DateTime? UpdatedTime { get; set; }
}