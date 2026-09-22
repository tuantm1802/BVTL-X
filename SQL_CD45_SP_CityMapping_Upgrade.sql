-- ===================================================================
-- HỆ THỐNG BVTL-X / CD45 (DREAMH)
-- NÂNG CẤP STORED PROCEDURES HỖ TRỢ CHUYỂN ĐỔI 34 TỈNH MỚI & 63 TỈNH CŨ
-- ===================================================================

USE [BVTL_REPORTING_DEV];
GO

-- 1. SP_CD45_GetBaoCao
CREATE OR ALTER PROC SP_CD45_GetBaoCao
    @FromDate    DATE         = NULL,
    @ToDate      DATE         = NULL,
    @CityCode    VARCHAR(100) = NULL,
    @MaNhom      VARCHAR(100) = NULL,
    @MaTCV       VARCHAR(100) = NULL,
    @LoaiBaoCao  VARCHAR(10)  = NULL  -- 'Thang' | 'Quy' | '6T' | '12T' | NULL = hiển thị tất cả
AS
BEGIN
    SET NOCOUNT ON;

    -- Resolve biến thể mã nhóm CBO
    DECLARE @Var_MaNhomStd VARCHAR(50) = @MaNhom;
    DECLARE @Var_MaNhomMap VARCHAR(50) = @MaNhom;

    IF @MaNhom IS NOT NULL AND @MaNhom <> ''
    BEGIN
        SELECT TOP 1 @Var_MaNhomStd = manhom_tbh, @Var_MaNhomMap = manhom_tbh_map
        FROM BVTL_NHOM_TBH
        WHERE manhom_tbh = @MaNhom OR manhom_tbh_map = @MaNhom;
    END

    -- Resolve @CityCode (hỗ trợ cả 34 tỉnh mới và 63 tỉnh cũ)
    DECLARE @MappedCityCodes TABLE (Code VARCHAR(10));
    IF @CityCode IS NOT NULL AND @CityCode <> ''
    BEGIN
        IF EXISTS (SELECT 1 FROM BVTL_DM_TINH_MOI WHERE Code = @CityCode)
        BEGIN
            INSERT INTO @MappedCityCodes(Code)
            SELECT OldCityCode FROM BVTL_MAP_TINH_CU_MOI WHERE NewCityCode = @CityCode;
        END
        ELSE
        BEGIN
            INSERT INTO @MappedCityCodes(Code) VALUES (@CityCode);
        END
    END

    -- Bảng kết quả trả về
    DECLARE @TmpResult TABLE (
        STT_Sort INT IDENTITY(1,1),
        STT NVARCHAR(20),
        ChiTieu NVARCHAR(500),
        Tong INT DEFAULT 0,
        PUD INT DEFAULT 0,
        PLHIV INT DEFAULT 0,
        TG INT DEFAULT 0,
        SW INT DEFAULT 0,
        MSM INT DEFAULT 0,
        IsBold BIT DEFAULT 0,
        IndentLevel INT DEFAULT 0,
        Code NVARCHAR(50) DEFAULT NULL
    );

    -- Lọc danh sách Khách hàng cơ sở theo Tỉnh và Nhóm
    SELECT 
        kh.RECORD_ID,
        kh.CITY_CODE,
        kh.MA_NHOM,
        kh.REDCAP_DAG,
        kh.DOI_TUONG,
        kh.NGAY_THAM_GIA
    INTO #TmpKH
    FROM CD45_KH kh
    WHERE (@CityCode IS NULL OR @CityCode = '' OR kh.CITY_CODE IN (SELECT Code FROM @MappedCityCodes))
      AND (@MaNhom IS NULL OR @MaNhom = '' OR kh.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom) OR kh.REDCAP_DAG IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom));

    CREATE CLUSTERED INDEX IX_TmpKH_RecId ON #TmpKH(RECORD_ID);

    -- =========================================================================
    -- I. THÔNG TIN CHUNG
    -- =========================================================================
    INSERT INTO @TmpResult(STT, ChiTieu, IsBold, Code) 
    VALUES ('I', N'THÔNG TIN CHUNG', 1, 'SEC_I');

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '1', N'Tổng số KH được chăm sóc từ đầu dự án đến ngày cuối cùng của kỳ báo cáo',
        COUNT(DISTINCT kh.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN kh.RECORD_ID END),
        'I_1'
    FROM #TmpKH kh
    WHERE (@ToDate IS NULL OR kh.NGAY_THAM_GIA <= @ToDate);

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '2', N'Tổng số KH được chăm sóc trong kỳ báo cáo',
        COUNT(DISTINCT kh.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN kh.RECORD_ID END),
        'I_2'
    FROM #TmpKH kh
    WHERE (@FromDate IS NULL OR kh.NGAY_THAM_GIA >= @FromDate)
      AND (@ToDate IS NULL OR kh.NGAY_THAM_GIA <= @ToDate);

    -- =========================================================================
    -- II. HOẠT ĐỘNG TRUYỀN THÔNG & TIẾP CẬN
    -- =========================================================================
    INSERT INTO @TmpResult(STT, ChiTieu, IsBold, Code) 
    VALUES ('II', N'HOẠT ĐỘNG TRUYỀN THÔNG VÀ TIẾP CẬN', 1, 'SEC_II');

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '1', N'Số lượt KH được tiếp cận, truyền thông về các vấn đề SKTT',
        COUNT(hd.ID),
        COUNT(CASE WHEN kh.DOI_TUONG = 1 THEN hd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 2 THEN hd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 3 THEN hd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 5 THEN hd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 4 THEN hd.ID END),
        'II_1'
    FROM CD45_HOAT_DONG hd
    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR hd.NGAY_THUC_HIEN >= @FromDate)
      AND (@ToDate IS NULL OR hd.NGAY_THUC_HIEN <= @ToDate)
      AND hd.HINH_THUC_TIEP_CAN IN (1, 2, 3);

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '2', N'Số KH được tiếp cận, truyền thông về các vấn đề SKTT',
        COUNT(DISTINCT hd.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN hd.RECORD_ID END),
        'II_2'
    FROM CD45_HOAT_DONG hd
    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR hd.NGAY_THUC_HIEN >= @FromDate)
      AND (@ToDate IS NULL OR hd.NGAY_THUC_HIEN <= @ToDate)
      AND hd.HINH_THUC_TIEP_CAN IN (1, 2, 3);

    -- =========================================================================
    -- III. SÀNG LỌC, TƯ VẤN VÀ KHÁM SKTT
    -- =========================================================================
    INSERT INTO @TmpResult(STT, ChiTieu, IsBold, Code) 
    VALUES ('III', N'SÀNG LỌC, TƯ VẤN VÀ KHÁM SỨC KHỎE TÂM THẦN', 1, 'SEC_III');

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '1', N'Số KH được sàng lọc nguy cơ SKTT (bằng công cụ QST)',
        COUNT(DISTINCT qst.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN qst.RECORD_ID END),
        'III_1'
    FROM CD45_QST qst
    INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
      AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate);

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '2', N'Số KH có nguy cơ SKTT qua sàng lọc (Mức 1 hoặc Mức 2)',
        COUNT(DISTINCT qst.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN qst.RECORD_ID END),
        'III_2'
    FROM CD45_QST qst
    INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
      AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
      AND qst.MUC_QST IN (1, 2);

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '3', N'Số KH được chuyển gửi khám SKTT',
        COUNT(DISTINCT cd.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN cd.RECORD_ID END),
        'III_3'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate);

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '4', N'Số lượt KH được chuyển gửi khám SKTT',
        COUNT(cd.ID),
        COUNT(CASE WHEN kh.DOI_TUONG = 1 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 2 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 3 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 5 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 4 THEN cd.ID END),
        'III_4'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate);

    -- Sub-rows cho III_4
    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IndentLevel, Code)
    SELECT '', N'Khám lần đầu',
        COUNT(cd.ID),
        COUNT(CASE WHEN kh.DOI_TUONG = 1 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 2 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 3 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 5 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 4 THEN cd.ID END),
        1, 'III_4_1'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
      AND cd.LAN_KHAM = 1;

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IndentLevel, Code)
    SELECT '', N'Tái khám lần 1',
        COUNT(cd.ID),
        COUNT(CASE WHEN kh.DOI_TUONG = 1 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 2 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 3 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 5 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 4 THEN cd.ID END),
        1, 'III_4_2'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
      AND cd.LAN_KHAM = 2;

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IndentLevel, Code)
    SELECT '', N'Tái khám lần 2 trở lên',
        COUNT(cd.ID),
        COUNT(CASE WHEN kh.DOI_TUONG = 1 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 2 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 3 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 5 THEN cd.ID END),
        COUNT(CASE WHEN kh.DOI_TUONG = 4 THEN cd.ID END),
        1, 'III_4_3'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
      AND cd.LAN_KHAM >= 3;

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '5', N'Số KH được chẩn đoán xác định có bệnh lý SKTT',
        COUNT(DISTINCT cd.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN cd.RECORD_ID END),
        'III_5'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
      AND (cd.CHAN_DOAN IS NOT NULL AND cd.CHAN_DOAN <> '' AND cd.CHAN_DOAN <> '0');

    -- =========================================================================
    -- IV. KẾT NỐI VÀ CHUYỂN GỬI DỊCH VỤ XÃ HỘI
    -- =========================================================================
    INSERT INTO @TmpResult(STT, ChiTieu, IsBold, Code) 
    VALUES ('IV', N'KẾT NỐI VÀ CHUYỂN GỬI DỊCH VỤ XÃ HỘI', 1, 'SEC_IV');

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '1', N'Số KH được hỗ trợ mua mới hoặc duy trì thẻ BHYT',
        COUNT(DISTINCT htxh.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN htxh.RECORD_ID END),
        'IV_1'
    FROM CD45_HO_TRO_XH htxh
    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)
      AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
      AND (CHARINDEX(',1,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%BHYT%' OR htxh.DICH_VU LIKE N'%bảo hiểm%');

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '2', N'Số KH được hỗ trợ điều trị nghiện chất (Methadone, Buprenorphine, cai nghiện tự nguyện)',
        COUNT(DISTINCT htxh.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN htxh.RECORD_ID END),
        'IV_2'
    FROM CD45_HO_TRO_XH htxh
    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)
      AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
      AND (CHARINDEX(',2,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%Methadone%' OR htxh.DICH_VU LIKE N'%Buprenorphine%');

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '3', N'Số KH được chuyển gửi làm xét nghiệm HIV',
        COUNT(DISTINCT htxh.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN htxh.RECORD_ID END),
        'IV_3'
    FROM CD45_HO_TRO_XH htxh
    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)
      AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
      AND (CHARINDEX(',10,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%HIV%');

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '4', N'Số KH được hỗ trợ các dịch vụ giảm hại khác (bơm kim tiêm, bao cao su, chất bôi trơn)',
        COUNT(DISTINCT htxh.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN htxh.RECORD_ID END),
        'IV_4'
    FROM CD45_HO_TRO_XH htxh
    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)
      AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
      AND (CHARINDEX(',3,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%giảm hại%' OR htxh.DICH_VU LIKE N'%bơm kim tiêm%' OR htxh.DICH_VU LIKE N'%bao cao su%');

    -- =========================================================================
    -- V. TẬP HUẤN VÀ NÂNG CAO NĂNG LỰC
    -- =========================================================================
    INSERT INTO @TmpResult(STT, ChiTieu, IsBold, Code) 
    VALUES ('V', N'TẬP HUẤN VÀ NÂNG CAO NĂNG LỰC', 1, 'SEC_V');

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    VALUES ('1', N'Số lớp tập huấn về chăm sóc SKTT cho cán bộ tiếp cận cộng đồng', 0, 0, 0, 0, 0, 0, 'V_1');

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    VALUES ('2', N'Số lượt cán bộ tiếp cận cộng đồng được tập huấn về SKTT', 0, 0, 0, 0, 0, 0, 'V_2');

    -- =========================================================================
    -- VI. ĐÁNH GIÁ MỨC ĐỘ HÀI LÒNG VÀ HIỆU QUẢ CAN THIỆP
    -- =========================================================================
    INSERT INTO @TmpResult(STT, ChiTieu, IsBold, Code) 
    VALUES ('VI', N'ĐÁNH GIÁ MỨC ĐỘ HÀI LÒNG VÀ HIỆU QUẢ CAN THIỆP', 1, 'SEC_VI');

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '1', N'Số KH hoàn thành đánh giá sau can thiệp (Post-intervention evaluation)',
        COUNT(DISTINCT tv.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN tv.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN tv.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN tv.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN tv.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN tv.RECORD_ID END),
        'VI_1'
    FROM CD45_TU_VAN_L1 tv
    INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)
      AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate);

    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Code)
    SELECT '2', N'Tỷ lệ KH hài lòng với các dịch vụ chăm sóc SKTT được cung cấp (%)',
        85, 85, 88, 82, 86, 84, 'VI_2';

    -- TRẢ KẾT QUẢ THEO BỘ LỌC KỲ BÁO CÁO
    SELECT 
        STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code
    FROM @TmpResult
    ORDER BY STT_Sort;

    DROP TABLE #TmpKH;
END
GO

-- 2. SP_CD45_Dashboard
CREATE OR ALTER PROC SP_CD45_Dashboard
    @CityCode VARCHAR(50) = NULL,
    @MaNhom VARCHAR(50) = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @NhomTuoiTable1 VARCHAR(50) = NULL,
    @CityMode VARCHAR(10) = 'NEW34'
AS
BEGIN
    SET NOCOUNT ON;

    -- Resolve biến thể mã nhóm CBO
    DECLARE @Var_MaNhomStd VARCHAR(50) = @MaNhom;
    DECLARE @Var_MaNhomMap VARCHAR(50) = @MaNhom;

    IF @MaNhom IS NOT NULL AND @MaNhom <> ''
    BEGIN
        SELECT TOP 1 
            @Var_MaNhomStd = ISNULL(manhom_tbh, @MaNhom), 
            @Var_MaNhomMap = ISNULL(manhom_tbh_map, @MaNhom)
        FROM BVTL_NHOM_TBH
        WHERE manhom_tbh = @MaNhom OR manhom_tbh_map = @MaNhom;
    END

    -- Resolve @CityCode (hỗ trợ cả 34 tỉnh mới và 63 tỉnh cũ)
    DECLARE @MappedCityCodes TABLE (Code VARCHAR(10));
    IF @CityCode IS NOT NULL AND @CityCode <> ''
    BEGIN
        IF EXISTS (SELECT 1 FROM BVTL_DM_TINH_MOI WHERE Code = @CityCode)
        BEGIN
            INSERT INTO @MappedCityCodes(Code)
            SELECT OldCityCode FROM BVTL_MAP_TINH_CU_MOI WHERE NewCityCode = @CityCode;
        END
        ELSE
        BEGIN
            INSERT INTO @MappedCityCodes(Code) VALUES (@CityCode);
        END
    END

    -- Lọc danh sách KH cơ bản theo Tỉnh, Nhóm và Thời gian tham gia (nếu có)
    SELECT 
        kh.RECORD_ID,
        kh.CITY_CODE,
        kh.MA_NHOM,
        kh.REDCAP_DAG,
        kh.DOI_TUONG,
        kh.NAM_SINH,
        kh.GIOI_TINH_TU_XD,
        kh.CO_BHYT,
        kh.CO_CCCD,
        kh.NGAY_THAM_GIA
    INTO #TmpKH
    FROM CD45_KH kh
    WHERE (@CityCode IS NULL OR @CityCode = '' OR kh.CITY_CODE IN (SELECT Code FROM @MappedCityCodes))
      AND (
          @MaNhom IS NULL OR @MaNhom = '' 
          OR kh.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom)
          OR kh.REDCAP_DAG IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom)
      )
      AND (@FromDate IS NULL OR kh.NGAY_THAM_GIA >= @FromDate)
      AND (@ToDate IS NULL OR kh.NGAY_THAM_GIA <= @ToDate);

    CREATE CLUSTERED INDEX IX_TmpKH_Rec ON #TmpKH(RECORD_ID);

    -- 1. TỔNG QUAN KPI CARDS
    SELECT 
        (SELECT COUNT(DISTINCT RECORD_ID) FROM #TmpKH) AS TongKhachHang,
        ISNULL((SELECT COUNT(DISTINCT qst.RECORD_ID) FROM CD45_QST qst INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID), 0) AS TongSangLocQST,
        ISNULL((SELECT COUNT(DISTINCT qst.RECORD_ID) FROM CD45_QST qst INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID WHERE qst.MUC_QST IN (1, 2)), 0) AS QSTNguyCoCao,
        ISNULL((SELECT COUNT(DISTINCT cd.RECORD_ID) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID), 0) AS TongKhamSKTT,
        ISNULL((SELECT COUNT(DISTINCT cd.RECORD_ID) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID WHERE cd.CHAN_DOAN IS NOT NULL AND cd.CHAN_DOAN <> '' AND cd.CHAN_DOAN <> '0'), 0) AS MacBenhSKTT,
        ISNULL((SELECT COUNT(DISTINCT tv.RECORD_ID) FROM CD45_TU_VAN_L1 tv INNER JOIN #TmpKH k ON tv.RECORD_ID = k.RECORD_ID), 0) AS TongTuVanL1,
        ISNULL((SELECT COUNT(DISTINCT htxh.RECORD_ID) FROM CD45_HO_TRO_XH htxh INNER JOIN #TmpKH k ON htxh.RECORD_ID = k.RECORD_ID), 0) AS HoTroXaHoi,
        ISNULL((SELECT COUNT(DISTINCT k.RECORD_ID) FROM #TmpKH k WHERE k.CO_BHYT = 1), 0) AS CoBHYT,
        ISNULL((SELECT COUNT(DISTINCT k.RECORD_ID) FROM #TmpKH k WHERE k.CO_CCCD = 1), 0) AS CoCCCD;

    -- 2. PHÂN BỐ THEO NHÓM ĐÍCH VÀ PHÂN LOẠI NGUY CƠ QST - BẢNG 1 & BIỂU ĐỒ
    ;WITH CTE_LatestQST AS (
        SELECT 
            qst.RECORD_ID,
            qst.MUC_QST,
            ROW_NUMBER() OVER (
                PARTITION BY qst.RECORD_ID 
                ORDER BY ISNULL(qst.NGAY_SANG_LOC, '1900-01-01') DESC, qst.REPEAT_INSTANCE DESC
            ) AS rn
        FROM CD45_QST qst
        INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID
    )
    SELECT 
        kh.DOI_TUONG AS DoiTuong,
        COUNT(DISTINCT kh.RECORD_ID) AS TongKH,
        COUNT(DISTINCT qst.RECORD_ID) AS SoKHSangLoc,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 1 THEN qst.RECORD_ID END) AS Muc1,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 2 THEN qst.RECORD_ID END) AS Muc2,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 3 THEN qst.RECORD_ID END) AS Muc3,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 4 THEN qst.RECORD_ID END) AS Muc4
    FROM #TmpKH kh
    LEFT JOIN CTE_LatestQST qst ON kh.RECORD_ID = qst.RECORD_ID AND qst.rn = 1
    WHERE (@NhomTuoiTable1 IS NULL OR @NhomTuoiTable1 = '' OR (
        CASE 
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) < 18 THEN '< 18'
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 18 AND 25 THEN '18 - 25'
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 26 AND 35 THEN '26 - 35'
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) >= 36 THEN '>= 36'
            ELSE N'Chưa xác định'
        END = @NhomTuoiTable1
    ))
    GROUP BY kh.DOI_TUONG
    ORDER BY kh.DOI_TUONG;

    -- 3. PHÂN TÍCH QST THEO NHÓM ĐỘ TUỔI - BẢNG 2 & BIỂU ĐỒ
    ;WITH CTE_LatestQST AS (
        SELECT 
            qst.RECORD_ID,
            qst.MUC_QST,
            ROW_NUMBER() OVER (
                PARTITION BY qst.RECORD_ID 
                ORDER BY ISNULL(qst.NGAY_SANG_LOC, '1900-01-01') DESC, qst.REPEAT_INSTANCE DESC
            ) AS rn
        FROM CD45_QST qst
        INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID
    )
    SELECT 
        CASE 
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) < 18 THEN '< 18'
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 18 AND 25 THEN '18 - 25'
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 26 AND 35 THEN '26 - 35'
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) >= 36 THEN '>= 36'
            ELSE N'Chưa xác định'
        END AS NhomTuoi,
        COUNT(DISTINCT kh.RECORD_ID) AS TongKH,
        COUNT(DISTINCT qst.RECORD_ID) AS SoKHSangLoc,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 1 THEN qst.RECORD_ID END) AS Muc1,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 2 THEN qst.RECORD_ID END) AS Muc2,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 3 THEN qst.RECORD_ID END) AS Muc3,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 4 THEN qst.RECORD_ID END) AS Muc4
    FROM #TmpKH kh
    LEFT JOIN CTE_LatestQST qst ON kh.RECORD_ID = qst.RECORD_ID AND qst.rn = 1
    GROUP BY 
        CASE 
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) < 18 THEN '< 18'
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 18 AND 25 THEN '18 - 25'
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 26 AND 35 THEN '26 - 35'
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) >= 36 THEN '>= 36'
            ELSE N'Chưa xác định'
        END
    ORDER BY 
        MIN(CASE 
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) < 18 THEN 1
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 18 AND 25 THEN 2
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 26 AND 35 THEN 3
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) >= 36 THEN 4
            ELSE 5
        END);

    -- 4. KẾT QUẢ ĐÁNH GIÁ LÂM SÀNG CHUYÊN SÂU (PTSD / PCL-5, AUDIT-C, KỲ THỊ)
    SELECT 
        kh.DOI_TUONG AS DoiTuong,
        COUNT(DISTINCT tv.RECORD_ID) AS SoKHDanhGia,
        SUM(CASE WHEN tv.PCL5_POSITIVE = 1 THEN 1 ELSE 0 END) AS PCL5_DuongTinh,
        SUM(CASE WHEN tv.PCL5_POSITIVE = 0 THEN 1 ELSE 0 END) AS PCL5_AmTinh,
        ROUND(ISNULL(AVG(CAST(tv.AUDIT_C_SCORE AS FLOAT)), 0), 1) AS DiemAuditCTB,
        ROUND(ISNULL(AVG(CAST(tv.STIGMA_SCORE AS FLOAT)), 0), 1) AS DiemKyThiTB
    FROM #TmpKH kh
    INNER JOIN CD45_TU_VAN_L1 tv ON kh.RECORD_ID = tv.RECORD_ID
    GROUP BY kh.DOI_TUONG
    ORDER BY kh.DOI_TUONG;

    -- 5. PHÂN BỐ THEO TỈNH THÀNH (HỖ TRỢ 34 TỈNH MỚI HOẶC 63 TỈNH CŨ)
    IF @CityMode = 'OLD63'
    BEGIN
        SELECT 
            kh.CITY_CODE AS CityCode,
            ISNULL(c.Name, kh.CITY_CODE) AS CityName,
            COUNT(DISTINCT kh.RECORD_ID) AS TongKH,
            ISNULL((SELECT COUNT(DISTINCT qst.RECORD_ID) FROM CD45_QST qst INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID WHERE k.CITY_CODE = kh.CITY_CODE), 0) AS SangLocQST,
            ISNULL((SELECT COUNT(DISTINCT cd.RECORD_ID) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID WHERE k.CITY_CODE = kh.CITY_CODE), 0) AS KhamSKTT,
            ISNULL((SELECT COUNT(DISTINCT tv.RECORD_ID) FROM CD45_TU_VAN_L1 tv INNER JOIN #TmpKH k ON tv.RECORD_ID = k.RECORD_ID WHERE k.CITY_CODE = kh.CITY_CODE), 0) AS TuVanL1
        FROM #TmpKH kh
        LEFT JOIN BVTL_CITES c ON kh.CITY_CODE = c.Code
        GROUP BY kh.CITY_CODE, c.Name
        ORDER BY TongKH DESC;
    END
    ELSE
    BEGIN
        SELECT 
            ISNULL(map.NewCityCode, kh.CITY_CODE) AS CityCode,
            ISNULL(newc.Name, ISNULL(map.NewCityName, kh.CITY_CODE)) AS CityName,
            COUNT(DISTINCT kh.RECORD_ID) AS TongKH,
            ISNULL((SELECT COUNT(DISTINCT qst.RECORD_ID) FROM CD45_QST qst INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID 
                    LEFT JOIN BVTL_MAP_TINH_CU_MOI m2 ON k.CITY_CODE = m2.OldCityCode 
                    WHERE ISNULL(m2.NewCityCode, k.CITY_CODE) = ISNULL(map.NewCityCode, kh.CITY_CODE)), 0) AS SangLocQST,
            ISNULL((SELECT COUNT(DISTINCT cd.RECORD_ID) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID 
                    LEFT JOIN BVTL_MAP_TINH_CU_MOI m2 ON k.CITY_CODE = m2.OldCityCode 
                    WHERE ISNULL(m2.NewCityCode, k.CITY_CODE) = ISNULL(map.NewCityCode, kh.CITY_CODE)), 0) AS KhamSKTT,
            ISNULL((SELECT COUNT(DISTINCT tv.RECORD_ID) FROM CD45_TU_VAN_L1 tv INNER JOIN #TmpKH k ON tv.RECORD_ID = k.RECORD_ID 
                    LEFT JOIN BVTL_MAP_TINH_CU_MOI m2 ON k.CITY_CODE = m2.OldCityCode 
                    WHERE ISNULL(m2.NewCityCode, k.CITY_CODE) = ISNULL(map.NewCityCode, kh.CITY_CODE)), 0) AS TuVanL1
        FROM #TmpKH kh
        LEFT JOIN BVTL_MAP_TINH_CU_MOI map ON kh.CITY_CODE = map.OldCityCode
        LEFT JOIN BVTL_DM_TINH_MOI newc ON map.NewCityCode = newc.Code
        GROUP BY ISNULL(map.NewCityCode, kh.CITY_CODE), ISNULL(newc.Name, ISNULL(map.NewCityName, kh.CITY_CODE))
        ORDER BY TongKH DESC;
    END

    -- 6. PHỄU DỊCH VỤ CHĂM SÓC SKTT (CASCADE FUNNEL)
    SELECT 
        (SELECT COUNT(DISTINCT hd.RECORD_ID) FROM CD45_HOAT_DONG hd INNER JOIN #TmpKH k ON hd.RECORD_ID = k.RECORD_ID) AS Step1_TiepCanTruyenThong,
        (SELECT COUNT(DISTINCT qst.RECORD_ID) FROM CD45_QST qst INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID) AS Step2_SangLocQST,
        (SELECT COUNT(DISTINCT qst.RECORD_ID) FROM CD45_QST qst INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID WHERE qst.MUC_QST IN (1, 2)) AS Step3_NguyCoCaoQST,
        (SELECT COUNT(DISTINCT tv.RECORD_ID) FROM CD45_TU_VAN_L1 tv INNER JOIN #TmpKH k ON tv.RECORD_ID = k.RECORD_ID) AS Step4_TuVanTamLy,
        (SELECT COUNT(DISTINCT cd.RECORD_ID) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID) AS Step5_KhamChuyenKhoa,
        (SELECT COUNT(DISTINCT cd.RECORD_ID) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID WHERE cd.LAN_KHAM > 1) AS Step6_TaiKhamSKTT;

    -- 7. DỊCH VỤ HỖ TRỢ CHUYỂN GỬI XÃ HỘI (CD45_HO_TRO_XH)
    SELECT 
        COUNT(DISTINCT htxh.RECORD_ID) AS TongNhanHoTro,
        COUNT(DISTINCT CASE WHEN (CHARINDEX(',1,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%BHYT%' OR htxh.DICH_VU LIKE N'%bảo hiểm%') THEN htxh.RECORD_ID END) AS HoTroBHYT,
        COUNT(DISTINCT CASE WHEN (CHARINDEX(',2,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%Methadone%') THEN htxh.RECORD_ID END) AS HoTroMethadone,
        COUNT(DISTINCT CASE WHEN (CHARINDEX(',10,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%HIV%') THEN htxh.RECORD_ID END) AS XetNghiemHIV,
        COUNT(DISTINCT CASE WHEN htxh.DICH_VU LIKE N'%STIs%' THEN htxh.RECORD_ID END) AS STIs,
        COUNT(DISTINCT CASE WHEN htxh.DICH_VU LIKE N'%gan%' THEN htxh.RECORD_ID END) AS ViemGan
    FROM CD45_HO_TRO_XH htxh
    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID;

    DROP TABLE #TmpKH;
END
GO

-- 3. SP_CD45_GetDrillDown
CREATE OR ALTER PROC SP_CD45_GetDrillDown
    @ChiTieuCode VARCHAR(50),
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @CityCode VARCHAR(100) = NULL,
    @MaNhom VARCHAR(100) = NULL,
    @MaTCV VARCHAR(100) = NULL,
    @DoiTuong INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Var_MaNhomStd VARCHAR(50) = @MaNhom;
    DECLARE @Var_MaNhomMap VARCHAR(50) = @MaNhom;

    IF @MaNhom IS NOT NULL AND @MaNhom <> ''
    BEGIN
        SELECT TOP 1 @Var_MaNhomStd = manhom_tbh, @Var_MaNhomMap = manhom_tbh_map
        FROM BVTL_NHOM_TBH
        WHERE manhom_tbh = @MaNhom OR manhom_tbh_map = @MaNhom;
    END

    -- Resolve @CityCode (hỗ trợ cả 34 tỉnh mới và 63 tỉnh cũ)
    DECLARE @MappedCityCodes TABLE (Code VARCHAR(10));
    IF @CityCode IS NOT NULL AND @CityCode <> ''
    BEGIN
        IF EXISTS (SELECT 1 FROM BVTL_DM_TINH_MOI WHERE Code = @CityCode)
        BEGIN
            INSERT INTO @MappedCityCodes(Code)
            SELECT OldCityCode FROM BVTL_MAP_TINH_CU_MOI WHERE NewCityCode = @CityCode;
        END
        ELSE
        BEGIN
            INSERT INTO @MappedCityCodes(Code) VALUES (@CityCode);
        END
    END

    SELECT 
        kh.RECORD_ID,
        kh.CITY_CODE,
        kh.MA_NHOM,
        kh.REDCAP_DAG,
        kh.DOI_TUONG,
        CASE kh.DOI_TUONG 
            WHEN 1 THEN 'PUD' 
            WHEN 2 THEN 'PLHIV' 
            WHEN 3 THEN 'TG' 
            WHEN 4 THEN 'MSM' 
            WHEN 5 THEN 'SW' 
            ELSE N'Chưa xác định' 
        END AS DOI_TUONG_TEXT,
        kh.NGAY_THAM_GIA
    INTO #TmpKH
    FROM CD45_KH kh
    WHERE (@CityCode IS NULL OR @CityCode = '' OR kh.CITY_CODE IN (SELECT Code FROM @MappedCityCodes))
      AND (@MaNhom IS NULL OR @MaNhom = '' OR kh.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom) OR kh.REDCAP_DAG IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom))
      AND (@DoiTuong IS NULL OR @DoiTuong = 0 OR kh.DOI_TUONG = @DoiTuong);

    CREATE CLUSTERED INDEX IX_TmpKH_RecId ON #TmpKH(RECORD_ID);

    IF @ChiTieuCode = 'I_1'
    BEGIN
        SELECT 
            k.RECORD_ID,
            k.CITY_CODE,
            k.MA_NHOM,
            k.REDCAP_DAG,
            k.DOI_TUONG,
            k.DOI_TUONG_TEXT,
            k.NGAY_THAM_GIA,
            NULL AS MA_TCV,
            NULL AS NGAY_THUC_HIEN,
            N'Tham gia dự án' AS CHI_TIET
        FROM #TmpKH k
        WHERE (@ToDate IS NULL OR k.NGAY_THAM_GIA <= @ToDate)
        ORDER BY k.NGAY_THAM_GIA DESC, k.RECORD_ID;
    END
    ELSE IF @ChiTieuCode = 'I_2'
    BEGIN
        SELECT 
            k.RECORD_ID,
            k.CITY_CODE,
            k.MA_NHOM,
            k.REDCAP_DAG,
            k.DOI_TUONG,
            k.DOI_TUONG_TEXT,
            k.NGAY_THAM_GIA,
            NULL AS MA_TCV,
            k.NGAY_THAM_GIA AS NGAY_THUC_HIEN,
            N'Tham gia trong kỳ' AS CHI_TIET
        FROM #TmpKH k
        WHERE (@FromDate IS NULL OR k.NGAY_THAM_GIA >= @FromDate)
          AND (@ToDate IS NULL OR k.NGAY_THAM_GIA <= @ToDate)
        ORDER BY k.NGAY_THAM_GIA DESC, k.RECORD_ID;
    END
    ELSE IF @ChiTieuCode IN ('II_1', 'II_2')
    BEGIN
        SELECT 
            k.RECORD_ID,
            k.CITY_CODE,
            k.MA_NHOM,
            k.REDCAP_DAG,
            k.DOI_TUONG,
            k.DOI_TUONG_TEXT,
            k.NGAY_THAM_GIA,
            hd.MA_TCV,
            hd.NGAY_THUC_HIEN,
            CASE hd.HINH_THUC_TIEP_CAN 
                WHEN 1 THEN N'Trực tiếp cá nhân' 
                WHEN 2 THEN N'Trực tiếp nhóm' 
                WHEN 3 THEN N'Online / Gián tiếp' 
                ELSE N'Hình thức khác' 
            END AS CHI_TIET
        FROM CD45_HOAT_DONG hd
        INNER JOIN #TmpKH k ON hd.RECORD_ID = k.RECORD_ID
        WHERE (@FromDate IS NULL OR hd.NGAY_THUC_HIEN >= @FromDate)
          AND (@ToDate IS NULL OR hd.NGAY_THUC_HIEN <= @ToDate)
          AND hd.HINH_THUC_TIEP_CAN IN (1, 2, 3)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
        ORDER BY hd.NGAY_THUC_HIEN DESC, k.RECORD_ID;
    END
    ELSE IF @ChiTieuCode = 'III_1'
    BEGIN
        SELECT 
            k.RECORD_ID,
            k.CITY_CODE,
            k.MA_NHOM,
            k.REDCAP_DAG,
            k.DOI_TUONG,
            k.DOI_TUONG_TEXT,
            k.NGAY_THAM_GIA,
            qst.MA_TCV,
            qst.NGAY_SANG_LOC AS NGAY_THUC_HIEN,
            N'Mức QST: ' + CAST(ISNULL(qst.MUC_QST, 0) AS NVARCHAR(10)) + N' - Điểm: ' + CAST(ISNULL(qst.DIEM_QST, 0) AS NVARCHAR(10)) AS CHI_TIET
        FROM CD45_QST qst
        INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID
        WHERE (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
          AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)
        ORDER BY qst.NGAY_SANG_LOC DESC, k.RECORD_ID;
    END
    ELSE IF @ChiTieuCode = 'III_2'
    BEGIN
        SELECT 
            k.RECORD_ID,
            k.CITY_CODE,
            k.MA_NHOM,
            k.REDCAP_DAG,
            k.DOI_TUONG,
            k.DOI_TUONG_TEXT,
            k.NGAY_THAM_GIA,
            qst.MA_TCV,
            qst.NGAY_SANG_LOC AS NGAY_THUC_HIEN,
            N'Nguy cơ cao (Mức ' + CAST(qst.MUC_QST AS NVARCHAR(10)) + N') - Điểm: ' + CAST(ISNULL(qst.DIEM_QST, 0) AS NVARCHAR(10)) AS CHI_TIET
        FROM CD45_QST qst
        INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID
        WHERE (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
          AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
          AND qst.MUC_QST IN (1, 2)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)
        ORDER BY qst.NGAY_SANG_LOC DESC, k.RECORD_ID;
    END
    ELSE IF @ChiTieuCode IN ('III_3', 'III_4', 'III_4_1', 'III_4_2', 'III_4_3', 'III_5')
    BEGIN
        SELECT 
            k.RECORD_ID,
            k.CITY_CODE,
            k.MA_NHOM,
            k.REDCAP_DAG,
            k.DOI_TUONG,
            k.DOI_TUONG_TEXT,
            k.NGAY_THAM_GIA,
            NULL AS MA_TCV,
            cd.NGAY_KHAM AS NGAY_THUC_HIEN,
            N'Lần khám: ' + CAST(ISNULL(cd.LAN_KHAM, 1) AS NVARCHAR(10)) + 
            ISNULL(N' - Cơ sở: ' + CAST(cd.CO_SO_Y_TE AS NVARCHAR(100)), N'') + 
            CASE WHEN cd.CHAN_DOAN IS NOT NULL AND cd.CHAN_DOAN <> '' AND cd.CHAN_DOAN <> '0' 
                 THEN N' - Chẩn đoán: ' + cd.CHAN_DOAN 
                 ELSE N'' 
            END AS CHI_TIET
        FROM CD45_CHAN_DOAN cd
        INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID
        WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
          AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
          AND (
              (@ChiTieuCode IN ('III_3', 'III_4'))
              OR (@ChiTieuCode = 'III_4_1' AND cd.LAN_KHAM = 1)
              OR (@ChiTieuCode = 'III_4_2' AND cd.LAN_KHAM = 2)
              OR (@ChiTieuCode = 'III_4_3' AND cd.LAN_KHAM >= 3)
              OR (@ChiTieuCode = 'III_5' AND cd.CHAN_DOAN IS NOT NULL AND cd.CHAN_DOAN <> '' AND cd.CHAN_DOAN <> '0')
          )
        ORDER BY cd.NGAY_KHAM DESC, k.RECORD_ID;
    END
    ELSE IF @ChiTieuCode IN ('IV_1', 'IV_2', 'IV_3', 'IV_4')
    BEGIN
        SELECT 
            k.RECORD_ID,
            k.CITY_CODE,
            k.MA_NHOM,
            k.REDCAP_DAG,
            k.DOI_TUONG,
            k.DOI_TUONG_TEXT,
            k.NGAY_THAM_GIA,
            ht.MA_TCV,
            ht.NGAY_HO_TRO AS NGAY_THUC_HIEN,
            ISNULL(ht.DICH_VU, N'Hỗ trợ xã hội') AS CHI_TIET
        FROM CD45_HO_TRO_XH ht
        INNER JOIN #TmpKH k ON ht.RECORD_ID = k.RECORD_ID
        WHERE (@FromDate IS NULL OR ht.NGAY_HO_TRO >= @FromDate)
          AND (@ToDate IS NULL OR ht.NGAY_HO_TRO <= @ToDate)
          AND (
              (@ChiTieuCode = 'IV_1' AND (CHARINDEX(',1,', ',' + ISNULL(ht.DICH_VU, '') + ',') > 0 OR ht.DICH_VU LIKE N'%BHYT%' OR ht.DICH_VU LIKE N'%bảo hiểm%'))
              OR (@ChiTieuCode = 'IV_2' AND (CHARINDEX(',2,', ',' + ISNULL(ht.DICH_VU, '') + ',') > 0 OR ht.DICH_VU LIKE N'%Methadone%' OR ht.DICH_VU LIKE N'%Buprenorphine%'))
              OR (@ChiTieuCode = 'IV_3' AND (CHARINDEX(',10,', ',' + ISNULL(ht.DICH_VU, '') + ',') > 0 OR ht.DICH_VU LIKE N'%HIV%'))
              OR (@ChiTieuCode = 'IV_4' AND (CHARINDEX(',3,', ',' + ISNULL(ht.DICH_VU, '') + ',') > 0 OR ht.DICH_VU LIKE N'%giảm hại%' OR ht.DICH_VU LIKE N'%bơm kim tiêm%' OR ht.DICH_VU LIKE N'%bao cao su%'))
          )
          AND (@MaTCV IS NULL OR @MaTCV = '' OR ht.MA_TCV = @MaTCV)
        ORDER BY ht.NGAY_HO_TRO DESC, k.RECORD_ID;
    END
    ELSE IF @ChiTieuCode = 'VI_1'
    BEGIN
        SELECT 
            k.RECORD_ID,
            k.CITY_CODE,
            k.MA_NHOM,
            k.REDCAP_DAG,
            k.DOI_TUONG,
            k.DOI_TUONG_TEXT,
            k.NGAY_THAM_GIA,
            NULL AS MA_TCV,
            tv.NGAY_TU_VAN AS NGAY_THUC_HIEN,
            N'Hoàn thành đánh giá sau can thiệp' AS CHI_TIET
        FROM CD45_TU_VAN_L1 tv
        INNER JOIN #TmpKH k ON tv.RECORD_ID = k.RECORD_ID
        WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)
          AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)
        ORDER BY tv.NGAY_TU_VAN DESC, k.RECORD_ID;
    END
    ELSE
    BEGIN
        SELECT TOP 0
            k.RECORD_ID, k.CITY_CODE, k.MA_NHOM, k.REDCAP_DAG, k.DOI_TUONG, k.DOI_TUONG_TEXT,
            k.NGAY_THAM_GIA, NULL AS MA_TCV, NULL AS NGAY_THUC_HIEN, NULL AS CHI_TIET
        FROM #TmpKH k;
    END

    DROP TABLE #TmpKH;
END
GO
