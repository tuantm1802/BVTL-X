-- =============================================================================
-- STORED PROCEDURE: SP_CD45_Scan_Duplicate_Clients
-- MỤC ĐÍCH: VR-02(2) [WARNING] Quét phát hiện trùng lặp hồ sơ đa trường (Composite Demographic Matching)
--          hoặc trùng Mã nghiên cứu giữa các Record ID khác nhau của dự án CD45.
-- =============================================================================
CREATE OR ALTER PROCEDURE SP_CD45_Scan_Duplicate_Clients
    @MaDuAn VARCHAR(50) = 'CD45'
AS
BEGIN
    SET NOCOUNT ON;

    -- Tìm các cặp khách hàng trùng lặp nhân khẩu học hoặc trùng mã nghiên cứu
    ;WITH CTE_Duplicates AS (
        SELECT 
            a.RECORD_ID AS RecordId1,
            b.RECORD_ID AS RecordId2,
            a.CITY_CODE,
            a.MA_NHOM,
            a.NGAY_THAM_GIA,
            a.NAM_SINH,
            a.GIOI_TINH_TU_XD,
            a.DOI_TUONG,
            CASE 
                WHEN a.MA_KH_NGHIEN_CUU IS NOT NULL AND a.MA_KH_NGHIEN_CUU <> '' AND a.MA_KH_NGHIEN_CUU = b.MA_KH_NGHIEN_CUU 
                     THEN N'Trùng Mã KH Nghiên cứu (' + a.MA_KH_NGHIEN_CUU + N')'
                ELSE N'Trùng khớp toàn bộ nhân khẩu học: Nhóm ' + ISNULL(a.MA_NHOM, '') + N', Ngày TG ' + CONVERT(VARCHAR(10), a.NGAY_THAM_GIA, 103) + N', Năm sinh ' + CAST(ISNULL(a.NAM_SINH, 0) AS VARCHAR)
            END AS LyDoTrung
        FROM CD45_KH a
        INNER JOIN CD45_KH b ON a.RECORD_ID < b.RECORD_ID AND a.MADUAN = b.MADUAN
        WHERE a.MADUAN = @MaDuAn
          AND (
              -- Trùng mã nghiên cứu (nếu có)
              (a.MA_KH_NGHIEN_CUU IS NOT NULL AND a.MA_KH_NGHIEN_CUU <> '' AND a.MA_KH_NGHIEN_CUU = b.MA_KH_NGHIEN_CUU)
              OR
              -- Trùng tổ hợp nhân khẩu học
              (
                  ISNULL(a.MA_NHOM, '') = ISNULL(b.MA_NHOM, '')
                  AND a.NGAY_THAM_GIA = b.NGAY_THAM_GIA
                  AND a.NAM_SINH = b.NAM_SINH
                  AND a.GIOI_TINH_TU_XD = b.GIOI_TINH_TU_XD
                  AND a.DOI_TUONG = b.DOI_TUONG
                  AND ISNULL(a.HON_NHAN, 0) = ISNULL(b.HON_NHAN, 0)
                  AND ISNULL(a.SO_CON, 0) = ISNULL(b.SO_CON, 0)
              )
          )
    )
    -- Ghi nhận cảnh báo nếu chưa được ghi
    INSERT INTO BVTL_DATA_STANDARDIZATION_LOG (
        MADUAN, REPORT_ID, API_CODE, TABLE_NAME, RECORD_ID, FIELD_NAME,
        OLD_VALUE, NEW_VALUE, RULE_CODE, SEVERITY, ACTION_TAKEN, MESSAGE,
        CREATED_DATE, IS_RESOLVED
    )
    SELECT 
        @MaDuAn, 'MANUAL_SCAN', 'CD45_KH_SCAN', 'CD45_KH', d.RecordId1, 'RECORD_ID',
        d.RecordId1, d.RecordId2, 'WARN_POTENTIAL_DUPLICATE_CLIENT', 'WARNING', 'FLAGGED_FOR_ADMIN',
        N'Nghi vấn trùng lặp hồ sơ giữa 2 Record ID: [' + d.RecordId1 + N'] và [' + d.RecordId2 + N']. ' + d.LyDoTrung + N'. Đề nghị CBO và M&E rà soát.',
        GETDATE(), 0
    FROM CTE_Duplicates d
    WHERE NOT EXISTS (
        SELECT 1 FROM BVTL_DATA_STANDARDIZATION_LOG log
        WHERE log.MADUAN = @MaDuAn
          AND log.RULE_CODE = 'WARN_POTENTIAL_DUPLICATE_CLIENT'
          AND log.RECORD_ID = d.RecordId1
          AND log.NEW_VALUE = d.RecordId2
    );

    SELECT @@ROWCOUNT AS NewWarningsCreated;
END;
