-- =============================================================================
-- Script: SQL_CD45_Fix_TCV_Names.sql
-- Mô tả: Chuẩn hóa lại 21 Tên Tiếp cận viên (TCV) trong bảng CD45_NHOM_TCV
-- Ngày tạo: 2026-09-14
-- Encoding: UTF-8 with BOM
-- =============================================================================

BEGIN TRANSACTION;

-- 1. Bùi Văn Bằng bị nhầm thành Bùi Văn Bắng (Nhóm Trăng Khuyết 35 - tk, TCV 2)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Bùi Văn Bằng'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'tk' AND MA_TCV = '2';

-- 2. Hà Quang Toản bị nhầm thành Hà Quang Toàn (Nhóm Gió Mới - gm, TCV 4)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Hà Quang Toản'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'gm' AND MA_TCV = '4';

-- 3. Nguyễn Hải Hưng bị nhầm thành Nguyễn Hải Hùng (Nhóm Gió Mới - gm, TCV 1)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Nguyễn Hải Hưng'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'gm' AND MA_TCV = '1';

-- 4. Nguyễn Chí Hiệu bị nhầm thành Nguyễn Chí Hiếu (Nhóm Cát Trắng - ct, TCV 4)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Nguyễn Chí Hiệu'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'ct' AND MA_TCV = '4';

-- 5. Nguyễn Văn Đỉnh bị nhầm thành Nguyễn Văn Định (Nhóm Cát Trắng - ct, TCV 1)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Nguyễn Văn Đỉnh'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'ct' AND MA_TCV = '1';

-- 6. Trương Thị Cúc bị nhầm thành Trương Thị Các (Nhóm Hải Đăng - hd, TCV 2)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Trương Thị Cúc'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'hd' AND MA_TCV = '2';

-- 7. Phạm Thị Thu Hồng bị nhầm thành Phạm Thị Thu Hằng (Nhóm Hải Đăng - hd, TCV 3)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Phạm Thị Thu Hồng'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'hd' AND MA_TCV = '3';

-- 8. Lê Viết Thử bị nhầm thành Lê Việt Thủy (Nhóm Hoa Sen - hs, TCV 3)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Lê Viết Thử'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'hs' AND MA_TCV = '3';

-- 9. Nguyễn Phương Thảo bị nhầm thành Nguyễn Phùng Thảo (Nhóm Hoa Trinh Nữ - htn, TCV 6)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Nguyễn Phương Thảo'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'htn' AND MA_TCV = '6';

-- 10. Phạm Nguyễn Thùy Dương bị nhầm thành Phạm Nguyễn Thúy Dung (Nhóm Its T time - itt, TCV 5)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Phạm Nguyễn Thùy Dương'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'itt' AND MA_TCV = '5';

-- 11. Đặng Thế Thanh bị nhầm thành Đặng Thị Thanh (Nhóm Liên Minh Hạnh Phúc - lmhp, TCV 13)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Đặng Thế Thanh'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'lmhp' AND MA_TCV = '13';

-- 12. Lộc Văn Hai bị nhầm thành Lạc Văn Hai (Nhóm Sao Va - sv, TCV 2)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Lộc Văn Hai'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'sv' AND MA_TCV = '2';

-- 13. Lô Văn Nhất bị nhầm thành Lê Văn Nhật (Nhóm Sao Va - sv, TCV 1)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Lô Văn Nhất'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'sv' AND MA_TCV = '1';

-- 14. Lữ Văn Hiệp bị nhầm thành Lỡ Văn Hiệp (Nhóm Sao Va - sv, TCV 5)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Lữ Văn Hiệp'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'sv' AND MA_TCV = '5';

-- 15. Trình Thị Hoà bị nhầm thành Trịnh Thị Hoà (Nhóm Về nhà - vn, TCV 6)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Trình Thị Hoà'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'vn' AND MA_TCV = '6';

-- 16. Lê Thị Hằng bị nhầm thành Lê Thị Hồng (Nhóm Bình Minh - bm, TCV 2)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Lê Thị Hằng'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'bm' AND MA_TCV = '2';

-- 17. Phạm Thị Hưng bị nhầm thành Phạm Thị Hằng (Nhóm Bình Minh - bm, TCV 5)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Phạm Thị Hưng'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'bm' AND MA_TCV = '5';

-- 18. Nguyễn Thị Hằng bị nhầm thành Nguyễn Thị Hồng (Nhóm Quỳnh Hương Xanh - qhx, TCV 3)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Nguyễn Thị Hằng'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'qhx' AND MA_TCV = '3';

-- 19. Nguyễn Thị Lý bị nhầm thành Nguyễn Thị Lệ (Nhóm Quỳnh Hương Xanh - qhx, TCV 4)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Nguyễn Thị Lý'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'qhx' AND MA_TCV = '4';

-- 20. Phạm Thị Hương bị nhầm thành Phạm Thị Hằng (Nhóm Quỳnh Hương Xanh - qhx, TCV 5)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Phạm Thị Hương'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'qhx' AND MA_TCV = '5';

-- 21. Sầm Thái Phương bị nhầm thành Sầm Thái Phùng (Nhóm Sao Va - sv, TCV 4)
UPDATE CD45_NHOM_TCV
SET TEN_TCV = N'Sầm Thái Phương'
WHERE MADUAN = 'CD45' AND MA_NHOM = 'sv' AND MA_TCV = '4';

COMMIT TRANSACTION;
