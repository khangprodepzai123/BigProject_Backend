using System;
using System.Collections.Generic;

namespace _65131433_BTL1.Models;

public partial class Thuoc
{
    public string MaThuoc { get; set; } = null!;

    public string TenThuoc { get; set; } = null!;

    public decimal GiaBan { get; set; }

    public int SoLuong { get; set; }

    public string? Hdsd { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual ICollection<ToaThuoc> ToaThuocs { get; set; } = new List<ToaThuoc>();
}
