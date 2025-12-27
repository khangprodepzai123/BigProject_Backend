using _65131433_BTL1.Models;
using _65131433_BTL1.Models.Api;
using _65131433_BTL1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _65131433_BTL1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenhNhanController : ControllerBase
    {
        private readonly PhongKhamDbContext _context;
        private readonly IJwtService _jwtService;

        public BenhNhanController(PhongKhamDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Đăng ký khám bệnh từ Android
        /// POST /api/benhnhan/register
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterKham([FromBody] RegisterKhamRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrWhiteSpace(request.HoTenBn) || string.IsNullOrWhiteSpace(request.Sdt))
                {
                    return BadRequest(new RegisterKhamResponse
                    {
                        Success = false,
                        Message = "Họ tên và số điện thoại không được để trống"
                    });
                }

                // Sinh mã bệnh nhân
                var lastBenhNhan = await _context.BenhNhans
                    .OrderByDescending(b => b.MaBn)
                    .FirstOrDefaultAsync();

                string maBn = "BN001";
                if (lastBenhNhan != null)
                {
                    string numberPart = lastBenhNhan.MaBn.Substring(2);
                    int nextNumber = int.Parse(numberPart) + 1;
                    maBn = "BN" + nextNumber.ToString("D3");
                }

                // Parse ngày sinh
                DateOnly? ngaySinh = null;
                if (!string.IsNullOrWhiteSpace(request.NgaySinh))
                {
                    if (DateOnly.TryParse(request.NgaySinh, out DateOnly parsedDate))
                    {
                        ngaySinh = parsedDate;
                    }
                }

                // Tạo bệnh nhân mới
                var benhNhan = new BenhNhan
                {
                    MaBn = maBn,
                    HoTenBn = request.HoTenBn,
                    Sdt = request.Sdt,
                    NgaySinh = ngaySinh,
                    Gt = string.IsNullOrWhiteSpace(request.Gt) ? null : request.Gt,
                    DoiTuong = string.IsNullOrWhiteSpace(request.DoiTuong) ? null : request.DoiTuong,
                    DiaChi = string.IsNullOrWhiteSpace(request.DiaChi) ? null : request.DiaChi,
                    Bhyt = string.IsNullOrWhiteSpace(request.Bhyt) ? null : request.Bhyt
                };

                _context.BenhNhans.Add(benhNhan);

                // Lấy bác sĩ mặc định
                var bacSiMacDinh = await _context.BacSis.FirstOrDefaultAsync();
                if (bacSiMacDinh == null)
                {
                    return BadRequest(new RegisterKhamResponse
                    {
                        Success = false,
                        Message = "Chưa có bác sĩ trong hệ thống!"
                    });
                }

                // Lấy chuẩn đoán mặc định
                var chuanDoanMacDinh = await _context.ChuanDoans.FirstOrDefaultAsync();
                if (chuanDoanMacDinh == null)
                {
                    return BadRequest(new RegisterKhamResponse
                    {
                        Success = false,
                        Message = "Chưa có chuẩn đoán trong hệ thống!"
                    });
                }

                // Sinh mã khám
                var lastKhamBenh = await _context.KhamBenhs
                    .OrderByDescending(k => k.MaKham)
                    .FirstOrDefaultAsync();

                string maKham = lastKhamBenh == null
                    ? "KB001"
                    : "KB" + (int.Parse(lastKhamBenh.MaKham.Substring(2)) + 1).ToString("D3");

                // Parse thời gian hẹn khám
                DateOnly ngayKham = DateOnly.FromDateTime(DateTime.Now);
                DateTime? thoiGianHenKham = null;
                if (!string.IsNullOrWhiteSpace(request.ThoiGianHenKham))
                {
                    if (DateTime.TryParse(request.ThoiGianHenKham, out DateTime parsedDateTime))
                    {
                        thoiGianHenKham = parsedDateTime;
                        ngayKham = DateOnly.FromDateTime(parsedDateTime);
                    }
                }

                // Tạo phiếu khám
                var khamBenh = new KhamBenh
                {
                    MaKham = maKham,
                    MaBn = maBn,
                    MaBs = bacSiMacDinh.MaBs,
                    MaCd = chuanDoanMacDinh.MaCd,
                    NgayKham = ngayKham,
                    LyDoKham = string.IsNullOrWhiteSpace(request.LyDoKham) ? null : request.LyDoKham,
                    LoaiKham = "Lâm sàng",
                    XuTriKham = "Kết thúc điều trị",
                    TrangThai = "Đang khám"
                };

                _context.KhamBenhs.Add(khamBenh);

                // Tự động liên kết tài khoản với bệnh nhân nếu có token
                var authHeader = Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    try
                    {
                        var token = authHeader.Substring("Bearer ".Length).Trim();
                        var principal = _jwtService.ValidateToken(token);
                        if (principal != null)
                        {
                            var maTkClaim = principal.FindFirst("maTk");
                            if (maTkClaim != null)
                            {
                                var taiKhoan = await _context.TaiKhoanBenhNhans
                                    .FirstOrDefaultAsync(t => t.MaTk == maTkClaim.Value);

                                if (taiKhoan != null)
                                {
                                    // Cập nhật MaBn (HoTenBn sẽ lấy từ MaBnNavigation khi cần)
                                    taiKhoan.MaBn = maBn;
                                    Console.WriteLine($"✓ Đã liên kết tài khoản {taiKhoan.MaTk} với bệnh nhân {maBn} - {benhNhan.HoTenBn}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi nhưng không fail request
                        Console.WriteLine($"Lỗi khi liên kết tài khoản: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new RegisterKhamResponse
                {
                    Success = true,
                    Message = "Đăng ký khám bệnh thành công",
                    Data = new RegisterKhamData
                    {
                        MaBn = maBn,
                        MaKham = maKham,
                        HoTenBn = benhNhan.HoTenBn,
                        ThoiGianHenKham = thoiGianHenKham
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RegisterKhamResponse
                {
                    Success = false,
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }
    }
}

