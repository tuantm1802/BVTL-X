-- =============================================
-- Author: BVTL Team
-- Create date: 2026-10-05
-- Description: Cập nhật Stored Procedure User_Get_By_Page
-- Hỗ trợ phân trang, tìm kiếm từ khóa, và lọc tài khoản đã xóa mềm (@IsDeleted)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[User_Get_By_Page] 
	@Keyword NVARCHAR(250) = NULL,
	@OrderByName VARCHAR(100) = 'UserName',
	@Page INT = 1,
	@PageSize INT = 10,
	@IsDeleted BIT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT u.*,
	       q.Name AS RoleName,
	       COUNT(u.ID) OVER() AS TotalRow 
	FROM [dbo].[BVTL_QT_NGUOI_DUNG] u
	INNER JOIN [dbo].[BVTL_QT_QUYEN] q ON u.GroupID = q.ID
	WHERE (u.IsAdmin = 0 OR u.IsAdmin IS NULL)
	  AND (
	      (@IsDeleted = 1 AND u.IsActive = 0)
	      OR ((@IsDeleted = 0 OR @IsDeleted IS NULL) AND (u.IsActive = 1 OR u.IsActive IS NULL))
	  )
	  AND (
	      @Keyword IS NULL 
	      OR @Keyword = '' 
	      OR (
	          u.UserName LIKE '%' + @Keyword + '%' 
	          OR u.Name LIKE N'%' + @Keyword + '%' 
	          OR u.Phone LIKE N'%' + @Keyword + '%'
	          OR u.IdNumber LIKE '%' + @Keyword + '%'
	          OR q.Name LIKE N'%' + @Keyword + '%'
	      )
	  )
	ORDER BY CASE @OrderByName
		WHEN 'UserName' THEN u.UserName
		WHEN 'Name' THEN u.Name
		WHEN 'Phone' THEN u.Phone
		ELSE u.UserName
		END 
	OFFSET ((@Page - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;
END
