using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;

namespace _65131433_BTL1.Pages
{
    public class ChiTietHoaDonModel : PageModel
    {
        private readonly PhongKhamDbContext _context;
        private const decimal TIEN_KHAM = 100m;

        public ChiTietHoaDonModel(PhongKhamDbContext context)
        {
            _context = context;
        }

        public HoaDon? HoaDon { get; set; }
        public BenhNhan? BenhNhan { get; set; }
        public KhamBenh? KhamBenh { get; set; }
        public BacSi? BacSi { get; set; }
        public List<ChiTietHoaDonDisplay> DanhSachChiTiet { get; set; } = new();
        public decimal TongTienThuoc { get; set; }
        public decimal GiamGiaBHYT { get; set; }

        public async Task<IActionResult> OnGetAsync(string maHd)
        {
            if (string.IsNullOrEmpty(maHd))
            {
                return RedirectToPage("/DanhSachBenhNhan");
            }

            // Load hóa ??n + chi ti?t
            HoaDon = await _context.HoaDons
                .Include(h => h.MaKhamNavigation)
                    .ThenInclude(k => k.MaBnNavigation)
                .Include(h => h.MaKhamNavigation)
                    .ThenInclude(k => k.MaBsNavigation)
                .Include(h => h.ChiTietHoaDons)
                .FirstOrDefaultAsync(h => h.MaHd == maHd);

            if (HoaDon == null)
            {
                TempData["ErrorMessage"] = "Không tìm th?y hóa ??n!";
                return RedirectToPage("/DanhSachBenhNhan");
            }

            KhamBenh = HoaDon.MaKhamNavigation;
            BenhNhan = KhamBenh?.MaBnNavigation;
            BacSi = KhamBenh?.MaBsNavigation;

            // Load chi ti?t hóa ??n
            DanhSachChiTiet = await _context.ChiTietHoaDons
                .Where(c => c.MaHd == maHd)
                .Join(_context.Thuocs,
                    ct => ct.MaThuoc,
                    t => t.MaThuoc,
                    (ct, t) => new ChiTietHoaDonDisplay
                    {
                        TenThuoc = t.TenThuoc,
                        SoLuong = ct.SoLuong,
                        DonGia = ct.DonGia,
                        ThanhTien = ct.SoLuong * ct.DonGia
                    })
                .ToListAsync();

            // Tính ti?n
            TongTienThuoc = DanhSachChiTiet.Sum(c => c.ThanhTien);

            // Tính gi?m giá
            if (!string.IsNullOrEmpty(BenhNhan?.Bhyt))
            {
                decimal tongCong = TIEN_KHAM + TongTienThuoc;
                GiamGiaBHYT = tongCong * 0.8m;
            }

            return Page();
        }
    }

    public class ChiTietHoaDonDisplay
    {
        public string TenThuoc { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}