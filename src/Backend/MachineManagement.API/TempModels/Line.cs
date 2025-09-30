using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class Line
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Station> Stations { get; set; } = new List<Station>();
}
