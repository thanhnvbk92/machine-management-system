using System;

namespace MachineClient.WPF.Models;

/// <summary>
/// Request model for machine registration
/// </summary>
public class MachineRegistrationRequest
{
    public string IP { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string? MachineName { get; set; }
    public string? AppVersion { get; set; }
    public DateTime RequestTime { get; set; } = DateTime.Now;
}

/// <summary>
/// Request model for MAC address update
/// </summary>
public class MacUpdateRequest
{
    public string IP { get; set; } = string.Empty;
    public string NewMacAddress { get; set; } = string.Empty;
    public string? MachineName { get; set; }
    public string? AppVersion { get; set; }
}

/// <summary>
/// Response model for machine registration
/// </summary>
public class MachineRegistrationResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public MachineDetailDto? MachineInfo { get; set; }
    public bool IsNewMachine { get; set; }
    public bool RequiresMacUpdate { get; set; } = false;
    public MachineDetailDto? ExistingMachine { get; set; }
}

/// <summary>
/// Response model for MAC address update
/// </summary>
public class MacUpdateResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public MachineDetailDto? MachineInfo { get; set; }
}

/// <summary>
/// Detailed machine information DTO (matches API response)
/// </summary>
public class MachineDetailDto
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? MachineTypeId { get; set; }
    public string IP { get; set; } = string.Empty;
    public string? GMES_Name { get; set; }
    public int? StationID { get; set; }
    public string? ProgramName { get; set; }
    public string MacAddress { get; set; } = string.Empty;
    
    // Display information from related entities
    public string BuyerName { get; set; } = string.Empty;
    public string LineName { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
}