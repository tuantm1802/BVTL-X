-- ============================================================================
-- SCRIPT: Thêm Menu "Cấu hình chỉ tiêu CD45" vào Menu "Hệ thống"
-- ============================================================================

DECLARE @ParentID INT;
SELECT TOP 1 @ParentID = [ID] 
FROM [dbo].[BVTL_QT_PAGE_MENU] 
WHERE [ID] = 1 OR [NAME] = N'Hệ thống';

IF @ParentID IS NULL
BEGIN
    SET @ParentID = 1;
END

-- 1. Thêm mới hoặc cập nhật thông tin Menu trong BVTL_QT_PAGE_MENU
DECLARE @PageID INT;
SELECT @PageID = [ID] 
FROM [dbo].[BVTL_QT_PAGE_MENU] 
WHERE [HREF_URL] = '/BaoCaoCD45/CauHinhChiTieu';

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
        N'Cấu hình chỉ tiêu CD45',
        N'Cấu hình danh mục chỉ tiêu hiển thị theo kỳ báo cáo dự án CD45',
        1,
        5, -- Hiển thị sau Đồng bộ dữ liệu (order 4)
        'BaoCaoCD45',
        '/BaoCaoCD45/CauHinhChiTieu',
        @ParentID,
        1,
        N'CD45',
        NULL
    );

    SET @PageID = SCOPE_IDENTITY();
    PRINT N'Đã thêm mới menu Cấu hình chỉ tiêu CD45, PageID = ' + CAST(@PageID AS NVARCHAR(20));
END
ELSE
BEGIN
    UPDATE [dbo].[BVTL_QT_PAGE_MENU]
    SET 
        [NAME] = N'Cấu hình chỉ tiêu CD45',
        [DESCRIPTION] = N'Cấu hình danh mục chỉ tiêu hiển thị theo kỳ báo cáo dự án CD45',
        [IS_ACTIVE] = 1,
        [ORDER_BY] = 5,
        [CONTROLLER_NAME] = 'BaoCaoCD45',
        [HREF_URL] = '/BaoCaoCD45/CauHinhChiTieu',
        [PARENT_PAGE_ID] = @ParentID,
        [IS_SYSTEM_ROLE] = 1,
        [TEN_DU_AN] = N'CD45',
        [TITLE_GROUP_MENU] = NULL
    WHERE [ID] = @PageID;

    PRINT N'Đã cập nhật menu Cấu hình chỉ tiêu CD45, PageID = ' + CAST(@PageID AS NVARCHAR(20));
END

-- 2. Phân quyền truy cập menu trong BVTL_QT_QUYEN_PAGE
-- Cấp quyền cho vai trò Quản trị (QT)
IF NOT EXISTS (SELECT 1 FROM [dbo].[BVTL_QT_QUYEN_PAGE] WHERE [RoleID] = 'QT' AND [PageID] = @PageID)
BEGIN
    INSERT INTO [dbo].[BVTL_QT_QUYEN_PAGE] ([RoleID], [PageID], [IS_ACTIVE], [CONTROL_STRING])
    VALUES ('QT', @PageID, 1, '');
    PRINT N'Đã cấp quyền truy cập cho vai trò QT';
END
ELSE
BEGIN
    UPDATE [dbo].[BVTL_QT_QUYEN_PAGE]
    SET [IS_ACTIVE] = 1
    WHERE [RoleID] = 'QT' AND [PageID] = @PageID;
    PRINT N'Đã kích hoạt quyền truy cập cho vai trò QT';
END

-- Cấp quyền cho các vai trò khác trong hệ thống để quản trị viên các cấp có thể cấu hình
INSERT INTO [dbo].[BVTL_QT_QUYEN_PAGE] ([RoleID], [PageID], [IS_ACTIVE], [CONTROL_STRING])
SELECT r.[ID], @PageID, 1, ''
FROM [dbo].[BVTL_QT_QUYEN] r
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[BVTL_QT_QUYEN_PAGE] qp 
    WHERE qp.[RoleID] = r.[ID] AND qp.[PageID] = @PageID
);

-- 3. Truy vấn kiểm tra lại menu Hệ thống
SELECT pm.ID, pm.NAME, pm.CONTROLLER_NAME, pm.HREF_URL, pm.PARENT_PAGE_ID, pm.ORDER_BY, pm.IS_ACTIVE
FROM [dbo].[BVTL_QT_PAGE_MENU] pm
WHERE pm.PARENT_PAGE_ID = @ParentID
ORDER BY pm.ORDER_BY;
