CREATE OR ALTER PROC SP_CD45_GetBaoCao
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @CityCode VARCHAR(100) = NULL,
    @MaNhom VARCHAR(100) = NULL,
    @MaTCV VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Resolve biến thể mã nhóm CBO
    DECLARE @Var_MaNhomStd VARCHAR(50) = @MaNhom;
    DECLARE @Var_MaNhomMap VARCHAR(50) = @MaNhom;

    IF @MaNhom IS NOT NULL AND @MaNhom <> ''
    BEGIN
        SELECT TOP 1 @Var_MaNhomStd = manhom_tbh, @Var_MaNhomMap = manhom_tbh_map
        FROM BVTL_NHOM_TBH
        WHERE manhom_tbh = @MaNhom OR manhom_tbh_map = @MaNhom;
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

    -- 2. Lọc danh sách Khách hàng cơ sở theo Tỉnh và Nhóm
    SELECT 
        kh.RECORD_ID,
        kh.CITY_CODE,
        kh.MA_NHOM,
        kh.REDCAP_DAG,
        kh.DOI_TUONG,
        kh.NGAY_THAM_GIA
    INTO #TmpKH
    FROM CD45_KH kh
    WHERE (@CityCode IS NULL OR @CityCode = '' OR kh.CITY_CODE = @CityCode)
      AND (@MaNhom IS NULL OR @MaNhom = '' OR kh.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom) OR kh.REDCAP_DAG = @MaNhom);

    CREATE CLUSTERED INDEX IX_TmpKH_RecId ON #TmpKH(RECORD_ID);

    -- =========================================================================
    -- SECTION I: THÔNG TIN CHUNG
    -- =========================================================================
    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)
    VALUES ('I', N'THÔNG TIN CHUNG', 1, 0, 'SEC_I');

    -- 1. Tổng số KH được chăm sóc từ đầu dự án
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '1', N'Tổng số KH được chăm sóc từ đầu dự án',
        COUNT(DISTINCT kh.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN kh.RECORD_ID END),
        0, 0, 'I_1'
    FROM #TmpKH kh;

    -- 2. Tổng số KH được chăm sóc trong kỳ báo cáo (Nhận bất kỳ dịch vụ nào trong kỳ)
    ;WITH CTE_KyKH AS (
        SELECT kh.RECORD_ID, kh.DOI_TUONG
        FROM #TmpKH kh
        WHERE (
            (@FromDate IS NULL OR kh.NGAY_THAM_GIA >= @FromDate) AND (@ToDate IS NULL OR kh.NGAY_THAM_GIA <= @ToDate)
            OR EXISTS (
                SELECT 1 FROM CD45_HOAT_DONG hd 
                WHERE hd.RECORD_ID = kh.RECORD_ID 
                  AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate) 
                  AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
                  AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
            )
            OR EXISTS (
                SELECT 1 FROM CD45_QST qst 
                WHERE qst.RECORD_ID = kh.RECORD_ID 
                  AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate) 
                  AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
                  AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)
            )
            OR EXISTS (
                SELECT 1 FROM CD45_CHAN_DOAN cd 
                WHERE cd.RECORD_ID = kh.RECORD_ID 
                  AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate) 
                  AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
                  AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV)
            )
            OR EXISTS (
                SELECT 1 FROM CD45_TU_VAN_L1 tv1 
                WHERE tv1.RECORD_ID = kh.RECORD_ID 
                  AND (@FromDate IS NULL OR tv1.NGAY_TU_VAN >= @FromDate) 
                  AND (@ToDate IS NULL OR tv1.NGAY_TU_VAN <= @ToDate)
                  AND (@MaTCV IS NULL OR @MaTCV = '' OR tv1.MA_TCV = @MaTCV)
            )
            OR EXISTS (
                SELECT 1 FROM CD45_TU_VAN_L2 tv2 
                WHERE tv2.RECORD_ID = kh.RECORD_ID 
                  AND (@FromDate IS NULL OR tv2.NGAY_TU_VAN >= @FromDate) 
                  AND (@ToDate IS NULL OR tv2.NGAY_TU_VAN <= @ToDate)
                  AND (@MaTCV IS NULL OR @MaTCV = '' OR tv2.MA_TCV = @MaTCV)
            )
            OR EXISTS (
                SELECT 1 FROM CD45_HO_TRO_XH htxh 
                WHERE htxh.RECORD_ID = kh.RECORD_ID 
                  AND (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate) 
                  AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
                  AND (@MaTCV IS NULL OR @MaTCV = '' OR htxh.MA_TCV = @MaTCV)
            )
        )
    )
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '2', N'Tổng số KH được chăm sóc trong kỳ báo cáo',
        COUNT(DISTINCT RECORD_ID),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 1 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 2 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 3 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 5 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 4 THEN RECORD_ID END),
        0, 0, 'I_2'
    FROM CTE_KyKH;

    -- 3. Số KH mất dấu trong kỳ báo cáo
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '3', N'Số KH mất dấu trong kỳ báo cáo',
        COUNT(DISTINCT td.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN td.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN td.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN td.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN td.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN td.RECORD_ID END),
        0, 0, 'I_3'
    FROM CD45_THEO_DAU td
    INNER JOIN #TmpKH kh ON td.RECORD_ID = kh.RECORD_ID
    WHERE td.MAT_DAU = 1
      AND (@FromDate IS NULL OR td.NGAY_THEO_DAU >= @FromDate)
      AND (@ToDate IS NULL OR td.NGAY_THEO_DAU <= @ToDate);

    -- =========================================================================
    -- SECTION II: HOẠT ĐỘNG TRUYỀN THÔNG
    -- =========================================================================
    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)
    VALUES ('II', N'HOẠT ĐỘNG TRUYỀN THÔNG', 1, 0, 'SEC_II');

    -- 1. Lần 1
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '1', N'Số KH được tham gia truyền thông lần 1',
        COUNT(DISTINCT hd.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN hd.RECORD_ID END),
        0, 0, 'II_1'
    FROM CD45_HOAT_DONG hd
    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
    WHERE hd.REPEAT_INSTANCE = 1
      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);

    -- 2. Lần 2+
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '2', N'Số KH được tham gia truyền thông lần 2',
        COUNT(DISTINCT hd.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN hd.RECORD_ID END),
        0, 0, 'II_2'
    FROM CD45_HOAT_DONG hd
    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
    WHERE hd.REPEAT_INSTANCE > 1
      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);

    -- 3. Tổng lượt
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '3', N'Tổng số lượt KH tham gia truyền thông',
        COUNT(*),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),
        0, 0, 'II_3'
    FROM CD45_HOAT_DONG hd
    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);

    -- =========================================================================
    -- SECTION III: SÀNG LỌC BẰNG HỎI QST VÀ CHUYỂN GỬI KHÁM, ĐIỀU TRỊ SKTT
    -- =========================================================================
    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)
    VALUES ('III', N'SÀNG LỌC BẰNG HỎI QST VÀ CHUYỂN GỬI KHÁM, ĐIỀU TRỊ SKTT', 1, 0, 'SEC_III');

    -- 1. Sàng lọc QST Lần 1
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '1', N'Số KH được sàng lọc bằng hỏi QST lần 1',
        COUNT(DISTINCT qst.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN qst.RECORD_ID END),
        0, 0, 'III_1'
    FROM CD45_QST qst
    INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID
    WHERE qst.REPEAT_INSTANCE = 1
      AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
      AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV);

    -- 1.1 - 1.4: Mức điểm QST Lần 1
    DECLARE @MucQst TABLE (Muc INT, Ten NVARCHAR(100), Code NVARCHAR(50));
    INSERT INTO @MucQst VALUES 
        (1, N'Mức 1 (>=8)', 'III_1_M1'), 
        (2, N'Mức 2 (6 - 7)', 'III_1_M2'), 
        (3, N'Mức 3 (4 - 5)', 'III_1_M3'), 
        (4, N'Mức 4 (<4)', 'III_1_M4');

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '', m.Ten,
        COUNT(DISTINCT qst.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN qst.RECORD_ID END),
        0, 1, m.Code
    FROM @MucQst m
    LEFT JOIN CD45_QST qst ON qst.MUC_QST = m.Muc AND qst.REPEAT_INSTANCE = 1
        AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
        AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
        AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)
    LEFT JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID
    GROUP BY m.Muc, m.Ten, m.Code
    ORDER BY m.Muc;

    -- 2. Sàng lọc QST Lần 2+
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '2', N'Số KH được sàng lọc lại bảng hỏi QST (từ lần 2 trở đi)',
        COUNT(DISTINCT qst.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN qst.RECORD_ID END),
        0, 0, 'III_2'
    FROM CD45_QST qst
    INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID
    WHERE qst.REPEAT_INSTANCE > 1
      AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
      AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV);

    -- 2.1 - 2.4: Mức điểm QST Lần 2+
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '', m.Ten,
        COUNT(DISTINCT qst.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN qst.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN qst.RECORD_ID END),
        0, 1, 'III_2_' + CAST(m.Muc AS VARCHAR)
    FROM @MucQst m
    LEFT JOIN CD45_QST qst ON qst.MUC_QST = m.Muc AND qst.REPEAT_INSTANCE > 1
        AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
        AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
        AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)
    LEFT JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID
    GROUP BY m.Muc, m.Ten
    ORDER BY m.Muc;

    -- 3. Số lượt KH được chuyển gửi khám SKTT
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '3', N'Số lượt KH được chuyển gửi khám SKTT',
        COUNT(*),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),
        0, 0, 'III_3'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);

    -- 4. Số KH được chuyển gửi khám SKTT, trong đó:
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '4', N'Số KH được chuyển gửi khám SKTT, trong đó:',
        COUNT(DISTINCT cd.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN cd.RECORD_ID END),
        0, 0, 'III_4'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);

    -- 4.1 Khám lần 1
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '', N'Số KH được chuyển gửi khám SKTT lần 1',
        COUNT(DISTINCT cd.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN cd.RECORD_ID END),
        0, 1, 'III_4_1'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE cd.LAN_KHAM = 1
      AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);

    -- 4.2 Tái khám SKTT (Số KH)
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '', N'Số KH được tái khám SKTT',
        COUNT(DISTINCT cd.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN cd.RECORD_ID END),
        0, 1, 'III_4_2'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE cd.LAN_KHAM > 1
      AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);

    -- 4.3 Tái khám SKTT (Số lượt)
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '', N'Số lượt KH được tái khám SKTT',
        COUNT(*),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),
        0, 1, 'III_4_3'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE cd.LAN_KHAM > 1
      AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);

    -- 5. Điều trị nội trú (nhập viện) - Mã '2' trong f6_treatment
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '5', N'Số KH được điều trị nội trú (nhập viện)',
        COUNT(DISTINCT cd.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN cd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN cd.RECORD_ID END),
        0, 0, 'III_5'
    FROM CD45_CHAN_DOAN cd
    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
    WHERE (CHARINDEX(',2,', ',' + ISNULL(cd.HINH_THUC_DIEU_TRI, '') + ',') > 0 
           OR cd.HINH_THUC_DIEU_TRI LIKE '%2%' 
           OR cd.HINH_THUC_DIEU_TRI LIKE N'%nội trú%')
      AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);

    -- 6. Hỗ trợ mua thẻ BHYT - Mã '1' trong f4_services
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '6', N'Số KH được hỗ trợ mua thẻ Bảo hiểm y tế',
        COUNT(DISTINCT htxh.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN htxh.RECORD_ID END),
        0, 0, 'III_6'
    FROM CD45_HO_TRO_XH htxh
    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID
    WHERE (CHARINDEX(',1,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 
           OR htxh.DICH_VU LIKE '%1%' 
           OR htxh.DICH_VU LIKE N'%BHYT%' 
           OR htxh.DICH_VU LIKE N'%bảo hiểm%')
      AND (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)
      AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR htxh.MA_TCV = @MaTCV);

    -- =========================================================================
    -- SECTION IV: CÁC CAN THIỆP CÁ NHÂN VÀ CAN THIỆP NHÓM
    -- =========================================================================
    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)
    VALUES ('IV', N'CÁC CAN THIỆP CÁ NHÂN VÀ CAN THIỆP NHÓM', 1, 0, 'SEC_IV');

    -- CTE Gộp tất cả các lượt tư vấn
    ;WITH CTE_TuVan AS (
        SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1
        UNION ALL
        SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2
    )
    -- 1. Số lượt KH được tư vấn cá nhân
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '1', N'Số lượt KH được tư vấn cá nhân',
        COUNT(*),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),
        0, 0, 'IV_1'
    FROM CTE_TuVan tv
    INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)
      AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV);

    -- 2. Số KH được tư vấn cá nhân, trong đó:
    ;WITH CTE_TuVanCount AS (
        SELECT tv.RECORD_ID, kh.DOI_TUONG, COUNT(*) AS SoLan
        FROM (
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1
            UNION ALL
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2
        ) tv
        INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID
        WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)
          AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV)
        GROUP BY tv.RECORD_ID, kh.DOI_TUONG
    )
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '2', N'Số KH được tư vấn cá nhân, trong đó:',
        COUNT(DISTINCT RECORD_ID),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 1 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 2 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 3 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 5 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 4 THEN RECORD_ID END),
        0, 0, 'IV_2'
    FROM CTE_TuVanCount;

    -- 2.1 Tư vấn 1 lần
    ;WITH CTE_TuVanCount AS (
        SELECT tv.RECORD_ID, kh.DOI_TUONG, COUNT(*) AS SoLan
        FROM (
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1
            UNION ALL
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2
        ) tv
        INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID
        WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)
          AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV)
        GROUP BY tv.RECORD_ID, kh.DOI_TUONG
    )
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '', N'Số KH được tư vấn 1 lần',
        COUNT(DISTINCT CASE WHEN SoLan = 1 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 1 AND SoLan = 1 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 2 AND SoLan = 1 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 3 AND SoLan = 1 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 5 AND SoLan = 1 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 4 AND SoLan = 1 THEN RECORD_ID END),
        0, 1, 'IV_2_1'
    FROM CTE_TuVanCount;

    -- 2.2 Tư vấn 2 lần
    ;WITH CTE_TuVanCount AS (
        SELECT tv.RECORD_ID, kh.DOI_TUONG, COUNT(*) AS SoLan
        FROM (
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1
            UNION ALL
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2
        ) tv
        INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID
        WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)
          AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV)
        GROUP BY tv.RECORD_ID, kh.DOI_TUONG
    )
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '', N'Số KH được tư vấn 2 lần',
        COUNT(DISTINCT CASE WHEN SoLan = 2 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 1 AND SoLan = 2 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 2 AND SoLan = 2 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 3 AND SoLan = 2 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 5 AND SoLan = 2 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 4 AND SoLan = 2 THEN RECORD_ID END),
        0, 1, 'IV_2_2'
    FROM CTE_TuVanCount;

    -- 2.3 Tư vấn từ 3 lần trở lên
    ;WITH CTE_TuVanCount AS (
        SELECT tv.RECORD_ID, kh.DOI_TUONG, COUNT(*) AS SoLan
        FROM (
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1
            UNION ALL
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2
        ) tv
        INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID
        WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)
          AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV)
        GROUP BY tv.RECORD_ID, kh.DOI_TUONG
    )
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '', N'Số KH được tư vấn từ 3 lần trở lên',
        COUNT(DISTINCT CASE WHEN SoLan >= 3 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 1 AND SoLan >= 3 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 2 AND SoLan >= 3 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 3 AND SoLan >= 3 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 5 AND SoLan >= 3 THEN RECORD_ID END),
        COUNT(DISTINCT CASE WHEN DOI_TUONG = 4 AND SoLan >= 3 THEN RECORD_ID END),
        0, 1, 'IV_2_3'
    FROM CTE_TuVanCount;

    -- 3. Số lượt KH tham gia sinh hoạt nhóm (LOAI_DV = 2)
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '3', N'Số lượt KH được tham gia sinh hoạt nhóm',
        COUNT(*),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),
        0, 0, 'IV_3'
    FROM CD45_HOAT_DONG hd
    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
    WHERE hd.LOAI_DV = 2
      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);

    -- 4. Số KH tham gia sinh hoạt nhóm
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '4', N'Số KH được tham gia sinh hoạt nhóm',
        COUNT(DISTINCT hd.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN hd.RECORD_ID END),
        0, 0, 'IV_4'
    FROM CD45_HOAT_DONG hd
    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
    WHERE hd.LOAI_DV = 2
      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);

    -- 5. Số lượt tham gia can thiệp chữa lành (LOAI_DV = 3 hoặc 4: Vòng tròn chia sẻ & Trị liệu nghệ thuật)
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '5', N'Số lượt KH được tham gia can thiệp chữa lành',
        COUNT(*),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),
        0, 0, 'IV_5'
    FROM CD45_HOAT_DONG hd
    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
    WHERE hd.LOAI_DV IN (3, 4)
      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);

    -- 6. Số KH tham gia can thiệp chữa lành
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '6', N'Số KH được tham gia can thiệp chữa lành',
        COUNT(DISTINCT hd.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN hd.RECORD_ID END),
        0, 0, 'IV_6'
    FROM CD45_HOAT_DONG hd
    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
    WHERE hd.LOAI_DV IN (3, 4)
      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);

    -- =========================================================================
    -- SECTION V: CAN THIỆP ONLINE
    -- =========================================================================
    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)
    VALUES ('V', N'CAN THIỆP ONLINE', 1, 0, 'SEC_V');

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '1', N'Số lượt KH được can thiệp online',
        0, 0, 0, 0, 0, 0, 0, 0, 'V_1';

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '2', N'Số KH được can thiệp online',
        0, 0, 0, 0, 0, 0, 0, 0, 'V_2';

    -- =========================================================================
    -- SECTION VI: DỊCH VỤ CHUYỂN GỬI KHÁC
    -- =========================================================================
    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)
    VALUES ('VI', N'DỊCH VỤ CHUYỂN GỬI KHÁC', 1, 0, 'SEC_VI');

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '1', N'Số KH được chuyển gửi dịch vụ/xét nghiệm thành công',
        COUNT(DISTINCT htxh.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN htxh.RECORD_ID END),
        0, 0, 'VI_1'
    FROM CD45_HO_TRO_XH htxh
    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)
      AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR htxh.MA_TCV = @MaTCV);

    -- Các dịch vụ con từ f4_services
    DECLARE @DvList TABLE (CodeStr VARCHAR(20), Ten NVARCHAR(100), SubCode VARCHAR(50));
    INSERT INTO @DvList VALUES 
        ('1', N'- Thẻ bảo hiểm y tế', 'VI_1_BHYT'),
        ('2', N'- Hỗ trợ chi phí điều trị Methadone', 'VI_1_METHADONE'),
        ('10', N'- Tư vấn và xét nghiệm nhanh HIV', 'VI_1_HIV'),
        ('STIS', N'- Khám/điều trị STIs', 'VI_1_STIS'),
        ('GAN', N'- XN/Điều trị Viêm gan B,C', 'VI_1_HEPATITIS'),
        ('KHAC', N'- Dịch vụ hỗ trợ xã hội & y tế khác', 'VI_1_OTHER');

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '', dv.Ten,
        COUNT(DISTINCT htxh.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN htxh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN htxh.RECORD_ID END),
        0, 1, dv.SubCode
    FROM @DvList dv
    LEFT JOIN CD45_HO_TRO_XH htxh ON (
            (dv.CodeStr = '1' AND (CHARINDEX(',1,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE '%1%'))
            OR (dv.CodeStr = '2' AND (CHARINDEX(',2,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE '%2%'))
            OR (dv.CodeStr = '10' AND (CHARINDEX(',10,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE '%10%'))
            OR (dv.CodeStr = 'STIS' AND htxh.DICH_VU LIKE '%STIs%')
            OR (dv.CodeStr = 'GAN' AND htxh.DICH_VU LIKE '%gan%')
            OR (dv.CodeStr = 'KHAC' AND (
                CHARINDEX(',3,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                CHARINDEX(',4,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                CHARINDEX(',5,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                CHARINDEX(',6,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                CHARINDEX(',7,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                CHARINDEX(',8,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                CHARINDEX(',9,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0
            ))
        )
        AND (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)
        AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
        AND (@MaTCV IS NULL OR @MaTCV = '' OR htxh.MA_TCV = @MaTCV)
    LEFT JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID
    GROUP BY dv.CodeStr, dv.Ten, dv.SubCode;

    -- =========================================================================
    -- SECTION VII: PHÁT TÀI LIỆU TRUYỀN THÔNG
    -- =========================================================================
    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)
    VALUES ('VII', N'PHÁT TÀI LIỆU TRUYỀN THÔNG', 1, 0, 'SEC_VII');

    -- 1. Số quyển tài liệu đã phát
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '1', N'Số quyển tài liệu đã phát',
        ISNULL(SUM(hd.SO_TAI_LIEU), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),
        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),
        0, 0, 'VII_1'
    FROM CD45_HOAT_DONG hd
    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);

    -- 2. Số KH nhận tài liệu
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '2', N'Số KH nhận tài liệu',
        COUNT(DISTINCT CASE WHEN ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),
        0, 0, 'VII_2'
    FROM CD45_HOAT_DONG hd
    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
    WHERE (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);

    -- =========================================================================
    -- TRẢ VỀ KẾT QUẢ
    -- =========================================================================
    SELECT 
        STT, 
        ChiTieu, 
        ISNULL(Tong, 0) AS Tong, 
        ISNULL(PUD, 0) AS PUD, 
        ISNULL(PLHIV, 0) AS PLHIV, 
        ISNULL(TG, 0) AS TG, 
        ISNULL(SW, 0) AS SW, 
        ISNULL(MSM, 0) AS MSM, 
        ISNULL(IsBold, 0) AS IsBold, 
        ISNULL(IndentLevel, 0) AS IndentLevel,
        Code
    FROM @TmpResult
    ORDER BY STT_Sort;

    DROP TABLE #TmpKH;
END
GO

-- =========================================================================
-- SP_CD45_Dashboard: Phục vụ Dashboard Tổng quan Trang chủ Dự án CD45
-- =========================================================================
CREATE OR ALTER PROC SP_CD45_Dashboard
    @CityCode VARCHAR(50) = NULL,
    @MaNhom VARCHAR(50) = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Resolve biến thể mã nhóm CBO (chuẩn hóa manhom_tbh và manhom_tbh_map)
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
    WHERE (@CityCode IS NULL OR @CityCode = '' OR kh.CITY_CODE = @CityCode)
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
        COUNT(DISTINCT kh.RECORD_ID) AS TongKhachHang,
        COUNT(DISTINCT qst.RECORD_ID) AS TongSangLocQST,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST IN (1, 2) THEN qst.RECORD_ID END) AS QSTNguyCoCao,
        COUNT(DISTINCT cd.RECORD_ID) AS TongKhamSKTT,
        COUNT(cd.RECORD_ID) AS TongLuotKhamSKTT,
        COUNT(DISTINCT tv.RECORD_ID) AS TongTuVanL1,
        COUNT(DISTINCT htxh.RECORD_ID) AS TongHoTroXH,
        ISNULL((SELECT SUM(hd.SO_TAI_LIEU) FROM CD45_HOAT_DONG hd INNER JOIN #TmpKH k ON hd.RECORD_ID = k.RECORD_ID), 0) AS TongTaiLieuPhat,
        ISNULL((SELECT MAX(NGAY_SYNC) FROM CD45_KH), GETDATE()) AS LastSyncTime
    FROM #TmpKH kh
    LEFT JOIN CD45_QST qst ON kh.RECORD_ID = qst.RECORD_ID
    LEFT JOIN CD45_CHAN_DOAN cd ON kh.RECORD_ID = cd.RECORD_ID
    LEFT JOIN CD45_TU_VAN_L1 tv ON kh.RECORD_ID = tv.RECORD_ID
    LEFT JOIN CD45_HO_TRO_XH htxh ON kh.RECORD_ID = htxh.RECORD_ID;

    -- 2. PHÂN TÍCH QST THEO NHÓM ĐÍCH (DOI_TUONG)
    SELECT 
        kh.DOI_TUONG AS DoiTuongId,
        CASE 
            WHEN kh.DOI_TUONG = 1 THEN N'PUD (Sử dụng ma túy)'
            WHEN kh.DOI_TUONG = 2 THEN N'PLHIV (Sống với HIV)'
            WHEN kh.DOI_TUONG = 3 THEN N'TG (Người chuyển giới)'
            WHEN kh.DOI_TUONG = 4 THEN N'MSM (Nam QHTD đồng giới)'
            WHEN kh.DOI_TUONG = 5 THEN N'SW (Người bán dâm)'
            ELSE N'Khác'
        END AS TenDoiTuong,
        COUNT(DISTINCT kh.RECORD_ID) AS TongKH,
        COUNT(DISTINCT qst.RECORD_ID) AS SoKHSangLoc,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 1 THEN qst.RECORD_ID END) AS Muc1_RatCao,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 2 THEN qst.RECORD_ID END) AS Muc2_Cao,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 3 THEN qst.RECORD_ID END) AS Muc3_TrungBinh,
        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 4 THEN qst.RECORD_ID END) AS Muc4_Thap
    FROM #TmpKH kh
    LEFT JOIN CD45_QST qst ON kh.RECORD_ID = qst.RECORD_ID
    GROUP BY kh.DOI_TUONG
    ORDER BY kh.DOI_TUONG;

    -- 3. PHÂN TÍCH QST THEO NHÓM ĐỘ TUỔI
    SELECT 
        CASE 
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
    LEFT JOIN CD45_QST qst ON kh.RECORD_ID = qst.RECORD_ID
    GROUP BY 
        CASE 
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 18 AND 25 THEN '18 - 25'
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 26 AND 35 THEN '26 - 35'
            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) >= 36 THEN '>= 36'
            ELSE N'Chưa xác định'
        END
    ORDER BY NhomTuoi;

    -- 4. ĐÁNH GIÁ SANG CHẤN PTSD (PCL-5), RƯỢU (AUDIT-C) & KỲ THỊ (STIGMA)
    SELECT 
        kh.DOI_TUONG AS DoiTuongId,
        CASE 
            WHEN kh.DOI_TUONG = 1 THEN N'PUD'
            WHEN kh.DOI_TUONG = 2 THEN N'PLHIV'
            WHEN kh.DOI_TUONG = 3 THEN N'TG'
            WHEN kh.DOI_TUONG = 4 THEN N'MSM'
            WHEN kh.DOI_TUONG = 5 THEN N'SW'
            ELSE N'Khác'
        END AS TenDoiTuong,
        COUNT(tv.RECORD_ID) AS SoCaTuVan,
        SUM(CASE WHEN tv.PCL5_POSITIVE = 1 THEN 1 ELSE 0 END) AS PCL5_DuongTinh,
        SUM(CASE WHEN tv.PCL5_POSITIVE = 0 THEN 1 ELSE 0 END) AS PCL5_AmTinh,
        ROUND(ISNULL(AVG(CAST(tv.AUDIT_C_SCORE AS FLOAT)), 0), 1) AS DiemAuditCTB,
        ROUND(ISNULL(AVG(CAST(tv.STIGMA_SCORE AS FLOAT)), 0), 1) AS DiemKyThiTB
    FROM #TmpKH kh
    INNER JOIN CD45_TU_VAN_L1 tv ON kh.RECORD_ID = tv.RECORD_ID
    GROUP BY kh.DOI_TUONG
    ORDER BY kh.DOI_TUONG;

    -- 5. PHÂN BỐ THEO TỈNH THÀNH
    SELECT 
        kh.CITY_CODE AS CityCode,
        CASE 
            WHEN kh.CITY_CODE = 'HNO' THEN N'Hà Nội'
            WHEN kh.CITY_CODE = 'HPG' THEN N'Hải Phòng'
            WHEN kh.CITY_CODE = 'NAN' THEN N'Nghệ An'
            WHEN kh.CITY_CODE = 'HCM' THEN N'TP. Hồ Chí Minh'
            WHEN kh.CITY_CODE = 'HYE' THEN N'Hưng Yên'
            WHEN kh.CITY_CODE = 'NBI' THEN N'Ninh Bình'
            ELSE kh.CITY_CODE
        END AS CityName,
        COUNT(DISTINCT kh.RECORD_ID) AS TongKH,
        COUNT(DISTINCT qst.RECORD_ID) AS SangLocQST,
        COUNT(DISTINCT cd.RECORD_ID) AS KhamSKTT,
        COUNT(DISTINCT tv.RECORD_ID) AS TuVanL1
    FROM #TmpKH kh
    LEFT JOIN CD45_QST qst ON kh.RECORD_ID = qst.RECORD_ID
    LEFT JOIN CD45_CHAN_DOAN cd ON kh.RECORD_ID = cd.RECORD_ID
    LEFT JOIN CD45_TU_VAN_L1 tv ON kh.RECORD_ID = tv.RECORD_ID
    GROUP BY kh.CITY_CODE
    ORDER BY TongKH DESC;

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
        COUNT(DISTINCT CASE WHEN CHARINDEX('1', ISNULL(htxh.DICH_VU, '')) > 0 OR htxh.DICH_VU LIKE '%BHYT%' THEN htxh.RECORD_ID END) AS HoTroBHYT,
        COUNT(DISTINCT CASE WHEN CHARINDEX('2', ISNULL(htxh.DICH_VU, '')) > 0 OR htxh.DICH_VU LIKE '%Methadone%' THEN htxh.RECORD_ID END) AS HoTroMethadone,
        COUNT(DISTINCT CASE WHEN CHARINDEX('10', ISNULL(htxh.DICH_VU, '')) > 0 OR htxh.DICH_VU LIKE '%HIV%' THEN htxh.RECORD_ID END) AS XetNghiemHIV,
        COUNT(DISTINCT CASE WHEN htxh.DICH_VU LIKE '%STIs%' THEN htxh.RECORD_ID END) AS STIs,
        COUNT(DISTINCT CASE WHEN htxh.DICH_VU LIKE '%gan%' THEN htxh.RECORD_ID END) AS ViemGan
    FROM CD45_HO_TRO_XH htxh
    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID;

    DROP TABLE #TmpKH;
END
GO

