using System;
using System.Collections.Generic;

namespace _65131433_BTL1.Models;

public partial class ToaThuoc
{
    public string MaKham { get; set; } = null!;

    public string MaThuoc { get; set; } = null!;

    public int SoLuong { get; set; }

    public string? LieuDung { get; set; }

    public string? CachDung { get; set; }

    public virtual KhamBenh MaKhamNavigation { get; set; } = null!;

    public virtual Thuoc MaThuocNavigation { get; set; } = null!;
}
