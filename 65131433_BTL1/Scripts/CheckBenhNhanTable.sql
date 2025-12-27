-- Script kiểm tra cấu trúc bảng BenhNhan
-- Chạy script này để xem các cột trong bảng BenhNhan

USE PhongKhamOnline;
GO

-- Kiểm tra cấu trúc bảng BenhNhan
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'BenhNhan'
ORDER BY ORDINAL_POSITION;

GO

-- Kiểm tra xem bảng có tồn tại không
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BenhNhan]') AND type in (N'U'))
BEGIN
    PRINT '✓ Bảng BenhNhan tồn tại';
    
    -- Đếm số bệnh nhân
    DECLARE @Count INT;
    SELECT @Count = COUNT(*) FROM BenhNhan;
    PRINT 'Số lượng bệnh nhân: ' + CAST(@Count AS VARCHAR(10));
    
    -- Hiển thị 5 bệnh nhân đầu tiên (nếu có)
    IF @Count > 0
    BEGIN
        SELECT TOP 5 * FROM BenhNhan;
    END
END
ELSE
BEGIN
    PRINT '❌ Bảng BenhNhan chưa tồn tại';
END
GO

