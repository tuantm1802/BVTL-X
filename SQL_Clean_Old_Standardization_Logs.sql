-- =========================================================================================
-- SCRIPT DỌN DẸP BẢN GHI CHUẨN HÓA THÔNG THƯỜNG (INFO) ĐÃ CŨ TRONG BẢNG LOG
-- Dự án: BVTL-X / CD45 (DREAMH)
-- Mục đích: Giải phóng dung lượng CSDL và loại bỏ các dòng R3_DAG_NORMALIZED lặp lại
-- =========================================================================================

-- 1. Thống kê trước khi dọn dẹp
SELECT 
    RULE_CODE, 
    SEVERITY, 
    COUNT(1) AS SoLuongBanGhi,
    MIN(CREATED_DATE) AS TuNgay,
    MAX(CREATED_DATE) AS DenNgay
FROM BVTL_DATA_STANDARDIZATION_LOG
GROUP BY RULE_CODE, SEVERITY
ORDER BY SoLuongBanGhi DESC;

-- 2. Xóa các log INFO chuẩn hóa DAG lặp lại cũ hơn 7 ngày
DELETE FROM BVTL_DATA_STANDARDIZATION_LOG
WHERE RULE_CODE = 'R3_DAG_NORMALIZED'
  AND SEVERITY = 'INFO'
  AND CREATED_DATE < DATEADD(DAY, -7, GETDATE());

-- 3. Xóa các log INFO viết hoa mã KH cũ hơn 7 ngày
DELETE FROM BVTL_DATA_STANDARDIZATION_LOG
WHERE RULE_CODE = 'R1_RECORD_ID_AUTO_UPPER'
  AND SEVERITY = 'INFO'
  AND CREATED_DATE < DATEADD(DAY, -7, GETDATE());

-- 4. Thống kê lại sau khi dọn dẹp
SELECT 
    RULE_CODE, 
    SEVERITY, 
    COUNT(1) AS SoLuongBanGhi,
    MIN(CREATED_DATE) AS TuNgay,
    MAX(CREATED_DATE) AS DenNgay
FROM BVTL_DATA_STANDARDIZATION_LOG
GROUP BY RULE_CODE, SEVERITY
ORDER BY SoLuongBanGhi DESC;
