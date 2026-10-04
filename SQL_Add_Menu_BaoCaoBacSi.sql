-- ============================================================================
-- SCRIPT: Thêm Menu "Báo cáo Bác sĩ CD45" vào Menu "Báo cáo"
-- ============================================================================

DECLARE @ParentID INT;
SELECT TOP 1 @ParentID = [ID] 
FROM [dbo].[BVTL_QT_PAGE_MENU] 
WHERE [ID] = 22 OR [NAME] = N'Báo cáo';

IF @ParentID IS NULL
BEGIN
    SET @ParentID = 22;
END

-- 1. Thêm mới hoặc cập nhật thông tin Menu trong BVTL_QT_PAGE_MENU
DECLARE @PageID INT;
SELECT @PageID = [ID] 
FROM [dbo].[BVTL_QT_PAGE_MENU] 
WHERE [HREF_URL] = '/BaoCaoBacSiCD45/Index';

IF @PageID IS NULL
BEGIN
    INSERT INTO [dbo].[BVTL_QT_PAGE_MENU] (
        [NAME],
        [DESCRIPTION],
        [IS_ACTIVE],
        [ORDER_BY],
        [CONTROLLER_NAME],
        [HREF_URL],
        [PARENT_PAGE_ID],
        [IS_SYSTEM_ROLE],
        [TEN_DU_AN],
        [TITLE_GROUP_MENU]
    )
    VALUES (
        N'Báo cáo Bác sĩ CD45',
        N'Báo cáo thanh toán chuyên môn Bác sĩ dự án CD45',
        1,
        32,
        'BaoCaoBacSiCD45',
        '/BaoCaoBacSiCD45/Index',
        @ParentID,
        0,
        N'CD45',
        N'Dự án CD45 (DREAMH)'
    );

    SET @PageID = SCOPE_IDENTITY();
    PRINT N'Đã thêm mới menu Báo cáo Bác sĩ CD45, PageID = ' + CAST(@PageID AS NVARCHAR(20));
END
ELSE
BEGIN
    UPDATE [dbo].[BVTL_QT_PAGE_MENU]
    SET 
        [NAME] = N'Báo cáo Bác sĩ CD45',
        [DESCRIPTION] = N'Báo cáo thanh toán chuyên môn Bác sĩ dự án CD45',
        [IS_ACTIVE] = 1,
        [ORDER_BY] = 32,
        [CONTROLLER_NAME] = 'BaoCaoBacSiCD45',
        [HREF_URL] = '/BaoCaoBacSiCD45/Index',
        [PARENT_PAGE_ID] = @ParentID,
        [IS_SYSTEM_ROLE] = 0,
        [TEN_DU_AN] = N'CD45',
        [TITLE_GROUP_MENU] = N'Dự án CD45 (DREAMH)'
    WHERE [ID] = @PageID;

    PRINT N'Đã cập nhật menu Báo cáo Bác sĩ CD45, PageID = ' + CAST(@PageID AS NVARCHAR(20));
END

-- 2. Phân quyền truy cập menu trong BVTL_QT_QUYEN_PAGE
INSERT INTO [dbo].[BVTL_QT_QUYEN_PAGE] ([RoleID], [PageID], [IS_ACTIVE], [CONTROL_STRING])
SELECT r.[ID], @PageID, 1, ''
FROM [dbo].[BVTL_QT_QUYEN] r
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[BVTL_QT_QUYEN_PAGE] qp 
    WHERE qp.[RoleID] = r.[ID] AND qp.[PageID] = @PageID
);

-- Kích hoạt nếu đã tồn tại nhưng IS_ACTIVE = 0
UPDATE [dbo].[BVTL_QT_QUYEN_PAGE]
SET [IS_ACTIVE] = 1
WHERE [PageID] = @PageID;

-- 3. Kiểm tra kết quả
SELECT pm.ID, pm.NAME, pm.CONTROLLER_NAME, pm.HREF_URL, pm.PARENT_PAGE_ID, pm.ORDER_BY, pm.IS_ACTIVE, pm.TITLE_GROUP_MENU
FROM [dbo].[BVTL_QT_PAGE_MENU] pm
WHERE pm.HREF_URL = '/BaoCaoBacSiCD45/Index';
