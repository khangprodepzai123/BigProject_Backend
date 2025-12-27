-- Script tạo tài khoản cho bệnh nhân BN002
-- Chạy script này để tạo tài khoản và liên kết với bệnh nhân BN002

USE PhongKhamOnline;
GO

-- Kiểm tra bệnh nhân BN002 có tồn tại không
IF NOT EXISTS (SELECT 1 FROM BenhNhan WHERE MaBn = 'BN002')
BEGIN
    PRINT N'⚠ Lỗi: Bệnh nhân BN002 không tồn tại!';
    PRINT N'Vui lòng tạo bệnh nhân BN002 trước.';
    RETURN;
END

-- Kiểm tra xem BN002 đã có tài khoản chưa
IF EXISTS (SELECT 1 FROM TaiKhoanBenhNhan WHERE MaBn = 'BN002')
BEGIN
    PRINT N'⚠ Bệnh nhân BN002 đã có tài khoản liên kết!';
    PRINT N'Thông tin tài khoản hiện tại:';
    
    SELECT 
        tk.MaTk AS N'Mã TK',
        tk.TenDangNhap AS N'Tên đăng nhập',
        tk.MatKhau AS N'Mật khẩu',
        tk.MaBn AS N'Mã BN',
        tk.HoTenBn AS N'Họ tên',
        tk.DiemTichLuy AS N'Điểm tích lũy'
    FROM TaiKhoanBenhNhan tk
    WHERE tk.MaBn = 'BN002';
    
    RETURN;
END

-- Lấy thông tin bệnh nhân BN002
DECLARE @HoTenBn NVARCHAR(100);
SELECT @HoTenBn = HoTenBn FROM BenhNhan WHERE MaBn = 'BN002';

-- Sinh mã tài khoản mới
DECLARE @MaTk VARCHAR(10);
DECLARE @LastMaTk VARCHAR(10);

SELECT TOP 1 @LastMaTk = MaTk 
FROM TaiKhoanBenhNhan 
ORDER BY MaTk DESC;

IF @LastMaTk IS NULL
BEGIN
    SET @MaTk = 'TK001';
END
ELSE
BEGIN
    DECLARE @NumberPart INT;
    SET @NumberPart = CAST(SUBSTRING(@LastMaTk, 3, LEN(@LastMaTk)) AS INT);
    SET @MaTk = 'TK' + RIGHT('000' + CAST(@NumberPart + 1 AS VARCHAR), 3);
END

-- Tạo tài khoản mới
INSERT INTO TaiKhoanBenhNhan (MaTk, TenDangNhap, MatKhau, DiemTichLuy, MaBn, HoTenBn)
VALUES (
    @MaTk,
    'bn002',  -- Tên đăng nhập
    '123456',  -- Mật khẩu (có thể thay đổi)
    0,         -- Điểm tích lũy ban đầu
    'BN002',   -- Mã bệnh nhân
    @HoTenBn   -- Họ tên bệnh nhân
);

PRINT N'';
PRINT N'========================================';
PRINT N'ĐÃ TẠO TÀI KHOẢN THÀNH CÔNG!';
PRINT N'========================================';
PRINT N'Mã tài khoản: ' + @MaTk;
PRINT N'Tên đăng nhập: bn002';
PRINT N'Mật khẩu: 123456';
PRINT N'Mã bệnh nhân: BN002';
PRINT N'Họ tên: ' + @HoTenBn;
PRINT N'';
PRINT N'Bạn có thể sử dụng thông tin này để đăng nhập trên Android!';
PRINT N'';

-- Hiển thị thông tin tài khoản vừa tạo
SELECT 
    tk.MaTk AS N'Mã TK',
    tk.TenDangNhap AS N'Tên đăng nhập',
    tk.MatKhau AS N'Mật khẩu',
    tk.MaBn AS N'Mã BN',
    tk.HoTenBn AS N'Họ tên',
    tk.DiemTichLuy AS N'Điểm tích lũy'
FROM TaiKhoanBenhNhan tk
WHERE tk.MaTk = @MaTk;

GO

