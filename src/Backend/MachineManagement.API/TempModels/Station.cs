using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class Station
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ModelProcessId { get; set; }

    public int LineId { get; set; }

    public virtual ICollection<Command> Commands { get; set; } = new List<Command>();

    public virtual Line Line { get; set; } = null!;

    public virtual ICollection<LogDatum> LogData { get; set; } = new List<LogDatum>();

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();

    public virtual Modelprocess ModelProcess { get; set; } = null!;
}
