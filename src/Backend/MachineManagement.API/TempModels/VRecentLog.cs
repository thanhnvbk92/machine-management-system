using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class VRecentLog
{
    public string MachineName { get; set; } = null!;

    public string? StationName { get; set; }

    public string? LineName { get; set; }

    public string? ModelName { get; set; }

    public string? LogLevel { get; set; }

    public string Result { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public string? Source { get; set; }

    public string Pid { get; set; } = null!;
}
