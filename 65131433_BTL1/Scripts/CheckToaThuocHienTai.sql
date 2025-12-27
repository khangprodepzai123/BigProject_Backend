-- Script kiểm tra dữ liệu để test API Toa thuốc hiện tại
-- Chạy script này để kiểm tra xem có dữ liệu để test không

USE PhongKhamOnline;
GO

PRINT N'========================================';
PRINT N'KIỂM TRA DỮ LIỆU TOA THUỐC HIỆN TẠI';
PRINT N'========================================';
PRINT N'';

-- 1. Kiểm tra tài khoản BN002
PRINT N'1. KIỂM TRA TÀI KHOẢN BN002:';
PRINT N'----------------------------------------';
SELECT 
    tk.MaTk AS N'Mã TK',
    tk.TenDangNhap AS N'Tên đăng nhập',
    tk.MaBn AS N'Mã BN',
    tk.HoTenBn AS N'Họ tên',
    CASE 
        WHEN tk.MaBn IS NOT NULL THEN N'Đã liên kết'
        ELSE N'Chưa liên kết'
    END AS N'Trạng thái'
FROM TaiKhoanBenhNhan tk
WHERE tk.MaBn = 'BN002' OR tk.TenDangNhap = 'bn002';

PRINT N'';

-- 2. Kiểm tra bệnh án của BN002
PRINT N'2. KIỂM TRA BỆNH ÁN CỦA BN002:';
PRINT N'----------------------------------------';
SELECT 
    ba.MaBenhAn,
    ba.MaKham,
    ba.MaBn,
    ba.NgayKham,
    ba.NgayLuu,
    ba.ChuanDoan,
    (SELECT COUNT(*) FROM BenhAnToaThuoc WHERE MaBenhAn = ba.MaBenhAn) AS N'Số thuốc'
FROM BenhAn ba
WHERE ba.MaBn = 'BN002'
ORDER BY ba.NgayKham DESC, ba.NgayLuu DESC;

DECLARE @SoBenhAn INT = (SELECT COUNT(*) FROM BenhAn WHERE MaBn = 'BN002');
IF @SoBenhAn = 0
BEGIN
    PRINT N'⚠ BN002 chưa có bệnh án nào!';
    PRINT N'   Cần lưu bệnh án từ lần khám đã thanh toán.';
END
ELSE
BEGIN
    PRINT N'✓ BN002 có ' + CAST(@SoBenhAn AS NVARCHAR(10)) + N' bệnh án';
END

PRINT N'';

-- 3. Kiểm tra toa thuốc trong bệnh án gần nhất
PRINT N'3. KIỂM TRA TOA THUỐC TRONG BỆNH ÁN GẦN NHẤT:';
PRINT N'----------------------------------------';
DECLARE @MaBenhAnGanNhat VARCHAR(10);
SELECT TOP 1 @MaBenhAnGanNhat = MaBenhAn
FROM BenhAn
WHERE MaBn = 'BN002'
ORDER BY NgayKham DESC, NgayLuu DESC;

IF @MaBenhAnGanNhat IS NOT NULL
BEGIN
    SELECT 
        batt.MaBenhAn,
        batt.MaThuoc,
        t.TenThuoc,
        batt.SoLuong,
        batt.LieuDung,
        batt.CachDung
    FROM BenhAnToaThuoc batt
    INNER JOIN Thuoc t ON batt.MaThuoc = t.MaThuoc
    WHERE batt.MaBenhAn = @MaBenhAnGanNhat;
    
    DECLARE @SoThuoc INT = (SELECT COUNT(*) FROM BenhAnToaThuoc WHERE MaBenhAn = @MaBenhAnGanNhat);
    IF @SoThuoc = 0
    BEGIN
        PRINT N'⚠ Bệnh án ' + @MaBenhAnGanNhat + N' chưa có thuốc nào!';
    END
    ELSE
    BEGIN
        PRINT N'✓ Bệnh án ' + @MaBenhAnGanNhat + N' có ' + CAST(@SoThuoc AS NVARCHAR(10)) + N' loại thuốc';
    END
END
ELSE
BEGIN
    PRINT N'⚠ Không tìm thấy bệnh án gần nhất của BN002!';
END

PRINT N'';

-- 4. Kiểm tra phiếu khám đã thanh toán của BN002
PRINT N'4. KIỂM TRA PHIẾU KHÁM ĐÃ THANH TOÁN CỦA BN002:';
PRINT N'----------------------------------------';
SELECT 
    kb.MaKham,
    kb.MaBn,
    kb.NgayKham,
    kb.TrangThai,
    CASE 
        WHEN EXISTS (SELECT 1 FROM BenhAn WHERE MaKham = kb.MaKham) THEN N'Đã lưu bệnh án'
        ELSE N'Chưa lưu bệnh án'
    END AS N'Trạng thái bệnh án'
FROM KhamBenh kb
WHERE kb.MaBn = 'BN002' AND kb.TrangThai = 'Đã thanh toán'
ORDER BY kb.NgayKham DESC;

DECLARE @SoKhamDaThanhToan INT = (
    SELECT COUNT(*) 
    FROM KhamBenh 
    WHERE MaBn = 'BN002' AND TrangThai = 'Đã thanh toán'
);
IF @SoKhamDaThanhToan = 0
BEGIN
    PRINT N'⚠ BN002 chưa có phiếu khám nào đã thanh toán!';
    PRINT N'   Cần thanh toán một phiếu khám trước.';
END
ELSE
BEGIN
    DECLARE @SoKhamDaCoBenhAn INT = (
        SELECT COUNT(*) 
        FROM KhamBenh kb
        INNER JOIN BenhAn ba ON kb.MaKham = ba.MaKham
        WHERE kb.MaBn = 'BN002' AND kb.TrangThai = 'Đã thanh toán'
    );
    IF @SoKhamDaCoBenhAn = 0
    BEGIN
        PRINT N'⚠ BN002 có ' + CAST(@SoKhamDaThanhToan AS NVARCHAR(10)) + N' phiếu khám đã thanh toán nhưng chưa lưu bệnh án!';
        PRINT N'   Cần lưu bệnh án từ danh sách bệnh nhân trên web.';
    END
    ELSE
    BEGIN
        PRINT N'✓ BN002 có ' + CAST(@SoKhamDaCoBenhAn AS NVARCHAR(10)) + N' bệnh án đã được lưu';
    END
END

PRINT N'';
PRINT N'========================================';
PRINT N'KẾT LUẬN:';
PRINT N'========================================';
PRINT N'Để API trả về toa thuốc, cần đảm bảo:';
PRINT N'1. Tài khoản đã liên kết với BN002 (có MaBn)';
PRINT N'2. BN002 có ít nhất 1 phiếu khám đã thanh toán';
PRINT N'3. Phiếu khám đó đã được lưu vào bệnh án (bảng BenhAn)';
PRINT N'4. Bệnh án đó có ít nhất 1 loại thuốc (bảng BenhAnToaThuoc)';
PRINT N'';

