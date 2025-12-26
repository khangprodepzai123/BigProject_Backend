using System;
using System.Collections.Generic;

namespace _65131433_BTL1.Models;

public partial class BacSi
{
    public string MaBs { get; set; } = null!;

    public string HoTenBs { get; set; } = null!;

    public virtual ICollection<KhamBenh> KhamBenhs { get; set; } = new List<KhamBenh>();
}
