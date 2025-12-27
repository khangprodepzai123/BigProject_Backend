using _65131433_BTL1.Models;
using _65131433_BTL1.Models.Api;
using _65131433_BTL1.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _65131433_BTL1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly PhongKhamDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(PhongKhamDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Đăng ký tài khoản
        /// POST /api/auth/signup
        /// </summary>
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrWhiteSpace(request.TenDangNhap) ||
                    string.IsNullOrWhiteSpace(request.MatKhau))
                {
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Message = "Tên đăng nhập và mật khẩu không được để trống"
                    });
                }

                // Kiểm tra tên đăng nhập đã tồn tại
                var existingAccount = await _context.TaiKhoanBenhNhans
                    .FirstOrDefaultAsync(t => t.TenDangNhap == request.TenDangNhap);

                if (existingAccount != null)
                {
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Message = "Tên đăng nhập đã tồn tại"
                    });
                }

                // Sinh mã TaiKhoan
                var lastTaiKhoan = await _context.TaiKhoanBenhNhans
                    .OrderByDescending(t => t.MaTk)
                    .FirstOrDefaultAsync();

                string maTk = "TK001";
                if (lastTaiKhoan != null)
                {
                    string numberPart = lastTaiKhoan.MaTk.Substring(2);
                    int nextNumber = int.Parse(numberPart) + 1;
                    maTk = "TK" + nextNumber.ToString("D3");
                }

                // Tạo tài khoản mới
                var newAccount = new TaiKhoanBenhNhan
                {
                    MaTk = maTk,
                    TenDangNhap = request.TenDangNhap,
                    MatKhau = request.MatKhau, // Không hash vì yêu cầu không hash
                    DiemTichLuy = 0,
                    MaBn = null // Độc lập, không liên kết BN
                };

                _context.TaiKhoanBenhNhans.Add(newAccount);
                await _context.SaveChangesAsync();

                // Sinh JWT Token
                var token = _jwtService.GenerateToken(maTk, request.TenDangNhap);

                return Ok(new AuthResponse
                {
                    Success = true,
                    Message = "Đăng ký thành công",
                    Data = new AuthResponseData
                    {
                        MaTk = maTk,
                        TenDangNhap = request.TenDangNhap,
                        MaBn = null,
                        HoTen = null,
                        DiemTichLuy = 0,
                        Token = token
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Đăng nhập
        /// POST /api/auth/login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Models.Api.LoginRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrWhiteSpace(request.TenDangNhap) ||
                    string.IsNullOrWhiteSpace(request.MatKhau))
                {
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Message = "Tên đăng nhập và mật khẩu không được để trống"
                    });
                }

                // Tìm tài khoản
                var account = await _context.TaiKhoanBenhNhans
                    .Include(t => t.MaBnNavigation)
                    .FirstOrDefaultAsync(t => t.TenDangNhap == request.TenDangNhap);

                // Kiểm tra mật khẩu
                if (account == null || account.MatKhau != request.MatKhau)
                {
                    return Unauthorized(new AuthResponse
                    {
                        Success = false,
                        Message = "Tên đăng nhập hoặc mật khẩu sai"
                    });
                }

                // Sinh JWT Token
                var token = _jwtService.GenerateToken(account.MaTk, account.TenDangNhap);

                // Lấy thông tin BN nếu có
                string? hoTen = null;
                if (!string.IsNullOrEmpty(account.MaBn))
                {
                    hoTen = account.MaBnNavigation?.HoTenBn;
                }

                return Ok(new AuthResponse
                {
                    Success = true,
                    Message = "Đăng nhập thành công",
                    Data = new AuthResponseData
                    {
                        MaTk = account.MaTk,
                        TenDangNhap = account.TenDangNhap,
                        MaBn = account.MaBn,
                        HoTen = hoTen,
                        DiemTichLuy = account.DiemTichLuy,
                        Token = token
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Lấy thông tin user hiện tại (cần JWT Token)
        /// GET /api/auth/me
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                // Lấy token từ header
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new UserInfoResponse
                    {
                        Success = false,
                        Message = "Token không hợp lệ"
                    });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Validate token
                var principal = _jwtService.ValidateToken(token);
                if (principal == null)
                {
                    return Unauthorized(new UserInfoResponse
                    {
                        Success = false,
                        Message = "Token không hợp lệ hoặc hết hạn"
                    });
                }

                // Lấy maTk từ token
                var maTkClaim = principal.FindFirst("maTk");
                if (maTkClaim == null)
                {
                    return Unauthorized(new UserInfoResponse
                    {
                        Success = false,
                        Message = "Token không chứa maTk"
                    });
                }

                // Lấy thông tin tài khoản
                var account = await _context.TaiKhoanBenhNhans
                    .Include(t => t.MaBnNavigation)
                    .FirstOrDefaultAsync(t => t.MaTk == maTkClaim.Value);

                if (account == null)
                {
                    return NotFound(new UserInfoResponse
                    {
                        Success = false,
                        Message = "Tài khoản không tồn tại"
                    });
                }

                // Lấy thông tin BN nếu có
                string? hoTen = null;
                if (!string.IsNullOrEmpty(account.MaBn))
                {
                    hoTen = account.MaBnNavigation?.HoTenBn;
                }

                return Ok(new UserInfoResponse
                {
                    Success = true,
                    Message = "Lấy thông tin thành công",
                    Data = new UserInfoData
                    {
                        MaTk = account.MaTk,
                        TenDangNhap = account.TenDangNhap,
                        MaBn = account.MaBn,
                        HoTen = hoTen,
                        DiemTichLuy = account.DiemTichLuy
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UserInfoResponse
                {
                    Success = false,
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Đăng xuất (Server chỉ xác nhận, Android tự xóa token)
        /// POST /api/auth/logout
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
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

                // Validate token
                var principal = _jwtService.ValidateToken(token);
                if (principal == null)
                {
                    return Unauthorized(new { success = false, message = "Token không hợp lệ hoặc hết hạn" });
                }

                return Ok(new { success = true, message = "Đăng xuất thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi server: {ex.Message}" });
            }
        }
    }
}