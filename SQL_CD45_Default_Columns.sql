-- ============================================================================
-- Cập nhật bảng CD45_BCTIEU_CAU_HINH: Thêm cấu hình Mặc định (Default)
-- ============================================================================

-- 1. Thêm các cột Default_* nếu chưa có
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD45_BCTIEU_CAU_HINH' AND COLUMN_NAME = 'Default_Thang')
BEGIN
    ALTER TABLE dbo.CD45_BCTIEU_CAU_HINH ADD Default_Thang BIT NOT NULL CONSTRAINT DF_CD45_BCTIEU_Default_Thang DEFAULT 0;
END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD45_BCTIEU_CAU_HINH' AND COLUMN_NAME = 'Default_Quy')
BEGIN
    ALTER TABLE dbo.CD45_BCTIEU_CAU_HINH ADD Default_Quy BIT NOT NULL CONSTRAINT DF_CD45_BCTIEU_Default_Quy DEFAULT 1;
END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD45_BCTIEU_CAU_HINH' AND COLUMN_NAME = 'Default_6T')
BEGIN
    ALTER TABLE dbo.CD45_BCTIEU_CAU_HINH ADD Default_6T BIT NOT NULL CONSTRAINT DF_CD45_BCTIEU_Default_6T DEFAULT 1;
END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD45_BCTIEU_CAU_HINH' AND COLUMN_NAME = 'Default_12T')
BEGIN
    ALTER TABLE dbo.CD45_BCTIEU_CAU_HINH ADD Default_12T BIT NOT NULL CONSTRAINT DF_CD45_BCTIEU_Default_12T DEFAULT 1;
END

-- 2. Cập nhật dữ liệu mặc định theo chuẩn ma trận Excel
-- Section Header: Tất cả = 0
UPDATE dbo.CD45_BCTIEU_CAU_HINH
SET Default_Thang = 0, Default_Quy = 0, Default_6T = 0, Default_12T = 0
WHERE IsSection = 1;

-- Chỉ tiêu con: Quý, 6T, 12T chọn toàn bộ (= 1)
-- Tháng: Chọn toàn bộ (= 1) trừ 5 chỉ tiêu: I_1, I_3, IV_2_1, IV_2_2, IV_2_3
UPDATE dbo.CD45_BCTIEU_CAU_HINH
SET Default_Quy = 1,
    Default_6T = 1,
    Default_12T = 1,
    Default_Thang = CASE 
        WHEN ChiTieuCode IN ('I_1', 'I_3', 'IV_2_1', 'IV_2_2', 'IV_2_3') THEN 0 
        ELSE 1 
    END
WHERE IsSection = 0;

-- Đồng thời reset lại các giá trị hiển thị hiện tại về đúng chuẩn mặc định
UPDATE dbo.CD45_BCTIEU_CAU_HINH
SET HienThi_Thang = Default_Thang,
    HienThi_Quy   = Default_Quy,
    HienThi_6T    = Default_6T,
    HienThi_12T   = Default_12T
WHERE IsSection = 0;

-- 3. Tạo/Cập nhật Stored Procedure khôi phục cấu hình mặc định
IF OBJECT_ID('dbo.SP_CD45_ResetCauHinhChiTieuMacDinh', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_CD45_ResetCauHinhChiTieuMacDinh;
