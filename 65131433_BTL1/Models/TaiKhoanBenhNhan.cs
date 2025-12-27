using System;
using System.Collections.Generic;

namespace _65131433_BTL1.Models;

public partial class TaiKhoanBenhNhan
{
    public string MaTk { get; set; } = null!;

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public int? DiemTichLuy { get; set; }

    public string? MaBn { get; set; }

    public string? HoTenBn { get; set; }

    public virtual BenhNhan? MaBnNavigation { get; set; }
}
