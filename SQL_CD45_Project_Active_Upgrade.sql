-- ==============================================================================
-- CẬP NHẬT TRẠNG THÁI ACTIVE CHO DỰ ÁN QUẢN LÝ (CHỈ ACTIVE CD45 - DREAMH)
-- ==============================================================================

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'BVTL_DU_AN' AND COLUMN_NAME = 'IsActive')
BEGIN
    ALTER TABLE BVTL_DU_AN ADD IsActive BIT NOT NULL DEFAULT 1;
END
GO

-- Kích hoạt CD45, vô hiệu hóa các dự án đã dừng hoạt động
UPDATE BVTL_DU_AN SET IsActive = 1 WHERE maduan = 'CD45';
UPDATE BVTL_DU_AN SET IsActive = 0 WHERE maduan != 'CD45';
GO
