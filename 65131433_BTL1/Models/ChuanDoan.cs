using System;
using System.Collections.Generic;

namespace _65131433_BTL1.Models;

public partial class ChuanDoan
{
    public string MaCd { get; set; } = null!;

    public string TenCd { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<KhamBenh> KhamBenhs { get; set; } = new List<KhamBenh>();
}
