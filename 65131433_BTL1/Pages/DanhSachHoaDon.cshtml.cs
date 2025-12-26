using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;

namespace _65131433_BTL1.Pages
{
    public class DanhSachHoaDonModel : PageModel
    {
        private readonly PhongKhamDbContext _context;

        public DanhSachHoaDonModel(PhongKhamDbContext context)
        {
            _context = context;
        }

        public List<HoaDonViewModel> DanhSachHoaDon { get; set; } = new();
        public int TongHoaDon { get; set; }
        public decimal TongDoanhThu { get; set; }

        public async Task OnGetAsync()
        {
            // Load danh sách hóa ??n + thông tin liên quan
            var hoaDonList = await _context.HoaDons
                .Include(h => h.MaKhamNavigation)
                    .ThenInclude(k => k.MaBnNavigation)
                .OrderByDescending(h => h.NgayLap)
                .ToListAsync();

            // Map sang ViewModel
            DanhSachHoaDon = hoaDonList.Select(h => new HoaDonViewModel
            {
                MaHd = h.MaHd,
                MaBn = h.MaKhamNavigation?.MaBnNavigation?.MaBn ?? "N/A",
                TenBn = h.MaKhamNavigation?.MaBnNavigation?.HoTenBn ?? "N/A",
                NgayLap = h.NgayLap,
                ThanhTien = h.ThanhTien,
                MaKham = h.MaKham
            }).ToList();

            TongHoaDon = DanhSachHoaDon.Count;
            TongDoanhThu = DanhSachHoaDon.Sum(h => h.ThanhTien);
        }
    }

    // ViewModel
    public class HoaDonViewModel
    {
        public string MaHd { get; set; } = string.Empty;
        public string MaBn { get; set; } = string.Empty;
        public string TenBn { get; set; } = string.Empty;
        public DateOnly? NgayLap { get; set; }
        public decimal ThanhTien { get; set; }
        public string MaKham { get; set; } = string.Empty;
    }
}