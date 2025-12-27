-- Script kiểm tra dữ liệu bệnh án
-- Chạy script này để kiểm tra xem có dữ liệu bệnh án không

USE PhongKhamOnline;
GO

-- 1. Kiểm tra bảng BenhAn có tồn tại không
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BenhAn]') AND type in (N'U'))
BEGIN
    PRINT '✓ Bảng BenhAn đã tồn tại';
    
    -- 2. Đếm số lượng bệnh án
    DECLARE @Count INT;
    SELECT @Count = COUNT(*) FROM BenhAn;
    PRINT 'Số lượng bệnh án: ' + CAST(@Count AS VARCHAR(10));
    
    -- 3. Hiển thị danh sách bệnh án (nếu có)
    IF @Count > 0
    BEGIN
        SELECT TOP 10 
            MaBenhAn,
            MaKham,
            MaBn,
            NgayKham,
            NgayLuu,
            ChuanDoan
        FROM BenhAn
        ORDER BY NgayLuu DESC;
    END
    ELSE
    BEGIN
        PRINT '⚠ Chưa có dữ liệu bệnh án. Cần lưu bệnh án từ danh sách bệnh nhân.';
    END
END
ELSE
BEGIN
    PRINT '❌ Bảng BenhAn chưa tồn tại. Cần chạy script CreateBenhAnTables.sql trước.';
END
GO

-- 4. Kiểm tra các bệnh nhân đã thanh toán (có thể lưu bệnh án)
SELECT 
    b.MaBn,
    b.HoTenBn,
    k.MaKham,
    k.TrangThai,
    k.NgayKham,
    CASE 
        WHEN ba.MaBenhAn IS NOT NULL THEN 'Đã lưu bệnh án'
        ELSE 'Chưa lưu bệnh án'
    END AS TrangThaiBenhAn
FROM BenhNhan b
INNER JOIN KhamBenh k ON b.MaBn = k.MaBn
LEFT JOIN BenhAn ba ON k.MaKham = ba.MaKham
WHERE k.TrangThai = 'Đã thanh toán'
ORDER BY k.NgayKham DESC;

