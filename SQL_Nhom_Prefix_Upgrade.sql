-- =========================================================================================
-- SCRIPT NÂNG CẤP XƯNG DANH RIÊNG (PREFIX / LOẠI HÌNH TỔ CHỨC) CHO CÁC NHÓM CBO
-- Dự án: Bảo Vệ Tương Lai (BVTL) & CD45 (DREAMH)
-- =========================================================================================

-- 1. Bổ sung cột PREFIX và SHORT_PREFIX vào bảng BVTL_NHOM_TBH
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BVTL_NHOM_TBH' AND COLUMN_NAME = 'PREFIX')
BEGIN
    ALTER TABLE dbo.BVTL_NHOM_TBH ADD PREFIX NVARCHAR(50) CONSTRAINT DF_BVTL_NHOM_TBH_PREFIX DEFAULT N'Nhóm' WITH VALUES;
    PRINT N'Đã thêm cột PREFIX vào BVTL_NHOM_TBH';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BVTL_NHOM_TBH' AND COLUMN_NAME = 'SHORT_PREFIX')
BEGIN
    ALTER TABLE dbo.BVTL_NHOM_TBH ADD SHORT_PREFIX NVARCHAR(20) CONSTRAINT DF_BVTL_NHOM_TBH_SHORT_PREFIX DEFAULT N'Nhóm' WITH VALUES;
    PRINT N'Đã thêm cột SHORT_PREFIX vào BVTL_NHOM_TBH';
END
GO

-- 2. Bổ sung cột PREFIX và SHORT_PREFIX vào bảng CD45_NHOM_TCV
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD45_NHOM_TCV' AND COLUMN_NAME = 'PREFIX')
BEGIN
    ALTER TABLE dbo.CD45_NHOM_TCV ADD PREFIX NVARCHAR(50) CONSTRAINT DF_CD45_NHOM_TCV_PREFIX DEFAULT N'Nhóm' WITH VALUES;
    PRINT N'Đã thêm cột PREFIX vào CD45_NHOM_TCV';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD45_NHOM_TCV' AND COLUMN_NAME = 'SHORT_PREFIX')
BEGIN
    ALTER TABLE dbo.CD45_NHOM_TCV ADD SHORT_PREFIX NVARCHAR(20) CONSTRAINT DF_CD45_NHOM_TCV_SHORT_PREFIX DEFAULT N'Nhóm' WITH VALUES;
    PRINT N'Đã thêm cột SHORT_PREFIX vào CD45_NHOM_TCV';
END
GO

-- 3. Cập nhật dữ liệu mặc định cho các nhóm TP.HCM (Doanh nghiệp xã hội / DNXH)
UPDATE dbo.BVTL_NHOM_TBH
SET PREFIX = N'Doanh nghiệp xã hội', 
    SHORT_PREFIX = N'DNXH'
WHERE city_code = 'HCM' 
   OR manhom_tbh IN ('HC_ALO', 'HC_G3V', 'HC_MYH', 'HC_TGA', 'HCM10', 'HCM15', 'HCM151', 'HCM19')
   OR manhom_tbh_map IN ('alo', 'g3vn', 'myh', 'tg', 'HC03');
PRINT N'Đã cập nhật xưng danh Doanh nghiệp xã hội cho các nhóm TP.HCM trong BVTL_NHOM_TBH';
GO

UPDATE dbo.CD45_NHOM_TCV
SET PREFIX = N'Doanh nghiệp xã hội', 
    SHORT_PREFIX = N'DNXH'
WHERE CITY_CODE = 'HCM' OR MA_NHOM IN ('alo', 'g3vn', 'myh', 'tg');
PRINT N'Đã cập nhật xưng danh Doanh nghiệp xã hội cho các nhóm TP.HCM trong CD45_NHOM_TCV';
GO

-- Đảm bảo các nhóm còn lại có giá trị mặc định là 'Nhóm' nếu NULL hoặc rỗng
UPDATE dbo.BVTL_NHOM_TBH
SET PREFIX = N'Nhóm'
WHERE PREFIX IS NULL OR LTRIM(RTRIM(PREFIX)) = '';

UPDATE dbo.BVTL_NHOM_TBH
SET SHORT_PREFIX = N'Nhóm'
WHERE SHORT_PREFIX IS NULL OR LTRIM(RTRIM(SHORT_PREFIX)) = '';

UPDATE dbo.CD45_NHOM_TCV
SET PREFIX = N'Nhóm'
WHERE PREFIX IS NULL OR LTRIM(RTRIM(PREFIX)) = '';

UPDATE dbo.CD45_NHOM_TCV
SET SHORT_PREFIX = N'Nhóm'
WHERE SHORT_PREFIX IS NULL OR LTRIM(RTRIM(SHORT_PREFIX)) = '';
GO

-- 4. Cập nhật Stored Procedure NhomTBH_Get_By_Page
CREATE OR ALTER PROCEDURE [dbo].[NhomTBH_Get_By_Page] 
	@Keyword NVARCHAR(250) = null,
	@OrderByName VARCHAR(100),
	@Page int = 1,
	@PageSize int = 10
AS
BEGIN
	SET NOCOUNT ON;
	SELECT ace.*,
	       c.Name as CityName,
	       count(ace.manhom_tbh) over() as TotalRow 
	FROM [dbo].[BVTL_NHOM_TBH] ace
	INNER JOIN [dbo].[BVTL_CITES] c ON c.Code = ace.city_code
	WHERE (@Keyword IS NULL OR (@Keyword IS NOT NULL AND (
	       ace.manhom_tbh LIKE '%'+@Keyword+'%' 
	       OR ace.tennhom_tbh LIKE N'%'+@Keyword+'%' 
	       OR ace.PREFIX LIKE N'%'+@Keyword+'%'
	       OR ace.SHORT_PREFIX LIKE N'%'+@Keyword+'%'
	       OR c.Name LIKE N'%'+@Keyword+'%')))
	ORDER BY CASE @OrderByName
		WHEN 'manhom_tbh' THEN ace.manhom_tbh
		WHEN 'tennhom_tbh' THEN ace.tennhom_tbh
		WHEN 'city_code' THEN ace.city_code
		WHEN 'PREFIX' THEN ace.PREFIX
		ELSE ace.manhom_tbh
		END 
	OFFSET ((@Page -1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- 5. Stored Procedure cập nhật Prefix cho Nhóm CBO
CREATE OR ALTER PROCEDURE [dbo].[SP_CD45_UpdateNhomPrefix]
    @MaNhom VARCHAR(20),
    @Prefix NVARCHAR(50),
    @ShortPrefix NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @Prefix = ISNULL(NULLIF(LTRIM(RTRIM(@Prefix)), ''), N'Nhóm');
    SET @ShortPrefix = ISNULL(NULLIF(LTRIM(RTRIM(@ShortPrefix)), ''), @Prefix);
    
    UPDATE dbo.BVTL_NHOM_TBH
    SET PREFIX = @Prefix,
        SHORT_PREFIX = @ShortPrefix
    WHERE manhom_tbh = @MaNhom OR manhom_tbh_map = @MaNhom;

    UPDATE dbo.CD45_NHOM_TCV
    SET PREFIX = @Prefix,
        SHORT_PREFIX = @ShortPrefix
    WHERE MA_NHOM = @MaNhom;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
