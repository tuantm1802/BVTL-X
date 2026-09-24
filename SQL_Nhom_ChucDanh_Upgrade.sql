-- =========================================================================================
-- SCRIPT NÂNG CẤP CHỨC DANH NGƯỜI KÝ ĐẠI DIỆN CHO CÁC NHÓM CBO (CHUC_DANH)
-- Dự án: Bảo Vệ Tương Lai (BVTL) & CD45 (DREAMH)
-- =========================================================================================

-- 1. Bổ sung cột CHUC_DANH vào bảng BVTL_NHOM_TBH
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BVTL_NHOM_TBH' AND COLUMN_NAME = 'CHUC_DANH')
BEGIN
    ALTER TABLE dbo.BVTL_NHOM_TBH ADD CHUC_DANH NVARCHAR(50) CONSTRAINT DF_BVTL_NHOM_TBH_CHUC_DANH DEFAULT N'Trưởng nhóm' WITH VALUES;
    PRINT N'Đã thêm cột CHUC_DANH vào BVTL_NHOM_TBH';
END
GO

-- 2. Bổ sung cột CHUC_DANH vào bảng CD45_NHOM_TCV
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD45_NHOM_TCV' AND COLUMN_NAME = 'CHUC_DANH')
BEGIN
    ALTER TABLE dbo.CD45_NHOM_TCV ADD CHUC_DANH NVARCHAR(50) CONSTRAINT DF_CD45_NHOM_TCV_CHUC_DANH DEFAULT N'Trưởng nhóm' WITH VALUES;
    PRINT N'Đã thêm cột CHUC_DANH vào CD45_NHOM_TCV';
END
GO

-- 3. Cập nhật dữ liệu mặc định cho các nhóm TP.HCM và nhóm "The Time" (Chức danh: "Giám đốc")
UPDATE dbo.BVTL_NHOM_TBH
SET CHUC_DANH = N'Giám đốc'
WHERE city_code = 'HCM' 
   OR manhom_tbh IN ('HC_ALO', 'HC_G3V', 'HC_MYH', 'HC_TGA', 'HCM10', 'HCM15', 'HCM151', 'HCM19', 'HN_TT')
   OR manhom_tbh_map IN ('alo', 'g3vn', 'myh', 'tg', 'HC03', 'tt')
   OR tennhom_tbh LIKE '%Time%';
PRINT N'Đã cập nhật Chức danh "Giám đốc" cho các nhóm TP.HCM và nhóm The Time trong BVTL_NHOM_TBH';
GO

UPDATE dbo.CD45_NHOM_TCV
SET CHUC_DANH = N'Giám đốc'
WHERE CITY_CODE = 'HCM' 
   OR MA_NHOM IN ('alo', 'g3vn', 'myh', 'tg', 'HN_TT', 'tt')
   OR TEN_NHOM LIKE '%Time%';
PRINT N'Đã cập nhật Chức danh "Giám đốc" cho các nhóm TP.HCM và nhóm The Time trong CD45_NHOM_TCV';
GO

-- Đảm bảo các nhóm còn lại có giá trị mặc định là 'Trưởng nhóm' nếu NULL hoặc rỗng
UPDATE dbo.BVTL_NHOM_TBH
SET CHUC_DANH = N'Trưởng nhóm'
WHERE CHUC_DANH IS NULL OR LTRIM(RTRIM(CHUC_DANH)) = '';

UPDATE dbo.CD45_NHOM_TCV
SET CHUC_DANH = N'Trưởng nhóm'
WHERE CHUC_DANH IS NULL OR LTRIM(RTRIM(CHUC_DANH)) = '';
GO

-- 4. Cập nhật Stored Procedure SP_CD45_UpdateNhomPrefix để hỗ trợ lưu thêm Chức danh
CREATE OR ALTER PROCEDURE [dbo].[SP_CD45_UpdateNhomPrefix]
    @MaNhom VARCHAR(20),
    @Prefix NVARCHAR(50),
    @ShortPrefix NVARCHAR(20),
    @ChucDanh NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @Prefix = ISNULL(NULLIF(LTRIM(RTRIM(@Prefix)), ''), N'Nhóm');
    SET @ShortPrefix = ISNULL(NULLIF(LTRIM(RTRIM(@ShortPrefix)), ''), @Prefix);
    SET @ChucDanh = ISNULL(NULLIF(LTRIM(RTRIM(@ChucDanh)), ''), N'Trưởng nhóm');
    
    UPDATE dbo.BVTL_NHOM_TBH
    SET PREFIX = @Prefix,
        SHORT_PREFIX = @ShortPrefix,
        CHUC_DANH = @ChucDanh
    WHERE manhom_tbh = @MaNhom OR manhom_tbh_map = @MaNhom;

    UPDATE dbo.CD45_NHOM_TCV
    SET PREFIX = @Prefix,
        SHORT_PREFIX = @ShortPrefix,
        CHUC_DANH = @ChucDanh
    WHERE MA_NHOM = @MaNhom;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
