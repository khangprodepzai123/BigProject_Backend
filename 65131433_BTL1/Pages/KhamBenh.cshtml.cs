using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _65131433_BTL1.Models;
using Microsoft.EntityFrameworkCore;

namespace _65131433_BTL1.Pages
{
    public class KhamBenhModel : PageModel
    {
        private readonly PhongKhamDbContext _context;

        public KhamBenhModel(PhongKhamDbContext context)
        {
            _context = context;
        }

        public BenhNhan? BenhNhan { get; set; }
        public KhamBenh? KhamBenh { get; set; }
        public List<BacSi> DanhSachBacSi { get; set; } = new List<BacSi>();
        public List<ChuanDoan> DanhSachChuanDoan { get; set; } = new List<ChuanDoan>();
        public List<ThuocViewModel> DanhSachThuoc { get; set; } = new List<ThuocViewModel>();
        public List<ToaThuoc> DanhSachThuocDaKe { get; set; } = new List<ToaThuoc>();

        public async Task<IActionResult> OnGetAsync(string maKham)
        {
            if (string.IsNullOrEmpty(maKham))
            {
                return RedirectToPage("/DanhSachBenhNhan");
            }

            // Load phi?u khám
            KhamBenh = await _context.KhamBenhs
                .Include(k => k.MaBnNavigation)
                .FirstOrDefaultAsync(k => k.MaKham == maKham);

            if (KhamBenh == null || KhamBenh.TrangThai == "?ã thanh toán")
            {
                TempData["ErrorMessage"] = "Không tìm th?y phi?u khám ho?c ?ã thanh toán!";
                return RedirectToPage("/DanhSachBenhNhan");
            }

            BenhNhan = KhamBenh.MaBnNavigation;

            // Load danh sách bác s?
            DanhSachBacSi = await _context.BacSis.OrderBy(b => b.HoTenBs).ToListAsync();

            // Load danh sách chu?n ?oán
            DanhSachChuanDoan = await _context.ChuanDoans.OrderBy(c => c.MaCd).ToListAsync();

            // Load danh sách thu?c (ch? thu?c còn trong kho)
            DanhSachThuoc = await _context.Thuocs
                .Where(t => t.SoLuong > 0)
                .OrderBy(t => t.TenThuoc)
                .Select(t => new ThuocViewModel
                {
                    MaThuoc = t.MaThuoc,
                    TenThuoc = t.TenThuoc,
                    GiaBan = t.GiaBan,
                    SoLuong = t.SoLuong
                })
                .ToListAsync();

            // Load toa thu?c ?ã kê (n?u có)
            DanhSachThuocDaKe = await _context.ToaThuocs
                .Where(t => t.MaKham == maKham)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                string maKham = Request.Form["MaKham"].ToString();
                string maBs = Request.Form["MaBs"].ToString();
                string maCd = Request.Form["MaCd"].ToString();
                string ngayKhamStr = Request.Form["NgayKham"].ToString();
                string? lyDoKham = Request.Form["LyDoKham"].ToString();
                string? quaTrinhBenhLy = Request.Form["QuaTrinhBenhLy"].ToString();
                string? tienSuBenhNhan = Request.Form["TienSuBenhNhan"].ToString();
                string? tienSuGiaDinh = Request.Form["TienSuGiaDinh"].ToString();
                string? khamBoPhan = Request.Form["KhamBoPhan"].ToString();
                string loaiKham = Request.Form["LoaiKham"].ToString();
                string xuTriKham = Request.Form["XuTriKham"].ToString();

                // Parse ngày khám
                DateOnly ngayKham = DateOnly.Parse(ngayKhamStr);

                // Tìm phi?u khám
                var khamBenh = await _context.KhamBenhs.FindAsync(maKham);
                if (khamBenh == null)
                {
                    TempData["ErrorMessage"] = "Không tìm th?y phi?u khám!";
                    return RedirectToPage("/DanhSachBenhNhan");
                }

                // C?p nh?t thông tin
                khamBenh.MaBs = maBs;
                khamBenh.MaCd = maCd;
                khamBenh.NgayKham = ngayKham;
                khamBenh.LyDoKham = string.IsNullOrWhiteSpace(lyDoKham) ? null : lyDoKham;
                khamBenh.QuaTrinhBenhLy = string.IsNullOrWhiteSpace(quaTrinhBenhLy) ? null : quaTrinhBenhLy;
                khamBenh.TienSuBenhNhan = string.IsNullOrWhiteSpace(tienSuBenhNhan) ? null : tienSuBenhNhan;
                khamBenh.TienSuGiaDinh = string.IsNullOrWhiteSpace(tienSuGiaDinh) ? null : tienSuGiaDinh;
                khamBenh.KhamBoPhan = string.IsNullOrWhiteSpace(khamBoPhan) ? null : khamBoPhan;
                khamBenh.LoaiKham = loaiKham;
                khamBenh.XuTriKham = xuTriKham;
                khamBenh.TrangThai = "?ã khám"; // Chuy?n sang ?ã khám

                // Xóa toa thu?c c?
                var toaThuocCu = await _context.ToaThuocs.Where(t => t.MaKham == maKham).ToListAsync();
                _context.ToaThuocs.RemoveRange(toaThuocCu);

                // L?u toa thu?c m?i
                var toaThuocKeys = Request.Form.Keys.Where(k => k.StartsWith("ToaThuoc[")).ToList();

                if (toaThuocKeys.Any())
                {
                    var thuocGroups = toaThuocKeys
                        .Select(k => k.Split('[')[1].Split(']')[0])
                        .Distinct();

                    foreach (var index in thuocGroups)
                    {
                        string? maThuoc = Request.Form[$"ToaThuoc[{index}].MaThuoc"].ToString();

                        if (string.IsNullOrEmpty(maThuoc))
                            continue;

                        string soLuongStr = Request.Form[$"ToaThuoc[{index}].SoLuong"].ToString();
                        int soLuong = string.IsNullOrEmpty(soLuongStr) ? 1 : int.Parse(soLuongStr);

                        string? lieuDung = Request.Form[$"ToaThuoc[{index}].LieuDung"].ToString();
                        string? cachDung = Request.Form[$"ToaThuoc[{index}].CachDung"].ToString();

                        var toaThuocItem = new ToaThuoc
                        {
                            MaKham = maKham,
                            MaThuoc = maThuoc,
                            SoLuong = soLuong,
                            LieuDung = string.IsNullOrWhiteSpace(lieuDung) ? null : lieuDung,
                            CachDung = string.IsNullOrWhiteSpace(cachDung) ? null : cachDung
                        };

                        _context.ToaThuocs.Add(toaThuocItem);
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"L?u phi?u khám {maKham} thành công!";
                return RedirectToPage("/DanhSachBenhNhan");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có l?i x?y ra: " + ex.Message;
                string maKham = Request.Form["MaKham"].ToString();
                return await OnGetAsync(maKham);
            }
        }
    }

    // ViewModel cho thu?c
    public class ThuocViewModel
    {
        public string MaThuoc { get; set; } = string.Empty;
        public string TenThuoc { get; set; } = string.Empty;
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
    }
}