using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;

namespace _65131433_BTL1.Pages
{
    public class ThemBenhNhanModel : PageModel
    {
        private readonly PhongKhamDbContext _context;

        public ThemBenhNhanModel(PhongKhamDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BenhNhan BenhNhan { get; set; } = new BenhNhan();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // T? ??ng sinh mã b?nh nhân
                BenhNhan.MaBn = await GenerateMaBenhNhan();

                _context.BenhNhans.Add(BenhNhan);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Thêm b?nh nhân {BenhNhan.HoTenBn} thành công! Mã BN: {BenhNhan.MaBn}";
                return RedirectToPage("/DanhSachBenhNhan");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có l?i x?y ra: " + ex.Message;
                return Page();
            }
        }

        private async Task<string> GenerateMaBenhNhan()
        {
            // L?y mã b?nh nhân l?n nh?t hi?n t?i
            var lastBenhNhan = await _context.BenhNhans
                .OrderByDescending(b => b.MaBn)
                .FirstOrDefaultAsync();

            if (lastBenhNhan == null)
            {
                return "BN001"; // Mã ??u tiên
            }

            // L?y ph?n s? t? mã (BN001 -> 001)
            string lastMa = lastBenhNhan.MaBn;
            string numberPart = lastMa.Substring(2); // B? "BN"
            int nextNumber = int.Parse(numberPart) + 1;

            // T?o mã m?i v?i format BN + 3 ch? s?
            return "BN" + nextNumber.ToString("D3");
        }
    }
}