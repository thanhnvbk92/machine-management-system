using System;

namespace MachineClient.WPF.Models;

/// <summary>
/// Represents a machine in the client application
/// </summary>
public class Machine
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public int MachineTypeId { get; set; }
    public string IP { get; set; } = string.Empty;
    public string? GMES_Name { get; set; }
    public int StationID { get; set; }
    public string? ProgramName { get; set; }
    public string MacAddress { get; set; } = string.Empty;
    
    // Display information from related entities
    public string BuyerName { get; set; } = string.Empty;
    public string LineName { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    
    // Additional info
    public DateTime? LastLogTime { get; set; }
    public string? AppVersion { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public DateTime? UpdatedTime { get; set; }
}