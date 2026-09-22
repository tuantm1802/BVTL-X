﻿CREATE OR ALTER PROC SP_CD45_GetBaoCao
    @FromDate    DATE         = NULL,
    @ToDate      DATE         = NULL,
    @CityCode    VARCHAR(100) = NULL,
    @MaNhom      VARCHAR(100) = NULL,
    @MaTCV       VARCHAR(100) = NULL,
    @LoaiBaoCao  VARCHAR(10)  = NULL  -- 'Thang' | 'Quy' | '6T' | '12T' | NULL = hiển thị tất cả
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

    -- Resolve @CityCode mapping (hỗ trợ cả 34 tỉnh mới và 63 tỉnh cũ)
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
    WHERE (@CityCode IS NULL OR @CityCode = '' OR kh.CITY_CODE IN (SELECT Code FROM @MappedCityCodes))
      AND (@MaNhom IS NULL OR @MaNhom = '' OR kh.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom) OR kh.REDCAP_DAG IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom));

    CREATE CLUSTERED INDEX IX_TmpKH_RecId ON #TmpKH(RECORD_ID);

    -- =========================================================================
    -- SECTION I: THÔNG TIN CHUNG
    -- =========================================================================
    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)
    VALUES ('I', N'THÔNG TIN CHUNG', 1, 0, 'SEC_I');

    -- 1. Tổng số KH được chăm sóc từ đầu dự án (F2 ∪ F3 ∪ F6 ∪ F7 ∪ F8 tính đến @ToDate)
    ;WITH CTE_AllTimeCare AS (
        SELECT RECORD_ID FROM CD45_HOAT_DONG 
        WHERE (@ToDate IS NULL OR NGAY_HOAT_DONG <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)
        UNION
        SELECT RECORD_ID FROM CD45_QST 
        WHERE (@ToDate IS NULL OR NGAY_SANG_LOC <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)
        UNION
        SELECT RECORD_ID FROM CD45_CHAN_DOAN 
        WHERE (@ToDate IS NULL OR NGAY_KHAM <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)
        UNION
        SELECT RECORD_ID FROM CD45_TU_VAN_L1 
        WHERE (@ToDate IS NULL OR NGAY_TU_VAN <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)
        UNION
        SELECT RECORD_ID FROM CD45_TU_VAN_L2 
        WHERE (@ToDate IS NULL OR NGAY_TU_VAN <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)
    )
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '1', N'Tổng số KH được chăm sóc từ đầu dự án',
        COUNT(DISTINCT c.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN c.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN c.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN c.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN c.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN c.RECORD_ID END),
        0, 0, 'I_1'
    FROM CTE_AllTimeCare c
    INNER JOIN #TmpKH kh ON c.RECORD_ID = kh.RECORD_ID;

    -- 2. Tổng số KH được chăm sóc trong kỳ báo cáo (F2 ∪ F3 ∪ F6 ∪ F7 ∪ F8 có ngày trong kỳ)
    ;WITH CTE_KyCare AS (
        SELECT RECORD_ID FROM CD45_HOAT_DONG 
        WHERE (@FromDate IS NULL OR NGAY_HOAT_DONG >= @FromDate) AND (@ToDate IS NULL OR NGAY_HOAT_DONG <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)
        UNION
        SELECT RECORD_ID FROM CD45_QST 
        WHERE (@FromDate IS NULL OR NGAY_SANG_LOC >= @FromDate) AND (@ToDate IS NULL OR NGAY_SANG_LOC <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)
        UNION
        SELECT RECORD_ID FROM CD45_CHAN_DOAN 
        WHERE (@FromDate IS NULL OR NGAY_KHAM >= @FromDate) AND (@ToDate IS NULL OR NGAY_KHAM <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)
        UNION
        SELECT RECORD_ID FROM CD45_TU_VAN_L1 
        WHERE (@FromDate IS NULL OR NGAY_TU_VAN >= @FromDate) AND (@ToDate IS NULL OR NGAY_TU_VAN <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)
        UNION
        SELECT RECORD_ID FROM CD45_TU_VAN_L2 
        WHERE (@FromDate IS NULL OR NGAY_TU_VAN >= @FromDate) AND (@ToDate IS NULL OR NGAY_TU_VAN <= @ToDate)
          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)
    )
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '2', N'Tổng số KH được chăm sóc trong kỳ báo cáo',
        COUNT(DISTINCT c.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN c.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN c.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN c.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN c.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN c.RECORD_ID END),
        0, 0, 'I_2'
    FROM CTE_KyCare c
    INNER JOIN #TmpKH kh ON c.RECORD_ID = kh.RECORD_ID;

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

    -- 1. Lần 1 (VR-06a: Xếp hạng thứ tự độc lập theo LOAI_DV = 1)
    ;WITH CTE_TruyenThong AS (
        SELECT hd.RECORD_ID, hd.NGAY_HOAT_DONG, hd.MA_TCV,
               ROW_NUMBER() OVER (PARTITION BY hd.RECORD_ID ORDER BY hd.NGAY_HOAT_DONG, hd.REPEAT_INSTANCE) AS TT_Order
        FROM CD45_HOAT_DONG hd
        WHERE hd.LOAI_DV = 1
    )
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '1', N'Số KH được tham gia truyền thông lần 1',
        COUNT(DISTINCT tt.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN tt.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN tt.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN tt.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN tt.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN tt.RECORD_ID END),
        0, 0, 'II_1'
    FROM CTE_TruyenThong tt
    INNER JOIN #TmpKH kh ON tt.RECORD_ID = kh.RECORD_ID
    WHERE tt.TT_Order = 1
      AND (@FromDate IS NULL OR tt.NGAY_HOAT_DONG >= @FromDate)
      AND (@ToDate IS NULL OR tt.NGAY_HOAT_DONG <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR tt.MA_TCV = @MaTCV);

    -- 2. Lần 2+ (VR-06a: Xếp hạng thứ tự độc lập theo LOAI_DV = 1)
    ;WITH CTE_TruyenThong AS (
        SELECT hd.RECORD_ID, hd.NGAY_HOAT_DONG, hd.MA_TCV,
               ROW_NUMBER() OVER (PARTITION BY hd.RECORD_ID ORDER BY hd.NGAY_HOAT_DONG, hd.REPEAT_INSTANCE) AS TT_Order
        FROM CD45_HOAT_DONG hd
        WHERE hd.LOAI_DV = 1
    )
    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold, IndentLevel, Code)
    SELECT 
        '2', N'Số KH được tham gia truyền thông lần 2',
        COUNT(DISTINCT tt.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN tt.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN tt.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN tt.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN tt.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN tt.RECORD_ID END),
        0, 0, 'II_2'
    FROM CTE_TruyenThong tt
    INNER JOIN #TmpKH kh ON tt.RECORD_ID = kh.RECORD_ID
    WHERE tt.TT_Order > 1
      AND (@FromDate IS NULL OR tt.NGAY_HOAT_DONG >= @FromDate)
      AND (@ToDate IS NULL OR tt.NGAY_HOAT_DONG <= @ToDate)
      AND (@MaTCV IS NULL OR @MaTCV = '' OR tt.MA_TCV = @MaTCV);

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
    WHERE hd.LOAI_DV = 1
      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
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
    LEFT JOIN (
        CD45_QST qst INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID
    ) ON qst.MUC_QST = m.Muc AND qst.REPEAT_INSTANCE = 1
        AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
        AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
        AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)
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
    LEFT JOIN (
        CD45_QST qst INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID
    ) ON qst.MUC_QST = m.Muc AND qst.REPEAT_INSTANCE > 1
        AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)
        AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
        AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)
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
    WHERE (CHARINDEX(',1,', ',' + REPLACE(ISNULL(htxh.DICH_VU, ''), ' ', '') + ',') > 0 
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
        COUNT(DISTINCT kh.RECORD_ID),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN kh.RECORD_ID END),
        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN kh.RECORD_ID END),
        0, 1, dv.SubCode
    FROM @DvList dv
    LEFT JOIN (
        CD45_HO_TRO_XH htxh
        INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID
    ) ON (
            (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)
            AND (@ToDate IS NULL OR htxh.NGAY_HO_TRO <= @ToDate)
            AND (@MaTCV IS NULL OR @MaTCV = '' OR htxh.MA_TCV = @MaTCV)
            AND (
                (dv.CodeStr = '1' AND (CHARINDEX(',1,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%BHYT%' OR htxh.DICH_VU LIKE N'%bảo hiểm%'))
                OR (dv.CodeStr = '2' AND (CHARINDEX(',2,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%Methadone%'))
                OR (dv.CodeStr = '10' AND (CHARINDEX(',10,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%HIV%'))
                OR (dv.CodeStr = 'STIS' AND (htxh.DICH_VU LIKE '%STIs%' OR CHARINDEX(',stis,', ',' + LOWER(ISNULL(htxh.DICH_VU, '')) + ',') > 0))
                OR (dv.CodeStr = 'GAN' AND (htxh.DICH_VU LIKE '%gan%' OR CHARINDEX(',gan,', ',' + LOWER(ISNULL(htxh.DICH_VU, '')) + ',') > 0))
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
        )
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

    -- ★ FILTER THEO CẤU HÌNH CHỈ TIÊU BÁO CÁO ★
    -- Khi @LoaiBaoCao được truyền vào ('Thang', 'Quy', '6T', '12T'),
    -- xóa các chỉ tiêu không được cấu hình cho kỳ đó khỏi kết quả trả về.
    -- Khi @LoaiBaoCao = NULL (Tùy chọn ngày), giữ nguyên toàn bộ chỉ tiêu.
    IF @LoaiBaoCao IS NOT NULL AND @LoaiBaoCao <> ''
    BEGIN
        -- Bước 1: Xóa các chỉ tiêu không được cấu hình cho kỳ này
        DELETE r FROM @TmpResult r
        WHERE r.Code IS NOT NULL
          AND r.Code NOT LIKE 'SEC_%'   -- Giữ section headers, xử lý riêng bên dưới
          AND NOT EXISTS (
            SELECT 1 FROM dbo.CD45_BCTIEU_CAU_HINH c
            WHERE c.ChiTieuCode = r.Code
              AND c.IsActive = 1
              AND (
                    (@LoaiBaoCao = 'Thang' AND c.HienThi_Thang = 1)
                 OR (@LoaiBaoCao = 'Quy'   AND c.HienThi_Quy   = 1)
                 OR (@LoaiBaoCao = '6T'    AND c.HienThi_6T    = 1)
                 OR (@LoaiBaoCao = '12T'   AND c.HienThi_12T   = 1)
                  )
          );

        -- Bước 2: Ẩn section header nếu không còn chỉ tiêu con nào
        DELETE r FROM @TmpResult r
        WHERE r.Code LIKE 'SEC_%'
          AND NOT EXISTS (
            SELECT 1 FROM @TmpResult r2
            WHERE r2.Code NOT LIKE 'SEC_%'
              AND r2.Code IS NOT NULL
              AND LEFT(r2.Code, CHARINDEX('_', r2.Code + '_') - 1) =
                  REPLACE(r.Code, 'SEC_', '')
          );
    END

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

GO



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

    -- Resolve @CityCode mapping (hỗ trợ cả 34 tỉnh mới và 63 tỉnh cũ)
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
        ISNULL((SELECT COUNT(*) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID), 0) AS TongLuotKhamSKTT,
        ISNULL((SELECT COUNT(DISTINCT tv.RECORD_ID) FROM CD45_TU_VAN_L1 tv INNER JOIN #TmpKH k ON tv.RECORD_ID = k.RECORD_ID), 0) AS TongTuVanL1,
        ISNULL((SELECT COUNT(DISTINCT htxh.RECORD_ID) FROM CD45_HO_TRO_XH htxh INNER JOIN #TmpKH k ON htxh.RECORD_ID = k.RECORD_ID), 0) AS TongHoTroXH,
        ISNULL((SELECT SUM(hd.SO_TAI_LIEU) FROM CD45_HOAT_DONG hd INNER JOIN #TmpKH k ON hd.RECORD_ID = k.RECORD_ID), 0) AS TongTaiLieuPhat,
        ISNULL((SELECT MAX(NGAY_SYNC) FROM CD45_KH), GETDATE()) AS LastSyncTime;

    -- CTE Lấy lần sàng lọc QST mới nhất của mỗi KH
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
    -- 2. PHÂN TÍCH QST THEO NHÓM ĐÍCH (DOI_TUONG) - BẢNG 1
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
        COUNT(DISTINCT CASE WHEN (CHARINDEX(',1,', ',' + REPLACE(ISNULL(htxh.DICH_VU, ''), ' ', '') + ',') > 0 OR htxh.DICH_VU LIKE N'%BHYT%' OR htxh.DICH_VU LIKE N'%bảo hiểm%') THEN htxh.RECORD_ID END) AS HoTroBHYT,
        COUNT(DISTINCT CASE WHEN (CHARINDEX(',2,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%Methadone%') THEN htxh.RECORD_ID END) AS HoTroMethadone,
        COUNT(DISTINCT CASE WHEN (CHARINDEX(',10,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%HIV%') THEN htxh.RECORD_ID END) AS XetNghiemHIV,
        COUNT(DISTINCT CASE WHEN htxh.DICH_VU LIKE N'%STIs%' THEN htxh.RECORD_ID END) AS STIs,
        COUNT(DISTINCT CASE WHEN htxh.DICH_VU LIKE N'%gan%' THEN htxh.RECORD_ID END) AS ViemGan
    FROM CD45_HO_TRO_XH htxh
    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID;

    DROP TABLE #TmpKH;
END

GO



-- =========================================================================

-- SP_CD45_GetDrillDown: Phục vụ Drill Down Chi tiết Báo cáo CD45

-- =========================================================================

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

    -- Resolve @CityCode mapping (hỗ trợ cả 34 tỉnh mới và 63 tỉnh cũ)
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

    -- =========================================================================
    -- SECTION I: THÔNG TIN CHUNG
    -- =========================================================================    -- 1. Tổng số KH từ đầu dự án (F2 ∪ F3 ∪ F6 ∪ F7 ∪ F8 tính đến @ToDate)
    IF @ChiTieuCode = 'I_1'
    BEGIN
        ;WITH CTE_AllCare AS (
            SELECT hd.RECORD_ID, hd.MA_TCV, hd.NGAY_HOAT_DONG AS ACT_DATE, N'Truyền thông/hoạt động' AS CHI_TIET
            FROM CD45_HOAT_DONG hd
            WHERE (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
            UNION ALL
            SELECT qst.RECORD_ID, qst.MA_TCV, qst.NGAY_SANG_LOC AS ACT_DATE, N'Sàng lọc QST' AS CHI_TIET
            FROM CD45_QST qst
            WHERE (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)
            UNION ALL
            SELECT cd.RECORD_ID, cd.MA_TCV, cd.NGAY_KHAM AS ACT_DATE, N'Khám SKTT' AS CHI_TIET
            FROM CD45_CHAN_DOAN cd
            WHERE (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV)
            UNION ALL
            SELECT tv1.RECORD_ID, tv1.MA_TCV, tv1.NGAY_TU_VAN AS ACT_DATE, N'Tư vấn lần 1' AS CHI_TIET
            FROM CD45_TU_VAN_L1 tv1
            WHERE (@ToDate IS NULL OR tv1.NGAY_TU_VAN <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR tv1.MA_TCV = @MaTCV)
            UNION ALL
            SELECT tv2.RECORD_ID, tv2.MA_TCV, tv2.NGAY_TU_VAN AS ACT_DATE, N'Tư vấn lần 2+' AS CHI_TIET
            FROM CD45_TU_VAN_L2 tv2
            WHERE (@ToDate IS NULL OR tv2.NGAY_TU_VAN <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR tv2.MA_TCV = @MaTCV)
        ),
        CTE_Rank AS (
            SELECT 
                kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, c.MA_TCV, kh.DOI_TUONG_TEXT,
                c.ACT_DATE,
                CONVERT(VARCHAR(10), c.ACT_DATE, 103) AS NGAY_THUC_HIEN,
                c.CHI_TIET,
                ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY c.ACT_DATE DESC) AS rn
            FROM CTE_AllCare c
            INNER JOIN #TmpKH kh ON c.RECORD_ID = kh.RECORD_ID
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE_Rank
        WHERE rn = 1
        ORDER BY ACT_DATE DESC, RECORD_ID;
    END
    -- 2. Tổng số KH trong kỳ (F2 ∪ F3 ∪ F6 ∪ F7 ∪ F8 có ngày trong kỳ)
    ELSE IF @ChiTieuCode = 'I_2'
    BEGIN
        ;WITH CTE_Act AS (
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
        ),
        CTE_Rank AS (
            SELECT 
                RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, ACT_DATE,
                CONVERT(VARCHAR(10), ACT_DATE, 103) AS NGAY_THUC_HIEN,
                CHI_TIET,
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
              AND hd.LOAI_DV = 1
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
              AND hd.LOAI_DV = 1
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
        WHERE hd.LOAI_DV = 1
          AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
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
                N'Lần khám: ' + CAST(ISNULL(cd.LAN_KHAM, 1) AS VARCHAR) + N' - Cơ sở: ' + ISNULL(cd.CO_SO_Y_TE, '') + CASE WHEN cd.CHAN_DOAN_CHINH IS NOT NULL AND cd.CHAN_DOAN_CHINH <> '' THEN N' - Chẩn đoán: ' + cd.CHAN_DOAN_CHINH ELSE N'' END AS CHI_TIET
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
                    N'Lần khám: ' + CAST(ISNULL(cd.LAN_KHAM, 1) AS VARCHAR) + N' - Cơ sở: ' + ISNULL(cd.CO_SO_Y_TE, '') + CASE WHEN cd.CHAN_DOAN_CHINH IS NOT NULL AND cd.CHAN_DOAN_CHINH <> '' THEN N' - Chẩn đoán: ' + cd.CHAN_DOAN_CHINH ELSE N'' END AS CHI_TIET,
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
            WHERE (CHARINDEX(',1,', ',' + REPLACE(ISNULL(htxh.DICH_VU, ''), ' ', '') + ',') > 0 OR htxh.DICH_VU LIKE N'%BHYT%' OR htxh.DICH_VU LIKE N'%bảo hiểm%')
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
                  OR (@ChiTieuCode = 'VI_1_BHYT' AND (CHARINDEX(',1,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%BHYT%' OR htxh.DICH_VU LIKE N'%bảo hiểm%'))
                  OR (@ChiTieuCode = 'VI_1_METHADONE' AND (CHARINDEX(',2,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%Methadone%'))
                  OR (@ChiTieuCode = 'VI_1_HIV' AND (CHARINDEX(',10,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%HIV%'))
                  OR (@ChiTieuCode = 'VI_1_STIS' AND (htxh.DICH_VU LIKE '%STIs%' OR CHARINDEX(',stis,', ',' + LOWER(ISNULL(htxh.DICH_VU, '')) + ',') > 0))
                  OR (@ChiTieuCode = 'VI_1_HEPATITIS' AND (htxh.DICH_VU LIKE '%gan%' OR CHARINDEX(',gan,', ',' + LOWER(ISNULL(htxh.DICH_VU, '')) + ',') > 0))
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
            N'Số quyển phát: ' + CAST(ISNULL(hd.SO_TAI_LIEU, 0) AS VARCHAR) AS CHI_TIET,
            ISNULL(hd.SO_TAI_LIEU, 0) AS SO_LUONG
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
                N'Khách hàng nhận tài liệu (' + CAST(ISNULL(hd.SO_TAI_LIEU, 0) AS VARCHAR) + N' quyển)' AS CHI_TIET,
                ISNULL(hd.SO_TAI_LIEU, 0) AS SO_LUONG,
                ROW_NUMBER() OVER(PARTITION BY kh.RECORD_ID ORDER BY hd.NGAY_HOAT_DONG DESC) AS rn
            FROM CD45_HOAT_DONG hd
            INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID
            WHERE ISNULL(hd.SO_TAI_LIEU, 0) > 0
              AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)
              AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)
              AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET, SO_LUONG
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

GO



