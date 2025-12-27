using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace _65131433_BTL1.Pages
{
    public class DanhSachBenhNhanModel : PageModel
    {
        private readonly PhongKhamDbContext _context;
        private const int PageSize = 5;

        public DanhSachBenhNhanModel(PhongKhamDbContext context)
        {
            _context = context;
        }

        public List<BenhNhan> BenhNhans { get; set; } = new List<BenhNhan>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string SearchString { get; set; } = string.Empty;
        public string TimeFilter { get; set; } = "all"; // all, today, week, month

        public async Task OnGetAsync(string searchString, string timeFilter = "all", int pageNumber = 1)
        {
            try
            {
                SearchString = searchString ?? string.Empty;
                TimeFilter = timeFilter ?? "all";
                CurrentPage = pageNumber < 1 ? 1 : pageNumber;

                var query = _context.BenhNhans
                    .Include(b => b.KhamBenh)
                    .AsQueryable();

            // Filter theo thời gian đăng ký khám
            var today = DateOnly.FromDateTime(DateTime.Now);
            switch (TimeFilter.ToLower())
            {
                case "today":
                    query = query.Where(b => b.KhamBenh != null && b.KhamBenh.NgayKham == today);
                    break;
                case "week":
                    var weekAgo = today.AddDays(-7);
                    query = query.Where(b => b.KhamBenh != null && 
                        b.KhamBenh.NgayKham >= weekAgo && b.KhamBenh.NgayKham <= today);
                    break;
                case "month":
                    var monthAgo = today.AddMonths(-1);
                    query = query.Where(b => b.KhamBenh != null && 
                        b.KhamBenh.NgayKham >= monthAgo && b.KhamBenh.NgayKham <= today);
                    break;
                case "all":
                default:
                    // Không filter, hiển thị tất cả
                    break;
            }

            // Filter theo tìm kiếm
            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(b => b.HoTenBn.Contains(SearchString));
            }

            TotalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }

                BenhNhans = await query
                    .OrderByDescending(b => b.KhamBenh != null ? b.KhamBenh.NgayKham : (DateOnly?)null)
                    .ThenBy(b => b.MaBn)
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ LỖI OnGetAsync: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                BenhNhans = new List<BenhNhan>();
            }
        }

        // Handler tạo phiếu khám (GET)
        public async Task<IActionResult> OnGetTaoPhieuKhamAsync(string maBn)
        {
            try
            {
                Console.WriteLine($"=== BẮT ĐẦU - MaBn: {maBn} ===");

                // Kiểm tra bệnh nhân
                var benhNhan = await _context.BenhNhans
                    .Include(b => b.KhamBenh)
                    .FirstOrDefaultAsync(b => b.MaBn == maBn);

                if (benhNhan == null)
                {
                    Console.WriteLine("KHÔNG TÌM THẤY BỆNH NHÂN!");
                    TempData["ErrorMessage"] = "Không tìm thấy bệnh nhân!";
                    return RedirectToPage();
                }

                Console.WriteLine($"Tìm thấy bệnh nhân: {benhNhan.HoTenBn}");

                // KIỂM TRA: Bệnh nhân đã có phiếu khám chưa?
                if (benhNhan.KhamBenh != null && benhNhan.KhamBenh.TrangThai != "Đã thanh toán")
                {
                    // Có phiếu chưa thanh toán → Mở phiếu đã có
                    Console.WriteLine($"Bệnh nhân đã có phiếu khám: {benhNhan.KhamBenh.MaKham}");
                    return RedirectToPage("/KhamBenh", new { maKham = benhNhan.KhamBenh.MaKham });
                }

                // Không có phiếu → TẠO PHIẾU MỚI
                Console.WriteLine("Bệnh nhân chưa có phiếu khám, tạo phiếu mới...");

                // Lấy bác sĩ đầu tiên
                var bacSiMacDinh = await _context.BacSis.FirstOrDefaultAsync();
                if (bacSiMacDinh == null)
                {
                    Console.WriteLine("KHÔNG CÓ BÁC SĨ!");
                    TempData["ErrorMessage"] = "Chưa có bác sĩ trong hệ thống!";
                    return RedirectToPage();
                }

                // Lấy chuẩn đoán đầu tiên
                var chuanDoanMacDinh = await _context.ChuanDoans.FirstOrDefaultAsync();
                if (chuanDoanMacDinh == null)
                {
                    Console.WriteLine("KHÔNG CÓ CHUẨN ĐOÁN!");
                    TempData["ErrorMessage"] = "Chưa có chuẩn đoán trong hệ thống!";
                    return RedirectToPage();
                }

                // Tạo mã khám
                var lastKhamBenh = await _context.KhamBenhs
                    .OrderByDescending(k => k.MaKham)
                    .FirstOrDefaultAsync();

                string maKham = lastKhamBenh == null
                    ? "KB001"
                    : "KB" + (int.Parse(lastKhamBenh.MaKham.Substring(2)) + 1).ToString("D3");

                Console.WriteLine($"Mã khám mới: {maKham}");

                // Tạo phiếu khám mới
                var khamBenh = new KhamBenh
                {
                    MaKham = maKham,
                    MaBn = maBn,
                    MaBs = bacSiMacDinh.MaBs,
                    MaCd = chuanDoanMacDinh.MaCd,
                    NgayKham = DateOnly.FromDateTime(DateTime.Now),
                    LoaiKham = "Lâm sàng",
                    XuTriKham = "Kết thúc điều trị",
                    TrangThai = "Đang khám"
                };

                _context.KhamBenhs.Add(khamBenh);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Tạo phiếu khám thành công: {maKham}");
                return RedirectToPage("/KhamBenh", new { maKham = maKham });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"LỖI DATABASE: {dbEx.InnerException?.Message}");
                TempData["ErrorMessage"] = $"Lỗi database: {dbEx.InnerException?.Message}";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LỖI: {ex.Message}");
                TempData["ErrorMessage"] = $"Có lỗi: {ex.Message}";
                return RedirectToPage();
            }
        }

        // Handler khám lại
        public async Task<IActionResult> OnGetKhamLaiAsync(string maBn)
        {
            try
            {
                var benhNhan = await _context.BenhNhans
                    .Include(b => b.KhamBenh)
                    .FirstOrDefaultAsync(b => b.MaBn == maBn);

                if (benhNhan == null || benhNhan.KhamBenh == null || benhNhan.KhamBenh.TrangThai != "Đã thanh toán")
                {
                    TempData["ErrorMessage"] = "Bệnh nhân chưa thanh toán, không thể khám lại!";
                    return RedirectToPage();
                }

                var oldMaKham = benhNhan.KhamBenh.MaKham;

                // Kiểm tra xem phiếu khám cũ đã được lưu vào bệnh án chưa
                var benhAnDaTonTai = await _context.BenhAns
                    .FirstOrDefaultAsync(b => b.MaKham == oldMaKham);

                if (benhAnDaTonTai == null)
                {
                    TempData["ErrorMessage"] = "Vui lòng lưu bệnh án trước khi khám lại!";
                    return RedirectToPage();
                }

                // Lấy bác sĩ mặc định
                var bacSiMacDinh = await _context.BacSis.FirstOrDefaultAsync();
                if (bacSiMacDinh == null)
                {
                    TempData["ErrorMessage"] = "Chưa có bác sĩ trong hệ thống!";
                    return RedirectToPage();
                }

                // Lấy chuẩn đoán mặc định
                var chuanDoanMacDinh = await _context.ChuanDoans.FirstOrDefaultAsync();
                if (chuanDoanMacDinh == null)
                {
                    TempData["ErrorMessage"] = "Chưa có chuẩn đoán trong hệ thống!";
                    return RedirectToPage();
                }

                // Xóa phiếu khám cũ và các dữ liệu liên quan (vì đã lưu vào bệnh án rồi)
                var oldKhamBenh = await _context.KhamBenhs
                    .Include(k => k.ToaThuocs)
                    .Include(k => k.HoaDon)
                        .ThenInclude(h => h.ChiTietHoaDons)
                    .FirstOrDefaultAsync(k => k.MaKham == oldMaKham);

                if (oldKhamBenh != null)
                {
                    // Xóa chi tiết hóa đơn
                    if (oldKhamBenh.HoaDon != null && oldKhamBenh.HoaDon.ChiTietHoaDons != null)
                    {
                        _context.ChiTietHoaDons.RemoveRange(oldKhamBenh.HoaDon.ChiTietHoaDons);
                    }

                    // Xóa hóa đơn
                    if (oldKhamBenh.HoaDon != null)
                    {
                        _context.HoaDons.Remove(oldKhamBenh.HoaDon);
                    }

                    // Xóa toa thuốc
                    if (oldKhamBenh.ToaThuocs != null && oldKhamBenh.ToaThuocs.Any())
                    {
                        _context.ToaThuocs.RemoveRange(oldKhamBenh.ToaThuocs);
                    }

                    // Xóa phiếu khám
                    _context.KhamBenhs.Remove(oldKhamBenh);
                    await _context.SaveChangesAsync();
                }

                // Tạo mã khám mới
                var lastKhamBenh = await _context.KhamBenhs
                    .OrderByDescending(k => k.MaKham)
                    .FirstOrDefaultAsync();

                string maKham = lastKhamBenh == null
                    ? "KB001"
                    : "KB" + (int.Parse(lastKhamBenh.MaKham.Substring(2)) + 1).ToString("D3");

                // Tạo phiếu khám mới
                var khamBenh = new KhamBenh
                {
                    MaKham = maKham,
                    MaBn = maBn,
                    MaBs = bacSiMacDinh.MaBs,
                    MaCd = chuanDoanMacDinh.MaCd,
                    NgayKham = DateOnly.FromDateTime(DateTime.Now),
                    LoaiKham = "Lâm sàng",
                    XuTriKham = "Kết thúc điều trị",
                    TrangThai = "Đang khám"
                };

                _context.KhamBenhs.Add(khamBenh);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Tạo phiếu khám mới thành công: {maKham}";
                return RedirectToPage("/KhamBenh", new { maKham = maKham });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"LỖI DATABASE: {dbEx.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {dbEx.StackTrace}");
                TempData["ErrorMessage"] = $"Lỗi database: {dbEx.InnerException?.Message ?? dbEx.Message}";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LỖI: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"Có lỗi: {ex.Message}";
                return RedirectToPage();
            }
        }

        // Handler lưu bệnh án
        public async Task<IActionResult> OnGetLuuBenhAnAsync(string maKham)
        {
            try
            {
                // Kiểm tra đã lưu chưa
                var benhAnDaTonTai = await _context.BenhAns
                    .FirstOrDefaultAsync(b => b.MaKham == maKham);

                if (benhAnDaTonTai != null)
                {
                    TempData["ErrorMessage"] = "Bệnh án này đã được lưu rồi!";
                    return RedirectToPage();
                }

                // Lấy thông tin khám bệnh
                var khamBenh = await _context.KhamBenhs
                    .Include(k => k.ToaThuocs)
                    .FirstOrDefaultAsync(k => k.MaKham == maKham);

                if (khamBenh == null || khamBenh.TrangThai != "Đã thanh toán")
                {
                    TempData["ErrorMessage"] = "Chỉ có thể lưu bệnh án khi đã thanh toán!";
                    return RedirectToPage();
                }

                // Sinh mã bệnh án
                var lastBenhAn = await _context.BenhAns
                    .OrderByDescending(b => b.MaBenhAn)
                    .FirstOrDefaultAsync();

                string maBenhAn = lastBenhAn == null
                    ? "BA001"
                    : "BA" + (int.Parse(lastBenhAn.MaBenhAn.Substring(2)) + 1).ToString("D3");

                // Tạo bệnh án
                var benhAn = new BenhAn
                {
                    MaBenhAn = maBenhAn,
                    MaKham = maKham,
                    MaBn = khamBenh.MaBn,
                    MaBs = khamBenh.MaBs,
                    LyDoKham = khamBenh.LyDoKham,
                    QuaTrinhBenhLy = khamBenh.QuaTrinhBenhLy,
                    TienSuBenhNhan = khamBenh.TienSuBenhNhan,
                    TienSuGiaDinh = khamBenh.TienSuGiaDinh,
                    KhamBoPhan = khamBenh.KhamBoPhan,
                    ChuanDoan = khamBenh.ChuanDoan,
                    HuongXuTri = khamBenh.HuongXuTri,
                    LoaiKham = khamBenh.LoaiKham,
                    XuTriKham = khamBenh.XuTriKham,
                    NgayKham = khamBenh.NgayKham,
                    NgayLuu = DateTime.Now
                };

                _context.BenhAns.Add(benhAn);

                // Lưu toa thuốc vào bệnh án
                foreach (var toaThuoc in khamBenh.ToaThuocs)
                {
                    var benhAnToaThuoc = new BenhAnToaThuoc
                    {
                        MaBenhAn = maBenhAn,
                        MaThuoc = toaThuoc.MaThuoc,
                        SoLuong = toaThuoc.SoLuong,
                        LieuDung = toaThuoc.LieuDung,
                        CachDung = toaThuoc.CachDung
                    };
                    _context.BenhAnToaThuocs.Add(benhAnToaThuoc);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Lưu bệnh án thành công! Mã bệnh án: {maBenhAn}";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi: {ex.Message}";
                return RedirectToPage();
            }
        }

    }
}
