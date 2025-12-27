-- Query kiểm tra bệnh án
-- Chạy script này để xem danh sách bệnh án

USE PhongKhamOnline;
GO

-- 1. Danh sách tất cả bệnh án
SELECT 
    ba.MaBenhAn,
    ba.MaKham,
    b.MaBn,
    b.HoTenBn,
    bs.HoTenBs AS TenBacSi,
    ba.NgayKham,
    ba.NgayLuu,
    ba.ChuanDoan,
    ba.LyDoKham
FROM BenhAn ba
INNER JOIN BenhNhan b ON ba.MaBn = b.MaBn
INNER JOIN BacSi bs ON ba.MaBs = bs.MaBs
ORDER BY ba.NgayLuu DESC;

GO

-- 2. Đếm số lượng bệnh án
SELECT 
    COUNT(*) AS TongSoBenhAn,
    COUNT(DISTINCT ba.MaBn) AS SoBenhNhanCoBenhAn
FROM BenhAn ba;

GO

-- 3. Bệnh án kèm toa thuốc
SELECT 
    ba.MaBenhAn,
    b.HoTenBn,
    ba.NgayKham,
    COUNT(batt.MaThuoc) AS SoLoaiThuoc,
    SUM(batt.SoLuong) AS TongSoLuongThuoc
FROM BenhAn ba
INNER JOIN BenhNhan b ON ba.MaBn = b.MaBn
LEFT JOIN BenhAnToaThuoc batt ON ba.MaBenhAn = batt.MaBenhAn
GROUP BY ba.MaBenhAn, b.HoTenBn, ba.NgayKham
ORDER BY ba.NgayKham DESC;

GO

-- 4. Chi tiết bệnh án và toa thuốc
SELECT 
    ba.MaBenhAn,
    b.HoTenBn,
    ba.NgayKham,
    ba.ChuanDoan,
    t.TenThuoc,
    batt.SoLuong,
    batt.LieuDung,
    batt.CachDung
FROM BenhAn ba
INNER JOIN BenhNhan b ON ba.MaBn = b.MaBn
LEFT JOIN BenhAnToaThuoc batt ON ba.MaBenhAn = batt.MaBenhAn
LEFT JOIN Thuoc t ON batt.MaThuoc = t.MaThuoc
ORDER BY ba.MaBenhAn, t.TenThuoc;

