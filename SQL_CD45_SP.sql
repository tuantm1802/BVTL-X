USE BVTL_REPORTING_DEV;
GO

IF OBJECT_ID('SP_CD45_GetBaoCao', 'P') IS NOT NULL DROP PROC SP_CD45_GetBaoCao;
GO
CREATE PROC SP_CD45_GetBaoCao
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @CityCode VARCHAR(10) = NULL,
    @MaNhom VARCHAR(20) = NULL,
    @MaTCV VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Temp Table for Results
    DECLARE @TmpResult TABLE (
        STT_Sort INT IDENTITY(1,1),
        STT VARCHAR(10),
        ChiTieu NVARCHAR(500),
        Tong INT DEFAULT 0,
        PUD INT DEFAULT 0,
        PLHIV INT DEFAULT 0,
        TG INT DEFAULT 0,
        SW INT DEFAULT 0,
        MSM INT DEFAULT 0,
        IsBold BIT DEFAULT 0
    );

    -- 2. Base Customers mapping
    SELECT RECORD_ID, DOI_TUONG, CITY_CODE
    INTO #TmpKH
    FROM CD45_KH
    WHERE (@CityCode IS NULL OR CITY_CODE = @CityCode);

    -- ==========================================
    -- I. THÔNG TIN CHUNG
    -- ==========================================
    INSERT INTO @TmpResult(STT, ChiTieu, IsBold) VALUES ('I', N'THÔNG TIN CHUNG', 1);

    -- 1. Tổng KH chăm sóc từ đầu dự án
    ;WITH CTE_I1 AS (
        SELECT DOI_TUONG, COUNT(DISTINCT RECORD_ID) AS Cnt
        FROM #TmpKH
        GROUP BY DOI_TUONG
    )
    INSERT INTO @TmpResult(STT, ChiTieu, PUD, PLHIV, TG, MSM, SW, Tong)
    SELECT '1', N'Tổng số KH được chăm sóc từ đầu dự án',
        ISNULL(SUM(CASE WHEN DOI_TUONG = 1 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 2 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 3 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 4 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 5 THEN Cnt END),0),
        ISNULL(SUM(Cnt),0)
    FROM CTE_I1;

    -- 2. Tổng KH chăm sóc trong kỳ
    ;WITH CTE_Act AS (
        SELECT RECORD_ID FROM CD45_KH WHERE NGAY_THAM_GIA BETWEEN ISNULL(@FromDate, '1900-01-01') AND ISNULL(@ToDate, '2099-12-31')
        UNION SELECT RECORD_ID FROM CD45_HOAT_DONG WHERE NGAY_HOAT_DONG BETWEEN ISNULL(@FromDate, '1900-01-01') AND ISNULL(@ToDate, '2099-12-31')
        UNION SELECT RECORD_ID FROM CD45_QST WHERE NGAY_SANG_LOC BETWEEN ISNULL(@FromDate, '1900-01-01') AND ISNULL(@ToDate, '2099-12-31')
    ), CTE_I2 AS (
        SELECT k.DOI_TUONG, COUNT(DISTINCT a.RECORD_ID) AS Cnt
        FROM CTE_Act a JOIN #TmpKH k ON a.RECORD_ID = k.RECORD_ID
        GROUP BY k.DOI_TUONG
    )
    INSERT INTO @TmpResult(STT, ChiTieu, PUD, PLHIV, TG, MSM, SW, Tong)
    SELECT '2', N'Tổng số KH được chăm sóc trong kỳ báo cáo',
        ISNULL(SUM(CASE WHEN DOI_TUONG = 1 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 2 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 3 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 4 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 5 THEN Cnt END),0),
        ISNULL(SUM(Cnt),0)
    FROM CTE_I2;

    -- 3. Số KH mất dấu
    ;WITH CTE_I3 AS (
        SELECT k.DOI_TUONG, COUNT(DISTINCT a.RECORD_ID) AS Cnt
        FROM CD45_THEO_DAU a JOIN #TmpKH k ON a.RECORD_ID = k.RECORD_ID
        WHERE a.MAT_DAU = 1 AND a.NGAY_THEO_DAU BETWEEN ISNULL(@FromDate, '1900-01-01') AND ISNULL(@ToDate, '2099-12-31')
        GROUP BY k.DOI_TUONG
    )
    INSERT INTO @TmpResult(STT, ChiTieu, PUD, PLHIV, TG, MSM, SW, Tong)
    SELECT '3', N'Số KH mất dấu trong kỳ báo cáo',
        ISNULL(SUM(CASE WHEN DOI_TUONG = 1 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 2 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 3 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 4 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 5 THEN Cnt END),0), ISNULL(SUM(Cnt),0)
    FROM CTE_I3;

    -- ==========================================
    -- II. HOẠT ĐỘNG TRUYỀN THÔNG (F2)
    -- ==========================================
    INSERT INTO @TmpResult(STT, ChiTieu, IsBold) VALUES ('II', N'HOẠT ĐỘNG TRUYỀN THÔNG', 1);

    -- Lần 1
    ;WITH CTE_II1 AS (
        SELECT k.DOI_TUONG, COUNT(DISTINCT a.RECORD_ID) AS Cnt FROM CD45_HOAT_DONG a JOIN #TmpKH k ON a.RECORD_ID = k.RECORD_ID
        WHERE a.REPEAT_INSTANCE = 1 AND a.NGAY_HOAT_DONG BETWEEN ISNULL(@FromDate, '1900-01-01') AND ISNULL(@ToDate, '2099-12-31')
        GROUP BY k.DOI_TUONG
    )
    INSERT INTO @TmpResult(STT, ChiTieu, PUD, PLHIV, TG, MSM, SW, Tong)
    SELECT '1', N'Số KH được tham gia truyền thông lần 1',
        ISNULL(SUM(CASE WHEN DOI_TUONG = 1 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 2 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 3 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 4 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 5 THEN Cnt END),0), ISNULL(SUM(Cnt),0)
    FROM CTE_II1;

    -- Lần 2+
    ;WITH CTE_II2 AS (
        SELECT k.DOI_TUONG, COUNT(DISTINCT a.RECORD_ID) AS Cnt FROM CD45_HOAT_DONG a JOIN #TmpKH k ON a.RECORD_ID = k.RECORD_ID
        WHERE a.REPEAT_INSTANCE > 1 AND a.NGAY_HOAT_DONG BETWEEN ISNULL(@FromDate, '1900-01-01') AND ISNULL(@ToDate, '2099-12-31')
        GROUP BY k.DOI_TUONG
    )
    INSERT INTO @TmpResult(STT, ChiTieu, PUD, PLHIV, TG, MSM, SW, Tong)
    SELECT '2', N'Số KH được tham gia truyền thông lần 2 trở lên',
        ISNULL(SUM(CASE WHEN DOI_TUONG = 1 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 2 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 3 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 4 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 5 THEN Cnt END),0), ISNULL(SUM(Cnt),0)
    FROM CTE_II2;

    -- Tổng lượt
    ;WITH CTE_II3 AS (
        SELECT k.DOI_TUONG, COUNT(a.RECORD_ID) AS Cnt FROM CD45_HOAT_DONG a JOIN #TmpKH k ON a.RECORD_ID = k.RECORD_ID
        WHERE a.NGAY_HOAT_DONG BETWEEN ISNULL(@FromDate, '1900-01-01') AND ISNULL(@ToDate, '2099-12-31')
        GROUP BY k.DOI_TUONG
    )
    INSERT INTO @TmpResult(STT, ChiTieu, PUD, PLHIV, TG, MSM, SW, Tong)
    SELECT '3', N'Tổng số lượt KH tham gia truyền thông',
        ISNULL(SUM(CASE WHEN DOI_TUONG = 1 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 2 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 3 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 4 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 5 THEN Cnt END),0), ISNULL(SUM(Cnt),0)
    FROM CTE_II3;

    -- ==========================================
    -- III. SÀNG LỌC QST
    -- ==========================================
    INSERT INTO @TmpResult(STT, ChiTieu, IsBold) VALUES ('III', N'SÀNG LỌC BẰNG HỎI QST VÀ CHUYỂN GỬI', 1);

    -- Lần 1
    ;WITH CTE_III1 AS (
        SELECT k.DOI_TUONG, COUNT(DISTINCT a.RECORD_ID) AS Cnt FROM CD45_QST a JOIN #TmpKH k ON a.RECORD_ID = k.RECORD_ID
        WHERE a.REPEAT_INSTANCE = 1 AND a.NGAY_SANG_LOC BETWEEN ISNULL(@FromDate, '1900-01-01') AND ISNULL(@ToDate, '2099-12-31')
        GROUP BY k.DOI_TUONG
    )
    INSERT INTO @TmpResult(STT, ChiTieu, PUD, PLHIV, TG, MSM, SW, Tong)
    SELECT '1', N'Số KH được sàng lọc bằng hỏi QST lần 1',
        ISNULL(SUM(CASE WHEN DOI_TUONG = 1 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 2 THEN Cnt END),0),
        ISNULL(SUM(CASE WHEN DOI_TUONG = 3 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 4 THEN Cnt END),0), ISNULL(SUM(CASE WHEN DOI_TUONG = 5 THEN Cnt END),0), ISNULL(SUM(Cnt),0)
    FROM CTE_III1;

    SELECT STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold FROM @TmpResult ORDER BY STT_Sort;
END
GO
