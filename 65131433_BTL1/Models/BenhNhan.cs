using System;
using System.Collections.Generic;

namespace _65131433_BTL1.Models;

public partial class BenhNhan
{
    public string MaBn { get; set; } = null!;

    public string HoTenBn { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? Gt { get; set; }

    public string? DoiTuong { get; set; }

    public string? DiaChi { get; set; }

    public string? Bhyt { get; set; }

    public virtual KhamBenh? KhamBenh { get; set; }

    public virtual TaiKhoanBenhNhan? TaiKhoanBenhNhan { get; set; }
}
