using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class Modelprocess
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ModelGroupId { get; set; }

    public virtual Modelgroup ModelGroup { get; set; } = null!;

    public virtual ICollection<Station> Stations { get; set; } = new List<Station>();
}
