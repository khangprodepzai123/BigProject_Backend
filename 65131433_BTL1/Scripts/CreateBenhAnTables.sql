-- Script tạo bảng BenhAn và BenhAnToaThuoc
-- Chạy script này trong SQL Server Management Studio hoặc Azure Data Studio

USE PhongKhamOnline;
GO

-- Tạo bảng BenhAn
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BenhAn]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BenhAn] (
        [MaBenhAn] [nvarchar](10) NOT NULL,
        [MaKham] [nvarchar](10) NOT NULL,
        [MaBn] [nvarchar](10) NOT NULL,
        [MaBs] [nvarchar](10) NOT NULL,
        [LyDoKham] [nvarchar](500) NULL,
        [QuaTrinhBenhLy] [nvarchar](1000) NULL,
        [TienSuBenhNhan] [nvarchar](500) NULL,
        [TienSuGiaDinh] [nvarchar](500) NULL,
        [KhamBoPhan] [nvarchar](1000) NULL,
        [ChuanDoan] [nvarchar](500) NULL,
        [HuongXuTri] [nvarchar](500) NULL,
        [LoaiKham] [nvarchar](50) NULL,
        [XuTriKham] [nvarchar](100) NULL,
        [NgayKham] [date] NULL,
        [NgayLuu] [datetime] NULL DEFAULT (getdate()),
        CONSTRAINT [PK__BenhAn__27250050DE04A84B] PRIMARY KEY CLUSTERED ([MaBenhAn] ASC)
    );
    
    -- Tạo Foreign Keys
    ALTER TABLE [dbo].[BenhAn]
    ADD CONSTRAINT [FK_BenhAn_BenhNhan] FOREIGN KEY ([MaBn])
    REFERENCES [dbo].[BenhNhan] ([MaBn]);
    
    ALTER TABLE [dbo].[BenhAn]
    ADD CONSTRAINT [FK_BenhAn_BacSi] FOREIGN KEY ([MaBs])
    REFERENCES [dbo].[BacSi] ([MaBs]);
    
    PRINT 'Bảng BenhAn đã được tạo thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng BenhAn đã tồn tại.';
END
GO

-- Tạo bảng BenhAnToaThuoc
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BenhAnToaThuoc]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BenhAnToaThuoc] (
        [MaBenhAn] [nvarchar](10) NOT NULL,
        [MaThuoc] [nvarchar](10) NOT NULL,
        [SoLuong] [int] NOT NULL,
        [LieuDung] [nvarchar](200) NULL,
        [CachDung] [nvarchar](300) NULL,
        CONSTRAINT [PK__BenhAnTo__7185851825F4C153] PRIMARY KEY CLUSTERED ([MaBenhAn] ASC, [MaThuoc] ASC)
    );
    
    -- Tạo Foreign Keys
    ALTER TABLE [dbo].[BenhAnToaThuoc]
    ADD CONSTRAINT [FK_BenhAnToaThuoc_BenhAn] FOREIGN KEY ([MaBenhAn])
    REFERENCES [dbo].[BenhAn] ([MaBenhAn]);
    
    ALTER TABLE [dbo].[BenhAnToaThuoc]
    ADD CONSTRAINT [FK_BenhAnToaThuoc_Thuoc] FOREIGN KEY ([MaThuoc])
    REFERENCES [dbo].[Thuoc] ([MaThuoc]);
    
    PRINT 'Bảng BenhAnToaThuoc đã được tạo thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng BenhAnToaThuoc đã tồn tại.';
END
GO

PRINT 'Hoàn tất!';

