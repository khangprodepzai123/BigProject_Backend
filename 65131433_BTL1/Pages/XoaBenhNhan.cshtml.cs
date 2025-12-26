using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;

namespace _65131433_BTL1.Pages
{
    public class XoaBenhNhanModel : PageModel
    {
        private readonly PhongKhamDbContext _context;

        public XoaBenhNhanModel(PhongKhamDbContext context)
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
                var benhNhan = await _context.BenhNhans.FindAsync(BenhNhan.MaBn);

                if (benhNhan == null)
                {
                    TempData["ErrorMessage"] = "Không tìm th?y b?nh nhân!";
                    return RedirectToPage("/DanhSachBenhNhan");
                }

                string hoTen = benhNhan.HoTenBn;

                _context.BenhNhans.Remove(benhNhan);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"?ã xóa b?nh nhân {hoTen} thành công!";
                return RedirectToPage("/DanhSachBenhNhan");
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không th? xóa b?nh nhân này vì còn d? li?u liên quan (khám b?nh, hóa ??n...)!";
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có l?i x?y ra: " + ex.Message;
                return Page();
            }
        }
    }
}