using System;
using System.Collections.Generic;

namespace _65131433_BTL1.Models;

public partial class KhamBenh
{
    public string MaKham { get; set; } = null!;

    public string? ChuanDoan { get; set; }

    public string? HuongXuTri { get; set; }

    public string MaBn { get; set; } = null!;

    public string MaBs { get; set; } = null!;

    public string? LyDoKham { get; set; }

    public string? QuaTrinhBenhLy { get; set; }

    public string? TienSuBenhNhan { get; set; }

    public string? TienSuGiaDinh { get; set; }

    public string? KhamBoPhan { get; set; }

    public string MaCd { get; set; } = null!;

    public string? LoaiKham { get; set; }

    public string? XuTriKham { get; set; }

    public DateOnly? NgayKham { get; set; }

    public string? TrangThai { get; set; }

    public virtual HoaDon? HoaDon { get; set; }

    public virtual BenhNhan MaBnNavigation { get; set; } = null!;

    public virtual BacSi MaBsNavigation { get; set; } = null!;

    public virtual ChuanDoan MaCdNavigation { get; set; } = null!;

    public virtual ICollection<ToaThuoc> ToaThuocs { get; set; } = new List<ToaThuoc>();
}
