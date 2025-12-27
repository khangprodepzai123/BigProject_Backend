-- Script sửa lỗi: Thêm cột HoTenBn vào bảng TaiKhoanBenhNhan
-- Chạy script này để sửa lỗi "Invalid column name 'HoTenBn'"

USE PhongKhamOnline;
GO

-- Bước 1: Kiểm tra và thêm cột HoTenBn nếu chưa có
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[TaiKhoanBenhNhan]') 
    AND name = 'HoTenBn'
)
BEGIN
    PRINT N'Đang thêm cột HoTenBn vào bảng TaiKhoanBenhNhan...';
    
    ALTER TABLE TaiKhoanBenhNhan
    ADD HoTenBn NVARCHAR(255) NULL;
    
    PRINT N'✓ Đã thêm cột HoTenBn thành công!';
END
ELSE
BEGIN
    PRINT N'Cột HoTenBn đã tồn tại trong bảng TaiKhoanBenhNhan.';
END
GO

-- Bước 2: Cập nhật dữ liệu từ bảng BenhNhan (chạy sau khi cột đã được tạo)
IF EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[TaiKhoanBenhNhan]') 
    AND name = 'HoTenBn'
)
BEGIN
    PRINT N'Đang cập nhật dữ liệu HoTenBn từ bảng BenhNhan...';
    
    UPDATE tk
    SET tk.HoTenBn = bn.HoTenBn
    FROM TaiKhoanBenhNhan tk
    INNER JOIN BenhNhan bn ON tk.MaBn = bn.MaBn
    WHERE tk.MaBn IS NOT NULL AND (tk.HoTenBn IS NULL OR tk.HoTenBn = '');
    
    DECLARE @UpdatedCount INT = @@ROWCOUNT;
    PRINT N'✓ Đã cập nhật ' + CAST(@UpdatedCount AS NVARCHAR(10)) + N' tài khoản';
END
ELSE
BEGIN
    PRINT N'⚠ Lỗi: Không thể cập nhật vì cột HoTenBn chưa tồn tại!';
END
GO

PRINT N'';
PRINT N'========================================';
PRINT N'HOÀN TẤT!';
PRINT N'========================================';
PRINT N'Bây giờ bạn có thể refresh trang "Danh sách tài khoản" trên web.';
PRINT N'';

