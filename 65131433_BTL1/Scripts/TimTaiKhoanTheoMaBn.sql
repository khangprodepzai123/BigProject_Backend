-- Script tìm tài khoản và mật khẩu của bệnh nhân theo MaBn
-- Thay đổi 'BN001' thành mã bệnh nhân bạn muốn tìm

USE PhongKhamOnline;
GO

DECLARE @MaBn VARCHAR(10) = 'BN001'; -- Thay đổi mã bệnh nhân ở đây

PRINT N'========================================';
PRINT N'TÌM TÀI KHOẢN THEO MÃ BỆNH NHÂN: ' + @MaBn;
PRINT N'========================================';
PRINT N'';

-- Tìm tài khoản liên kết với bệnh nhân
SELECT 
    tk.MaTk AS N'Mã tài khoản',
    tk.TenDangNhap AS N'Tên đăng nhập',
    tk.MatKhau AS N'Mật khẩu',
    tk.MaBn AS N'Mã bệnh nhân',
    tk.HoTenBn AS N'Họ tên bệnh nhân (từ TK)',
    bn.HoTenBn AS N'Họ tên bệnh nhân (từ BN)',
    tk.DiemTichLuy AS N'Điểm tích lũy',
    CASE 
        WHEN tk.MaBn IS NOT NULL THEN N'Đã liên kết'
        ELSE N'Chưa liên kết'
    END AS N'Trạng thái'
FROM TaiKhoanBenhNhan tk
LEFT JOIN BenhNhan bn ON tk.MaBn = bn.MaBn
WHERE tk.MaBn = @MaBn;

-- Nếu không tìm thấy, hiển thị thông tin bệnh nhân
IF NOT EXISTS (SELECT 1 FROM TaiKhoanBenhNhan WHERE MaBn = @MaBn)
BEGIN
    PRINT N'⚠ Không tìm thấy tài khoản nào liên kết với mã bệnh nhân: ' + @MaBn;
    PRINT N'';
    PRINT N'Thông tin bệnh nhân:';
    
    SELECT 
        MaBn AS N'Mã bệnh nhân',
        HoTenBn AS N'Họ tên',
        SDT AS N'Số điện thoại',
        NgaySinh AS N'Ngày sinh',
        GT AS N'Giới tính',
        DoiTuong AS N'Đối tượng',
        DiaChi AS N'Địa chỉ',
        BHYT AS N'BHYT'
    FROM BenhNhan
    WHERE MaBn = @MaBn;
    
    PRINT N'';
    PRINT N'💡 Gợi ý: Bệnh nhân này chưa có tài khoản liên kết.';
    PRINT N'   Bạn có thể tìm tất cả tài khoản chưa liên kết bằng cách chạy:';
    PRINT N'   SELECT * FROM TaiKhoanBenhNhan WHERE MaBn IS NULL;';
END
ELSE
BEGIN
    DECLARE @Count INT = (SELECT COUNT(*) FROM TaiKhoanBenhNhan WHERE MaBn = @MaBn);
    PRINT N'';
    PRINT N'✓ Tìm thấy ' + CAST(@Count AS NVARCHAR(10)) + N' tài khoản liên kết với mã bệnh nhân: ' + @MaBn;
END

PRINT N'';
PRINT N'========================================';
PRINT N'HOÀN TẤT!';
PRINT N'========================================';

