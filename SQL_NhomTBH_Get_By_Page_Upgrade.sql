-- =============================================
-- Author: BVTL Team
-- Create date: 2026-09-30
-- Description: Cập nhật Stored Procedure NhomTBH_Get_By_Page
-- Hỗ trợ phân trang, lọc theo dự án CD45, lọc theo mã tỉnh (@CityCode) và tìm kiếm từ khóa
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[NhomTBH_Get_By_Page] 
	@Keyword NVARCHAR(250) = NULL,
	@OrderByName VARCHAR(100) = 'manhom_tbh',
	@Page INT = 1,
	@PageSize INT = 10,
	@CityCode VARCHAR(100) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ace.*,
	       c.Name AS CityName,
	       COUNT(ace.manhom_tbh) OVER() AS TotalRow 
	FROM [dbo].[BVTL_NHOM_TBH] ace
	INNER JOIN [dbo].[BVTL_CITES] c ON c.Code = ace.city_code
	WHERE (ace.maduan = 'CD45' OR ace.maduan IS NULL)
	  AND (ace.maduan IS NULL OR ace.maduan NOT IN ('CH07'))
	  AND (
	      @CityCode IS NULL 
	      OR @CityCode = '' 
	      OR ace.city_code IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@CityCode, ','))
	  )
	  AND (
	      @Keyword IS NULL 
	      OR @Keyword = '' 
	      OR (
	          ace.manhom_tbh LIKE '%' + @Keyword + '%' 
	          OR ace.tennhom_tbh LIKE N'%' + @Keyword + '%' 
	          OR ace.PREFIX LIKE N'%' + @Keyword + '%'
	          OR ace.SHORT_PREFIX LIKE N'%' + @Keyword + '%'
	          OR c.Name LIKE N'%' + @Keyword + '%'
	      )
	  )
	ORDER BY CASE @OrderByName
		WHEN 'manhom_tbh' THEN ace.manhom_tbh
		WHEN 'tennhom_tbh' THEN ace.tennhom_tbh
		WHEN 'city_code' THEN ace.city_code
		WHEN 'PREFIX' THEN ace.PREFIX
		ELSE ace.manhom_tbh
		END 
	OFFSET ((@Page - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;
END
