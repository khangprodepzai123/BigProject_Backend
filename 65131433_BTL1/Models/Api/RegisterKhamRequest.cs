namespace _65131433_BTL1.Models.Api
{
    public class RegisterKhamRequest
    {
        // Thông tin bệnh nhân
        public string HoTenBn { get; set; } = string.Empty;
        public string Sdt { get; set; } = string.Empty;
        public string? NgaySinh { get; set; } // Format: "yyyy-MM-dd"
        public string? Gt { get; set; }
        public string? DoiTuong { get; set; }
        public string? DiaChi { get; set; }
        public string? Bhyt { get; set; }

        // Thông tin đăng ký khám
        public string? LyDoKham { get; set; }
        public string? ThoiGianHenKham { get; set; } // Format: "yyyy-MM-dd HH:mm" (ngày giờ hẹn khám)
    }
}

