# Hướng dẫn cấu hình Menu Báo cáo Tuần trong Cơ sở Dữ liệu

Để menu **Báo cáo Tuần** xuất hiện trên thanh sidebar điều hướng và phân quyền truy cập cho người dùng, bạn cần thực hiện chạy các câu lệnh SQL dưới đây trong SQL Server Management Studio (SSMS).

---

## Bước 1: Thêm Menu vào bảng `BVTL_QT_PAGE_MENU`

Câu lệnh này khai báo trang báo cáo tuần vào danh mục quản trị trang của hệ thống:

```sql
-- 1. Tìm PARENT_PAGE_ID của phân hệ báo cáo (nếu có)
DECLARE @ParentID INT;
SELECT TOP 1 @ParentID = PARENT_PAGE_ID 
FROM [dbo].[BVTL_QT_PAGE_MENU] 
WHERE [CONTROLLER_NAME] = 'BaoCaoThang';

-- 2. Chèn thông tin menu Báo cáo Tuần
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
    N'Báo cáo tuần', 
    N'Báo cáo hoạt động theo tuần', 
    1, 
    2, -- Vị trí hiển thị (sắp xếp sau báo cáo tháng)
    'BaoCaoTuan', 
    '/BaoCaoTuan/Index', 
    @ParentID, 
    0, 
    N'Dự án sàng lọc y tế', -- Tên dự án tương ứng nhóm menu (Hoặc 'BVTL')
    N'Báo cáo' -- Nhóm hiển thị
);
```

---

## Bước 2: Phân quyền truy cập cho vai trò Quản trị (Admin)

Sau khi thêm trang, bạn cần cấp quyền cho các nhóm người dùng (ví dụ: nhóm quản trị viên - Admin) để họ có quyền xem và thao tác trên trang này thông qua bảng `BVTL_QT_QUYEN_PAGE`:

```sql
-- 1. Lấy ID của trang Báo cáo tuần vừa tạo
DECLARE @PageID INT;
SELECT @PageID = [ID] 
FROM [dbo].[BVTL_QT_PAGE_MENU] 
WHERE [CONTROLLER_NAME] = 'BaoCaoTuan';

-- 2. Phân quyền cho tất cả các vai trò hoặc vai trò cụ thể (Admin)
-- Ở đây ví dụ phân quyền cho tất cả Quyền (Role) hiện có để kiểm thử nhanh
INSERT INTO [dbo].[BVTL_QT_QUYEN_PAGE] (
    [RoleID], 
    [PageID], 
    [IS_ACTIVE], 
    [CONTROL_STRING]
)
SELECT 
    r.[ID], 
    @PageID, 
    1, 
    'btnSearch' -- Cho phép nút tìm kiếm & xuất Excel (btnSearch)
FROM [dbo].[BVTL_QT_QUYEN] r;
```

---

## Bước 3: Đăng nhập lại để cập nhật Session
Do phân hệ phân quyền được nạp trực tiếp vào Session khi đăng nhập (`Session["Menus"]`), sau khi chạy các lệnh SQL trên:
1. Đăng xuất khỏi hệ thống.
2. Đăng nhập lại để cập nhật danh sách menu mới trên thanh Sidebar.
