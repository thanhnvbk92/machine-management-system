using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class Model
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ModelGroupId { get; set; }

    public virtual ICollection<LogDatum> LogData { get; set; } = new List<LogDatum>();

    public virtual Modelgroup ModelGroup { get; set; } = null!;
}
