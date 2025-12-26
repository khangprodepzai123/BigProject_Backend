using System;
using System.Collections.Generic;

namespace _65131433_BTL1.Models;

public partial class ChiTietHoaDon
{
    public string MaHd { get; set; } = null!;

    public string MaThuoc { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    public virtual Thuoc MaThuocNavigation { get; set; } = null!;
}
