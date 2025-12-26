using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;

namespace _65131433_BTL1.Pages
{
    public class ThanhToanModel : PageModel
    {
        private readonly PhongKhamDbContext _context;
        private const decimal TIEN_KHAM = 100m;

        public ThanhToanModel(PhongKhamDbContext context)
        {
            _context = context;
        }

        public BenhNhan? BenhNhan { get; set; }
        public KhamBenh? KhamBenh { get; set; }
        public List<ChiTietThuocThanhToanViewModel> DanhSachThuoc { get; set; } = new();

        public decimal TienKham { get; set; } = TIEN_KHAM;
        public decimal TongTienThuoc { get; set; }
        public decimal TongCong { get; set; }
        public decimal GiamGiaBHYT { get; set; }
        public decimal ThanhTien { get; set; }
        public bool CoMaBHYT { get; set; }

        public async Task<IActionResult> OnGetAsync(string maBn)
        {
            if (string.IsNullOrEmpty(maBn))
            {
                return RedirectToPage("/DanhSachBenhNhan");
            }

            // Load b?nh nhân + phi?u khám
            BenhNhan = await _context.BenhNhans
                .Include(b => b.KhamBenh)
                .FirstOrDefaultAsync(b => b.MaBn == maBn);

            if (BenhNhan == null || BenhNhan.KhamBenh == null)
            {
                TempData["ErrorMessage"] = "Không tìm th?y b?nh nhân ho?c phi?u khám!";
                return RedirectToPage("/DanhSachBenhNhan");
            }

            KhamBenh = BenhNhan.KhamBenh;

            // Ki?m tra tr?ng thái
            if (KhamBenh.TrangThai != "?ã khám")
            {
                TempData["ErrorMessage"] = $"Ch? có th? thanh toán khi phi?u khám ? tr?ng thái '?ã khám'. Hi?n t?i: {KhamBenh.TrangThai}";
                return RedirectToPage("/DanhSachBenhNhan");
            }

            // Ki?m tra có BHYT
            CoMaBHYT = !string.IsNullOrEmpty(BenhNhan.Bhyt);

            // Load danh sách thu?c t? toa
            DanhSachThuoc = await _context.ToaThuocs
                .Where(t => t.MaKham == KhamBenh.MaKham)
                .Join(_context.Thuocs,
                    tt => tt.MaThuoc,
                    t => t.MaThuoc,
                    (tt, t) => new ChiTietThuocThanhToanViewModel
                    {
                        MaThuoc = t.MaThuoc,
                        TenThuoc = t.TenThuoc,
                        SoLuong = tt.SoLuong,
                        DonGia = t.GiaBan,
                        ThanhTien = tt.SoLuong * t.GiaBan,
                        SoLuongConTrongKho = t.SoLuong
                    })
                .ToListAsync();

            // Tính ti?n
            TongTienThuoc = DanhSachThuoc.Sum(t => t.ThanhTien);
            TongCong = TienKham + TongTienThuoc;

            // N?u có BHYT ? Gi?m 80%, ch? tr? 20%
            if (CoMaBHYT)
            {
                GiamGiaBHYT = TongCong * 0.8m;
                ThanhTien = TongCong * 0.2m;
            }
            else
            {
                ThanhTien = TongCong;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string maBn)
        {
            try
            {
                // Load b?nh nhân + phi?u khám
                var benhNhan = await _context.BenhNhans
                    .Include(b => b.KhamBenh)
                    .Include(b => b.TaiKhoanBenhNhan)
                    .FirstOrDefaultAsync(b => b.MaBn == maBn);

                if (benhNhan == null || benhNhan.KhamBenh == null)
                {
                    TempData["ErrorMessage"] = "Không tìm th?y b?nh nhân!";
                    return RedirectToPage("/DanhSachBenhNhan");
                }

                var khamBenh = benhNhan.KhamBenh;

                // Ki?m tra tr?ng thái
                if (khamBenh.TrangThai != "?ã khám")
                {
                    TempData["ErrorMessage"] = "Phi?u khám không ? tr?ng thái '?ã khám'!";
                    return RedirectToPage("/DanhSachBenhNhan");
                }

                // Tính ti?n
                var danhSachThuoc = await _context.ToaThuocs
                    .Where(t => t.MaKham == khamBenh.MaKham)
                    .Join(_context.Thuocs,
                        tt => tt.MaThuoc,
                        t => t.MaThuoc,
                        (tt, t) => new { tt.MaThuoc, tt.SoLuong, t.GiaBan })
                    .ToListAsync();

                decimal tongTienThuoc = danhSachThuoc.Sum(t => t.SoLuong * t.GiaBan);
                decimal tongCong = TIEN_KHAM + tongTienThuoc;

                bool coMaBHYT = !string.IsNullOrEmpty(benhNhan.Bhyt);
                decimal thanhTien = coMaBHYT ? tongCong * 0.2m : tongCong;

                // Sinh mã hóa ??n
                string maHd = await GenerateMaHoaDon();

                // T?o hóa ??n
                var hoaDon = new HoaDon
                {
                    MaHd = maHd,
                    ThanhTien = thanhTien,
                    NgayLap = DateOnly.FromDateTime(DateTime.Now),
                    DiemTichLuySuDung = 0,
                    MaKham = khamBenh.MaKham,
                    MaNv = "NV001"
                };

                _context.HoaDons.Add(hoaDon);

                // L?u chi ti?t hóa ??n + Tr? kho thu?c
                foreach (var thuoc in danhSachThuoc)
                {
                    // Thêm chi ti?t hóa ??n
                    var chiTiet = new ChiTietHoaDon
                    {
                        MaHd = maHd,
                        MaThuoc = thuoc.MaThuoc,
                        SoLuong = thuoc.SoLuong,
                        DonGia = thuoc.GiaBan
                    };
                    _context.ChiTietHoaDons.Add(chiTiet);

                    // Tr? s? l??ng t? kho
                    var thuocTrongKho = await _context.Thuocs.FindAsync(thuoc.MaThuoc);
                    if (thuocTrongKho != null)
                    {
                        thuocTrongKho.SoLuong -= thuoc.SoLuong;
                    }
                }

                // C?p nh?t tr?ng thái phi?u khám
                khamBenh.TrangThai = "?ã thanh toán";

                // C?ng ?i?m tích l?y (+1)
                if (benhNhan.TaiKhoanBenhNhan != null)
                {
                    benhNhan.TaiKhoanBenhNhan.DiemTichLuy = (benhNhan.TaiKhoanBenhNhan.DiemTichLuy ?? 0) + 1;
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Thanh toán thành công! Mã hóa ??n: {maHd}";
                return RedirectToPage("/ChiTietHoaDon", new { maHd = maHd });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có l?i x?y ra: {ex.Message}";
                return await OnGetAsync(maBn);
            }
        }

        private async Task<string> GenerateMaHoaDon()
        {
            var lastHoaDon = await _context.HoaDons
                .OrderByDescending(h => h.MaHd)
                .FirstOrDefaultAsync();

            if (lastHoaDon == null)
            {
                return "HD001";
            }

            string lastMa = lastHoaDon.MaHd;
            string numberPart = lastMa.Substring(2);
            int nextNumber = int.Parse(numberPart) + 1;

            return "HD" + nextNumber.ToString("D3");
        }
    }

    // ViewModel
    public class ChiTietThuocThanhToanViewModel
    {
        public string MaThuoc { get; set; } = string.Empty;
        public string TenThuoc { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public int SoLuongConTrongKho { get; set; }
    }
}