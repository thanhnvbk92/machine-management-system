using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class ClientConfig
{
    public int ConfigId { get; set; }

    public int? MachineId { get; set; }

    public string ConfigKey { get; set; } = null!;

    public string? ConfigValue { get; set; }

    public string? Description { get; set; }

    public DateTime? UpdatedTime { get; set; }

    public virtual Machine? Machine { get; set; }
}
