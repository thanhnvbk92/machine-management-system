using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class LogFile
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public int MachineId { get; set; }

    public DateOnly DateCreated { get; set; }

    public long? FileSize { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedTime { get; set; }

    public virtual ICollection<LogDatum> LogData { get; set; } = new List<LogDatum>();

    public virtual Machine Machine { get; set; } = null!;
}
