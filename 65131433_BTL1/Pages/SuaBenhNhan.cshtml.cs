using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;

namespace _65131433_BTL1.Pages
{
    public class SuaBenhNhanModel : PageModel
    {
        private readonly PhongKhamDbContext _context;

        public SuaBenhNhanModel(PhongKhamDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BenhNhan BenhNhan { get; set; } = new BenhNhan();

        public async Task<IActionResult> OnGetAsync(string maBn)
        {
            if (string.IsNullOrEmpty(maBn))
            {
                return RedirectToPage("/DanhSachBenhNhan");
            }

            BenhNhan = await _context.BenhNhans.FindAsync(maBn);

            if (BenhNhan == null)
            {
                return RedirectToPage("/DanhSachBenhNhan");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _context.Attach(BenhNhan).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"C?p nh?t thông tin b?nh nhân {BenhNhan.HoTenBn} thành công!";
                return RedirectToPage("/DanhSachBenhNhan");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có l?i x?y ra: " + ex.Message;
                return Page();
            }
        }
    }
}