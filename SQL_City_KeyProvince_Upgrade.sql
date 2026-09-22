-- ===================================================================
-- SCRIPT NÂNG CẤP STORED PROCEDURE City_Get_By_Page & HỖ TRỢ TỈNH TRỌNG ĐIỂM
-- Hệ thống: BVTL-X / CD45 (DREAMH)
-- ===================================================================

USE [BVTL_REPORTING_DEV];
GO

ALTER PROCEDURE [dbo].[City_Get_By_Page] 
	@Keyword NVARCHAR(250) = null,
	@OrderByName VARCHAR(100) = 'KeyFirst',
	@Page int = 1,
	@PageSize int = 20,
	@IsKeyOnly bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *, count(Code) over() as TotalRow  
    FROM [dbo].[BVTL_CITES]
    WHERE (@Keyword IS NULL OR (Code LIKE '%'+@Keyword+'%' OR [Name] LIKE N'%'+@Keyword+'%' OR Code_Map LIKE '%'+@Keyword+'%'))
      AND (@IsKeyOnly = 0 OR (@IsKeyOnly = 1 AND Code_Map IS NOT NULL AND RTRIM(Code_Map) <> ''))
    ORDER BY 
        CASE 
            WHEN @OrderByName = 'KeyFirst' AND Code_Map IS NOT NULL AND RTRIM(Code_Map) <> '' THEN 0
            WHEN @OrderByName = 'KeyFirst' THEN 1
            ELSE 0 
        END,
        CASE @OrderByName
            WHEN 'Name' THEN [Name]
            ELSE Code
        END ASC
    OFFSET ((@Page - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
