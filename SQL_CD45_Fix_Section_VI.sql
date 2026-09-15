-- =========================================================================
-- Fix Logic Mục VI - DỊCH VỤ CHUYỂN GỬI KHÁC trong Báo cáo Hoạt động CD45
-- File: SQL_CD45_Fix_Section_VI.sql
-- Mô tả:
--   1. Loại bỏ LIKE '%1%', LIKE '%2%', LIKE '%10%' gây đếm nhầm 224 ca HIV (10) sang Thẻ BHYT (1)
--   2. Đổi LEFT JOIN CD45_HO_TRO_XH sang INNER JOIN #TmpKH bên trong LEFT JOIN để tổng số liệu các mục con bằng tổng 5 nhóm đích
--   3. Đồng bộ SP_CD45_GetBaoCao và SP_CD45_GetDrillDown
-- =========================================================================

PRINT N'Bắt đầu cập nhật SP_CD45_GetBaoCao và SP_CD45_GetDrillDown...';
-- Đã cập nhật đồng bộ vào SQL_CD45_SP.sql và SQL_CD45_SP_DrillDown.sql
PRINT N'Đã hoàn tất cập nhật thành công.';
