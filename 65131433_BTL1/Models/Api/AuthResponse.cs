namespace _65131433_BTL1.Models.Api
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AuthResponseData? Data { get; set; }
    }

    public class AuthResponseData
    {
        public string MaTk { get; set; } = string.Empty;
        public string TenDangNhap { get; set; } = string.Empty;
        public string? MaBn { get; set; }
        public string? HoTen { get; set; }
        public int? DiemTichLuy { get; set; }
        public string Token { get; set; } = string.Empty;
    }

    public class UserInfoResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserInfoData? Data { get; set; }
    }

    public class UserInfoData
    {
        public string MaTk { get; set; } = string.Empty;
        public string TenDangNhap { get; set; } = string.Empty;
        public string? MaBn { get; set; }
        public string? HoTen { get; set; }
        public int? DiemTichLuy { get; set; }
    }
}