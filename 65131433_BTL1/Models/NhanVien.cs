using System;
using System.Collections.Generic;

namespace _65131433_BTL1.Models;

public partial class NhanVien
{
    public string MaNv { get; set; } = null!;

    public string HoTenNv { get; set; } = null!;

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
