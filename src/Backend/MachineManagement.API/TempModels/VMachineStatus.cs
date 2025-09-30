using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class VMachineStatus
{
    public int MachineId { get; set; }

    public string MachineName { get; set; } = null!;

    public string? MachineStatus { get; set; }

    public string? ClientStatus { get; set; }

    public string? Ip { get; set; }

    public string? GmesName { get; set; }

    public string? ProgramName { get; set; }

    public string? StationName { get; set; }

    public string? LineName { get; set; }

    public DateTime? LastSeen { get; set; }

    public string? AppVersion { get; set; }
}
