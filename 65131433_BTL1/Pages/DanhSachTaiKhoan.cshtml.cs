using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace _65131433_BTL1.Pages
{
    public class DanhSachTaiKhoanModel : PageModel
    {
        private readonly PhongKhamDbContext _context;
        private const int PageSize = 10;

        public DanhSachTaiKhoanModel(PhongKhamDbContext context)
        {
            _context = context;
        }

        public List<TaiKhoanBenhNhan> TaiKhoans { get; set; } = new List<TaiKhoanBenhNhan>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string SearchString { get; set; } = string.Empty;

        public async Task OnGetAsync(string searchString, int pageNumber = 1)
        {
            try
            {
                SearchString = searchString ?? string.Empty;
                CurrentPage = pageNumber < 1 ? 1 : pageNumber;

                var query = _context.TaiKhoanBenhNhans
                    .Include(t => t.MaBnNavigation)
                    .AsQueryable();

                // Filter theo tìm kiếm
                if (!string.IsNullOrEmpty(SearchString))
                {
                    query = query.Where(t => 
                        t.TenDangNhap.Contains(SearchString) ||
                        (t.MaBnNavigation != null && t.MaBnNavigation.HoTenBn.Contains(SearchString)));
                }

                TotalItems = await query.CountAsync();
                TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

                if (CurrentPage > TotalPages && TotalPages > 0)
                {
                    CurrentPage = TotalPages;
                }

                TaiKhoans = await query
                    .OrderByDescending(t => t.MaBn != null)
                    .ThenBy(t => t.TenDangNhap)
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ LỖI OnGetAsync DanhSachTaiKhoan: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                TaiKhoans = new List<TaiKhoanBenhNhan>();
            }
        }
    }
}

