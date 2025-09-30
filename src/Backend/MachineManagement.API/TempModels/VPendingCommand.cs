using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class VPendingCommand
{
    public int CommandId { get; set; }

    public string? MachineName { get; set; }

    public string? StationName { get; set; }

    public string CommandType { get; set; } = null!;

    public string? ProgramName { get; set; }

    public string? Status { get; set; }

    public int? Priority { get; set; }

    public DateTime? CreatedTime { get; set; }
}
