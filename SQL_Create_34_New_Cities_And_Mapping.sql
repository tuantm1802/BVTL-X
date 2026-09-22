-- ===================================================================
-- HỆ THỐNG BVTL-X / CD45 (DREAMH)
-- NÂNG CẤP BẢNG DANH MỤC 34 TỈNH MỚI (NGHỊ QUYẾT 202/2025/QH15)
-- VÀ BẢNG ÁNH XẠ 63 TỈNH CŨ SANG 34 TỈNH MỚI
-- ===================================================================

USE [BVTL_REPORTING_DEV];
GO

-- 1. BẢNG DANH MỤC 34 TỈNH MỚI
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BVTL_DM_TINH_MOI')
BEGIN
    CREATE TABLE [dbo].[BVTL_DM_TINH_MOI] (
        [Code] VARCHAR(10) PRIMARY KEY NOT NULL,
        [Name] NVARCHAR(150) NOT NULL,
        [Code_Map] VARCHAR(10) NULL,
        [IsKeyProvince] BIT NOT NULL DEFAULT 0,
        [OldCount] INT NOT NULL DEFAULT 1,
        [OldNamesSummary] NVARCHAR(250) NULL,
        [DisplayOrder] INT NOT NULL DEFAULT 99,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 2. BẢNG ÁNH XẠ TỈNH CŨ VÀ TỈNH MỚI (63 TỈNH CŨ -> 34 TỈNH MỚI)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BVTL_MAP_TINH_CU_MOI')
BEGIN
    CREATE TABLE [dbo].[BVTL_MAP_TINH_CU_MOI] (
        [OldCityCode] VARCHAR(10) PRIMARY KEY NOT NULL,
        [NewCityCode] VARCHAR(10) NOT NULL,
        [OldCityName] NVARCHAR(150) NOT NULL,
        [NewCityName] NVARCHAR(150) NOT NULL,
        [EffectiveDate] DATETIME NOT NULL DEFAULT '2025-07-01'
    );
END
GO

-- 3. XÓA DỮ LIỆU CŨ ĐỂ NẠP LẠI CHUẨN XÁC
DELETE FROM [dbo].[BVTL_MAP_TINH_CU_MOI];
DELETE FROM [dbo].[BVTL_DM_TINH_MOI];
GO

-- 4. INSERT 34 TỈNH MỚI (NQ 202/2025/QH15)
INSERT INTO [dbo].[BVTL_DM_TINH_MOI] ([Code], [Name], [Code_Map], [IsKeyProvince], [OldCount], [OldNamesSummary], [DisplayOrder], [IsActive]) VALUES
('HNO', N'Hà Nội', 'HN', 1, 1, N'Hà Nội', 1, 1),
('HPG', N'Hải Phòng', 'HP', 1, 2, N'Hải Phòng + Hải Dương', 2, 1),
('NBI', N'Ninh Bình', 'NB', 1, 3, N'Ninh Bình + Hà Nam + Nam Định', 3, 1),
('HYE', N'Hưng Yên', 'HY', 1, 2, N'Hưng Yên + Thái Bình', 4, 1),
('NAN', N'Nghệ An', 'NA', 1, 1, N'Nghệ An', 5, 1),
('HCM', N'TP. Hồ Chí Minh', 'HC', 1, 3, N'TP. Hồ Chí Minh + Bình Dương + Bà Rịa – Vũng Tàu', 6, 1),
('TTH', N'Thừa Thiên Huế', NULL, 0, 1, N'Thừa Thiên Huế', 7, 1),
('DNG', N'Đà Nẵng', NULL, 0, 2, N'Đà Nẵng + Quảng Nam', 8, 1),
('CTH', N'Cần Thơ', NULL, 0, 3, N'Cần Thơ + Sóc Trăng + Hậu Giang', 9, 1),
('BNI', N'Bắc Ninh', NULL, 0, 2, N'Bắc Ninh + Bắc Giang', 10, 1),
('PTH', N'Phú Thọ', NULL, 0, 3, N'Phú Thọ + Vĩnh Phúc + Hòa Bình', 11, 1),
('QNH', N'Quảng Ninh', NULL, 0, 1, N'Quảng Ninh', 12, 1),
('TNG', N'Thái Nguyên', NULL, 0, 2, N'Thái Nguyên + Bắc Kạn', 13, 1),
('LCI', N'Lào Cai', NULL, 0, 2, N'Lào Cai + Yên Bái', 14, 1),
('TQU', N'Tuyên Quang', NULL, 0, 2, N'Tuyên Quang + Hà Giang', 15, 1),
('LSN', N'Lạng Sơn', NULL, 0, 1, N'Lạng Sơn', 16, 1),
('CBA', N'Cao Bằng', NULL, 0, 1, N'Cao Bằng', 17, 1),
('SLA', N'Sơn La', NULL, 0, 1, N'Sơn La', 18, 1),
('DBI', N'Điện Biên', NULL, 0, 1, N'Điện Biên', 19, 1),
('LCA', N'Lai Châu', NULL, 0, 1, N'Lai Châu', 20, 1),
('THA', N'Thanh Hóa', NULL, 0, 1, N'Thanh Hóa', 21, 1),
('HTI', N'Hà Tĩnh', NULL, 0, 1, N'Hà Tĩnh', 22, 1),
('QTR', N'Quảng Trị', NULL, 0, 2, N'Quảng Trị + Quảng Bình', 23, 1),
('QNG', N'Quảng Ngãi', NULL, 0, 2, N'Quảng Ngãi + Kon Tum', 24, 1),
('GLA', N'Gia Lai', NULL, 0, 2, N'Gia Lai + Bình Định', 25, 1),
('KHA', N'Khánh Hòa', NULL, 0, 2, N'Khánh Hòa + Ninh Thuận', 26, 1),
('DLA', N'Đắk Lắk', NULL, 0, 2, N'Đắk Lắk + Phú Yên', 27, 1),
('LDG', N'Lâm Đồng', NULL, 0, 3, N'Lâm Đồng + Đắk Nông + Bình Thuận', 28, 1),
('DNA', N'Đồng Nai', NULL, 0, 2, N'Đồng Nai + Bình Phước', 29, 1),
('TNI', N'Tây Ninh', NULL, 0, 2, N'Tây Ninh + Long An', 30, 1),
('DTP', N'Đồng Tháp', NULL, 0, 2, N'Đồng Tháp + Tiền Giang', 31, 1),
('VLG', N'Vĩnh Long', NULL, 0, 3, N'Vĩnh Long + Bến Tre + Trà Vinh', 32, 1),
('AGI', N'An Giang', NULL, 0, 2, N'An Giang + Kiên Giang', 33, 1),
('CMU', N'Cà Mau', NULL, 0, 2, N'Cà Mau + Bạc Liêu', 34, 1);
GO

-- 5. INSERT ÁNH XẠ 63 TỈNH CŨ VÀO 34 TỈNH MỚI
INSERT INTO [dbo].[BVTL_MAP_TINH_CU_MOI] ([OldCityCode], [NewCityCode], [OldCityName], [NewCityName]) VALUES
-- Hà Nội (1)
('HNO', 'HNO', N'Hà Nội', N'Hà Nội'),
-- Hải Phòng (2)
('HPG', 'HPG', N'Hải Phòng', N'Hải Phòng'),
('HDU', 'HPG', N'Hải Dương', N'Hải Phòng'),
-- Ninh Bình (3)
('NBI', 'NBI', N'Ninh Bình', N'Ninh Bình'),
('HNA', 'NBI', N'Hà Nam', N'Ninh Bình'),
('NDH', 'NBI', N'Nam Định', N'Ninh Bình'),
-- Hưng Yên (2)
('HYE', 'HYE', N'Hưng Yên', N'Hưng Yên'),
('TBH', 'HYE', N'Thái Bình', N'Hưng Yên'),
-- Nghệ An (1)
('NAN', 'NAN', N'Nghệ An', N'Nghệ An'),
-- TP. Hồ Chí Minh (3)
('HCM', 'HCM', N'TP Hồ Chí Minh', N'TP. Hồ Chí Minh'),
('BDU', 'HCM', N'Bình Dương', N'TP. Hồ Chí Minh'),
('VTB', 'HCM', N'Bà Rịa – Vũng Tàu', N'TP. Hồ Chí Minh'),
-- Thừa Thiên Huế (1)
('TTH', 'TTH', N'Thừa Thiên - Huế', N'Thừa Thiên Huế'),
-- Đà Nẵng (2)
('DNG', 'DNG', N'Đà Nẵng', N'Đà Nẵng'),
('QNA', 'DNG', N'Quảng Nam', N'Đà Nẵng'),
-- Cần Thơ (3)
('CTH', 'CTH', N'Cần Thơ', N'Cần Thơ'),
('STG', 'CTH', N'Sóc Trăng', N'Cần Thơ'),
('HAG', 'CTH', N'Hậu Giang', N'Cần Thơ'),
-- Bắc Ninh (2)
('BNI', 'BNI', N'Bắc Ninh', N'Bắc Ninh'),
('BGI', 'BNI', N'Bắc Giang', N'Bắc Ninh'),
-- Phú Thọ (3)
('PTH', 'PTH', N'Phú Thọ', N'Phú Thọ'),
('VPH', 'PTH', N'Vĩnh Phúc', N'Phú Thọ'),
('HBI', 'PTH', N'Hòa Bình', N'Phú Thọ'),
-- Quảng Ninh (1)
('QNH', 'QNH', N'Quảng Ninh', N'Quảng Ninh'),
-- Thái Nguyên (2)
('TNG', 'TNG', N'Thái Nguyên', N'Thái Nguyên'),
('BKA', 'TNG', N'Bắc Kan', N'Thái Nguyên'),
-- Lào Cai (2)
('LCI', 'LCI', N'Lào Cai', N'Lào Cai'),
('YBA', 'LCI', N'Yên Bái', N'Lào Cai'),
-- Tuyên Quang (2)
('TQU', 'TQU', N'Tuyên Quang', N'Tuyên Quang'),
('HGI', 'TQU', N'Hà Giang', N'Tuyên Quang'),
-- Lạng Sơn (1)
('LSN', 'LSN', N'Lạng Sơn', N'Lạng Sơn'),
-- Cao Bằng (1)
('CBA', 'CBA', N'Cao Bằng', N'Cao Bằng'),
-- Sơn La (1)
('SLA', 'SLA', N'Sơn La', N'Sơn La'),
-- Điện Biên (1)
('DBI', 'DBI', N'Điện Biên', N'Điện Biên'),
-- Lai Châu (1)
('LCA', 'LCA', N'Lai Châu', N'Lai Châu'),
-- Thanh Hóa (1)
('THA', 'THA', N'Thanh Hóa', N'Thanh Hóa'),
-- Hà Tĩnh (1)
('HTI', 'HTI', N'Hà Tĩnh', N'Hà Tĩnh'),
-- Quảng Trị (2)
('QTR', 'QTR', N'Quảng Trị', N'Quảng Trị'),
('QBI', 'QTR', N'Quảng Bình', N'Quảng Trị'),
-- Quảng Ngãi (2)
('QNG', 'QNG', N'Quảng Ngãi', N'Quảng Ngãi'),
('KTU', 'QNG', N'Kon Tum', N'Quảng Ngãi'),
-- Gia Lai (2)
('GLA', 'GLA', N'Gia Lai', N'Gia Lai'),
('BDI', 'GLA', N'Bình Định', N'Gia Lai'),
-- Khánh Hòa (2)
('KHA', 'KHA', N'Khánh Hòa', N'Khánh Hòa'),
('NTH', 'KHA', N'Ninh Thuận', N'Khánh Hòa'),
-- Đắk Lắk (2)
('DLA', 'DLA', N'Đắc Lắc', N'Đắk Lắk'),
('PYE', 'DLA', N'Phú Yên', N'Đắk Lắk'),
-- Lâm Đồng (3)
('LDG', 'LDG', N'Lâm Đồng', N'Lâm Đồng'),
('DKN', 'LDG', N'Đăk Nông', N'Lâm Đồng'),
('BTN', 'LDG', N'Bình Thuận', N'Lâm Đồng'),
-- Đồng Nai (2)
('DNA', 'DNA', N'Đồng Nai', N'Đồng Nai'),
('BPC', 'DNA', N'Bình Phước', N'Đồng Nai'),
-- Tây Ninh (2)
('TNI', 'TNI', N'Tây Ninh', N'Tây Ninh'),
('LAN', 'TNI', N'Long An', N'Tây Ninh'),
-- Đồng Tháp (2)
('DTP', 'DTP', N'Đồng Tháp', N'Đồng Tháp'),
('TGG', 'DTP', N'Tiền Giang', N'Đồng Tháp'),
-- Vĩnh Long (3)
('VLG', 'VLG', N'Vĩnh Long', N'Vĩnh Long'),
('BTR', 'VLG', N'Bến Tre', N'Vĩnh Long'),
('TVH', 'VLG', N'Trà Vinh', N'Vĩnh Long'),
-- An Giang (2)
('AGI', 'AGI', N'An Giang', N'An Giang'),
('KGI', 'AGI', N'Kiên Giang', N'An Giang'),
-- Cà Mau (2)
('CMU', 'CMU', N'Cà Mau', N'Cà Mau'),
('BLI', 'CMU', N'Bạc Liêu', N'Cà Mau');
GO

-- 6. NÂNG CẤP STORED PROCEDURE City_Get_By_Page HỖ TRỢ @CityMode ('NEW34' | 'OLD63')
CREATE OR ALTER PROCEDURE [dbo].[City_Get_By_Page] 
	@Keyword NVARCHAR(250) = null,
	@OrderByName VARCHAR(100) = 'KeyFirst',
	@Page int = 1,
	@PageSize int = 20,
	@IsKeyOnly bit = 0,
	@CityMode VARCHAR(10) = 'NEW34'
AS
BEGIN
    SET NOCOUNT ON;

    IF @CityMode = 'OLD63'
    BEGIN
        SELECT 
            c.Code,
            c.[Name],
            c.Code_Map,
            c.IsActive,
            c.CreatedDate,
            c.CreatedBy,
            c.LastUpdateDate,
            c.LastUpdateBy,
            CAST(CASE WHEN c.Code_Map IS NOT NULL AND RTRIM(c.Code_Map) <> '' THEN 1 ELSE 0 END AS BIT) AS IsKeyProvince,
            1 AS OldCount,
            c.[Name] AS OldNamesSummary,
            count(c.Code) over() as TotalRow  
        FROM [dbo].[BVTL_CITES] c
        WHERE (@Keyword IS NULL OR (c.Code LIKE '%'+@Keyword+'%' OR c.[Name] LIKE N'%'+@Keyword+'%' OR c.Code_Map LIKE '%'+@Keyword+'%'))
          AND (@IsKeyOnly = 0 OR (@IsKeyOnly = 1 AND c.Code_Map IS NOT NULL AND RTRIM(c.Code_Map) <> ''))
        ORDER BY 
            CASE 
                WHEN @OrderByName = 'KeyFirst' AND c.Code_Map IS NOT NULL AND RTRIM(c.Code_Map) <> '' THEN 0
                WHEN @OrderByName = 'KeyFirst' THEN 1
                ELSE 0 
            END,
            CASE @OrderByName
                WHEN 'Name' THEN c.[Name]
                ELSE c.Code
            END ASC
        OFFSET ((@Page - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE
    BEGIN
        -- Chế độ 34 Tỉnh mới (NQ 202/2025/QH15)
        SELECT 
            m.Code,
            m.[Name],
            m.Code_Map,
            m.IsActive,
            m.CreatedDate,
            1 AS CreatedBy,
            NULL AS LastUpdateDate,
            NULL AS LastUpdateBy,
            m.IsKeyProvince,
            m.OldCount,
            m.OldNamesSummary,
            count(m.Code) over() as TotalRow  
        FROM [dbo].[BVTL_DM_TINH_MOI] m
        WHERE (@Keyword IS NULL OR (m.Code LIKE '%'+@Keyword+'%' OR m.[Name] LIKE N'%'+@Keyword+'%' OR m.Code_Map LIKE '%'+@Keyword+'%' OR m.OldNamesSummary LIKE N'%'+@Keyword+'%'))
          AND (@IsKeyOnly = 0 OR (@IsKeyOnly = 1 AND m.IsKeyProvince = 1))
        ORDER BY 
            CASE 
                WHEN @OrderByName = 'KeyFirst' AND m.IsKeyProvince = 1 THEN 0
                WHEN @OrderByName = 'KeyFirst' THEN 1
                ELSE 0 
            END,
            m.DisplayOrder ASC,
            CASE @OrderByName
                WHEN 'Name' THEN m.[Name]
                ELSE m.Code
            END ASC
        OFFSET ((@Page - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
END
GO