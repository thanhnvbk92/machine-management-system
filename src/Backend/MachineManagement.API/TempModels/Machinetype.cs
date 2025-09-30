using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class Machinetype
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
}
