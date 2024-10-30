using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PersonVirus
{
    public int PersonId { get; set; }

    public int VirusId { get; set; }

    public double? ResistanceRate { get; set; }

    public virtual Person Person { get; set; } = null!;

    public virtual Virus Virus { get; set; } = null!;
}
