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
    WHERE (@CityCode IS NULL OR @CityCode = '' OR kh.CITY_CODE = @CityCode)
      AND (@MaNhom IS NULL OR @MaNhom = '' OR kh.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom) OR kh.REDCAP_DAG = @MaNhom)
      AND (@DoiTuong IS NULL OR @DoiTuong = 0 OR kh.DOI_TUONG = @DoiTuong);

    CREATE CLUSTERED INDEX IX_TmpKH_RecId ON #TmpKH(RECORD_ID);

    -- =========================================================================
    -- SECTION I: THÔNG TIN CHUNG
    -- =========================================================================
    -- 1. Tổng số KH từ đầu dự án
    IF @ChiTieuCode = 'I_1'
    BEGIN
        SELECT 
            kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, '' AS MA_TCV, kh.DOI_TUONG_TEXT, 
            CONVERT(VARCHAR(10), kh.NGAY_THAM_GIA, 103) AS NGAY_THUC_HIEN,
            N'Ngày tham gia: ' + ISNULL(CONVERT(VARCHAR(10), kh.NGAY_THAM_GIA, 103), '') AS CHI_TIET
        FROM #TmpKH kh
        ORDER BY kh.NGAY_THAM_GIA DESC, kh.RECORD_ID;
    END
    -- 2. Tổng số KH trong kỳ
    ELSE IF @ChiTieuCode = 'I_2'
    BEGIN
        ;WITH CTE_Act AS (
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, '' AS MA_TCV, kh.DOI_TUONG_TEXT,
                   kh.NGAY_THAM_GIA AS ACT_DATE, N'KH mới tham gia' AS CHI_TIET
            FROM #TmpKH kh
            WHERE (@FromDate IS NULL OR kh.NGAY_THAM_GIA >= @FromDate) AND (@ToDate IS NULL OR kh.NGAY_THAM_GIA <= @ToDate)
            UNION ALL
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, hd.MA_TCV, kh.DOI_TUONG_TEXT,
                   hd.NGAY_HOAT_DONG AS ACT_DATE, N'Truyền thông/hoạt động' AS CHI_TIET
            FROM CD45_HOAT_DONG hd INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
            WHERE (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate) AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
            UNION ALL
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, qst.MA_TCV, kh.DOI_TUONG_TEXT,
                   qst.NGAY_SANG_LOC AS ACT_DATE, N'Sàng lọc QST' AS CHI_TIET
            FROM CD45_QST qst INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID
            WHERE (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate) AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)
            UNION ALL
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, cd.MA_TCV, kh.DOI_TUONG_TEXT,
                   cd.NGAY_KHAM AS ACT_DATE, N'Khám SKTT' AS CHI_TIET
            FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
            WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate) AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV)
            UNION ALL
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, tv1.MA_TCV, kh.DOI_TUONG_TEXT,
                   tv1.NGAY_TU_VAN AS ACT_DATE, N'Tư vấn lần 1' AS CHI_TIET
            FROM CD45_TU_VAN_L1 tv1 INNER JOIN #TmpKH kh ON tv1.RECORD_ID = kh.RECORD_ID
            WHERE (@FromDate IS NULL OR tv1.NGAY_TU_VAN >= @FromDate) AND (@ToDate IS NULL OR tv1.NGAY_TU_VAN <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR tv1.MA_TCV = @MaTCV)
            UNION ALL
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, tv2.MA_TCV, kh.DOI_TUONG_TEXT,
                   tv2.NGAY_TU_VAN AS ACT_DATE, N'Tư vấn lần 2+' AS CHI_TIET
            FROM CD45_TU_VAN_L2 tv2 INNER JOIN #TmpKH kh ON tv2.RECORD_ID = kh.RECORD_ID
            WHERE (@FromDate IS NULL OR tv2.NGAY_TU_VAN >= @FromDate) AND (@ToDate IS NULL OR tv2.NGAY_TU_VAN <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR tv2.MA_TCV = @MaTCV)
            UNION ALL
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, htxh.MA_TCV, kh.DOI_TUONG_TEXT,
                   htxh.NGAY_HO_TRO AS ACT_DATE, N'Hỗ trợ xã hội' AS CHI_TIET
            FROM CD45_HO_TRO_XH htxh INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID
            WHERE (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate) AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR htxh.MA_TCV = @MaTCV)
        ),
        CTE_Rank AS (
            SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, ACT_DATE, CHI_TIET,
                   ROW_NUMBER() OVER(PARTITION BY RECORD_ID ORDER BY ACT_DATE DESC) AS rn
            FROM CTE_Act
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT,
               CONVERT(VARCHAR(10), ACT_DATE, 103) AS NGAY_THUC_HIEN,
               CHI_TIET
        FROM CTE_Rank
        WHERE rn = 1
        ORDER BY ACT_DATE DESC, RECORD_ID;
    END
    -- 3. Mất dấu
    ELSE IF @ChiTieuCode = 'I_3'
    BEGIN
        ;WITH CTE AS (
            SELECT 
                kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, '' AS MA_TCV, kh.DOI_TUONG_TEXT, 
                td.NGAY_THEO_DAU,
                CONVERT(VARCHAR(10), td.NGAY_THEO_DAU, 103) AS NGAY_THUC_HIEN,
                N'Mất dấu' AS CHI_TIET,
                ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY td.NGAY_THEO_DAU DESC) AS rn
            FROM CD45_THEO_DAU td
            INNER JOIN #TmpKH kh ON td.RECORD_ID = kh.RECORD_ID
            WHERE td.MAT_DAU = 1
              AND (@FromDate IS NULL OR td.NGAY_THEO_DAU >= @FromDate)
              AND (@ToDate IS NULL OR td.NGAY_THEO_DAU <= @ToDate)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE
        WHERE rn = 1
        ORDER BY NGAY_THEO_DAU DESC, RECORD_ID;
    END

    -- =========================================================================
    -- SECTION II: HOẠT ĐỘNG TRUYỀN THÔNG
    -- =========================================================================
    -- II.1 Truyền thông lần 1
    ELSE IF @ChiTieuCode = 'II_1'
    BEGIN
        ;WITH CTE AS (
            SELECT 
                kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, hd.MA_TCV, kh.DOI_TUONG_TEXT, 
                hd.NGAY_HOAT_DONG,
                CONVERT(VARCHAR(10), hd.NGAY_HOAT_DONG, 103) AS NGAY_THUC_HIEN,
                N'Chủ đề: ' + ISNULL(hd.CHU_DE, '') AS CHI_TIET,
                ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY hd.NGAY_HOAT_DONG DESC) AS rn
            FROM CD45_HOAT_DONG hd
            INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
            WHERE hd.REPEAT_INSTANCE = 1
              AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
              AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE
        WHERE rn = 1
        ORDER BY NGAY_HOAT_DONG DESC, RECORD_ID;
    END
    -- II.2 Truyền thông lần 2+
    ELSE IF @ChiTieuCode = 'II_2'
    BEGIN
        ;WITH CTE AS (
            SELECT 
                kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, hd.MA_TCV, kh.DOI_TUONG_TEXT, 
                hd.NGAY_HOAT_DONG,
                CONVERT(VARCHAR(10), hd.NGAY_HOAT_DONG, 103) AS NGAY_THUC_HIEN,
                N'Lần ' + CAST(hd.REPEAT_INSTANCE AS VARCHAR) + N' - Chủ đề: ' + ISNULL(hd.CHU_DE, '') AS CHI_TIET,
                ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY hd.NGAY_HOAT_DONG DESC) AS rn
            FROM CD45_HOAT_DONG hd
            INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
            WHERE hd.REPEAT_INSTANCE > 1
              AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
              AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE
        WHERE rn = 1
        ORDER BY NGAY_HOAT_DONG DESC, RECORD_ID;
    END
    -- II.3 Tổng số lượt truyền thông
    ELSE IF @ChiTieuCode = 'II_3'
    BEGIN
        SELECT 
            kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, hd.MA_TCV, kh.DOI_TUONG_TEXT, 
            CONVERT(VARCHAR(10), hd.NGAY_HOAT_DONG, 103) AS NGAY_THUC_HIEN,
            N'Lần ' + CAST(hd.REPEAT_INSTANCE AS VARCHAR) + N' - Chủ đề: ' + ISNULL(hd.CHU_DE, '') AS CHI_TIET
        FROM CD45_HOAT_DONG hd
        INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
        WHERE (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
          AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
        ORDER BY hd.NGAY_HOAT_DONG DESC, kh.RECORD_ID;
    END

    -- =========================================================================
    -- SECTION III: SÀNG LỌC QST VÀ KHÁM, ĐIỀU TRỊ SKTT
    -- =========================================================================
    -- III.1 Sàng lọc QST lần 1 & các mức
    ELSE IF @ChiTieuCode LIKE 'III_1%'
    BEGIN
        DECLARE @FilterMuc1 INT = NULL;
        IF @ChiTieuCode = 'III_1_M1' SET @FilterMuc1 = 1;
        ELSE IF @ChiTieuCode = 'III_1_M2' SET @FilterMuc1 = 2;
        ELSE IF @ChiTieuCode = 'III_1_M3' SET @FilterMuc1 = 3;
        ELSE IF @ChiTieuCode = 'III_1_M4' SET @FilterMuc1 = 4;

        ;WITH CTE AS (
            SELECT 
                kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, qst.MA_TCV, kh.DOI_TUONG_TEXT, 
                qst.NGAY_SANG_LOC,
                CONVERT(VARCHAR(10), qst.NGAY_SANG_LOC, 103) AS NGAY_THUC_HIEN,
                N'Điểm QST: ' + CAST(ISNULL(qst.DIEM_QST, 0) AS VARCHAR) + N' (Mức ' + CAST(ISNULL(qst.MUC_QST, 0) AS VARCHAR) + ')' AS CHI_TIET,
                ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY qst.NGAY_SANG_LOC DESC) AS rn
            FROM CD45_QST qst
            INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID
            WHERE qst.REPEAT_INSTANCE = 1
              AND (@FilterMuc1 IS NULL OR qst.MUC_QST = @FilterMuc1)
              AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
              AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE
        WHERE rn = 1
        ORDER BY NGAY_SANG_LOC DESC, RECORD_ID;
    END
    -- III.2 Sàng lọc lại QST lần 2+ & các mức
    ELSE IF @ChiTieuCode LIKE 'III_2%'
    BEGIN
        DECLARE @FilterMuc2 INT = NULL;
        IF @ChiTieuCode = 'III_2_1' SET @FilterMuc2 = 1;
        ELSE IF @ChiTieuCode = 'III_2_2' SET @FilterMuc2 = 2;
        ELSE IF @ChiTieuCode = 'III_2_3' SET @FilterMuc2 = 3;
        ELSE IF @ChiTieuCode = 'III_2_4' SET @FilterMuc2 = 4;

        ;WITH CTE AS (
            SELECT 
                kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, qst.MA_TCV, kh.DOI_TUONG_TEXT, 
                qst.NGAY_SANG_LOC,
                CONVERT(VARCHAR(10), qst.NGAY_SANG_LOC, 103) AS NGAY_THUC_HIEN,
                N'Lần ' + CAST(qst.REPEAT_INSTANCE AS VARCHAR) + N' - Điểm QST: ' + CAST(ISNULL(qst.DIEM_QST, 0) AS VARCHAR) + N' (Mức ' + CAST(ISNULL(qst.MUC_QST, 0) AS VARCHAR) + ')' AS CHI_TIET,
                ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY qst.NGAY_SANG_LOC DESC) AS rn
            FROM CD45_QST qst
            INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID
            WHERE qst.REPEAT_INSTANCE > 1
              AND (@FilterMuc2 IS NULL OR qst.MUC_QST = @FilterMuc2)
              AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
              AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE
        WHERE rn = 1
        ORDER BY NGAY_SANG_LOC DESC, RECORD_ID;
    END
    -- III.3, III.4, III.4.1, III.4.2, III.4.3, III.5: Khám SKTT
    ELSE IF @ChiTieuCode IN ('III_3', 'III_4', 'III_4_1', 'III_4_2', 'III_4_3', 'III_5')
    BEGIN
        IF @ChiTieuCode IN ('III_3', 'III_4_3') -- Lượt
        BEGIN
            SELECT 
                kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, cd.MA_TCV, kh.DOI_TUONG_TEXT, 
                CONVERT(VARCHAR(10), cd.NGAY_KHAM, 103) AS NGAY_THUC_HIEN,
                N'Lần khám: ' + CAST(ISNULL(cd.LAN_KHAM, 1) AS VARCHAR) + N' - Cơ sở: ' + ISNULL(cd.CO_SO_Y_TE, '') + N' - ' + ISNULL(cd.CHAN_DOAN_CHINH, '') AS CHI_TIET
            FROM CD45_CHAN_DOAN cd
            INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
            WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
              AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV)
              AND (@ChiTieuCode <> 'III_4_3' OR cd.LAN_KHAM > 1)
            ORDER BY cd.NGAY_KHAM DESC, kh.RECORD_ID;
        END
        ELSE -- Số KH
        BEGIN
            ;WITH CTE AS (
                SELECT 
                    kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, cd.MA_TCV, kh.DOI_TUONG_TEXT, 
                    cd.NGAY_KHAM,
                    CONVERT(VARCHAR(10), cd.NGAY_KHAM, 103) AS NGAY_THUC_HIEN,
                    N'Lần khám: ' + CAST(ISNULL(cd.LAN_KHAM, 1) AS VARCHAR) + N' - Cơ sở: ' + ISNULL(cd.CO_SO_Y_TE, '') + N' - ' + ISNULL(cd.CHAN_DOAN_CHINH, '') AS CHI_TIET,
                    ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY cd.NGAY_KHAM DESC) AS rn
                FROM CD45_CHAN_DOAN cd
                INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID
                WHERE (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)
                  AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
                  AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV)
                  AND (@ChiTieuCode <> 'III_4_1' OR cd.LAN_KHAM = 1)
                  AND (@ChiTieuCode <> 'III_4_2' OR cd.LAN_KHAM > 1)
                  AND (@ChiTieuCode <> 'III_5' OR (CHARINDEX(',2,', ',' + ISNULL(cd.HINH_THUC_DIEU_TRI, '') + ',') > 0 OR cd.HINH_THUC_DIEU_TRI LIKE '%2%'))
            )
            SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
            FROM CTE
            WHERE rn = 1
            ORDER BY NGAY_KHAM DESC, RECORD_ID;
        END
    END
    -- III.6 Thẻ BHYT
    ELSE IF @ChiTieuCode = 'III_6'
    BEGIN
        ;WITH CTE AS (
            SELECT 
                kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, htxh.MA_TCV, kh.DOI_TUONG_TEXT, 
                htxh.NGAY_HO_TRO,
                CONVERT(VARCHAR(10), htxh.NGAY_HO_TRO, 103) AS NGAY_THUC_HIEN,
                N'Hỗ trợ thẻ BHYT' AS CHI_TIET,
                ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY htxh.NGAY_HO_TRO DESC) AS rn
            FROM CD45_HO_TRO_XH htxh
            INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID
            WHERE (CHARINDEX(',1,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE '%1%')
              AND (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)
              AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR htxh.MA_TCV = @MaTCV)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE
        WHERE rn = 1
        ORDER BY NGAY_HO_TRO DESC, RECORD_ID;
    END

    -- =========================================================================
    -- SECTION IV: TƯ VẤN CÁ NHÂN VÀ CAN THIỆP NHÓM
    -- =========================================================================
    -- IV.1 Số lượt tư vấn cá nhân
    ELSE IF @ChiTieuCode = 'IV_1'
    BEGIN
        ;WITH CTE_AllTuVan AS (
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV, N'Tư vấn lần 1' AS LOAI FROM CD45_TU_VAN_L1
            UNION ALL
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV, N'Tư vấn lần 2+' AS LOAI FROM CD45_TU_VAN_L2
        )
        SELECT 
            kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, tv.MA_TCV, kh.DOI_TUONG_TEXT, 
            CONVERT(VARCHAR(10), tv.NGAY_TU_VAN, 103) AS NGAY_THUC_HIEN,
            tv.LOAI AS CHI_TIET
        FROM CTE_AllTuVan tv
        INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID
        WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)
          AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV)
        ORDER BY tv.NGAY_TU_VAN DESC, kh.RECORD_ID;
    END
    -- IV.2 Số KH được tư vấn cá nhân & các phân nhóm (1 lần, 2 lần, 3+ lần)
    ELSE IF @ChiTieuCode LIKE 'IV_2%'
    BEGIN
        ;WITH CTE_AllTuVan AS (
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1
            UNION ALL
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2
        ),
        CTE_TV_Grouped AS (
            SELECT tv.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, MAX(tv.MA_TCV) AS MA_TCV, kh.DOI_TUONG_TEXT,
                   COUNT(*) AS SoLanTuVan, MAX(tv.NGAY_TU_VAN) AS NGAY_MOI_NHAT
            FROM CTE_AllTuVan tv
            INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID
            WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)
              AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV)
            GROUP BY tv.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, kh.DOI_TUONG_TEXT
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT,
               CONVERT(VARCHAR(10), NGAY_MOI_NHAT, 103) AS NGAY_THUC_HIEN,
               N'Tổng số lần tư vấn: ' + CAST(SoLanTuVan AS VARCHAR) AS CHI_TIET
        FROM CTE_TV_Grouped
        WHERE (@ChiTieuCode = 'IV_2')
           OR (@ChiTieuCode = 'IV_2_1' AND SoLanTuVan = 1)
           OR (@ChiTieuCode = 'IV_2_2' AND SoLanTuVan = 2)
           OR (@ChiTieuCode = 'IV_2_3' AND SoLanTuVan >= 3)
        ORDER BY NGAY_MOI_NHAT DESC, RECORD_ID;
    END
    -- IV.3, IV.4, IV.5, IV.6: Sinh hoạt nhóm & Can thiệp chữa lành
    ELSE IF @ChiTieuCode IN ('IV_3', 'IV_4', 'IV_5', 'IV_6')
    BEGIN
        IF @ChiTieuCode IN ('IV_3', 'IV_5') -- Lượt
        BEGIN
            SELECT 
                kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, hd.MA_TCV, kh.DOI_TUONG_TEXT, 
                CONVERT(VARCHAR(10), hd.NGAY_HOAT_DONG, 103) AS NGAY_THUC_HIEN,
                CASE hd.LOAI_DV WHEN 2 THEN N'Sinh hoạt nhóm' WHEN 3 THEN N'Vòng tròn chia sẻ' WHEN 4 THEN N'Trị liệu nghệ thuật' ELSE N'Khác' END 
                + ISNULL(' - ' + hd.CHU_DE, '') AS CHI_TIET
            FROM CD45_HOAT_DONG hd
            INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
            WHERE (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
              AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
              AND ((@ChiTieuCode = 'IV_3' AND hd.LOAI_DV = 2) OR (@ChiTieuCode = 'IV_5' AND hd.LOAI_DV IN (3, 4)))
            ORDER BY hd.NGAY_HOAT_DONG DESC, kh.RECORD_ID;
        END
        ELSE -- Số KH
        BEGIN
            ;WITH CTE AS (
                SELECT 
                    kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, hd.MA_TCV, kh.DOI_TUONG_TEXT, 
                    hd.NGAY_HOAT_DONG,
                    CONVERT(VARCHAR(10), hd.NGAY_HOAT_DONG, 103) AS NGAY_THUC_HIEN,
                    CASE hd.LOAI_DV WHEN 2 THEN N'Sinh hoạt nhóm' WHEN 3 THEN N'Vòng tròn chia sẻ' WHEN 4 THEN N'Trị liệu nghệ thuật' ELSE N'Khác' END 
                    + ISNULL(' - ' + hd.CHU_DE, '') AS CHI_TIET,
                    ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY hd.NGAY_HOAT_DONG DESC) AS rn
                FROM CD45_HOAT_DONG hd
                INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
                WHERE (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
                  AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
                  AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
                  AND ((@ChiTieuCode = 'IV_4' AND hd.LOAI_DV = 2) OR (@ChiTieuCode = 'IV_6' AND hd.LOAI_DV IN (3, 4)))
            )
            SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
            FROM CTE
            WHERE rn = 1
            ORDER BY NGAY_HOAT_DONG DESC, RECORD_ID;
        END
    END

    -- =========================================================================
    -- SECTION V: CAN THIỆP ONLINE
    -- =========================================================================
    ELSE IF @ChiTieuCode IN ('V_1', 'V_2')
    BEGIN
        SELECT TOP 0
            kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, '' AS MA_TCV, kh.DOI_TUONG_TEXT, 
            CONVERT(VARCHAR(10), kh.NGAY_THAM_GIA, 103) AS NGAY_THUC_HIEN,
            N'Can thiệp online' AS CHI_TIET
        FROM #TmpKH kh;
    END

    -- =========================================================================
    -- SECTION VI: DỊCH VỤ CHUYỂN GỬI KHÁC
    -- =========================================================================
    ELSE IF @ChiTieuCode LIKE 'VI_1%'
    BEGIN
        DECLARE @TenDV NVARCHAR(100) = N'Chuyển gửi dịch vụ';
        IF @ChiTieuCode = 'VI_1_BHYT' SET @TenDV = N'Hỗ trợ thẻ BHYT';
        ELSE IF @ChiTieuCode = 'VI_1_METHADONE' SET @TenDV = N'Hỗ trợ chi phí điều trị Methadone';
        ELSE IF @ChiTieuCode = 'VI_1_HIV' SET @TenDV = N'Tư vấn và xét nghiệm nhanh HIV';
        ELSE IF @ChiTieuCode = 'VI_1_STIS' SET @TenDV = N'Khám/điều trị STIs';
        ELSE IF @ChiTieuCode = 'VI_1_HEPATITIS' SET @TenDV = N'XN/Điều trị Viêm gan B,C';
        ELSE IF @ChiTieuCode = 'VI_1_OTHER' SET @TenDV = N'Dịch vụ hỗ trợ xã hội & y tế khác';

        ;WITH CTE AS (
            SELECT 
                kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, htxh.MA_TCV, kh.DOI_TUONG_TEXT, 
                htxh.NGAY_HO_TRO,
                CONVERT(VARCHAR(10), htxh.NGAY_HO_TRO, 103) AS NGAY_THUC_HIEN,
                @TenDV AS CHI_TIET,
                ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY htxh.NGAY_HO_TRO DESC) AS rn
            FROM CD45_HO_TRO_XH htxh
            INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID
            WHERE (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)
              AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR htxh.MA_TCV = @MaTCV)
              AND (
                  (@ChiTieuCode = 'VI_1')
                  OR (@ChiTieuCode = 'VI_1_BHYT' AND (CHARINDEX(',1,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE '%1%'))
                  OR (@ChiTieuCode = 'VI_1_METHADONE' AND (CHARINDEX(',2,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE '%2%'))
                  OR (@ChiTieuCode = 'VI_1_HIV' AND (CHARINDEX(',10,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE '%10%'))
                  OR (@ChiTieuCode = 'VI_1_STIS' AND htxh.DICH_VU LIKE '%STIs%')
                  OR (@ChiTieuCode = 'VI_1_HEPATITIS' AND htxh.DICH_VU LIKE '%gan%')
                  OR (@ChiTieuCode = 'VI_1_OTHER' AND (
                      CHARINDEX(',3,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                      CHARINDEX(',4,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                      CHARINDEX(',5,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                      CHARINDEX(',6,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                      CHARINDEX(',7,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                      CHARINDEX(',8,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR 
                      CHARINDEX(',9,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0
                  ))
              )
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE
        WHERE rn = 1
        ORDER BY NGAY_HO_TRO DESC, RECORD_ID;
    END

    -- =========================================================================
    -- SECTION VII: PHÁT TÀI LIỆU
    -- =========================================================================
    -- VII.1 Số quyển phát
    ELSE IF @ChiTieuCode = 'VII_1'
    BEGIN
        SELECT 
            kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, hd.MA_TCV, kh.DOI_TUONG_TEXT, 
            CONVERT(VARCHAR(10), hd.NGAY_HOAT_DONG, 103) AS NGAY_THUC_HIEN,
            N'Số quyển phát: ' + CAST(ISNULL(hd.SO_TAI_LIEU, 0) AS VARCHAR) AS CHI_TIET
        FROM CD45_HOAT_DONG hd
        INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
        WHERE ISNULL(hd.SO_TAI_LIEU, 0) > 0
          AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
          AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
        ORDER BY hd.NGAY_HOAT_DONG DESC, kh.RECORD_ID;
    END
    -- VII.2 Số KH nhận tài liệu
    ELSE IF @ChiTieuCode = 'VII_2'
    BEGIN
        ;WITH CTE AS (
            SELECT 
                kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, hd.MA_TCV, kh.DOI_TUONG_TEXT, 
                hd.NGAY_HOAT_DONG,
                CONVERT(VARCHAR(10), hd.NGAY_HOAT_DONG, 103) AS NGAY_THUC_HIEN,
                N'Số quyển phát: ' + CAST(ISNULL(hd.SO_TAI_LIEU, 0) AS VARCHAR) AS CHI_TIET,
                ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY hd.NGAY_HOAT_DONG DESC) AS rn
            FROM CD45_HOAT_DONG hd
            INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
            WHERE ISNULL(hd.SO_TAI_LIEU, 0) > 0
              AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
              AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE
        WHERE rn = 1
        ORDER BY NGAY_HOAT_DONG DESC, RECORD_ID;
    END

    -- Default fallback
    ELSE
    BEGIN
        SELECT 
            kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, '' AS MA_TCV, kh.DOI_TUONG_TEXT, 
            CONVERT(VARCHAR(10), kh.NGAY_THAM_GIA, 103) AS NGAY_THUC_HIEN,
            N'Thông tin khách hàng' AS CHI_TIET
        FROM #TmpKH kh
        ORDER BY kh.NGAY_THAM_GIA DESC, kh.RECORD_ID;
    END

    DROP TABLE #TmpKH;
END
