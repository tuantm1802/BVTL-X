-- ===================================================================================
-- SCRIPT NÂNG CẤP GIAI ĐOẠN 2: TÁI CẤU TRÚC DỮ LIỆU & RÀNG BUỘC TOÀN VẸN (PHASE 2)
-- Hệ thống: BVTL-X / CSDL: BVTL_REPORTING_DEV
-- Ngày thực hiện: 29/09/2026
-- ===================================================================================

USE [BVTL_REPORTING_DEV];
GO

SET NOCOUNT ON;
PRINT N'=== BẮT ĐẦU NÂNG CẤP GIAI ĐOẠN 2 ===';

-- -----------------------------------------------------------------------------------
-- BƯỚC 1: TẠO BẢNG LIÊN KẾT CHUẨN BVTL_QT_NGUOI_DUNG_CITY (TASK 2.2)
-- -----------------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.BVTL_QT_NGUOI_DUNG_CITY', N'U') IS NULL
BEGIN
    PRINT N'1. Tạo bảng liên kết BVTL_QT_NGUOI_DUNG_CITY...';
    CREATE TABLE [dbo].[BVTL_QT_NGUOI_DUNG_CITY] (
        [Id]          INT IDENTITY(1,1) NOT NULL,
        [NguoiDungId] BIGINT NOT NULL,
        [CityCode]    VARCHAR(10) NOT NULL,
        [IsActive]    BIT NOT NULL CONSTRAINT [DF_BVTL_QT_NGUOI_DUNG_CITY_IsActive] DEFAULT (1),
        [CreatedDate] DATETIME NULL CONSTRAINT [DF_BVTL_QT_NGUOI_DUNG_CITY_CreatedDate] DEFAULT (GETDATE()),
        CONSTRAINT [PK_BVTL_QT_NGUOI_DUNG_CITY] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_BVTL_QT_NGUOI_DUNG_CITY] UNIQUE NONCLUSTERED ([NguoiDungId] ASC, [CityCode] ASC)
    );
    PRINT N'  -> Đã tạo bảng BVTL_QT_NGUOI_DUNG_CITY thành công.';
END
ELSE
BEGIN
    PRINT N'1. Bảng BVTL_QT_NGUOI_DUNG_CITY đã tồn tại.';
END
GO

-- -----------------------------------------------------------------------------------
-- BƯỚC 2: MIGRATION DỮ LIỆU TỪ CityCodes VÀO BẢNG BVTL_QT_NGUOI_DUNG_CITY
-- -----------------------------------------------------------------------------------
PRINT N'2. Di chuyển dữ liệu phân quyền tỉnh từ chuỗi CityCodes vào bảng liên kết...';
INSERT INTO [dbo].[BVTL_QT_NGUOI_DUNG_CITY] ([NguoiDungId], [CityCode], [IsActive], [CreatedDate])
SELECT DISTINCT 
    u.[ID] AS [NguoiDungId], 
    LTRIM(RTRIM(c.[value])) AS [CityCode], 
    1 AS [IsActive], 
    GETDATE() AS [CreatedDate]
FROM [dbo].[BVTL_QT_NGUOI_DUNG] u
CROSS APPLY STRING_SPLIT(u.[CityCodes], ',') c
INNER JOIN [dbo].[BVTL_CITES] cit ON LTRIM(RTRIM(c.[value])) = cit.[Code]
WHERE u.[CityCodes] IS NOT NULL 
  AND LTRIM(RTRIM(c.[value])) != ''
  AND NOT EXISTS (
      SELECT 1 FROM [dbo].[BVTL_QT_NGUOI_DUNG_CITY] dc 
      WHERE dc.[NguoiDungId] = u.[ID] AND dc.[CityCode] = LTRIM(RTRIM(c.[value]))
  );
PRINT N'  -> Đã đồng bộ phân quyền tỉnh vào BVTL_QT_NGUOI_DUNG_CITY.';
GO

-- -----------------------------------------------------------------------------------
-- BƯỚC 3: DỌN DẸP ORPHAN INACTIVE VÀ CHUẨN HÓA KIỂU DỮ LIỆU BẢNG MAPPING (TASK 2.1)
-- -----------------------------------------------------------------------------------
PRINT N'3. Dọn dẹp bản ghi mapping orphan và chuẩn hóa kiểu dữ liệu...';

-- Xóa các bản ghi orphan không có nhóm CBO thực tế (đã backup an toàn tại BVTL_QT_NGUOI_DUNG_NHOM_TBH_BAK_20260929)
DELETE FROM [dbo].[BVTL_QT_NGUOI_DUNG_NHOM_TBH]
WHERE [NhomTBHMa] NOT IN (SELECT [manhom_tbh] FROM [dbo].[BVTL_NHOM_TBH]);

-- Chuẩn hóa độ dài NhomTBHMa từ VARCHAR(50) sang VARCHAR(6) để khớp PK BVTL_NHOM_TBH
ALTER TABLE [dbo].[BVTL_QT_NGUOI_DUNG_NHOM_TBH] 
ALTER COLUMN [NhomTBHMa] VARCHAR(6) NOT NULL;

-- Chuẩn hóa kiểu NguoiDungId từ INT sang BIGINT để khớp PK BVTL_QT_NGUOI_DUNG
ALTER TABLE [dbo].[BVTL_QT_NGUOI_DUNG_NHOM_TBH] 
ALTER COLUMN [NguoiDungId] BIGINT NOT NULL;

PRINT N'  -> Đã chuẩn hóa kiểu dữ liệu bảng mapping thành công.';
GO

-- -----------------------------------------------------------------------------------
-- BƯỚC 4: THÊM CÁC RÀNG BUỘC KHÓA NGOẠI (FOREIGN KEYS) VẬT LÝ
-- -----------------------------------------------------------------------------------
PRINT N'4. Thêm các ràng buộc Foreign Key vật lý...';

-- FK1: BVTL_QT_NGUOI_DUNG -> BVTL_QT_QUYEN
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_NguoiDung_Quyen')
BEGIN
    ALTER TABLE [dbo].[BVTL_QT_NGUOI_DUNG] 
    ADD CONSTRAINT [FK_NguoiDung_Quyen] FOREIGN KEY ([GroupID]) 
    REFERENCES [dbo].[BVTL_QT_QUYEN] ([ID]);
    PRINT N'  -> Đã tạo FK_NguoiDung_Quyen thành công.';
END

-- FK2: BVTL_QT_NGUOI_DUNG_NHOM_TBH -> BVTL_QT_NGUOI_DUNG
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_NDNhomTBH_NguoiDung')
BEGIN
    ALTER TABLE [dbo].[BVTL_QT_NGUOI_DUNG_NHOM_TBH] 
    ADD CONSTRAINT [FK_NDNhomTBH_NguoiDung] FOREIGN KEY ([NguoiDungId]) 
    REFERENCES [dbo].[BVTL_QT_NGUOI_DUNG] ([ID]) ON DELETE CASCADE;
    PRINT N'  -> Đã tạo FK_NDNhomTBH_NguoiDung thành công.';
END

-- FK3: BVTL_QT_NGUOI_DUNG_NHOM_TBH -> BVTL_NHOM_TBH
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_NDNhomTBH_NhomTBH')
BEGIN
    ALTER TABLE [dbo].[BVTL_QT_NGUOI_DUNG_NHOM_TBH] 
    ADD CONSTRAINT [FK_NDNhomTBH_NhomTBH] FOREIGN KEY ([NhomTBHMa]) 
    REFERENCES [dbo].[BVTL_NHOM_TBH] ([manhom_tbh]);
    PRINT N'  -> Đã tạo FK_NDNhomTBH_NhomTBH thành công.';
END

-- FK4: BVTL_QT_NGUOI_DUNG_CITY -> BVTL_QT_NGUOI_DUNG
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_NDCity_NguoiDung')
BEGIN
    ALTER TABLE [dbo].[BVTL_QT_NGUOI_DUNG_CITY] 
    ADD CONSTRAINT [FK_NDCity_NguoiDung] FOREIGN KEY ([NguoiDungId]) 
    REFERENCES [dbo].[BVTL_QT_NGUOI_DUNG] ([ID]) ON DELETE CASCADE;
    PRINT N'  -> Đã tạo FK_NDCity_NguoiDung thành công.';
END

-- FK5: BVTL_QT_NGUOI_DUNG_CITY -> BVTL_CITES
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_NDCity_City')
BEGIN
    ALTER TABLE [dbo].[BVTL_QT_NGUOI_DUNG_CITY] 
    ADD CONSTRAINT [FK_NDCity_City] FOREIGN KEY ([CityCode]) 
    REFERENCES [dbo].[BVTL_CITES] ([Code]);
    PRINT N'  -> Đã tạo FK_NDCity_City thành công.';
END
GO

-- -----------------------------------------------------------------------------------
-- BƯỚC 5: XÓA CÁC CỘT BỎ HOANG ProvinceID, DistrictID (TASK 2.3)
-- -----------------------------------------------------------------------------------
PRINT N'5. Xóa 2 cột bỏ hoang ProvinceID, DistrictID trong BVTL_QT_NGUOI_DUNG...';

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.BVTL_QT_NGUOI_DUNG') AND name = N'ProvinceID')
BEGIN
    ALTER TABLE [dbo].[BVTL_QT_NGUOI_DUNG] DROP COLUMN [ProvinceID];
    PRINT N'  -> Đã xóa cột ProvinceID.';
END

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.BVTL_QT_NGUOI_DUNG') AND name = N'DistrictID')
BEGIN
    ALTER TABLE [dbo].[BVTL_QT_NGUOI_DUNG] DROP COLUMN [DistrictID];
    PRINT N'  -> Đã xóa cột DistrictID.';
END
GO

PRINT N'=== HOÀN THÀNH TOÀN BỘ NÂNG CẤP GIAI ĐOẠN 2 ===';
GO
