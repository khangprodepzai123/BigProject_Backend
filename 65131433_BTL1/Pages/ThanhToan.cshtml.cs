using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;
using System;

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

            // Load bệnh nhân + phiếu khám
            BenhNhan = await _context.BenhNhans
                .Include(b => b.KhamBenh)
                .FirstOrDefaultAsync(b => b.MaBn == maBn);

            if (BenhNhan == null || BenhNhan.KhamBenh == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bệnh nhân hoặc phiếu khám!";
                return RedirectToPage("/DanhSachBenhNhan");
            }

            KhamBenh = BenhNhan.KhamBenh;

            // TẠM TẮT VALIDATE TRẠNG THÁI - Cho phép thanh toán bất kỳ lúc nào
            // Kiểm tra trạng thái
            // if (KhamBenh.TrangThai != "Đã khám")
            // {
            //     TempData["ErrorMessage"] = $"Chỉ có thể thanh toán khi phiếu khám ở trạng thái 'Đã khám'. Hiện tại: {KhamBenh.TrangThai}";
            //     return RedirectToPage("/DanhSachBenhNhan");
            // }

            // Kiểm tra có BHYT
            CoMaBHYT = !string.IsNullOrEmpty(BenhNhan.Bhyt);

            // Load danh sách thuốc từ toa
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

            // Tính tiền
            TongTienThuoc = DanhSachThuoc.Sum(t => t.ThanhTien);
            TongCong = TienKham + TongTienThuoc;

            // Nếu có BHYT → Giảm 80%, chỉ trả 20%
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
                // Load bệnh nhân + phiếu khám
                var benhNhan = await _context.BenhNhans
                    .Include(b => b.KhamBenh)
                    .Include(b => b.TaiKhoanBenhNhan)
                    .FirstOrDefaultAsync(b => b.MaBn == maBn);

                if (benhNhan == null || benhNhan.KhamBenh == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bệnh nhân!";
                    return RedirectToPage("/DanhSachBenhNhan");
                }

                var khamBenh = benhNhan.KhamBenh;

                // TẠM TẮT VALIDATE TRẠNG THÁI - Cho phép thanh toán bất kỳ lúc nào
                // Kiểm tra trạng thái
                // if (khamBenh.TrangThai != "Đã khám")
                // {
                //     TempData["ErrorMessage"] = "Phiếu khám không ở trạng thái 'Đã khám'!";
                //     return RedirectToPage("/DanhSachBenhNhan");
                // }

                // Tính tiền
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

                // Sinh mã hóa đơn
                string maHd = await GenerateMaHoaDon();

                // Tạo hóa đơn
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

                // Lưu chi tiết hóa đơn + Trừ kho thuốc
                foreach (var thuoc in danhSachThuoc)
                {
                    // Thêm chi tiết hóa đơn
                    var chiTiet = new ChiTietHoaDon
                    {
                        MaHd = maHd,
                        MaThuoc = thuoc.MaThuoc,
                        SoLuong = thuoc.SoLuong,
                        DonGia = thuoc.GiaBan
                    };
                    _context.ChiTietHoaDons.Add(chiTiet);

                    // Trừ số lượng từ kho
                    var thuocTrongKho = await _context.Thuocs.FindAsync(thuoc.MaThuoc);
                    if (thuocTrongKho != null)
                    {
                        thuocTrongKho.SoLuong -= thuoc.SoLuong;
                    }
                }

                // Cập nhật trạng thái phiếu khám
                khamBenh.TrangThai = "Đã thanh toán";

                // Cộng điểm tích lũy (+1)
                if (benhNhan.TaiKhoanBenhNhan != null)
                {
                    benhNhan.TaiKhoanBenhNhan.DiemTichLuy = (benhNhan.TaiKhoanBenhNhan.DiemTichLuy ?? 0) + 1;
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Thanh toán thành công! Mã hóa đơn: {maHd}";
                return RedirectToPage("/ChiTietHoaDon", new { maHd = maHd });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
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
