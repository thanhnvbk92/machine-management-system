using System;
using System.Collections.Generic;

namespace MachineManagement.API.TempModels;

public partial class Modelgroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int BuyerId { get; set; }

    public virtual Buyer Buyer { get; set; } = null!;

    public virtual ICollection<Modelprocess> Modelprocesses { get; set; } = new List<Modelprocess>();

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
