namespace _65131433_BTL1.Models.Api
{
    public class RegisterKhamResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public RegisterKhamData? Data { get; set; }
    }

    public class RegisterKhamData
    {
        public string MaBn { get; set; } = string.Empty;
        public string MaKham { get; set; } = string.Empty;
        public string HoTenBn { get; set; } = string.Empty;
        public DateTime? ThoiGianHenKham { get; set; }
    }
}

