using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace _65131433_BTL1.Pages
{
    public class DanhSachBenhAnModel : PageModel
    {
        private readonly PhongKhamDbContext _context;
        private const int PageSize = 10;

        public DanhSachBenhAnModel(PhongKhamDbContext context)
        {
            _context = context;
        }

        public List<BenhAn> BenhAns { get; set; } = new List<BenhAn>();
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

                var query = _context.BenhAns
                    .Include(b => b.MaBnNavigation)
                    .Include(b => b.MaBsNavigation)
                    .Include(b => b.BenhAnToaThuocs)
                        .ThenInclude(t => t.MaThuocNavigation)
                    .AsQueryable();

                // Filter theo tìm kiếm
                if (!string.IsNullOrEmpty(SearchString))
                {
                    query = query.Where(b => b.MaBnNavigation.HoTenBn.Contains(SearchString));
                }

                TotalItems = await query.CountAsync();
                TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

                if (CurrentPage > TotalPages && TotalPages > 0)
                {
                    CurrentPage = TotalPages;
                }

                BenhAns = await query
                    .OrderByDescending(b => b.NgayKham)
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ LỖI OnGetAsync DanhSachBenhAn: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}. Có thể bảng BenhAn chưa được tạo trong database.";
                BenhAns = new List<BenhAn>();
            }
        }
    }
}

