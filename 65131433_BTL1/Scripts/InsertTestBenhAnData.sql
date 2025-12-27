-- Script tạo dữ liệu test cho bệnh án
-- Chạy script này sau khi đã chạy CreateBenhAnTables.sql

USE PhongKhamOnline;
GO

-- Kiểm tra và tạo dữ liệu test
BEGIN TRANSACTION;

-- 1. Kiểm tra có bệnh nhân, bác sĩ, thuốc, chuẩn đoán không
IF NOT EXISTS (SELECT 1 FROM BenhNhan)
BEGIN
    PRINT '⚠ Chưa có bệnh nhân. Đang tạo bệnh nhân test...';
    
    -- Tạo bệnh nhân test
    IF NOT EXISTS (SELECT 1 FROM BenhNhan WHERE MaBn = 'BN001')
    BEGIN
        INSERT INTO BenhNhan (MaBn, HoTenBn, SDT, NgaySinh, GT, DoiTuong, DiaChi, BHYT)
        VALUES ('BN001', N'Nguyễn Văn A', '0123456789', '1990-01-15', N'Nam', N'Bảo hiểm y tế', N'123 Đường ABC', 'BH001');
    END
    
    IF NOT EXISTS (SELECT 1 FROM BenhNhan WHERE MaBn = 'BN002')
    BEGIN
        INSERT INTO BenhNhan (MaBn, HoTenBn, SDT, NgaySinh, GT, DoiTuong, DiaChi, BHYT)
        VALUES ('BN002', N'Trần Thị B', '0987654321', '1985-05-20', N'Nữ', N'Miễn giảm', N'456 Đường XYZ', NULL);
    END
    
    PRINT '✓ Đã tạo bệnh nhân test';
END

-- 2. Kiểm tra có bác sĩ không
IF NOT EXISTS (SELECT 1 FROM BacSi)
BEGIN
    PRINT '⚠ Chưa có bác sĩ. Đang tạo bác sĩ test...';
    
    IF NOT EXISTS (SELECT 1 FROM BacSi WHERE MaBs = 'BS001')
    BEGIN
        INSERT INTO BacSi (MaBs, HoTenBs)
        VALUES ('BS001', 'Bác sĩ Nguyễn Văn C');
    END
    
    PRINT '✓ Đã tạo bác sĩ test';
END

-- 3. Kiểm tra có chuẩn đoán không
IF NOT EXISTS (SELECT 1 FROM ChuanDoan)
BEGIN
    PRINT '⚠ Chưa có chuẩn đoán. Đang tạo chuẩn đoán test...';
    
    IF NOT EXISTS (SELECT 1 FROM ChuanDoan WHERE MaCd = 'CD001')
    BEGIN
        INSERT INTO ChuanDoan (MaCd, TenCd, MoTa)
        VALUES ('CD001', 'Cảm cúm thông thường', 'Bệnh nhân bị cảm cúm, sốt nhẹ');
    END
    
    PRINT '✓ Đã tạo chuẩn đoán test';
END

-- 4. Kiểm tra và tạo thuốc test (luôn kiểm tra từng thuốc, không phụ thuộc vào bảng có rỗng hay không)
IF NOT EXISTS (SELECT 1 FROM Thuoc WHERE MaThuoc = 'T001')
BEGIN
    PRINT '⚠ Đang tạo thuốc test T001...';
    INSERT INTO Thuoc (MaThuoc, TenThuoc, GiaBan, SoLuong, Hdsd)
    VALUES ('T001', N'Paracetamol 500mg', 5000, 1000, N'Uống sau ăn, 1 viên/lần');
    PRINT '✓ Đã tạo thuốc T001';
END

IF NOT EXISTS (SELECT 1 FROM Thuoc WHERE MaThuoc = 'T002')
BEGIN
    PRINT '⚠ Đang tạo thuốc test T002...';
    INSERT INTO Thuoc (MaThuoc, TenThuoc, GiaBan, SoLuong, Hdsd)
    VALUES ('T002', N'Amoxicillin 250mg', 8000, 500, N'Uống sau ăn, 1 viên/lần');
    PRINT '✓ Đã tạo thuốc T002';
END

-- 5. Tạo phiếu khám đã thanh toán (để có thể lưu bệnh án)
-- Xóa phiếu khám cũ nếu đã có (do unique constraint trên MaBn)
DECLARE @OldMaKham1 VARCHAR(10);
DECLARE @OldMaKham2 VARCHAR(10);

IF EXISTS (SELECT 1 FROM KhamBenh WHERE MaBn = 'BN001')
BEGIN
    SELECT @OldMaKham1 = MaKham FROM KhamBenh WHERE MaBn = 'BN001';
    
    -- Xóa chi tiết hóa đơn
    DELETE FROM ChiTietHoaDon WHERE MaHd IN (SELECT MaHd FROM HoaDon WHERE MaKham = @OldMaKham1);
    -- Xóa hóa đơn
    DELETE FROM HoaDon WHERE MaKham = @OldMaKham1;
    -- Xóa toa thuốc
    DELETE FROM ToaThuoc WHERE MaKham = @OldMaKham1;
    -- Xóa phiếu khám
    DELETE FROM KhamBenh WHERE MaKham = @OldMaKham1;
    
    PRINT 'Đã xóa phiếu khám cũ cho BN001';
END

IF EXISTS (SELECT 1 FROM KhamBenh WHERE MaBn = 'BN002')
BEGIN
    SELECT @OldMaKham2 = MaKham FROM KhamBenh WHERE MaBn = 'BN002';
    
    -- Xóa chi tiết hóa đơn
    DELETE FROM ChiTietHoaDon WHERE MaHd IN (SELECT MaHd FROM HoaDon WHERE MaKham = @OldMaKham2);
    -- Xóa hóa đơn
    DELETE FROM HoaDon WHERE MaKham = @OldMaKham2;
    -- Xóa toa thuốc
    DELETE FROM ToaThuoc WHERE MaKham = @OldMaKham2;
    -- Xóa phiếu khám
    DELETE FROM KhamBenh WHERE MaKham = @OldMaKham2;
    
    PRINT 'Đã xóa phiếu khám cũ cho BN002';
END

-- Tạo phiếu khám mới
IF NOT EXISTS (SELECT 1 FROM KhamBenh WHERE MaKham = 'KB001')
BEGIN
    PRINT 'Đang tạo phiếu khám test KB001...';
    
    INSERT INTO KhamBenh (MaKham, MaBn, MaBs, MaCd, NgayKham, LyDoKham, QuaTrinhBenhLy, 
                          TienSuBenhNhan, TienSuGiaDinh, KhamBoPhan, ChuanDoan, HuongXuTri, 
                          LoaiKham, XuTriKham, TrangThai)
    VALUES ('KB001', 'BN001', 'BS001', 'CD001', '2024-01-15', 
            N'Sốt, đau đầu, ho', 
            N'Bệnh nhân bị sốt từ 2 ngày trước, kèm đau đầu và ho khan',
            N'Không có tiền sử bệnh lý đặc biệt',
            N'Gia đình không có tiền sử bệnh di truyền',
            N'Khám tổng quát: Họng đỏ, không có dấu hiệu viêm phổi',
            N'Cảm cúm thông thường',
            N'Nghỉ ngơi, uống nhiều nước, dùng thuốc theo toa',
            N'Lâm sàng',
            N'Kết thúc điều trị',
            N'Đã thanh toán');
    
    PRINT '✓ Đã tạo phiếu khám KB001';
END

IF NOT EXISTS (SELECT 1 FROM KhamBenh WHERE MaKham = 'KB002')
BEGIN
    PRINT 'Đang tạo phiếu khám test KB002...';
    
    INSERT INTO KhamBenh (MaKham, MaBn, MaBs, MaCd, NgayKham, LyDoKham, QuaTrinhBenhLy, 
                          TienSuBenhNhan, TienSuGiaDinh, KhamBoPhan, ChuanDoan, HuongXuTri, 
                          LoaiKham, XuTriKham, TrangThai)
    VALUES ('KB002', 'BN002', 'BS001', 'CD001', '2024-01-20', 
            N'Đau bụng, buồn nôn', 
            N'Bệnh nhân đau bụng từ sáng, kèm buồn nôn nhưng không nôn',
            N'Có tiền sử viêm dạ dày',
            N'Mẹ có tiền sử đau dạ dày',
            N'Khám bụng: Đau vùng thượng vị, không có dấu hiệu cấp cứu',
            N'Viêm dạ dày cấp',
            N'Uống thuốc giảm đau, kiêng đồ cay nóng',
            N'Lâm sàng',
            N'Kết thúc điều trị',
            N'Đã thanh toán');
    
    PRINT '✓ Đã tạo phiếu khám KB002';
END

-- 6. Tạo toa thuốc cho phiếu khám (chỉ tạo nếu phiếu khám và thuốc đã tồn tại)
IF EXISTS (SELECT 1 FROM KhamBenh WHERE MaKham = 'KB001') AND EXISTS (SELECT 1 FROM Thuoc WHERE MaThuoc = 'T001')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM ToaThuoc WHERE MaKham = 'KB001' AND MaThuoc = 'T001')
    BEGIN
        INSERT INTO ToaThuoc (MaKham, MaThuoc, SoLuong, LieuDung, CachDung)
        VALUES ('KB001', 'T001', 20, N'1 viên/lần', N'Uống 3 lần/ngày sau ăn');
    END
END

IF EXISTS (SELECT 1 FROM KhamBenh WHERE MaKham = 'KB001') AND EXISTS (SELECT 1 FROM Thuoc WHERE MaThuoc = 'T002')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM ToaThuoc WHERE MaKham = 'KB001' AND MaThuoc = 'T002')
    BEGIN
        INSERT INTO ToaThuoc (MaKham, MaThuoc, SoLuong, LieuDung, CachDung)
        VALUES ('KB001', 'T002', 14, N'1 viên/lần', N'Uống 2 lần/ngày sau ăn');
    END
END

IF EXISTS (SELECT 1 FROM KhamBenh WHERE MaKham = 'KB001')
BEGIN
    PRINT '✓ Đã tạo toa thuốc cho KB001';
END

IF EXISTS (SELECT 1 FROM KhamBenh WHERE MaKham = 'KB002') AND EXISTS (SELECT 1 FROM Thuoc WHERE MaThuoc = 'T001')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM ToaThuoc WHERE MaKham = 'KB002' AND MaThuoc = 'T001')
    BEGIN
        INSERT INTO ToaThuoc (MaKham, MaThuoc, SoLuong, LieuDung, CachDung)
        VALUES ('KB002', 'T001', 10, N'1 viên/lần', N'Uống khi đau');
    END
END

IF EXISTS (SELECT 1 FROM KhamBenh WHERE MaKham = 'KB002')
BEGIN
    PRINT '✓ Đã tạo toa thuốc cho KB002';
END

-- 6.5. Kiểm tra có nhân viên không (cần cho hóa đơn)
IF NOT EXISTS (SELECT 1 FROM NhanVien)
BEGIN
    PRINT '⚠ Chưa có nhân viên. Đang tạo nhân viên test...';
    
    IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE MaNv = 'NV001')
    BEGIN
        INSERT INTO NhanVien (MaNv, HoTenNv)
        VALUES ('NV001', 'Nhân viên Thu ngân');
    END
    
    PRINT '✓ Đã tạo nhân viên test';
END

-- 7. Tạo hóa đơn (chỉ tạo nếu phiếu khám đã tồn tại)
IF EXISTS (SELECT 1 FROM KhamBenh WHERE MaKham = 'KB001')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM HoaDon WHERE MaHd = 'HD001')
    BEGIN
        INSERT INTO HoaDon (MaHd, MaKham, MaNv, NgayLap, ThanhTien, DiemTichLuySuDung)
        VALUES ('HD001', 'KB001', 'NV001', '2024-01-15', 230000, 0);
        
        PRINT '✓ Đã tạo hóa đơn HD001';
    END
END

IF EXISTS (SELECT 1 FROM KhamBenh WHERE MaKham = 'KB002')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM HoaDon WHERE MaHd = 'HD002')
    BEGIN
        INSERT INTO HoaDon (MaHd, MaKham, MaNv, NgayLap, ThanhTien, DiemTichLuySuDung)
        VALUES ('HD002', 'KB002', 'NV001', '2024-01-20', 50000, 0);
        
        PRINT '✓ Đã tạo hóa đơn HD002';
    END
END

-- 8. Tạo chi tiết hóa đơn (chỉ tạo nếu hóa đơn và thuốc đã tồn tại)
IF EXISTS (SELECT 1 FROM HoaDon WHERE MaHd = 'HD001') AND EXISTS (SELECT 1 FROM Thuoc WHERE MaThuoc = 'T001')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM ChiTietHoaDon WHERE MaHd = 'HD001' AND MaThuoc = 'T001')
    BEGIN
        INSERT INTO ChiTietHoaDon (MaHd, MaThuoc, SoLuong, DonGia)
        VALUES ('HD001', 'T001', 20, 5000);
    END
    
    IF EXISTS (SELECT 1 FROM Thuoc WHERE MaThuoc = 'T002')
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM ChiTietHoaDon WHERE MaHd = 'HD001' AND MaThuoc = 'T002')
        BEGIN
            INSERT INTO ChiTietHoaDon (MaHd, MaThuoc, SoLuong, DonGia)
            VALUES ('HD001', 'T002', 14, 8000);
        END
    END
    
    PRINT '✓ Đã tạo chi tiết hóa đơn HD001';
END

IF EXISTS (SELECT 1 FROM HoaDon WHERE MaHd = 'HD002') AND EXISTS (SELECT 1 FROM Thuoc WHERE MaThuoc = 'T001')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM ChiTietHoaDon WHERE MaHd = 'HD002' AND MaThuoc = 'T001')
    BEGIN
        INSERT INTO ChiTietHoaDon (MaHd, MaThuoc, SoLuong, DonGia)
        VALUES ('HD002', 'T001', 10, 5000);
    END
    
    PRINT '✓ Đã tạo chi tiết hóa đơn HD002';
END

-- 9. Tạo bệnh án từ phiếu khám đã thanh toán (chỉ tạo nếu phiếu khám đã tồn tại)
IF EXISTS (SELECT 1 FROM KhamBenh WHERE MaKham = 'KB001' AND TrangThai = 'Đã thanh toán')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM BenhAn WHERE MaBenhAn = 'BA001')
    BEGIN
        PRINT 'Đang tạo bệnh án BA001 từ KB001...';
        
        INSERT INTO BenhAn (MaBenhAn, MaKham, MaBn, MaBs, LyDoKham, QuaTrinhBenhLy, 
                            TienSuBenhNhan, TienSuGiaDinh, KhamBoPhan, ChuanDoan, HuongXuTri, 
                            LoaiKham, XuTriKham, NgayKham, NgayLuu)
        SELECT 'BA001', 'KB001', MaBn, MaBs, LyDoKham, QuaTrinhBenhLy, 
               TienSuBenhNhan, TienSuGiaDinh, KhamBoPhan, ChuanDoan, HuongXuTri, 
               LoaiKham, XuTriKham, NgayKham, GETDATE()
        FROM KhamBenh
        WHERE MaKham = 'KB001';
        
        -- Copy toa thuốc vào bệnh án
        IF EXISTS (SELECT 1 FROM ToaThuoc WHERE MaKham = 'KB001')
        BEGIN
            INSERT INTO BenhAnToaThuoc (MaBenhAn, MaThuoc, SoLuong, LieuDung, CachDung)
            SELECT 'BA001', MaThuoc, SoLuong, LieuDung, CachDung
            FROM ToaThuoc
            WHERE MaKham = 'KB001';
        END
        
        PRINT '✓ Đã tạo bệnh án BA001';
    END
END

IF EXISTS (SELECT 1 FROM KhamBenh WHERE MaKham = 'KB002' AND TrangThai = 'Đã thanh toán')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM BenhAn WHERE MaBenhAn = 'BA002')
    BEGIN
        PRINT 'Đang tạo bệnh án BA002 từ KB002...';
        
        INSERT INTO BenhAn (MaBenhAn, MaKham, MaBn, MaBs, LyDoKham, QuaTrinhBenhLy, 
                            TienSuBenhNhan, TienSuGiaDinh, KhamBoPhan, ChuanDoan, HuongXuTri, 
                            LoaiKham, XuTriKham, NgayKham, NgayLuu)
        SELECT 'BA002', 'KB002', MaBn, MaBs, LyDoKham, QuaTrinhBenhLy, 
               TienSuBenhNhan, TienSuGiaDinh, KhamBoPhan, ChuanDoan, HuongXuTri, 
               LoaiKham, XuTriKham, NgayKham, GETDATE()
        FROM KhamBenh
        WHERE MaKham = 'KB002';
        
        -- Copy toa thuốc vào bệnh án
        IF EXISTS (SELECT 1 FROM ToaThuoc WHERE MaKham = 'KB002')
        BEGIN
            INSERT INTO BenhAnToaThuoc (MaBenhAn, MaThuoc, SoLuong, LieuDung, CachDung)
            SELECT 'BA002', MaThuoc, SoLuong, LieuDung, CachDung
            FROM ToaThuoc
            WHERE MaKham = 'KB002';
        END
        
        PRINT '✓ Đã tạo bệnh án BA002';
    END
END

COMMIT TRANSACTION;

PRINT '';
PRINT '========================================';
PRINT 'HOÀN TẤT TẠO DỮ LIỆU TEST!';
PRINT '========================================';
PRINT '';
PRINT 'Đã tạo:';
PRINT '- 2 bệnh nhân (BN001, BN002)';
PRINT '- 1 bác sĩ (BS001)';
PRINT '- 1 chuẩn đoán (CD001)';
PRINT '- 2 thuốc (T001, T002)';
PRINT '- 2 phiếu khám đã thanh toán (KB001, KB002)';
PRINT '- 2 hóa đơn (HD001, HD002)';
PRINT '- 2 bệnh án (BA001, BA002)';
PRINT '';
PRINT 'Bây giờ bạn có thể:';
PRINT '1. Vào trang "Hồ sơ bệnh án" trên web để xem';
PRINT '2. Test chức năng lưu bệnh án từ danh sách bệnh nhân';
PRINT '';

-- Hiển thị kết quả
SELECT 
    ba.MaBenhAn,
    ba.MaKham,
    b.HoTenBn,
    ba.NgayKham,
    ba.ChuanDoan,
    ba.NgayLuu
FROM BenhAn ba
INNER JOIN BenhNhan b ON ba.MaBn = b.MaBn
ORDER BY ba.NgayLuu DESC;

