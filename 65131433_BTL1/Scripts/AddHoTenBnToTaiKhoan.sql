-- Script thêm cột HoTenBn vào bảng TaiKhoanBenhNhan
-- Chạy script này để thêm cột mới

USE PhongKhamOnline;
GO

-- Kiểm tra và thêm cột HoTenBn
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TaiKhoanBenhNhan]') AND name = 'HoTenBn')
BEGIN
    ALTER TABLE TaiKhoanBenhNhan
    ADD HoTenBn NVARCHAR(255) NULL;
    
    PRINT '✓ Đã thêm cột HoTenBn vào bảng TaiKhoanBenhNhan';
    
    -- Cập nhật dữ liệu từ bảng BenhNhan cho các tài khoản đã có MaBn
    UPDATE tk
    SET tk.HoTenBn = bn.HoTenBn
    FROM TaiKhoanBenhNhan tk
    INNER JOIN BenhNhan bn ON tk.MaBn = bn.MaBn
    WHERE tk.MaBn IS NOT NULL AND tk.HoTenBn IS NULL;
    
    PRINT '✓ Đã cập nhật HoTenBn từ bảng BenhNhan cho các tài khoản đã liên kết';
END
ELSE
BEGIN
    PRINT 'Cột HoTenBn đã tồn tại.';
END
GO

PRINT 'Hoàn tất!';

