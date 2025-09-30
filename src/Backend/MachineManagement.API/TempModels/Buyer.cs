using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class Buyer
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Modelgroup> Modelgroups { get; set; } = new List<Modelgroup>();
}
