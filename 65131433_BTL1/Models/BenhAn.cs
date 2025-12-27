using System;
using System.Collections.Generic;

namespace _65131433_BTL1.Models;

public partial class BenhAn
{
    public string MaBenhAn { get; set; } = null!;

    public string MaKham { get; set; } = null!;

    public string MaBn { get; set; } = null!;

    public string MaBs { get; set; } = null!;

    public string? LyDoKham { get; set; }

    public string? QuaTrinhBenhLy { get; set; }

    public string? TienSuBenhNhan { get; set; }

    public string? TienSuGiaDinh { get; set; }

    public string? KhamBoPhan { get; set; }

    public string? ChuanDoan { get; set; }

    public string? HuongXuTri { get; set; }

    public string? LoaiKham { get; set; }

    public string? XuTriKham { get; set; }

    public DateOnly? NgayKham { get; set; }

    public DateTime? NgayLuu { get; set; }

    public virtual BenhNhan MaBnNavigation { get; set; } = null!;

    public virtual BacSi MaBsNavigation { get; set; } = null!;

    public virtual ICollection<BenhAnToaThuoc> BenhAnToaThuocs { get; set; } = new List<BenhAnToaThuoc>();
}
