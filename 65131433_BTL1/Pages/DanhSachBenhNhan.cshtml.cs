using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

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

        public async Task OnGetAsync(string searchString, int pageNumber = 1)
        {
            SearchString = searchString ?? string.Empty;
            CurrentPage = pageNumber < 1 ? 1 : pageNumber;

            //var query = _context.BenhNhans.AsQueryable();
            var query = _context.BenhNhans
       .Include(b => b.KhamBenh)  // ? Thêm dòng này ?? load KhamBenh
       .AsQueryable();

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
                .OrderBy(b => b.MaBn)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        // Handler t?o phi?u khám (GET)
        // Handler t?o/m? phi?u khám (GET)
        public async Task<IActionResult> OnGetTaoPhieuKhamAsync(string maBn)
        {
            try
            {
                Console.WriteLine($"=== B?T ??U - MaBn: {maBn} ===");

                // Ki?m tra b?nh nhân
                var benhNhan = await _context.BenhNhans
                    .Include(b => b.KhamBenh)
                    .FirstOrDefaultAsync(b => b.MaBn == maBn);

                if (benhNhan == null)
                {
                    Console.WriteLine("KHÔNG TÌM TH?Y B?NH NHÂN!");
                    TempData["ErrorMessage"] = "Không tìm th?y b?nh nhân!";
                    return RedirectToPage();
                }

                Console.WriteLine($"Tìm th?y b?nh nhân: {benhNhan.HoTenBn}");

                // KI?M TRA: B?nh nhân ?ã có phi?u khám ch?a?
                if (benhNhan.KhamBenh != null && benhNhan.KhamBenh.TrangThai != "?ã thanh toán")
                {
                    // Có phi?u ch?a thanh toán ? M? phi?u ?ó
                    Console.WriteLine($"B?nh nhân ?ã có phi?u khám: {benhNhan.KhamBenh.MaKham}");
                    return RedirectToPage("/KhamBenh", new { maKham = benhNhan.KhamBenh.MaKham });
                }

                // Không có phi?u ? T?O PHI?U M?I
                Console.WriteLine("B?nh nhân ch?a có phi?u khám, t?o phi?u m?i...");

                // L?y bác s? ??u tiên
                var bacSiMacDinh = await _context.BacSis.FirstOrDefaultAsync();
                if (bacSiMacDinh == null)
                {
                    Console.WriteLine("KHÔNG CÓ BÁC S?!");
                    TempData["ErrorMessage"] = "Ch?a có bác s? trong h? th?ng!";
                    return RedirectToPage();
                }

                // L?y chu?n ?oán ??u tiên
                var chuanDoanMacDinh = await _context.ChuanDoans.FirstOrDefaultAsync();
                if (chuanDoanMacDinh == null)
                {
                    Console.WriteLine("KHÔNG CÓ CHU?N ?OÁN!");
                    TempData["ErrorMessage"] = "Ch?a có chu?n ?oán trong h? th?ng!";
                    return RedirectToPage();
                }

                // T?o mã khám
                var lastKhamBenh = await _context.KhamBenhs
                    .OrderByDescending(k => k.MaKham)
                    .FirstOrDefaultAsync();

                string maKham = lastKhamBenh == null
                    ? "KB001"
                    : "KB" + (int.Parse(lastKhamBenh.MaKham.Substring(2)) + 1).ToString("D3");

                Console.WriteLine($"Mã khám m?i: {maKham}");

                // T?o phi?u khám m?i
                var khamBenh = new KhamBenh
                {
                    MaKham = maKham,
                    MaBn = maBn,
                    MaBs = bacSiMacDinh.MaBs,
                    MaCd = chuanDoanMacDinh.MaCd,
                    NgayKham = DateOnly.FromDateTime(DateTime.Now),
                    LoaiKham = "Lâm sàng",
                    XuTriKham = "K?t thúc ?i?u tr?",
                    TrangThai = "?ang khám"
                };

                _context.KhamBenhs.Add(khamBenh);
                await _context.SaveChangesAsync();

                Console.WriteLine($"T?o phi?u khám thành công: {maKham}");
                return RedirectToPage("/KhamBenh", new { maKham = maKham });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"L?I DATABASE: {dbEx.InnerException?.Message}");
                TempData["ErrorMessage"] = $"L?i database: {dbEx.InnerException?.Message}";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"L?I: {ex.Message}");
                TempData["ErrorMessage"] = $"Có l?i: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
}