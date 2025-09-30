using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class Command
{
    public int CommandId { get; set; }

    public int? MachineId { get; set; }

    public int? StationId { get; set; }

    public string CommandType { get; set; } = null!;

    public string? ProgramName { get; set; }

    public string? Parameters { get; set; }

    public string? Status { get; set; }

    public int? Priority { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? SentTime { get; set; }

    public DateTime? ExecutedTime { get; set; }

    public string? ResultMessage { get; set; }

    public virtual Machine? Machine { get; set; }

    public virtual Station? Station { get; set; }
}
