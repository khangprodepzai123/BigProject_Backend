using _65131433_BTL1.Models;
using _65131433_BTL1.Models.Api;
using _65131433_BTL1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace _65131433_BTL1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenhAnController : ControllerBase
    {
        private readonly PhongKhamDbContext _context;
        private readonly IJwtService _jwtService;

        public BenhAnController(PhongKhamDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Lấy danh sách bệnh án của bệnh nhân đang đăng nhập
        /// GET /api/benhan/me
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyBenhAn()
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

                // Lấy danh sách bệnh án
                var benhAns = await _context.BenhAns
                    .Include(b => b.MaBsNavigation)
                    .Include(b => b.BenhAnToaThuocs)
                        .ThenInclude(t => t.MaThuocNavigation)
                    .Where(b => b.MaBn == account.MaBn)
                    .OrderByDescending(b => b.NgayKham)
                    .ToListAsync();

                var result = benhAns.Select(b => new
                {
                    maBenhAn = b.MaBenhAn,
                    maKham = b.MaKham,
                    ngayKham = b.NgayKham?.ToString("yyyy-MM-dd"),
                    ngayLuu = b.NgayLuu?.ToString("yyyy-MM-dd HH:mm"),
                    bacSi = b.MaBsNavigation?.HoTenBs,
                    lyDoKham = b.LyDoKham,
                    quaTrinhBenhLy = b.QuaTrinhBenhLy,
                    tienSuBenhNhan = b.TienSuBenhNhan,
                    tienSuGiaDinh = b.TienSuGiaDinh,
                    khamBoPhan = b.KhamBoPhan,
                    chuanDoan = b.ChuanDoan,
                    huongXuTri = b.HuongXuTri,
                    loaiKham = b.LoaiKham,
                    xuTriKham = b.XuTriKham,
                    toaThuoc = b.BenhAnToaThuocs.Select(t => new
                    {
                        maThuoc = t.MaThuoc,
                        tenThuoc = t.MaThuocNavigation?.TenThuoc,
                        soLuong = t.SoLuong,
                        lieuDung = t.LieuDung,
                        cachDung = t.CachDung
                    }).ToList()
                }).ToList();

                return Ok(new { success = true, message = "Lấy danh sách bệnh án thành công", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi server: {ex.Message}" });
            }
        }

        /// <summary>
        /// Lấy toa thuốc hiện tại (lần khám đã thanh toán gần nhất)
        /// GET /api/benhan/toathuoc-hientai
        /// </summary>
        [HttpGet("toathuoc-hientai")]
        public async Task<IActionResult> GetToaThuocHienTai()
        {
            try
            {
                // Lấy token từ header
                var authHeader = Request.Headers["Authorization"].ToString();
                Console.WriteLine($"🔍 DEBUG GetToaThuocHienTai - Authorization Header: '{authHeader}'");
                
                if (string.IsNullOrEmpty(authHeader))
                {
                    Console.WriteLine("❌ DEBUG - Authorization header rỗng!");
                    return Unauthorized(new { success = false, message = "Token không hợp lệ - Header Authorization rỗng. Vui lòng thêm header: Authorization: Bearer YOUR_TOKEN" });
                }
                
                if (!authHeader.StartsWith("Bearer "))
                {
                    Console.WriteLine($"❌ DEBUG - Authorization header không bắt đầu bằng 'Bearer '. Giá trị: '{authHeader}'");
                    return Unauthorized(new { success = false, message = "Token không hợp lệ - Phải bắt đầu bằng 'Bearer ' (có khoảng trắng). Ví dụ: Bearer eyJhbGci..." });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                Console.WriteLine($"🔍 DEBUG - Token extracted (first 30 chars): '{token.Substring(0, Math.Min(30, token.Length))}...'");
                
                var principal = _jwtService.ValidateToken(token);
                if (principal == null)
                {
                    Console.WriteLine("❌ DEBUG - Token validation failed!");
                    return Unauthorized(new { success = false, message = "Token không hợp lệ hoặc hết hạn. Vui lòng đăng nhập lại để lấy token mới." });
                }
                
                Console.WriteLine("✓ DEBUG - Token validated successfully");

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
                    return Ok(new { success = true, message = "Tài khoản chưa liên kết với bệnh nhân", data = (object?)null });
                }

                // Lấy bệnh án gần nhất (đã thanh toán)
                var benhAnGanNhat = await _context.BenhAns
                    .Include(b => b.MaBsNavigation)
                    .Include(b => b.BenhAnToaThuocs)
                        .ThenInclude(t => t.MaThuocNavigation)
                    .Where(b => b.MaBn == account.MaBn)
                    .OrderByDescending(b => b.NgayKham)
                    .ThenByDescending(b => b.NgayLuu)
                    .FirstOrDefaultAsync();

                if (benhAnGanNhat == null)
                {
                    return Ok(new { success = true, message = "Chưa có toa thuốc nào", data = (object?)null });
                }

                // Parse LieuDung để tính số lần uống mỗi ngày
                var toaThuocList = benhAnGanNhat.BenhAnToaThuocs.Select(t => 
                {
                    int soLanUongMoiNgay = ParseSoLanUongMoiNgay(t.LieuDung ?? "");
                    
                    return new
                    {
                        maThuoc = t.MaThuoc,
                        tenThuoc = t.MaThuocNavigation?.TenThuoc ?? "",
                        soLuong = t.SoLuong,
                        lieuDung = t.LieuDung ?? "",
                        cachDung = t.CachDung ?? "",
                        soLanUongMoiNgay = soLanUongMoiNgay
                    };
                }).ToList();

                var result = new
                {
                    maBenhAn = benhAnGanNhat.MaBenhAn,
                    maKham = benhAnGanNhat.MaKham,
                    ngayKham = benhAnGanNhat.NgayKham?.ToString("yyyy-MM-dd"),
                    ngayLuu = benhAnGanNhat.NgayLuu?.ToString("yyyy-MM-dd HH:mm"),
                    bacSi = benhAnGanNhat.MaBsNavigation?.HoTenBs ?? "",
                    chuanDoan = benhAnGanNhat.ChuanDoan ?? "",
                    toaThuoc = toaThuocList
                };

                return Ok(new { success = true, message = "Lấy toa thuốc hiện tại thành công", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi server: {ex.Message}" });
            }
        }

        /// <summary>
        /// Parse LieuDung để tính số lần uống mỗi ngày
        /// Ví dụ: "1 viên/lần, 3 lần/ngày" -> 3
        /// </summary>
        private int ParseSoLanUongMoiNgay(string lieuDung)
        {
            if (string.IsNullOrWhiteSpace(lieuDung))
                return 1; // Mặc định 1 lần/ngày

            lieuDung = lieuDung.ToLower();

            // Tìm pattern "X lần/ngày" hoặc "X lần/ ngày"
            var patterns = new[]
            {
                @"(\d+)\s*lần\s*/?\s*ngày",
                @"(\d+)\s*lần\s*/?\s*ngay",
                @"ngày\s*(\d+)\s*lần",
                @"(\d+)\s*lần"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(lieuDung, pattern);
                if (match.Success && match.Groups.Count > 1)
                {
                    if (int.TryParse(match.Groups[1].Value, out int soLan))
                    {
                        return soLan;
                    }
                }
            }

            // Nếu không tìm thấy, mặc định 1 lần/ngày
            return 1;
        }
    }
}

