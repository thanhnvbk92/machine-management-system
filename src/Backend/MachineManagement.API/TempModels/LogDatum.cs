using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class LogDatum
{
    public int LogId { get; set; }

    public int FileId { get; set; }

    public int MachineId { get; set; }

    public int? StationId { get; set; }

    public int? ModelId { get; set; }

    public string? Eqpinfo { get; set; }

    public string? Procinfo { get; set; }

    public string Pid { get; set; } = null!;

    public string? Fid { get; set; }

    public string? PartNo { get; set; }

    public string? Variant { get; set; }

    public string Result { get; set; } = null!;

    public string Jobfile { get; set; } = null!;

    public string GmesStatus { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? StepNg { get; set; }

    public string? Measure { get; set; }

    public string? SpecMin { get; set; }

    public string? SpecMax { get; set; }

    public string? LogLevel { get; set; }

    public string? Source { get; set; }

    public string? RawData { get; set; }

    public DateTime? ReceivedTime { get; set; }

    public virtual LogFile File { get; set; } = null!;

    public virtual Machine Machine { get; set; } = null!;

    public virtual Model? Model { get; set; }

    public virtual Station? Station { get; set; }
}
