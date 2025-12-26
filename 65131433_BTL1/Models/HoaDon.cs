using System;
using System.Collections.Generic;

namespace _65131433_BTL1.Models;

public partial class HoaDon
{
    public string MaHd { get; set; } = null!;

    public decimal ThanhTien { get; set; }

    public DateOnly? NgayLap { get; set; }

    public int? DiemTichLuySuDung { get; set; }

    public string MaKham { get; set; } = null!;

    public string MaNv { get; set; } = null!;

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual KhamBenh MaKhamNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
