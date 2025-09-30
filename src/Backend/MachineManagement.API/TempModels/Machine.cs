using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class Machine
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Status { get; set; }

    public int? MachineTypeId { get; set; }

    public string? Ip { get; set; }

    public string? GmesName { get; set; }

    public int? StationId { get; set; }

    public string? ProgramName { get; set; }

    /// <summary>
    /// MAC address of the machine
    /// </summary>
    public string? MacAddress { get; set; }

    public DateTime? LastLogTime { get; set; }

    public string? AppVersion { get; set; }

    public string? ClientStatus { get; set; }

    public DateTime? LastSeen { get; set; }

    public virtual ICollection<ClientConfig> ClientConfigs { get; set; } = new List<ClientConfig>();

    public virtual ICollection<Command> Commands { get; set; } = new List<Command>();

    public virtual ICollection<LogDatum> LogData { get; set; } = new List<LogDatum>();

    public virtual ICollection<LogFile> LogFiles { get; set; } = new List<LogFile>();

    public virtual Machinetype? MachineType { get; set; }

    public virtual Station? Station { get; set; }
}
