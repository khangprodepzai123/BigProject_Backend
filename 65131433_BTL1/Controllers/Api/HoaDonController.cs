using _65131433_BTL1.Models;
using _65131433_BTL1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _65131433_BTL1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class HoaDonController : ControllerBase
    {
        private readonly PhongKhamDbContext _context;
        private readonly IJwtService _jwtService;

        public HoaDonController(PhongKhamDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Lấy danh sách hóa đơn đã thanh toán của bệnh nhân đang đăng nhập
        /// GET /api/hoadon/me
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyHoaDon()
        {
            try
            {
                // Lấy token từ header
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { success = false, message = "Token không hợp lệ" });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var principal = _jwtService.ValidateToken(token);
                if (principal == null)
                {
                    return Unauthorized(new { success = false, message = "Token không hợp lệ hoặc hết hạn" });
                }

                // Lấy maTk từ token
                var maTkClaim = principal.FindFirst("maTk");
                if (maTkClaim == null)
                {
                    return Unauthorized(new { success = false, message = "Token không chứa maTk" });
                }

                // Lấy tài khoản và bệnh nhân
                var account = await _context.TaiKhoanBenhNhans
                    .Include(t => t.MaBnNavigation)
                    .FirstOrDefaultAsync(t => t.MaTk == maTkClaim.Value);

                if (account == null || string.IsNullOrEmpty(account.MaBn))
                {
                    return Ok(new { success = true, message = "Tài khoản chưa liên kết với bệnh nhân", data = new List<object>() });
                }

                // Lấy danh sách hóa đơn đã thanh toán
                var hoaDons = await _context.HoaDons
                    .Include(h => h.MaKhamNavigation)
                        .ThenInclude(k => k.MaBsNavigation)
                    .Include(h => h.ChiTietHoaDons)
                        .ThenInclude(c => c.MaThuocNavigation)
                    .Where(h => h.MaKhamNavigation.MaBn == account.MaBn)
                    .OrderByDescending(h => h.NgayLap)
                    .ToListAsync();

                var result = hoaDons.Select(h => new
                {
                    maHd = h.MaHd,
                    maKham = h.MaKham,
                    ngayLap = h.NgayLap?.ToString("yyyy-MM-dd"),
                    thanhTien = h.ThanhTien,
                    diemTichLuySuDung = h.DiemTichLuySuDung,
                    bacSi = h.MaKhamNavigation?.MaBsNavigation?.HoTenBs,
                    chiTiet = h.ChiTietHoaDons.Select(c => new
                    {
                        maThuoc = c.MaThuoc,
                        tenThuoc = c.MaThuocNavigation?.TenThuoc,
                        soLuong = c.SoLuong,
                        donGia = c.DonGia,
                        thanhTien = c.SoLuong * c.DonGia
                    }).ToList()
                }).ToList();

                return Ok(new { success = true, message = "Lấy danh sách hóa đơn thành công", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi server: {ex.Message}" });
            }
        }
    }
}

