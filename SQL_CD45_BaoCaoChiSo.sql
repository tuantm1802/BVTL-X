-- ============================================================================
-- SCRIPT TRIỂN KHAI BÁO CÁO CHỈ SỐ DỰ ÁN DREAMH (CD45)
-- An toàn tuyệt đối: Chỉ bổ sung cột nullable, bảng mới, SP mới, Menu mới.
-- Không chỉnh sửa các Stored Procedure hay bảng cấu hình báo cáo đang chạy.
-- ============================================================================

SET NOCOUNT ON;

-- 1. Bổ sung cột vào CD45_TUAN_THU (lưu Câu 5 Phiếu F5: f5_feeling & f5_feeling_sign)
IF COL_LENGTH('dbo.CD45_TUAN_THU', 'TINH_TRANG_SKTT') IS NULL
BEGIN
    ALTER TABLE dbo.CD45_TUAN_THU ADD TINH_TRANG_SKTT TINYINT NULL;
    PRINT N'Đã thêm cột TINH_TRANG_SKTT vào CD45_TUAN_THU';
END

IF COL_LENGTH('dbo.CD45_TUAN_THU', 'DAU_HIEU_NGUY_CO') IS NULL
BEGIN
    ALTER TABLE dbo.CD45_TUAN_THU ADD DAU_HIEU_NGUY_CO BIT NULL;
    PRINT N'Đã thêm cột DAU_HIEU_NGUY_CO vào CD45_TUAN_THU';
END
GO

-- 2. Bổ sung cột vào CD45_NHOM_TCV (Giới tính TCV & Cờ tham gia xây dựng mô hình Năm 1)
IF COL_LENGTH('dbo.CD45_NHOM_TCV', 'GIOI_TINH') IS NULL
BEGIN
    ALTER TABLE dbo.CD45_NHOM_TCV ADD GIOI_TINH TINYINT NULL; -- 1=Nam, 2=Nữ, 3=Khác
    PRINT N'Đã thêm cột GIOI_TINH vào CD45_NHOM_TCV';
END

IF COL_LENGTH('dbo.CD45_NHOM_TCV', 'LA_TCV_NAM_1') IS NULL
BEGIN
    ALTER TABLE dbo.CD45_NHOM_TCV ADD LA_TCV_NAM_1 BIT NULL CONSTRAINT DF_CD45_NHOM_TCV_LA_TCV_NAM_1 DEFAULT 1;
    PRINT N'Đã thêm cột LA_TCV_NAM_1 vào CD45_NHOM_TCV';
END
GO

UPDATE dbo.CD45_NHOM_TCV
SET LA_TCV_NAM_1 = 1
WHERE LA_TCV_NAM_1 IS NULL;
GO

-- 3. Tạo bảng cấu hình Chỉ số Quản trị Dự án (KQ1.1 override, KQ1.3, KQ5.1 - KQ5.3, GF Denom)
IF OBJECT_ID('dbo.CD45_CHI_SO_DU_AN_CONFIG', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CD45_CHI_SO_DU_AN_CONFIG (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        NAM INT NOT NULL CONSTRAINT DF_CD45_CSDA_NAM DEFAULT 2026,
        MA_KY VARCHAR(20) NOT NULL, -- '6T', '12T', 'ALL'
        MA_THAM_SO VARCHAR(50) NOT NULL,
        CHI_SO_CODE VARCHAR(30) NULL,
        TEN_THAM_SO NVARCHAR(350) NOT NULL,
        GIA_TRI INT NULL,
        GHI_CHU NVARCHAR(500) NULL,
        SORT_ORDER INT NOT NULL DEFAULT 0,
        UPDATED_AT DATETIME NULL,
        UPDATED_BY VARCHAR(100) NULL
    );

    CREATE UNIQUE INDEX UX_CD45_CSDA_Key ON dbo.CD45_CHI_SO_DU_AN_CONFIG(NAM, MA_KY, MA_THAM_SO);
    PRINT N'Đã tạo bảng CD45_CHI_SO_DU_AN_CONFIG';
END
GO

-- Seed dữ liệu mặc định cho các kỳ (2026: 6T, 12T, ALL)
;WITH SeedPeriods AS (
    SELECT 2026 AS NAM, '6T' AS MA_KY
    UNION ALL SELECT 2026, '12T'
    UNION ALL SELECT 2026, 'ALL'
),
SeedParams AS (
    SELECT 'CFG_GF_DENOM' AS MA_THAM_SO, 'MTCT1.2' AS CHI_SO_CODE,
           N'Tổng số KH tiếp cận bởi dự án Quỹ Toàn Cầu tại 6 tỉnh (Để 0 để tự lấy tổng F1)' AS TEN_THAM_SO,
           0 AS GIA_TRI,
           N'Nếu > 0 và không lọc theo tỉnh/nhóm, hệ thống sẽ dùng làm mẫu số cho MTCT1.2' AS GHI_CHU,
           1 AS SORT_ORDER
    UNION ALL SELECT 'CFG_KQ1_1_OVERRIDE', 'MTCT-KQ1.1',
           N'Số đơn vị áp dụng mô hình mới (Để 0 để tự đếm số CBO hoạt động; nhập nếu gộp cả CSYT)',
           0,
           N'Mặc định tự động đếm các CBO/DNXH có phát sinh dữ liệu hoạt động trong kỳ',
           2
    UNION ALL SELECT 'CFG_KQ1_3', 'MTCT-KQ1.3',
           N'Số lượng chuyên gia y tế tham gia xây dựng mô hình can thiệp và bộ công cụ dự án',
           30,
           N'Bao gồm chuyên gia tuyến TW và các bác sĩ chuyên khoa tâm thần tham gia dự án',
           3
    UNION ALL SELECT 'CFG_KQ5_1', 'MTCT-KQ5.1',
           N'Số lượng đối tác (Cơ sở y tế / Đơn vị) đã ký Biên bản ghi nhớ (MOU)',
           9,
           N'Các bệnh viện tâm thần / phòng khám chuyên khoa đã ký MOU phối hợp tại 6 tỉnh',
           4
    UNION ALL SELECT 'CFG_KQ5_2_TW', 'MTCT-KQ5.2',
           N'Đối tác / bên liên quan tham gia dự án: Cấp Trung ương',
           0,
           N'Viện SKTT, Cục phòng chống HIV/AIDS, Hội chuyên ngành...',
           5
    UNION ALL SELECT 'CFG_KQ5_2_DP', 'MTCT-KQ5.2',
           N'Đối tác / bên liên quan tham gia dự án: Cấp Địa phương (6 tỉnh)',
           0,
           N'Sở Y tế, CDC 6 tỉnh, Sở LĐTBXH, chính quyền địa phương...',
           6
    UNION ALL SELECT 'CFG_KQ5_2_YT', 'MTCT-KQ5.2',
           N'Đối tác theo lĩnh vực: Y tế',
           0,
           N'Bệnh viện tâm thần, trung tâm y tế, phòng khám chuyên khoa',
           7
    UNION ALL SELECT 'CFG_KQ5_2_CQ', 'MTCT-KQ5.2',
           N'Đối tác theo lĩnh vực: Chính quyền địa phương',
           0,
           N'UBND, Phòng LĐTBXH, cơ quan quản lý địa phương',
           8
    UNION ALL SELECT 'CFG_KQ5_2_CA', 'MTCT-KQ5.2',
           N'Đối tác theo lĩnh vực: Cảnh sát / Công an',
           0,
           N'Cơ quan công an phối hợp hỗ trợ cộng đồng',
           9
    UNION ALL SELECT 'CFG_KQ5_2_HT', 'MTCT-KQ5.2',
           N'Đối tác theo lĩnh vực: Giới học thuật / Nghiên cứu',
           0,
           N'Trường Đại học Y, Viện nghiên cứu',
           10
    UNION ALL SELECT 'CFG_KQ5_3', 'MTCT-KQ5.3',
           N'Số tài liệu dự án (M&E, kỹ thuật, giới, truyền thông, MSC) đã được thẩm định & phổ biến',
           0,
           N'SOP sàng lọc QST, Bộ công cụ tư vấn F7-F8, tài liệu truyền thông, câu chuyện thay đổi...',
           11
)
INSERT INTO dbo.CD45_CHI_SO_DU_AN_CONFIG (NAM, MA_KY, MA_THAM_SO, CHI_SO_CODE, TEN_THAM_SO, GIA_TRI, GHI_CHU, SORT_ORDER, UPDATED_AT, UPDATED_BY)
SELECT p.NAM, p.MA_KY, s.MA_THAM_SO, s.CHI_SO_CODE, s.TEN_THAM_SO, s.GIA_TRI, s.GHI_CHU, s.SORT_ORDER, GETDATE(), 'SYSTEM_SEED'
FROM SeedPeriods p
CROSS JOIN SeedParams s
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.CD45_CHI_SO_DU_AN_CONFIG c
    WHERE c.NAM = p.NAM AND c.MA_KY = p.MA_KY AND c.MA_THAM_SO = s.MA_THAM_SO
);
GO

-- ============================================================================
-- 4. STORED PROCEDURE: SP_CD45_GetBaoCaoChiSo
-- ============================================================================
CREATE OR ALTER PROC dbo.SP_CD45_GetBaoCaoChiSo
    @FromDate     DATE         = NULL,
    @ToDate       DATE         = NULL,
    @CityCode     VARCHAR(100) = NULL,
    @MaNhom       VARCHAR(100) = NULL,
    @IsCumulative BIT          = 1,
    @Nam          INT          = NULL,
    @MaKy         VARCHAR(20)  = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EffFromDate DATE = CASE WHEN @IsCumulative = 1 THEN NULL ELSE @FromDate END;
    DECLARE @RefYear INT = YEAR(ISNULL(@ToDate, GETDATE()));
    DECLARE @CfgYear INT = ISNULL(@Nam, YEAR(ISNULL(@ToDate, GETDATE())));
    DECLARE @CfgKy VARCHAR(20) = CASE
        WHEN @MaKy IN ('6T', '12T', 'ALL') THEN @MaKy
        WHEN @IsCumulative = 1 AND @FromDate IS NULL THEN 'ALL'
        ELSE '12T'
    END;
    DECLARE @IsNationalScope BIT = CASE WHEN (NULLIF(LTRIM(RTRIM(@CityCode)), '') IS NULL AND NULLIF(LTRIM(RTRIM(@MaNhom)), '') IS NULL) THEN 1 ELSE 0 END;
    DECLARE @HasGroupFilter BIT = CASE WHEN NULLIF(LTRIM(RTRIM(@MaNhom)), '') IS NOT NULL THEN 1 ELSE 0 END;

    -- Resolve biến thể mã nhóm CBO
    DECLARE @Var_MaNhomStd VARCHAR(50) = @MaNhom;
    DECLARE @Var_MaNhomMap VARCHAR(50) = @MaNhom;
    IF @MaNhom IS NOT NULL AND @MaNhom <> ''
    BEGIN
        SELECT TOP 1 @Var_MaNhomStd = manhom_tbh, @Var_MaNhomMap = manhom_tbh_map
        FROM dbo.BVTL_NHOM_TBH
        WHERE manhom_tbh = @MaNhom OR manhom_tbh_map = @MaNhom;
    END

    -- Resolve @CityCode mapping (34 tỉnh mới / 63 tỉnh cũ)
    DECLARE @MappedCityCodes TABLE (Code VARCHAR(10));
    IF @CityCode IS NOT NULL AND @CityCode <> ''
    BEGIN
        INSERT INTO @MappedCityCodes(Code)
        SELECT DISTINCT m.OldCityCode
        FROM STRING_SPLIT(@CityCode, ',') s
        JOIN dbo.BVTL_MAP_TINH_CU_MOI m ON LTRIM(RTRIM(s.value)) = m.NewCityCode
        UNION
        SELECT DISTINCT LTRIM(RTRIM(s.value))
        FROM STRING_SPLIT(@CityCode, ',') s
        WHERE NOT EXISTS (SELECT 1 FROM dbo.BVTL_MAP_TINH_CU_MOI m WHERE m.NewCityCode = LTRIM(RTRIM(s.value)))
          AND LTRIM(RTRIM(s.value)) <> '';
    END

    -- Tập khách hàng cơ sở F1 hợp lệ theo Tỉnh & Nhóm
    SELECT
        kh.RECORD_ID,
        kh.CITY_CODE,
        kh.MA_NHOM,
        kh.REDCAP_DAG,
        kh.DOI_TUONG,
        kh.NGAY_THAM_GIA,
        CASE
            WHEN kh.GIOI_TINH_TU_XD IN (3, 4) OR kh.GIOI_TINH_KHAI_SINH = 3 THEN 3
            WHEN kh.GIOI_TINH_TU_XD = 1 OR (kh.GIOI_TINH_TU_XD IS NULL AND kh.GIOI_TINH_KHAI_SINH = 1) THEN 1
            WHEN kh.GIOI_TINH_TU_XD = 2 OR (kh.GIOI_TINH_TU_XD IS NULL AND kh.GIOI_TINH_KHAI_SINH = 2) THEN 2
            ELSE 3
        END AS GIOI_TINH,
        CASE
            WHEN (@RefYear - kh.NAM_SINH) BETWEEN 18 AND 25 THEN 1
            WHEN (@RefYear - kh.NAM_SINH) BETWEEN 26 AND 35 THEN 2
            WHEN (@RefYear - kh.NAM_SINH) >= 36 THEN 3
            ELSE 2
        END AS NHOM_TUOI
    INTO #TmpKH
    FROM dbo.CD45_KH kh
    WHERE (@CityCode IS NULL OR @CityCode = '' OR kh.CITY_CODE IN (SELECT Code FROM @MappedCityCodes))
      AND (@MaNhom IS NULL OR @MaNhom = '' OR kh.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom) OR kh.REDCAP_DAG IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom))
      AND kh.COMPLETE_STATUS = '2';

    CREATE CLUSTERED INDEX IX_TmpKH_RecId ON #TmpKH(RECORD_ID);

    -- Bảng tạm tích lũy từng dòng chỉ số
    CREATE TABLE #Res (
        SortOrder INT IDENTITY(1,1),
        STT NVARCHAR(20),
        Code NVARCHAR(50),
        ChiTieu NVARCHAR(500),
        DonVi NVARCHAR(50),
        RowType VARCHAR(20),
        ParentCode NVARCHAR(50),
        Tong INT NULL,
        PUD INT NULL,
        PLHIV INT NULL,
        TG INT NULL,
        SW INT NULL,
        MSM INT NULL,
        Nam INT NULL,
        Nu INT NULL,
        Khac INT NULL,
        Tuoi_18_25 INT NULL,
        Tuoi_26_35 INT NULL,
        Tuoi_Tren35 INT NULL,
        Denom_Tong INT NULL,
        Denom_PUD INT NULL,
        Denom_PLHIV INT NULL,
        Denom_TG INT NULL,
        Denom_SW INT NULL,
        Denom_MSM INT NULL,
        Denom_Nam INT NULL,
        Denom_Nu INT NULL,
        Denom_Khac INT NULL,
        Denom_Tuoi_18_25 INT NULL,
        Denom_Tuoi_26_35 INT NULL,
        Denom_Tuoi_Tren35 INT NULL,
        NguonDuLieu NVARCHAR(500),
        GhiChu NVARCHAR(500),
        IsBold BIT DEFAULT 0,
        IndentLevel INT DEFAULT 0,
        CanDrillDown BIT DEFAULT 0
    );

    -- =========================================================================
    -- TẠO CÁC BẢNG TẬP HỢP COHORT (ĐẢM BẢO TỬ SỐ ⊆ MẪU SỐ TUYỆT ĐỐI)
    -- =========================================================================

    -- A. Tập KH tiếp cận (Mẫu số cho MTCT1.1 & MTCT1.2)
    SELECT DISTINCT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI
    INTO #Cohort_Approach
    FROM #TmpKH kh
    WHERE (
        @IsCumulative = 1
        AND (@ToDate IS NULL OR kh.NGAY_THAM_GIA IS NULL OR kh.NGAY_THAM_GIA <= @ToDate)
    )
    OR (
        @IsCumulative = 0
        AND (
            ((@EffFromDate IS NULL OR kh.NGAY_THAM_GIA >= @EffFromDate) AND (@ToDate IS NULL OR kh.NGAY_THAM_GIA <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_HOAT_DONG h WHERE h.RECORD_ID = kh.RECORD_ID AND h.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate) AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_QST q WHERE q.RECORD_ID = kh.RECORD_ID AND q.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR q.NGAY_SANG_LOC >= @EffFromDate) AND (@ToDate IS NULL OR q.NGAY_SANG_LOC <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_CHAN_DOAN c WHERE c.RECORD_ID = kh.RECORD_ID AND c.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate) AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_TU_VAN_L1 t1 WHERE t1.RECORD_ID = kh.RECORD_ID AND t1.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t1.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t1.NGAY_TU_VAN <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_TU_VAN_L2 t2 WHERE t2.RECORD_ID = kh.RECORD_ID AND t2.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t2.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t2.NGAY_TU_VAN <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_TUAN_THU tt WHERE tt.RECORD_ID = kh.RECORD_ID AND tt.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR tt.NGAY_HO_TRO >= @EffFromDate) AND (@ToDate IS NULL OR tt.NGAY_HO_TRO <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_HO_TRO_XH xh WHERE xh.RECORD_ID = kh.RECORD_ID AND xh.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR xh.NGAY_HO_TRO >= @EffFromDate) AND (@ToDate IS NULL OR xh.NGAY_HO_TRO <= @ToDate))
        )
    );

    -- B. Tập KH Truyền thông (F2 LOAI_DV = 1)
    SELECT DISTINCT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI
    INTO #Cohort_TruyenThong
    FROM #Cohort_Approach kh
    INNER JOIN dbo.CD45_HOAT_DONG h ON kh.RECORD_ID = h.RECORD_ID
    WHERE h.COMPLETE_STATUS = '2'
      AND h.LOAI_DV = 1
      AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate)
      AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate);

    -- C. Tập KH Sàng lọc QST (F3)
    SELECT DISTINCT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI
    INTO #Cohort_QST
    FROM #Cohort_Approach kh
    INNER JOIN dbo.CD45_QST q ON kh.RECORD_ID = q.RECORD_ID
    WHERE q.COMPLETE_STATUS = '2'
      AND (@EffFromDate IS NULL OR q.NGAY_SANG_LOC >= @EffFromDate)
      AND (@ToDate IS NULL OR q.NGAY_SANG_LOC <= @ToDate);

    -- D. Tập KH thuộc QST + nhận ít nhất 1 can thiệp CBO (MTCT1.3)
    SELECT DISTINCT q.RECORD_ID, q.DOI_TUONG, q.GIOI_TINH, q.NHOM_TUOI
    INTO #Cohort_MTCT13_All
    FROM #Cohort_QST q
    WHERE EXISTS (SELECT 1 FROM dbo.CD45_HOAT_DONG h WHERE h.RECORD_ID = q.RECORD_ID AND h.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate) AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate))
       OR EXISTS (SELECT 1 FROM dbo.CD45_TU_VAN_L1 t1 WHERE t1.RECORD_ID = q.RECORD_ID AND t1.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t1.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t1.NGAY_TU_VAN <= @ToDate))
       OR EXISTS (SELECT 1 FROM dbo.CD45_TU_VAN_L2 t2 WHERE t2.RECORD_ID = q.RECORD_ID AND t2.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t2.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t2.NGAY_TU_VAN <= @ToDate))
       OR EXISTS (SELECT 1 FROM dbo.CD45_CHAN_DOAN c WHERE c.RECORD_ID = q.RECORD_ID AND c.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate) AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate))
       OR EXISTS (SELECT 1 FROM dbo.CD45_TUAN_THU tt WHERE tt.RECORD_ID = q.RECORD_ID AND tt.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR tt.NGAY_HO_TRO >= @EffFromDate) AND (@ToDate IS NULL OR tt.NGAY_HO_TRO <= @ToDate))
       OR EXISTS (SELECT 1 FROM dbo.CD45_HO_TRO_XH xh WHERE xh.RECORD_ID = q.RECORD_ID AND xh.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR xh.NGAY_HO_TRO >= @EffFromDate) AND (@ToDate IS NULL OR xh.NGAY_HO_TRO <= @ToDate));

    SELECT DISTINCT q.RECORD_ID, q.DOI_TUONG, q.GIOI_TINH, q.NHOM_TUOI
    INTO #Cohort_MTCT13_AfterQST
    FROM #Cohort_QST q
    WHERE EXISTS (SELECT 1 FROM dbo.CD45_HOAT_DONG h WHERE h.RECORD_ID = q.RECORD_ID AND h.COMPLETE_STATUS = '2' AND h.LOAI_DV IN (2, 3, 4) AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate) AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate))
       OR EXISTS (SELECT 1 FROM dbo.CD45_TU_VAN_L1 t1 WHERE t1.RECORD_ID = q.RECORD_ID AND t1.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t1.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t1.NGAY_TU_VAN <= @ToDate))
       OR EXISTS (SELECT 1 FROM dbo.CD45_TU_VAN_L2 t2 WHERE t2.RECORD_ID = q.RECORD_ID AND t2.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t2.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t2.NGAY_TU_VAN <= @ToDate))
       OR EXISTS (SELECT 1 FROM dbo.CD45_CHAN_DOAN c WHERE c.RECORD_ID = q.RECORD_ID AND c.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate) AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate))
       OR EXISTS (SELECT 1 FROM dbo.CD45_TUAN_THU tt WHERE tt.RECORD_ID = q.RECORD_ID AND tt.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR tt.NGAY_HO_TRO >= @EffFromDate) AND (@ToDate IS NULL OR tt.NGAY_HO_TRO <= @ToDate))
       OR EXISTS (SELECT 1 FROM dbo.CD45_HO_TRO_XH xh WHERE xh.RECORD_ID = q.RECORD_ID AND xh.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR xh.NGAY_HO_TRO >= @EffFromDate) AND (@ToDate IS NULL OR xh.NGAY_HO_TRO <= @ToDate));

    -- E. Tập KH QST Dương tính Mức 1 & Mức 2 (Mẫu số MTCT1.4) và Đã khám F6 (Tử số MTCT1.4)
    SELECT DISTINCT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI
    INTO #Cohort_QST_Pos
    FROM #TmpKH kh
    INNER JOIN dbo.CD45_QST q ON kh.RECORD_ID = q.RECORD_ID
    WHERE q.COMPLETE_STATUS = '2'
      AND q.MUC_QST IN (1, 2)
      AND (@EffFromDate IS NULL OR q.NGAY_SANG_LOC >= @EffFromDate)
      AND (@ToDate IS NULL OR q.NGAY_SANG_LOC <= @ToDate);

    SELECT DISTINCT p.RECORD_ID, p.DOI_TUONG, p.GIOI_TINH, p.NHOM_TUOI
    INTO #Cohort_QST_Pos_F6
    FROM #Cohort_QST_Pos p
    INNER JOIN dbo.CD45_CHAN_DOAN c ON p.RECORD_ID = c.RECORD_ID
    WHERE c.COMPLETE_STATUS = '2'
      AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate);

    -- F. Tập KH đang điều trị có Phiếu F5 (Mẫu số MTCT1.5) và Đánh giá cải thiện 'Tốt hơn' (Tử số MTCT1.5)
    -- Lấy lần theo dõi F5 gần nhất trong kỳ của mỗi KH đang điều trị (HINH_THUC_DIEU_TRI IN 1,2,3,5)
    ;WITH CTE_F5_Ranked AS (
        SELECT
            kh.RECORD_ID,
            kh.DOI_TUONG,
            kh.GIOI_TINH,
            kh.NHOM_TUOI,
            tt.TINH_TRANG_SKTT,
            tt.TUAN_THU,
            ROW_NUMBER() OVER (PARTITION BY kh.RECORD_ID ORDER BY tt.NGAY_HO_TRO DESC, ISNULL(tt.REPEAT_INSTANCE, 1) DESC, tt.ID DESC) AS rn
        FROM #TmpKH kh
        INNER JOIN dbo.CD45_TUAN_THU tt ON kh.RECORD_ID = tt.RECORD_ID
        WHERE tt.COMPLETE_STATUS = '2'
          AND ISNULL(tt.HINH_THUC_DIEU_TRI, 0) IN (1, 2, 3, 5)
          AND (@EffFromDate IS NULL OR tt.NGAY_HO_TRO >= @EffFromDate)
          AND (@ToDate IS NULL OR tt.NGAY_HO_TRO <= @ToDate)
    )
    SELECT RECORD_ID, DOI_TUONG, GIOI_TINH, NHOM_TUOI, TINH_TRANG_SKTT, TUAN_THU
    INTO #Cohort_F5_Latest
    FROM CTE_F5_Ranked
    WHERE rn = 1;

    -- Đọc giá trị cấu hình Dự án từ CD45_CHI_SO_DU_AN_CONFIG
    DECLARE @Cfg_GF_Denom INT = 0, @Cfg_KQ1_1_Override INT = 0, @Cfg_KQ1_3 INT = 30,
            @Cfg_KQ5_1 INT = 9, @Cfg_KQ5_2_TW INT = 0, @Cfg_KQ5_2_DP INT = 0,
            @Cfg_KQ5_2_YT INT = 0, @Cfg_KQ5_2_CQ INT = 0, @Cfg_KQ5_2_CA INT = 0, @Cfg_KQ5_2_HT INT = 0,
            @Cfg_KQ5_3 INT = 0;

    -- Fallback nếu chưa có config cho năm @CfgYear thì lấy năm gần nhất
    DECLARE @ResolvedCfgYear INT = (SELECT TOP 1 NAM FROM dbo.CD45_CHI_SO_DU_AN_CONFIG WHERE MA_KY = @CfgKy ORDER BY CASE WHEN NAM = @CfgYear THEN 0 ELSE 1 END, NAM DESC);
    IF @ResolvedCfgYear IS NOT NULL
    BEGIN
        SELECT
            @Cfg_GF_Denom      = ISNULL(MAX(CASE WHEN MA_THAM_SO = 'CFG_GF_DENOM' THEN GIA_TRI END), 0),
            @Cfg_KQ1_1_Override= ISNULL(MAX(CASE WHEN MA_THAM_SO = 'CFG_KQ1_1_OVERRIDE' THEN GIA_TRI END), 0),
            @Cfg_KQ1_3         = ISNULL(MAX(CASE WHEN MA_THAM_SO = 'CFG_KQ1_3' THEN GIA_TRI END), 30),
            @Cfg_KQ5_1         = ISNULL(MAX(CASE WHEN MA_THAM_SO = 'CFG_KQ5_1' THEN GIA_TRI END), 9),
            @Cfg_KQ5_2_TW      = ISNULL(MAX(CASE WHEN MA_THAM_SO = 'CFG_KQ5_2_TW' THEN GIA_TRI END), 0),
            @Cfg_KQ5_2_DP      = ISNULL(MAX(CASE WHEN MA_THAM_SO = 'CFG_KQ5_2_DP' THEN GIA_TRI END), 0),
            @Cfg_KQ5_2_YT      = ISNULL(MAX(CASE WHEN MA_THAM_SO = 'CFG_KQ5_2_YT' THEN GIA_TRI END), 0),
            @Cfg_KQ5_2_CQ      = ISNULL(MAX(CASE WHEN MA_THAM_SO = 'CFG_KQ5_2_CQ' THEN GIA_TRI END), 0),
            @Cfg_KQ5_2_CA      = ISNULL(MAX(CASE WHEN MA_THAM_SO = 'CFG_KQ5_2_CA' THEN GIA_TRI END), 0),
            @Cfg_KQ5_2_HT      = ISNULL(MAX(CASE WHEN MA_THAM_SO = 'CFG_KQ5_2_HT' THEN GIA_TRI END), 0),
            @Cfg_KQ5_3         = ISNULL(MAX(CASE WHEN MA_THAM_SO = 'CFG_KQ5_3' THEN GIA_TRI END), 0)
        FROM dbo.CD45_CHI_SO_DU_AN_CONFIG
        WHERE NAM = @ResolvedCfgYear AND MA_KY = @CfgKy;
    END

    -- =========================================================================
    -- SECTION I: NHÓM I - CÁC CHỈ SỐ MỤC TIÊU CỤ THỂ (MTCT 1.1 - 1.5)
    -- =========================================================================
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, IsBold, IndentLevel)
    VALUES ('I', 'SEC_I', N'NHÓM I: CÁC CHỈ SỐ MỤC TIÊU CỤ THỂ (MTCT 1.1 – MTCT 1.5)', '', 'SEC', 1, 0);

    -- Biến tổng hợp nhanh cho các chỉ số tỷ lệ
    DECLARE @App_T INT,@App_P INT,@App_PL INT,@App_TG INT,@App_SW INT,@App_M INT,@App_Nam INT,@App_Nu INT,@App_Khac INT,@App_T1 INT,@App_T2 INT,@App_T3 INT;
    SELECT @App_T=COUNT(*),
           @App_P=ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           @App_PL=ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           @App_TG=ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           @App_SW=ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           @App_M=ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           @App_Nam=ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           @App_Nu=ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           @App_Khac=ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           @App_T1=ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           @App_T2=ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           @App_T3=ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0)
    FROM #Cohort_Approach;

    -- 1. MTCT1.1
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode,
                      Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35,
                      Denom_Tong, Denom_PUD, Denom_PLHIV, Denom_TG, Denom_SW, Denom_MSM,
                      Denom_Nam, Denom_Nu, Denom_Khac, Denom_Tuoi_18_25, Denom_Tuoi_26_35, Denom_Tuoi_Tren35,
                      NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '1', 'MTCT1.1',
           N'Tỷ lệ KH được tiếp cận, được nâng cao nhận thức về sức khoẻ tâm thần (= được nhận thông tin)',
           N'Tỷ lệ (%)', 'PCT', NULL,
           COUNT(*),
           ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
           @App_T, @App_P, @App_PL, @App_TG, @App_SW, @App_M, @App_Nam, @App_Nu, @App_Khac, @App_T1, @App_T2, @App_T3,
           N'Tử số: Phiếu F2 (Truyền thông LOAI_DV=1) / Mẫu số: Tổng KH tiếp cận (F1)',
           N'Đếm KH có phiếu Truyền thông F2 hoàn thành trong kỳ', 1, 0, 1
    FROM #Cohort_TruyenThong;

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '', 'MTCT1.1_NUM', N'+ Tử số: Số KH được tham gia truyền thông nâng cao nhận thức SKTT', N'Khách hàng', 'SUB', 'MTCT1.1',
           Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35,
           N'CD45_HOAT_DONG (LOAI_DV = 1, COMPLETE_STATUS = 2)', N'Click vào số liệu để xem danh sách KH', 0, 1, 1
    FROM #Res WHERE Code = 'MTCT1.1';

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('', 'MTCT1.1_DEN', N'+ Mẫu số: Tổng số KH được tiếp cận bởi dự án', N'Khách hàng', 'SUB', 'MTCT1.1',
            @App_T, @App_P, @App_PL, @App_TG, @App_SW, @App_M, @App_Nam, @App_Nu, @App_Khac, @App_T1, @App_T2, @App_T3,
            N'CD45_KH (COMPLETE_STATUS = 2)', N'Tổng số KH tiếp cận hợp lệ của dự án DREAMH', 0, 1, 1);

    -- 2. MTCT1.2 (Cơ chế Mẫu số Kép: Mặc định = F1; nếu @IsNationalScope=1 và @Cfg_GF_Denom > 0 thì lấy @Cfg_GF_Denom cho Denom_Tong)
    DECLARE @UseGfOverride BIT = CASE WHEN (@IsNationalScope = 1 AND @Cfg_GF_Denom > 0) THEN 1 ELSE 0 END;

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode,
                      Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35,
                      Denom_Tong, Denom_PUD, Denom_PLHIV, Denom_TG, Denom_SW, Denom_MSM,
                      Denom_Nam, Denom_Nu, Denom_Khac, Denom_Tuoi_18_25, Denom_Tuoi_26_35, Denom_Tuoi_Tren35,
                      NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '2', 'MTCT1.2',
           N'Tỷ lệ KH được sàng lọc sức khoẻ tâm thần so với tổng số KH được tiếp cận bởi dự án Quỹ toàn cầu tại 6 tỉnh',
           N'Tỷ lệ (%)', 'PCT', NULL,
           COUNT(*),
           ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
           CASE WHEN @UseGfOverride = 1 THEN @Cfg_GF_Denom ELSE @App_T END,
           CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_P END,
           CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_PL END,
           CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_TG END,
           CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_SW END,
           CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_M END,
           CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_Nam END,
           CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_Nu END,
           CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_Khac END,
           CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_T1 END,
           CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_T2 END,
           CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_T3 END,
           CASE WHEN @UseGfOverride = 1 THEN N'Tử số: Phiếu F3 (QST) / Mẫu số: Chỉ tiêu Quỹ Toàn Cầu cấu hình'
                ELSE N'Tử số: Phiếu F3 (QST) / Mẫu số: Tổng KH tiếp cận (F1)' END,
           CASE WHEN @UseGfOverride = 1 THEN N'Đang dùng Mẫu số GF cấu hình (' + CAST(@Cfg_GF_Denom AS VARCHAR) + N' KH)'
                ELSE N'Đếm KH được sàng lọc bảng hỏi QST ít nhất 1 lần' END,
           1, 0, 1
    FROM #Cohort_QST;

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '', 'MTCT1.2_NUM', N'+ Tử số: Số KH được sàng lọc bảng hỏi QST ít nhất 1 lần', N'Khách hàng', 'SUB', 'MTCT1.2',
           Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35,
           N'CD45_QST (COMPLETE_STATUS = 2)', N'Số KH duy nhất đã làm bảng hỏi QST', 0, 1, 1
    FROM #Res WHERE Code = 'MTCT1.2';

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('', 'MTCT1.2_DEN',
            CASE WHEN @UseGfOverride = 1 THEN N'+ Mẫu số: Tổng số KH tiếp cận bởi dự án Quỹ toàn cầu (Cấu hình ngoại sinh)'
                 ELSE N'+ Mẫu số: Tổng số KH tiếp cận tại 6 tỉnh (REDCap DREAMH)' END,
            N'Khách hàng', 'SUB', 'MTCT1.2',
            CASE WHEN @UseGfOverride = 1 THEN @Cfg_GF_Denom ELSE @App_T END,
            CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_P END,
            CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_PL END,
            CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_TG END,
            CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_SW END,
            CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_M END,
            CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_Nam END,
            CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_Nu END,
            CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_Khac END,
            CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_T1 END,
            CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_T2 END,
            CASE WHEN @UseGfOverride = 1 THEN NULL ELSE @App_T3 END,
            CASE WHEN @UseGfOverride = 1 THEN N'CD45_CHI_SO_DU_AN_CONFIG (CFG_GF_DENOM)' ELSE N'CD45_KH (COMPLETE_STATUS = 2)' END,
            CASE WHEN @UseGfOverride = 1 THEN N'Số liệu nhập từ Cấu hình Chỉ số Dự án' ELSE N'Có thể cấu hình mẫu số GF nếu khác tổng F1' END,
            0, 1, CASE WHEN @UseGfOverride = 1 THEN 0 ELSE 1 END);

    -- 3. MTCT1.3
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '3', 'MTCT1.3',
           N'Số KH được nhận ít nhất một can thiệp cung cấp bởi CBO trong số những KH được sàng lọc SKTT (1/truyền thông, 2/tư vấn cá nhân & chữa lành, 3/chuyển gửi, 4/hỗ trợ tuân thủ)',
           N'Khách hàng', 'MAIN', NULL,
           COUNT(*),
           ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
           N'KH thuộc F3 (QST) ∩ (F2 ∪ F7 ∪ F8 ∪ F6 ∪ F5 ∪ F4)',
           N'Bao gồm cả hoạt động 1/Truyền thông theo định nghĩa văn kiện', 1, 0, 1
    FROM #Cohort_MTCT13_All;

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '', 'MTCT1.3_SUB',
           N'+ Trong đó: Số KH nhận ít nhất 1 can thiệp chuyên sâu sau sàng lọc (không tính truyền thông ban đầu)',
           N'Khách hàng', 'SUB', 'MTCT1.3',
           COUNT(*),
           ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
           N'KH thuộc F3 (QST) ∩ (F2[2,3,4] ∪ F7 ∪ F8 ∪ F6 ∪ F5 ∪ F4)',
           N'Phản ánh số KH kết nối can thiệp thực chất sau sàng lọc QST', 0, 1, 1
    FROM #Cohort_MTCT13_AfterQST;

    -- 4. MTCT1.4
    DECLARE @Pos_T INT,@Pos_P INT,@Pos_PL INT,@Pos_TG INT,@Pos_SW INT,@Pos_M INT,@Pos_Nam INT,@Pos_Nu INT,@Pos_Khac INT,@Pos_T1 INT,@Pos_T2 INT,@Pos_T3 INT;
    SELECT @Pos_T=COUNT(*),
           @Pos_P=ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           @Pos_PL=ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           @Pos_TG=ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           @Pos_SW=ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           @Pos_M=ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           @Pos_Nam=ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           @Pos_Nu=ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           @Pos_Khac=ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           @Pos_T1=ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           @Pos_T2=ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           @Pos_T3=ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0)
    FROM #Cohort_QST_Pos;

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode,
                      Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35,
                      Denom_Tong, Denom_PUD, Denom_PLHIV, Denom_TG, Denom_SW, Denom_MSM,
                      Denom_Nam, Denom_Nu, Denom_Khac, Denom_Tuoi_18_25, Denom_Tuoi_26_35, Denom_Tuoi_Tren35,
                      NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '4', 'MTCT1.4',
           N'Tỷ lệ KH có kết quả sàng lọc QST dương tính (mức 1 và mức 2) đồng ý và được chuyển gửi khám SKTT ít nhất 1 lần',
           N'Tỷ lệ (%)', 'PCT', NULL,
           COUNT(*),
           ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
           @Pos_T, @Pos_P, @Pos_PL, @Pos_TG, @Pos_SW, @Pos_M, @Pos_Nam, @Pos_Nu, @Pos_Khac, @Pos_T1, @Pos_T2, @Pos_T3,
           N'Tử số: QST Mức 1,2 có Phiếu F6 / Mẫu số: Tổng KH QST Mức 1,2',
           N'Đảm bảo Tử số thuộc tập Mẫu số QST dương tính', 1, 0, 1
    FROM #Cohort_QST_Pos_F6;

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '', 'MTCT1.4_NUM', N'+ Tử số: Số KH QST dương tính (Mức 1, 2) đã được chuyển gửi và khám SKTT (Phiếu F6)', N'Khách hàng', 'SUB', 'MTCT1.4',
           Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35,
           N'CD45_QST (MUC_QST IN 1,2) ∩ CD45_CHAN_DOAN', N'Đã có ít nhất 1 phiếu khám chuyên khoa F6', 0, 1, 1
    FROM #Res WHERE Code = 'MTCT1.4';

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('', 'MTCT1.4_DEN', N'+ Mẫu số: Tổng số KH có kết quả sàng lọc QST dương tính (Mức 1 và Mức 2)', N'Khách hàng', 'SUB', 'MTCT1.4',
            @Pos_T, @Pos_P, @Pos_PL, @Pos_TG, @Pos_SW, @Pos_M, @Pos_Nam, @Pos_Nu, @Pos_Khac, @Pos_T1, @Pos_T2, @Pos_T3,
            N'CD45_QST (MUC_QST IN 1,2, COMPLETE_STATUS = 2)', N'Mức 1 (>=8đ hoặc tự hại) và Mức 2 (6-7đ)', 0, 1, 1);

    -- 5. MTCT1.5 (Cải thiện vấn đề SKTT trong quá trình điều trị - Câu 5 Phiếu F5 f5_feeling = 1 'Tốt hơn')
    DECLARE @F5_T INT,@F5_P INT,@F5_PL INT,@F5_TG INT,@F5_SW INT,@F5_M INT,@F5_Nam INT,@F5_Nu INT,@F5_Khac INT,@F5_T1 INT,@F5_T2 INT,@F5_T3 INT;
    SELECT @F5_T=COUNT(*),
           @F5_P=ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           @F5_PL=ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           @F5_TG=ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           @F5_SW=ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           @F5_M=ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           @F5_Nam=ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           @F5_Nu=ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           @F5_Khac=ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           @F5_T1=ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           @F5_T2=ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           @F5_T3=ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0)
    FROM #Cohort_F5_Latest;

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode,
                      Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35,
                      Denom_Tong, Denom_PUD, Denom_PLHIV, Denom_TG, Denom_SW, Denom_MSM,
                      Denom_Nam, Denom_Nu, Denom_Khac, Denom_Tuoi_18_25, Denom_Tuoi_26_35, Denom_Tuoi_Tren35,
                      NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '5', 'MTCT1.5',
           N'Tỷ lệ KH cải thiện vấn đề SKTT trong quá trình điều trị',
           N'Tỷ lệ (%)', 'PCT', NULL,
           COUNT(*),
           ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
           @F5_T, @F5_P, @F5_PL, @F5_TG, @F5_SW, @F5_M, @F5_Nam, @F5_Nu, @F5_Khac, @F5_T1, @F5_T2, @F5_T3,
           N'Tử số: Câu 5 Phiếu F5 f5_feeling = 1 (Tốt hơn) / Mẫu số: KH đang điều trị có F5',
           N'Lấy theo lần theo dõi tuân thủ F5 gần nhất trong kỳ của KH đang điều trị', 1, 0, 1
    FROM #Cohort_F5_Latest
    WHERE TINH_TRANG_SKTT = 1;

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '', 'MTCT1.5_NUM', N'+ Tử số: Số KH đang điều trị có đánh giá tình trạng SKTT "Tốt hơn" (Phiếu F5)', N'Khách hàng', 'SUB', 'MTCT1.5',
           Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35,
           N'CD45_TUAN_THU (TINH_TRANG_SKTT = 1)', N'Lần theo dõi gần nhất đánh giá cảm thấy tốt hơn', 0, 1, 1
    FROM #Res WHERE Code = 'MTCT1.5';

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('', 'MTCT1.5_DEN', N'+ Mẫu số: Tổng số KH đang trong quá trình điều trị được theo dõi (Phiếu F5)', N'Khách hàng', 'SUB', 'MTCT1.5',
            @F5_T, @F5_P, @F5_PL, @F5_TG, @F5_SW, @F5_M, @F5_Nam, @F5_Nu, @F5_Khac, @F5_T1, @F5_T2, @F5_T3,
            N'CD45_TUAN_THU (HINH_THUC_DIEU_TRI IN 1,2,3,5)', N'Loại trừ trường hợp không có chỉ định điều trị', 0, 1, 1);

    -- =========================================================================
    -- SECTION II: NHÓM II - CÁC CHỈ SỐ KẾT QUẢ ĐẦU RA (MTCT-KQ1.1 - KQ5.3)
    -- =========================================================================
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, IsBold, IndentLevel)
    VALUES ('II', 'SEC_II', N'NHÓM II: CÁC CHỈ SỐ KẾT QUẢ ĐẦU RA (MTCT-KQ1.1 – MTCT-KQ5.3)', '', 'SEC', 1, 0);

    -- 6. MTCT-KQ1.1: Số đơn vị đang tích cực áp dụng mô hình mới
    DECLARE @ActiveCBOs INT = 0, @ActiveHospitals INT = 0;
    ;WITH CTE_ActiveGroups AS (
        SELECT DISTINCT kh.MA_NHOM FROM #TmpKH kh
        INNER JOIN dbo.CD45_HOAT_DONG h ON kh.RECORD_ID = h.RECORD_ID
        WHERE h.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate) AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate)
        UNION
        SELECT DISTINCT kh.MA_NHOM FROM #TmpKH kh
        INNER JOIN dbo.CD45_QST q ON kh.RECORD_ID = q.RECORD_ID
        WHERE q.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR q.NGAY_SANG_LOC >= @EffFromDate) AND (@ToDate IS NULL OR q.NGAY_SANG_LOC <= @ToDate)
        UNION
        SELECT DISTINCT kh.MA_NHOM FROM #TmpKH kh
        INNER JOIN dbo.CD45_CHAN_DOAN c ON kh.RECORD_ID = c.RECORD_ID
        WHERE c.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate) AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate)
    )
    SELECT @ActiveCBOs = COUNT(DISTINCT MA_NHOM) FROM CTE_ActiveGroups WHERE MA_NHOM IS NOT NULL AND MA_NHOM <> '';

    SELECT @ActiveHospitals = COUNT(DISTINCT LTRIM(RTRIM(c.CO_SO_Y_TE)))
    FROM dbo.CD45_CHAN_DOAN c
    INNER JOIN #TmpKH kh ON c.RECORD_ID = kh.RECORD_ID
    WHERE c.COMPLETE_STATUS = '2'
      AND c.CO_SO_Y_TE IS NOT NULL AND LTRIM(RTRIM(c.CO_SO_Y_TE)) <> ''
      AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate)
      AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate);

    DECLARE @KQ11_Val INT = CASE WHEN (@IsNationalScope = 1 AND @Cfg_KQ1_1_Override > 0) THEN @Cfg_KQ1_1_Override ELSE @ActiveCBOs END;

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('6', 'MTCT-KQ1.1',
            N'Số đơn vị đang tích cực áp dụng mô hình mới, bao gồm các công cụ, quy trình chuẩn (SOP) và tài liệu có nhạy cảm giới và các nhóm KP khác',
            N'Đơn vị', 'ORG', NULL,
            @KQ11_Val,
            N'Số CBO/DNXH có phát sinh dữ liệu hoạt động trong kỳ' + CASE WHEN @IsNationalScope = 1 AND @Cfg_KQ1_1_Override > 0 THEN N' (Đã ghi đè cấu hình)' ELSE N'' END,
            N'Gồm ' + CAST(@ActiveCBOs AS VARCHAR) + N' CBO/DNXH và ' + CAST(@ActiveHospitals AS VARCHAR) + N' Cơ sở KCB SKTT phối hợp',
            1, 0, 1);

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('', 'KQ1.1_CBO', N'+ Số CBO / DNXH đang tích cực triển khai mô hình', N'CBO/DNXH', 'SUB', 'MTCT-KQ1.1',
            @ActiveCBOs, N'REDCap (F2, F3, F6)', N'Đơn vị cộng đồng có dữ liệu hoàn thành trong kỳ', 0, 1, 1);

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('', 'KQ1.1_CSYT', N'+ Số Cơ sở khám chữa bệnh SKTT phối hợp tiếp nhận KH', N'Cơ sở y tế', 'SUB', 'MTCT-KQ1.1',
            @ActiveHospitals, N'CD45_CHAN_DOAN (f6_hospital)', N'Cơ sở y tế có tiếp nhận ca chuyển gửi trong kỳ', 0, 1, 1);

    -- 7. MTCT-KQ1.2: Số TCV tham gia xây dựng mô hình (phân tách theo giới tính)
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, Nam, Nu, Khac, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '7', 'MTCT-KQ1.2',
           N'Số lượng TCV đã tham gia xây dựng mô hình can thiệp và các công cụ của dự án (phân tách theo giới tính)',
           N'Tiếp cận viên', 'TCV', NULL,
           COUNT(*),
           ISNULL(SUM(CASE WHEN tcv.GIOI_TINH = 1 THEN 1 ELSE 0 END), 0),
           ISNULL(SUM(CASE WHEN tcv.GIOI_TINH = 2 THEN 1 ELSE 0 END), 0),
           ISNULL(SUM(CASE WHEN ISNULL(tcv.GIOI_TINH, 3) NOT IN (1, 2) THEN 1 ELSE 0 END), 0),
           N'CD45_NHOM_TCV (LA_TCV_NAM_1 = 1)',
           N'Cột Khác bao gồm TCV giới tính Khác hoặc chưa cập nhật giới tính',
           1, 0, 1
    FROM dbo.CD45_NHOM_TCV tcv
    WHERE tcv.MADUAN = 'CD45'
      AND ISNULL(tcv.LA_TCV_NAM_1, 1) = 1
      AND (@CityCode IS NULL OR @CityCode = '' OR tcv.CITY_CODE IN (SELECT Code FROM @MappedCityCodes))
      AND (@MaNhom IS NULL OR @MaNhom = '' OR tcv.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom));

    -- 8. MTCT-KQ1.3: Số chuyên gia y tế tham gia xây dựng mô hình
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('8', 'MTCT-KQ1.3',
            N'Số lượng chuyên gia y tế tham gia xây dựng mô hình can thiệp và các công cụ của dự án',
            N'Chuyên gia', 'ORG', NULL,
            CASE WHEN @HasGroupFilter = 1 THEN NULL ELSE @Cfg_KQ1_3 END,
            N'CD45_CHI_SO_DU_AN_CONFIG (CFG_KQ1_3) / Danh mục Bác sĩ',
            CASE WHEN @HasGroupFilter = 1 THEN N'N/A (Chỉ số cấp dự án)' ELSE N'Quản lý số liệu tại Modal Cấu hình Chỉ số Dự án' END,
            1, 0, 0);

    -- 9. MTCT-KQ2.1: Số TCV đáp ứng tiêu chí tại thời điểm báo cáo (phân tách theo giới tính)
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, Nam, Nu, Khac, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '9', 'MTCT-KQ2.1',
           N'Số lượng TCV đáp ứng tiêu chí của dự án để cung cấp các can thiệp SKTT cơ bản theo mô hình mới (phân tách theo giới tính)',
           N'Tiếp cận viên', 'TCV', NULL,
           COUNT(*),
           ISNULL(SUM(CASE WHEN tcv.GIOI_TINH = 1 THEN 1 ELSE 0 END), 0),
           ISNULL(SUM(CASE WHEN tcv.GIOI_TINH = 2 THEN 1 ELSE 0 END), 0),
           ISNULL(SUM(CASE WHEN ISNULL(tcv.GIOI_TINH, 3) NOT IN (1, 2) THEN 1 ELSE 0 END), 0),
           N'CD45_NHOM_TCV (IsActive = 1)',
           N'Tất cả TCV đang hoạt động tại thời điểm báo cáo',
           1, 0, 1
    FROM dbo.CD45_NHOM_TCV tcv
    WHERE tcv.MADUAN = 'CD45'
      AND tcv.IsActive = 1
      AND (@CityCode IS NULL OR @CityCode = '' OR tcv.CITY_CODE IN (SELECT Code FROM @MappedCityCodes))
      AND (@MaNhom IS NULL OR @MaNhom = '' OR tcv.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom));

    -- 10. MTCT-KQ2.2: Số CBO/DNXH được SCDI nâng cao năng lực
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '10', 'MTCT-KQ2.2',
           N'Số lượng CBO/DNXH được SCDI nâng cao năng lực để triển khai các can thiệp SKTT theo mô hình mới thông qua các TCV',
           N'CBO/DNXH', 'ORG', NULL,
           COUNT(DISTINCT n.manhom_tbh),
           N'BVTL_NHOM_TBH (maduan = CD45, IS_ACTIVE = 1)',
           N'Tổng hợp tất cả các CBO/DNXH thuộc dự án tại thời điểm báo cáo',
           1, 0, 1
    FROM dbo.BVTL_NHOM_TBH n
    WHERE n.maduan = 'CD45'
      AND ISNULL(n.IS_ACTIVE, 1) = 1
      AND (@CityCode IS NULL OR @CityCode = '' OR n.city_code IN (SELECT Code FROM @MappedCityCodes))
      AND (@MaNhom IS NULL OR @MaNhom = '' OR n.manhom_tbh IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom) OR n.manhom_tbh_map IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom));

    -- 11. MTCT-KQ3.1: Số lượng can thiệp cơ bản (5 loại x Số KH & Số lượt)
    -- Tạo bảng tạm 5 loại can thiệp (lượt) để cộng dòng cha chính xác
    SELECT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI, 1 AS LoaiCT
    INTO #CT_CoBan
    FROM #TmpKH kh INNER JOIN dbo.CD45_QST q ON kh.RECORD_ID = q.RECORD_ID
    WHERE q.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR q.NGAY_SANG_LOC >= @EffFromDate) AND (@ToDate IS NULL OR q.NGAY_SANG_LOC <= @ToDate)
    UNION ALL
    SELECT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI, 2 AS LoaiCT
    FROM #TmpKH kh INNER JOIN dbo.CD45_HOAT_DONG h ON kh.RECORD_ID = h.RECORD_ID
    WHERE h.COMPLETE_STATUS = '2' AND h.LOAI_DV = 1 AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate) AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate)
    UNION ALL
    SELECT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI, 3 AS LoaiCT
    FROM #TmpKH kh INNER JOIN dbo.CD45_TU_VAN_L1 t1 ON kh.RECORD_ID = t1.RECORD_ID
    WHERE t1.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t1.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t1.NGAY_TU_VAN <= @ToDate)
    UNION ALL
    SELECT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI, 3 AS LoaiCT
    FROM #TmpKH kh INNER JOIN dbo.CD45_TU_VAN_L2 t2 ON kh.RECORD_ID = t2.RECORD_ID
    WHERE t2.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t2.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t2.NGAY_TU_VAN <= @ToDate)
    UNION ALL
    SELECT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI, 4 AS LoaiCT
    FROM #TmpKH kh INNER JOIN dbo.CD45_CHAN_DOAN c ON kh.RECORD_ID = c.RECORD_ID
    WHERE c.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate) AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate)
    UNION ALL
    SELECT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI, 5 AS LoaiCT
    FROM #TmpKH kh INNER JOIN dbo.CD45_TUAN_THU tt ON kh.RECORD_ID = tt.RECORD_ID
    WHERE tt.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR tt.NGAY_HO_TRO >= @EffFromDate) AND (@ToDate IS NULL OR tt.NGAY_HO_TRO <= @ToDate);

    -- Dòng cha MTCT-KQ3.1 (Tổng số lượt can thiệp cơ bản)
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '11', 'MTCT-KQ3.1',
           N'Số lượng can thiệp được TCV triển khai theo mô hình SKTT mới (1/sàng lọc, 2/truyền thông, 3/tư vấn cá nhân, 4/chuyển gửi, 5/hỗ trợ tuân thủ điều trị)',
           N'Lượt', 'MAIN', NULL,
           COUNT(*),
           ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
           N'Tổng hợp 5 loại can thiệp cơ bản (F2, F3, F5, F6, F7, F8)',
           N'Dòng chính tính Tổng số lượt; bên dưới hiển thị cặp Số KH & Số lượt từng loại',
           1, 0, 0
    FROM #CT_CoBan;

    -- Helper chèn 5 loại con (mỗi loại gồm Số KH + Số lượt)
    DECLARE @LoaiMap TABLE (LoaiCT INT, CodeBase VARCHAR(20), Ten NVARCHAR(200), Nguon NVARCHAR(200));
    INSERT INTO @LoaiMap VALUES
        (1, 'KQ3.1_1', N'1/ Sàng lọc SKTT (QST)', N'Phiếu F3 (CD45_QST)'),
        (2, 'KQ3.1_2', N'2/ Truyền thông SKTT', N'Phiếu F2 (LOAI_DV = 1)'),
        (3, 'KQ3.1_3', N'3/ Tư vấn cá nhân', N'Phiếu F7 ∪ Phiếu F8'),
        (4, 'KQ3.1_4', N'4/ Chuyển gửi khám SKTT', N'Phiếu F6 (CD45_CHAN_DOAN)'),
        (5, 'KQ3.1_5', N'5/ Hỗ trợ tuân thủ điều trị', N'Phiếu F5 (CD45_TUAN_THU)');

    DECLARE @i INT = 1, @cBase VARCHAR(20), @cTen NVARCHAR(200), @cSrc NVARCHAR(200);
    WHILE @i <= 5
    BEGIN
        SELECT @cBase = CodeBase, @cTen = Ten, @cSrc = Nguon FROM @LoaiMap WHERE LoaiCT = @i;

        INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
        SELECT '', @cBase + '_KH', N'+ ' + @cTen + N' — Số khách hàng (người)', N'Khách hàng', 'SUB', 'MTCT-KQ3.1',
               COUNT(DISTINCT RECORD_ID),
               COUNT(DISTINCT CASE WHEN DOI_TUONG=1 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN DOI_TUONG=2 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN DOI_TUONG=3 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN DOI_TUONG=5 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN DOI_TUONG=4 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN GIOI_TINH=1 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN GIOI_TINH=2 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN GIOI_TINH=3 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN NHOM_TUOI=1 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN NHOM_TUOI=2 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN NHOM_TUOI=3 THEN RECORD_ID END),
               @cSrc, N'Số khách hàng duy nhất nhận can thiệp', 0, 1, 1
        FROM #CT_CoBan WHERE LoaiCT = @i;

        INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
        SELECT '', @cBase + '_LUOT', N'+ ' + @cTen + N' — Số lượt can thiệp', N'Lượt', 'SUB', 'MTCT-KQ3.1',
               COUNT(*),
               ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
               @cSrc, N'Tổng số lượt thực hiện can thiệp', 0, 1, 1
        FROM #CT_CoBan WHERE LoaiCT = @i;

        SET @i = @i + 1;
    END

    -- 12. MTCT-KQ3.2: Can thiệp chữa lành (1/ Vòng tròn chia sẻ LOAI_DV=3, 2/ Nghệ thuật LOAI_DV=4, 3/ Khác - Sinh hoạt nhóm LOAI_DV=2)
    SELECT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI,
           CASE h.LOAI_DV WHEN 3 THEN 1 WHEN 4 THEN 2 WHEN 2 THEN 3 END AS LoaiCL
    INTO #CT_ChuaLanh
    FROM #TmpKH kh INNER JOIN dbo.CD45_HOAT_DONG h ON kh.RECORD_ID = h.RECORD_ID
    WHERE h.COMPLETE_STATUS = '2'
      AND h.LOAI_DV IN (2, 3, 4)
      AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate)
      AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate);

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '12', 'MTCT-KQ3.2',
           N'Số can thiệp chữa lành theo mô hình mới được CBO/DNXH thực hiện (1/vòng tròn chia sẻ, 2/sinh hoạt dựa vào nghệ thuật, 3/các can thiệp khác khi có)',
           N'Lượt', 'MAIN', NULL,
           COUNT(*),
           ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
           N'Phiếu F2 (CD45_HOAT_DONG: LOAI_DV IN 3, 4, 2)',
           N'Mục 3 (Các can thiệp khác) bao gồm Sinh hoạt nhóm (LOAI_DV = 2)',
           1, 0, 0
    FROM #CT_ChuaLanh;

    DECLARE @ClMap TABLE (LoaiCL INT, CodeBase VARCHAR(20), Ten NVARCHAR(200), Nguon NVARCHAR(200));
    INSERT INTO @ClMap VALUES
        (1, 'KQ3.2_1', N'1/ Vòng tròn chia sẻ', N'Phiếu F2 (LOAI_DV = 3)'),
        (2, 'KQ3.2_2', N'2/ Sinh hoạt dựa vào nghệ thuật', N'Phiếu F2 (LOAI_DV = 4)'),
        (3, 'KQ3.2_3', N'3/ Các can thiệp khác (Sinh hoạt nhóm)', N'Phiếu F2 (LOAI_DV = 2)');

    SET @i = 1;
    WHILE @i <= 3
    BEGIN
        SELECT @cBase = CodeBase, @cTen = Ten, @cSrc = Nguon FROM @ClMap WHERE LoaiCL = @i;

        INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
        SELECT '', @cBase + '_KH', N'+ ' + @cTen + N' — Số khách hàng (người)', N'Khách hàng', 'SUB', 'MTCT-KQ3.2',
               COUNT(DISTINCT RECORD_ID),
               COUNT(DISTINCT CASE WHEN DOI_TUONG=1 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN DOI_TUONG=2 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN DOI_TUONG=3 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN DOI_TUONG=5 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN DOI_TUONG=4 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN GIOI_TINH=1 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN GIOI_TINH=2 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN GIOI_TINH=3 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN NHOM_TUOI=1 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN NHOM_TUOI=2 THEN RECORD_ID END),
               COUNT(DISTINCT CASE WHEN NHOM_TUOI=3 THEN RECORD_ID END),
               @cSrc, N'Số khách hàng duy nhất tham gia', 0, 1, 1
        FROM #CT_ChuaLanh WHERE LoaiCL = @i;

        INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
        SELECT '', @cBase + '_LUOT', N'+ ' + @cTen + N' — Số lượt tham gia', N'Lượt', 'SUB', 'MTCT-KQ3.2',
               COUNT(*),
               ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
               ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
               @cSrc, N'Tổng số lượt tham gia', 0, 1, 1
        FROM #CT_ChuaLanh WHERE LoaiCL = @i;

        SET @i = @i + 1;
    END

    -- 13. MTCT-KQ4.1: Số cơ sở KCB về SKTT phối hợp với dự án
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('13', 'MTCT-KQ4.1',
            N'Số lượng cơ sở khám chữa bệnh về sức khoẻ tâm thần phối hợp với dự án và thường xuyên tiếp nhận KH dự án tại mỗi tỉnh',
            N'Cơ sở KCB', 'ORG', NULL,
            @ActiveHospitals,
            N'CD45_CHAN_DOAN (f6_hospital)',
            N'Đếm các cơ sở KCB tiếp nhận khám SKTT trong kỳ theo phạm vi lọc',
            1, 0, 1);

    -- 14. MTCT-KQ4.2: Số KH được bác sĩ tiếp nhận khám (F6)
    SELECT DISTINCT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI
    INTO #Cohort_F6
    FROM #TmpKH kh INNER JOIN dbo.CD45_CHAN_DOAN c ON kh.RECORD_ID = c.RECORD_ID
    WHERE c.COMPLETE_STATUS = '2'
      AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate)
      AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate);

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '14', 'MTCT-KQ4.2',
           N'Số lượng KH do CBO/DNXH chuyển gửi và được các bác sĩ của dự án tiếp nhận tại 6 tỉnh (phân tách theo nhóm KP, độ tuổi và giới tính)',
           N'Khách hàng', 'MAIN', NULL,
           COUNT(*),
           ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
           N'CD45_CHAN_DOAN (Phiếu F6, COMPLETE_STATUS = 2)',
           N'Trùng khớp chỉ tiêu III.4 trên Báo cáo hoạt động CD45', 1, 0, 1
    FROM #Cohort_F6;

    -- 15. MTCT-KQ4.3: Số KH được bác sĩ phản hồi/lưu ý cho TCV sau khám
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '15', 'MTCT-KQ4.3',
           N'Số lượng KH được bác sĩ phản hồi/lưu ý cho TCV sau khám và trong quá trình tuân thủ điều trị (phân tách theo nhóm KP, độ tuổi và giới tính)',
           N'Khách hàng', 'MAIN', NULL,
           Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35,
           N'CD45_CHAN_DOAN (Mục 5 & 7 bắt buộc nhập trên REDCap F6)',
           N'100% phiếu khám F6 hoàn thành đều có khuyến nghị & ghi chú của bác sĩ cho TCV', 1, 0, 1
    FROM #Res WHERE Code = 'MTCT-KQ4.2';

    -- 16. MTCT-KQ4.4: Số KH được chỉ định điều trị (ngoại trú + nội trú) được TCV hỗ trợ tuân thủ (F6 ∩ F5)
    SELECT DISTINCT kh.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI
    INTO #Cohort_F6_NgoaiNoiTru
    FROM #TmpKH kh INNER JOIN dbo.CD45_CHAN_DOAN c ON kh.RECORD_ID = c.RECORD_ID
    WHERE c.COMPLETE_STATUS = '2'
      AND (c.HINH_THUC_DIEU_TRI LIKE '%1%' OR c.HINH_THUC_DIEU_TRI LIKE '%2%')
      AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate)
      AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate);

    SELECT DISTINCT f6.RECORD_ID, f6.DOI_TUONG, f6.GIOI_TINH, f6.NHOM_TUOI
    INTO #Cohort_KQ44
    FROM #Cohort_F6_NgoaiNoiTru f6
    INNER JOIN dbo.CD45_TUAN_THU tt ON f6.RECORD_ID = tt.RECORD_ID
    WHERE tt.COMPLETE_STATUS = '2'
      AND (@ToDate IS NULL OR tt.NGAY_HO_TRO <= @ToDate);

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '16', 'MTCT-KQ4.4',
           N'Số lượng KH được chỉ định điều trị (ngoại trú + nội trú) được TCV hỗ trợ tuân thủ điều trị (phân tách theo nhóm KP, độ tuổi và giới tính)',
           N'Khách hàng', 'MAIN', NULL,
           COUNT(*),
           ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
           N'Phiếu F6 (Ngoại trú/Nội trú) ∩ Phiếu F5 (CD45_TUAN_THU Complete)',
           N'Đảm bảo có bằng chứng phiếu hỗ trợ tuân thủ F5 trên REDCap', 1, 0, 1
    FROM #Cohort_KQ44;

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    SELECT '', 'MTCT-KQ4.4_REF',
           N'+ Tham chiếu: Tổng số KH có chỉ định điều trị ngoại trú/nội trú trên Phiếu khám F6',
           N'Khách hàng', 'SUB', 'MTCT-KQ4.4',
           COUNT(*),
           ISNULL(SUM(CASE WHEN DOI_TUONG=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=5 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN DOI_TUONG=4 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN GIOI_TINH=3 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=1 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=2 THEN 1 ELSE 0 END),0),
           ISNULL(SUM(CASE WHEN NHOM_TUOI=3 THEN 1 ELSE 0 END),0),
           N'Phiếu F6 (HINH_THUC_DIEU_TRI chứa 1 hoặc 2)',
           N'Giúp M&E theo dõi tỷ lệ bao phủ nhập phiếu F5 của TCV', 0, 1, 1
    FROM #Cohort_F6_NgoaiNoiTru;

    -- 17. MTCT-KQ5.1: Số lượng đối tác ký MOU
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('17', 'MTCT-KQ5.1',
            N'Số lượng đối tác ký biên bản ghi nhớ (MOU) để phối hợp triển khai dự án',
            N'Đối tác', 'ORG', NULL,
            CASE WHEN @HasGroupFilter = 1 THEN NULL ELSE @Cfg_KQ5_1 END,
            N'CD45_CHI_SO_DU_AN_CONFIG (CFG_KQ5_1)',
            CASE WHEN @HasGroupFilter = 1 THEN N'N/A (Chỉ số cấp dự án)' ELSE N'Số cơ sở y tế / đối tác đã ký MOU tại kỳ báo cáo' END,
            1, 0, 0);

    -- 18. MTCT-KQ5.2: Số lượng đối tác / các bên liên quan tham gia dự án
    DECLARE @KQ52_Total INT = CASE
        WHEN (@Cfg_KQ5_2_TW + @Cfg_KQ5_2_DP) > 0 THEN (@Cfg_KQ5_2_TW + @Cfg_KQ5_2_DP)
        ELSE (@Cfg_KQ5_2_YT + @Cfg_KQ5_2_CQ + @Cfg_KQ5_2_CA + @Cfg_KQ5_2_HT)
    END;

    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('18', 'MTCT-KQ5.2',
            N'Số lượng đối tác/các bên liên quan tham gia dự án thông qua việc thường xuyên đóng góp ý kiến, tham gia triển khai, rà soát và phản hồi cho dự án',
            N'Đối tác', 'ORG', NULL,
            CASE WHEN @HasGroupFilter = 1 THEN NULL ELSE @KQ52_Total END,
            N'CD45_CHI_SO_DU_AN_CONFIG (CFG_KQ5_2_*)',
            CASE WHEN @HasGroupFilter = 1 THEN N'N/A (Chỉ số cấp dự án)' ELSE N'Tổng hợp từ các cấp TW/Địa phương & Lĩnh vực' END,
            1, 0, 0);

    IF @HasGroupFilter = 0
    BEGIN
        INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
        VALUES
            ('', 'KQ5.2_TW', N'+ Phân theo cấp: Đối tác cấp Trung ương', N'Đối tác', 'SUB', 'MTCT-KQ5.2', @Cfg_KQ5_2_TW, N'Cấu hình Dự án', '', 0, 1, 0),
            ('', 'KQ5.2_DP', N'+ Phân theo cấp: Đối tác cấp Địa phương', N'Đối tác', 'SUB', 'MTCT-KQ5.2', @Cfg_KQ5_2_DP, N'Cấu hình Dự án', '', 0, 1, 0),
            ('', 'KQ5.2_YT', N'+ Phân theo lĩnh vực: Y tế', N'Đối tác', 'SUB', 'MTCT-KQ5.2', @Cfg_KQ5_2_YT, N'Cấu hình Dự án', '', 0, 1, 0),
            ('', 'KQ5.2_CQ', N'+ Phân theo lĩnh vực: Chính quyền địa phương', N'Đối tác', 'SUB', 'MTCT-KQ5.2', @Cfg_KQ5_2_CQ, N'Cấu hình Dự án', '', 0, 1, 0),
            ('', 'KQ5.2_CA', N'+ Phân theo lĩnh vực: Cảnh sát / Công an', N'Đối tác', 'SUB', 'MTCT-KQ5.2', @Cfg_KQ5_2_CA, N'Cấu hình Dự án', '', 0, 1, 0),
            ('', 'KQ5.2_HT', N'+ Phân theo lĩnh vực: Giới học thuật / Nghiên cứu', N'Đối tác', 'SUB', 'MTCT-KQ5.2', @Cfg_KQ5_2_HT, N'Cấu hình Dự án', '', 0, 1, 0);
    END

    -- 19. MTCT-KQ5.3: Số tài liệu dự án đã thẩm định và phổ biến
    INSERT INTO #Res (STT, Code, ChiTieu, DonVi, RowType, ParentCode, Tong, NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown)
    VALUES ('19', 'MTCT-KQ5.3',
            N'Số tài liệu dự án được xây dựng liên quan đến M&E, tài liệu kỹ thuật, giới, vận động chính sách và truyền thông (ấn phẩm, câu chuyện thay đổi...) đã được thẩm định và phổ biến',
            N'Tài liệu', 'ORG', NULL,
            CASE WHEN @HasGroupFilter = 1 THEN NULL ELSE @Cfg_KQ5_3 END,
            N'CD45_CHI_SO_DU_AN_CONFIG (CFG_KQ5_3)',
            CASE WHEN @HasGroupFilter = 1 THEN N'N/A (Chỉ số cấp dự án)' ELSE N'Cập nhật tại Modal Cấu hình Chỉ số Dự án' END,
            1, 0, 0);

    SELECT
        STT, Code, ChiTieu, DonVi, RowType, ParentCode,
        Tong, PUD, PLHIV, TG, SW, MSM,
        Nam, Nu, Khac,
        Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35,
        Denom_Tong, Denom_PUD, Denom_PLHIV, Denom_TG, Denom_SW, Denom_MSM,
        Denom_Nam, Denom_Nu, Denom_Khac,
        Denom_Tuoi_18_25, Denom_Tuoi_26_35, Denom_Tuoi_Tren35,
        NguonDuLieu, GhiChu, IsBold, IndentLevel, CanDrillDown
    FROM #Res
    ORDER BY SortOrder;

    DROP TABLE #TmpKH, #Cohort_Approach, #Cohort_TruyenThong, #Cohort_QST,
               #Cohort_MTCT13_All, #Cohort_MTCT13_AfterQST,
               #Cohort_QST_Pos, #Cohort_QST_Pos_F6, #Cohort_F5_Latest,
               #CT_CoBan, #CT_ChuaLanh, #Cohort_F6, #Cohort_F6_NgoaiNoiTru, #Cohort_KQ44, #Res;
END
GO

-- ============================================================================
-- 5. STORED PROCEDURE: SP_CD45_GetBaoCaoChiSo_DrillDown
-- Hỗ trợ Drill-down theo Mã chỉ số, Nhóm KP, Giới tính, Nhóm tuổi
-- ============================================================================
CREATE OR ALTER PROC dbo.SP_CD45_GetBaoCaoChiSo_DrillDown
    @ChiTieuCode  VARCHAR(50),
    @FromDate     DATE         = NULL,
    @ToDate       DATE         = NULL,
    @CityCode     VARCHAR(100) = NULL,
    @MaNhom       VARCHAR(100) = NULL,
    @IsCumulative BIT          = 1,
    @DoiTuong     INT          = NULL,
    @GioiTinh     INT          = NULL,
    @NhomTuoi     INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EffFromDate DATE = CASE WHEN @IsCumulative = 1 THEN NULL ELSE @FromDate END;
    DECLARE @RefYear INT = YEAR(ISNULL(@ToDate, GETDATE()));

    DECLARE @Var_MaNhomStd VARCHAR(50) = @MaNhom;
    DECLARE @Var_MaNhomMap VARCHAR(50) = @MaNhom;
    IF @MaNhom IS NOT NULL AND @MaNhom <> ''
    BEGIN
        SELECT TOP 1 @Var_MaNhomStd = manhom_tbh, @Var_MaNhomMap = manhom_tbh_map
        FROM dbo.BVTL_NHOM_TBH
        WHERE manhom_tbh = @MaNhom OR manhom_tbh_map = @MaNhom;
    END

    DECLARE @MappedCityCodes TABLE (Code VARCHAR(10));
    IF @CityCode IS NOT NULL AND @CityCode <> ''
    BEGIN
        INSERT INTO @MappedCityCodes(Code)
        SELECT DISTINCT m.OldCityCode
        FROM STRING_SPLIT(@CityCode, ',') s
        JOIN dbo.BVTL_MAP_TINH_CU_MOI m ON LTRIM(RTRIM(s.value)) = m.NewCityCode
        UNION
        SELECT DISTINCT LTRIM(RTRIM(s.value))
        FROM STRING_SPLIT(@CityCode, ',') s
        WHERE NOT EXISTS (SELECT 1 FROM dbo.BVTL_MAP_TINH_CU_MOI m WHERE m.NewCityCode = LTRIM(RTRIM(s.value)))
          AND LTRIM(RTRIM(s.value)) <> '';
    END

    -- Trường hợp Drill-down danh mục TCV (MTCT-KQ1.2, MTCT-KQ2.1)
    IF @ChiTieuCode IN ('MTCT-KQ1.2', 'MTCT-KQ2.1')
    BEGIN
        SELECT
            CAST(tcv.MA_TCV AS VARCHAR(50)) AS RECORD_ID,
            tcv.CITY_CODE,
            tcv.MA_NHOM,
            tcv.MA_TCV,
            CASE tcv.GIOI_TINH WHEN 1 THEN N'Nam' WHEN 2 THEN N'Nữ' ELSE N'Khác / Chưa rõ' END AS DOI_TUONG_TEXT,
            CONVERT(VARCHAR(10), ISNULL(tcv.CreatedDate, GETDATE()), 103) AS NGAY_THUC_HIEN,
            tcv.TEN_TCV + CASE WHEN tcv.IsActive = 1 THEN N' (Đang hoạt động)' ELSE N' (Ngừng hoạt động)' END AS CHI_TIET
        FROM dbo.CD45_NHOM_TCV tcv
        WHERE tcv.MADUAN = 'CD45'
          AND (@ChiTieuCode <> 'MTCT-KQ1.2' OR ISNULL(tcv.LA_TCV_NAM_1, 1) = 1)
          AND (@ChiTieuCode <> 'MTCT-KQ2.1' OR tcv.IsActive = 1)
          AND (@CityCode IS NULL OR @CityCode = '' OR tcv.CITY_CODE IN (SELECT Code FROM @MappedCityCodes))
          AND (@MaNhom IS NULL OR @MaNhom = '' OR tcv.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom))
          AND (@GioiTinh IS NULL OR @GioiTinh = 0 OR (@GioiTinh = 1 AND tcv.GIOI_TINH = 1) OR (@GioiTinh = 2 AND tcv.GIOI_TINH = 2) OR (@GioiTinh = 3 AND ISNULL(tcv.GIOI_TINH, 3) NOT IN (1, 2)))
        ORDER BY tcv.CITY_CODE, tcv.MA_NHOM, TRY_CAST(tcv.MA_TCV AS INT);
        RETURN;
    END

    -- Tập KH đã lọc theo Tỉnh, Nhóm, KP, Giới tính, Nhóm tuổi
    SELECT
        kh.RECORD_ID,
        kh.CITY_CODE,
        kh.MA_NHOM,
        kh.DOI_TUONG,
        CASE kh.DOI_TUONG
            WHEN 1 THEN 'PUD'
            WHEN 2 THEN 'PLHIV'
            WHEN 3 THEN 'TG'
            WHEN 4 THEN 'MSM'
            WHEN 5 THEN 'SW'
            ELSE N'Khác'
        END AS DOI_TUONG_TEXT,
        kh.NGAY_THAM_GIA,
        CASE
            WHEN kh.GIOI_TINH_TU_XD IN (3, 4) OR kh.GIOI_TINH_KHAI_SINH = 3 THEN 3
            WHEN kh.GIOI_TINH_TU_XD = 1 OR (kh.GIOI_TINH_TU_XD IS NULL AND kh.GIOI_TINH_KHAI_SINH = 1) THEN 1
            WHEN kh.GIOI_TINH_TU_XD = 2 OR (kh.GIOI_TINH_TU_XD IS NULL AND kh.GIOI_TINH_KHAI_SINH = 2) THEN 2
            ELSE 3
        END AS GIOI_TINH,
        CASE
            WHEN (@RefYear - kh.NAM_SINH) BETWEEN 18 AND 25 THEN 1
            WHEN (@RefYear - kh.NAM_SINH) BETWEEN 26 AND 35 THEN 2
            WHEN (@RefYear - kh.NAM_SINH) >= 36 THEN 3
            ELSE 2
        END AS NHOM_TUOI
    INTO #TmpKH_Full
    FROM dbo.CD45_KH kh
    WHERE (@CityCode IS NULL OR @CityCode = '' OR kh.CITY_CODE IN (SELECT Code FROM @MappedCityCodes))
      AND (@MaNhom IS NULL OR @MaNhom = '' OR kh.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom) OR kh.REDCAP_DAG IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom))
      AND kh.COMPLETE_STATUS = '2';

    -- Lọc theo chiều phân tách từ ô người dùng click
    SELECT * INTO #TmpKH FROM #TmpKH_Full
    WHERE (@DoiTuong IS NULL OR @DoiTuong = 0 OR DOI_TUONG = @DoiTuong)
      AND (@GioiTinh IS NULL OR @GioiTinh = 0 OR GIOI_TINH = @GioiTinh)
      AND (@NhomTuoi IS NULL OR @NhomTuoi = 0 OR NHOM_TUOI = @NhomTuoi);

    CREATE CLUSTERED INDEX IX_DD_TmpKH ON #TmpKH(RECORD_ID);

    -- Drill-down CBO (MTCT-KQ1.1, KQ1.1_CBO, MTCT-KQ2.2)
    IF @ChiTieuCode IN ('MTCT-KQ1.1', 'KQ1.1_CBO', 'MTCT-KQ2.2')
    BEGIN
        IF @ChiTieuCode = 'MTCT-KQ2.2'
        BEGIN
            SELECT n.manhom_tbh AS RECORD_ID, n.city_code AS CITY_CODE, ISNULL(n.manhom_tbh_map, n.manhom_tbh) AS MA_NHOM,
                   '' AS MA_TCV, N'CBO/DNXH' AS DOI_TUONG_TEXT, '' AS NGAY_THUC_HIEN, n.tennhom_tbh AS CHI_TIET
            FROM dbo.BVTL_NHOM_TBH n
            WHERE n.maduan = 'CD45' AND ISNULL(n.IS_ACTIVE, 1) = 1
              AND (@CityCode IS NULL OR @CityCode = '' OR n.city_code IN (SELECT Code FROM @MappedCityCodes))
              AND (@MaNhom IS NULL OR @MaNhom = '' OR n.manhom_tbh IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom) OR n.manhom_tbh_map IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom))
            ORDER BY n.city_code, n.tennhom_tbh;
        END
        ELSE
        BEGIN
            ;WITH CTE_ActiveGroups AS (
                SELECT DISTINCT kh.MA_NHOM, kh.CITY_CODE FROM #TmpKH_Full kh INNER JOIN dbo.CD45_HOAT_DONG h ON kh.RECORD_ID = h.RECORD_ID
                WHERE h.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate) AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate)
                UNION
                SELECT DISTINCT kh.MA_NHOM, kh.CITY_CODE FROM #TmpKH_Full kh INNER JOIN dbo.CD45_QST q ON kh.RECORD_ID = q.RECORD_ID
                WHERE q.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR q.NGAY_SANG_LOC >= @EffFromDate) AND (@ToDate IS NULL OR q.NGAY_SANG_LOC <= @ToDate)
                UNION
                SELECT DISTINCT kh.MA_NHOM, kh.CITY_CODE FROM #TmpKH_Full kh INNER JOIN dbo.CD45_CHAN_DOAN c ON kh.RECORD_ID = c.RECORD_ID
                WHERE c.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate) AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate)
            )
            SELECT DISTINCT g.MA_NHOM AS RECORD_ID, g.CITY_CODE, g.MA_NHOM, '' AS MA_TCV, N'CBO/DNXH' AS DOI_TUONG_TEXT,
                   '' AS NGAY_THUC_HIEN, ISNULL(n.tennhom_tbh, g.MA_NHOM) AS CHI_TIET
            FROM CTE_ActiveGroups g
            LEFT JOIN dbo.BVTL_NHOM_TBH n ON n.maduan = 'CD45' AND (n.manhom_tbh = g.MA_NHOM OR n.manhom_tbh_map = g.MA_NHOM)
            WHERE g.MA_NHOM IS NOT NULL AND g.MA_NHOM <> ''
            ORDER BY g.CITY_CODE, g.MA_NHOM;
        END
        RETURN;
    END

    -- Drill-down Cơ sở y tế (MTCT-KQ4.1, KQ1.1_CSYT)
    IF @ChiTieuCode IN ('MTCT-KQ4.1', 'KQ1.1_CSYT')
    BEGIN
        SELECT
            LTRIM(RTRIM(c.CO_SO_Y_TE)) AS RECORD_ID,
            MAX(kh.CITY_CODE) AS CITY_CODE,
            '' AS MA_NHOM,
            '' AS MA_TCV,
            N'Cơ sở KCB' AS DOI_TUONG_TEXT,
            CONVERT(VARCHAR(10), MAX(c.NGAY_KHAM), 103) AS NGAY_THUC_HIEN,
            N'Lần khám: 1 - Cơ sở: ' + LTRIM(RTRIM(c.CO_SO_Y_TE)) + N' (' + CAST(COUNT(DISTINCT c.RECORD_ID) AS VARCHAR) + N' KH)' AS CHI_TIET
        FROM dbo.CD45_CHAN_DOAN c
        INNER JOIN #TmpKH_Full kh ON c.RECORD_ID = kh.RECORD_ID
        WHERE c.COMPLETE_STATUS = '2'
          AND c.CO_SO_Y_TE IS NOT NULL AND LTRIM(RTRIM(c.CO_SO_Y_TE)) <> ''
          AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate)
          AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate)
        GROUP BY LTRIM(RTRIM(c.CO_SO_Y_TE))
        ORDER BY RECORD_ID;
        RETURN;
    END

    -- Chuẩn bị tập Cohort Approach cho MTCT1.1 - 1.3
    SELECT DISTINCT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, kh.DOI_TUONG_TEXT, kh.NGAY_THAM_GIA
    INTO #DD_Approach
    FROM #TmpKH kh
    WHERE (
        @IsCumulative = 1
        AND (@ToDate IS NULL OR kh.NGAY_THAM_GIA IS NULL OR kh.NGAY_THAM_GIA <= @ToDate)
    )
    OR (
        @IsCumulative = 0
        AND (
            ((@EffFromDate IS NULL OR kh.NGAY_THAM_GIA >= @EffFromDate) AND (@ToDate IS NULL OR kh.NGAY_THAM_GIA <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_HOAT_DONG h WHERE h.RECORD_ID = kh.RECORD_ID AND h.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate) AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_QST q WHERE q.RECORD_ID = kh.RECORD_ID AND q.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR q.NGAY_SANG_LOC >= @EffFromDate) AND (@ToDate IS NULL OR q.NGAY_SANG_LOC <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_CHAN_DOAN c WHERE c.RECORD_ID = kh.RECORD_ID AND c.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate) AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_TU_VAN_L1 t1 WHERE t1.RECORD_ID = kh.RECORD_ID AND t1.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t1.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t1.NGAY_TU_VAN <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_TU_VAN_L2 t2 WHERE t2.RECORD_ID = kh.RECORD_ID AND t2.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t2.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t2.NGAY_TU_VAN <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_TUAN_THU tt WHERE tt.RECORD_ID = kh.RECORD_ID AND tt.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR tt.NGAY_HO_TRO >= @EffFromDate) AND (@ToDate IS NULL OR tt.NGAY_HO_TRO <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_HO_TRO_XH xh WHERE xh.RECORD_ID = kh.RECORD_ID AND xh.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR xh.NGAY_HO_TRO >= @EffFromDate) AND (@ToDate IS NULL OR xh.NGAY_HO_TRO <= @ToDate))
        )
    );

    -- MTCT1.1_DEN / MTCT1.2_DEN
    IF @ChiTieuCode IN ('MTCT1.1_DEN', 'MTCT1.2_DEN')
    BEGIN
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, '' AS MA_TCV, DOI_TUONG_TEXT,
               CONVERT(VARCHAR(10), NGAY_THAM_GIA, 103) AS NGAY_THUC_HIEN,
               N'Hồ sơ khách hàng tiếp cận (F1)' AS CHI_TIET
        FROM #DD_Approach
        ORDER BY NGAY_THAM_GIA DESC, RECORD_ID;
    END
    -- MTCT1.1 / MTCT1.1_NUM / KQ3.1_2_KH
    ELSE IF @ChiTieuCode IN ('MTCT1.1', 'MTCT1.1_NUM', 'KQ3.1_2_KH')
    BEGIN
        ;WITH CTE AS (
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, h.MA_TCV, kh.DOI_TUONG_TEXT, h.NGAY_HOAT_DONG,
                   CONVERT(VARCHAR(10), h.NGAY_HOAT_DONG, 103) AS NGAY_THUC_HIEN,
                   N'Truyền thông: ' + ISNULL(h.CHU_DE, '') AS CHI_TIET,
                   ROW_NUMBER() OVER (PARTITION BY kh.RECORD_ID ORDER BY h.NGAY_HOAT_DONG DESC, h.REPEAT_INSTANCE DESC) AS rn
            FROM #DD_Approach kh
            INNER JOIN dbo.CD45_HOAT_DONG h ON kh.RECORD_ID = h.RECORD_ID
            WHERE h.COMPLETE_STATUS = '2' AND h.LOAI_DV = 1
              AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate)
              AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE WHERE rn = 1 ORDER BY NGAY_HOAT_DONG DESC, RECORD_ID;
    END
    -- KQ3.1_2_LUOT
    ELSE IF @ChiTieuCode = 'KQ3.1_2_LUOT'
    BEGIN
        SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, h.MA_TCV, kh.DOI_TUONG_TEXT,
               CONVERT(VARCHAR(10), h.NGAY_HOAT_DONG, 103) AS NGAY_THUC_HIEN,
               N'Lần ' + CAST(ISNULL(h.REPEAT_INSTANCE, 1) AS VARCHAR) + N' - Truyền thông: ' + ISNULL(h.CHU_DE, '') AS CHI_TIET
        FROM #TmpKH kh INNER JOIN dbo.CD45_HOAT_DONG h ON kh.RECORD_ID = h.RECORD_ID
        WHERE h.COMPLETE_STATUS = '2' AND h.LOAI_DV = 1
          AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate)
          AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate)
        ORDER BY h.NGAY_HOAT_DONG DESC, kh.RECORD_ID;
    END
    -- MTCT1.2 / MTCT1.2_NUM / KQ3.1_1_KH
    ELSE IF @ChiTieuCode IN ('MTCT1.2', 'MTCT1.2_NUM', 'KQ3.1_1_KH')
    BEGIN
        ;WITH CTE AS (
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, q.MA_TCV, kh.DOI_TUONG_TEXT, q.NGAY_SANG_LOC,
                   CONVERT(VARCHAR(10), q.NGAY_SANG_LOC, 103) AS NGAY_THUC_HIEN,
                   N'Điểm QST: ' + CAST(ISNULL(q.DIEM_QST, 0) AS VARCHAR) + N' (Mức ' + CAST(ISNULL(q.MUC_QST, 0) AS VARCHAR) + ')' AS CHI_TIET,
                   ROW_NUMBER() OVER (PARTITION BY kh.RECORD_ID ORDER BY q.NGAY_SANG_LOC DESC, q.REPEAT_INSTANCE DESC) AS rn
            FROM #DD_Approach kh
            INNER JOIN dbo.CD45_QST q ON kh.RECORD_ID = q.RECORD_ID
            WHERE q.COMPLETE_STATUS = '2'
              AND (@EffFromDate IS NULL OR q.NGAY_SANG_LOC >= @EffFromDate)
              AND (@ToDate IS NULL OR q.NGAY_SANG_LOC <= @ToDate)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE WHERE rn = 1 ORDER BY NGAY_SANG_LOC DESC, RECORD_ID;
    END
    -- KQ3.1_1_LUOT
    ELSE IF @ChiTieuCode = 'KQ3.1_1_LUOT'
    BEGIN
        SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, q.MA_TCV, kh.DOI_TUONG_TEXT,
               CONVERT(VARCHAR(10), q.NGAY_SANG_LOC, 103) AS NGAY_THUC_HIEN,
               N'Lần ' + CAST(ISNULL(q.REPEAT_INSTANCE, 1) AS VARCHAR) + N' - Điểm QST: ' + CAST(ISNULL(q.DIEM_QST, 0) AS VARCHAR) + N' (Mức ' + CAST(ISNULL(q.MUC_QST, 0) AS VARCHAR) + ')' AS CHI_TIET
        FROM #TmpKH kh INNER JOIN dbo.CD45_QST q ON kh.RECORD_ID = q.RECORD_ID
        WHERE q.COMPLETE_STATUS = '2'
          AND (@EffFromDate IS NULL OR q.NGAY_SANG_LOC >= @EffFromDate)
          AND (@ToDate IS NULL OR q.NGAY_SANG_LOC <= @ToDate)
        ORDER BY q.NGAY_SANG_LOC DESC, kh.RECORD_ID;
    END
    -- MTCT1.3 / MTCT1.3_SUB
    ELSE IF @ChiTieuCode IN ('MTCT1.3', 'MTCT1.3_SUB')
    BEGIN
        SELECT DISTINCT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, '' AS MA_TCV, kh.DOI_TUONG_TEXT,
               CONVERT(VARCHAR(10), q.NGAY_SANG_LOC, 103) AS NGAY_THUC_HIEN,
               CASE WHEN @ChiTieuCode = 'MTCT1.3_SUB' THEN N'Đã QST & nhận can thiệp sau sàng lọc' ELSE N'Đã QST & nhận >= 1 can thiệp CBO' END AS CHI_TIET
        FROM #DD_Approach kh
        INNER JOIN dbo.CD45_QST q ON kh.RECORD_ID = q.RECORD_ID AND q.COMPLETE_STATUS = '2'
          AND (@EffFromDate IS NULL OR q.NGAY_SANG_LOC >= @EffFromDate) AND (@ToDate IS NULL OR q.NGAY_SANG_LOC <= @ToDate)
        WHERE (
            (@ChiTieuCode = 'MTCT1.3' AND EXISTS (SELECT 1 FROM dbo.CD45_HOAT_DONG h WHERE h.RECORD_ID = kh.RECORD_ID AND h.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate) AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate)))
            OR (@ChiTieuCode = 'MTCT1.3_SUB' AND EXISTS (SELECT 1 FROM dbo.CD45_HOAT_DONG h WHERE h.RECORD_ID = kh.RECORD_ID AND h.COMPLETE_STATUS = '2' AND h.LOAI_DV IN (2,3,4) AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate) AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate)))
            OR EXISTS (SELECT 1 FROM dbo.CD45_TU_VAN_L1 t1 WHERE t1.RECORD_ID = kh.RECORD_ID AND t1.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t1.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t1.NGAY_TU_VAN <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_TU_VAN_L2 t2 WHERE t2.RECORD_ID = kh.RECORD_ID AND t2.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR t2.NGAY_TU_VAN >= @EffFromDate) AND (@ToDate IS NULL OR t2.NGAY_TU_VAN <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_CHAN_DOAN c WHERE c.RECORD_ID = kh.RECORD_ID AND c.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate) AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_TUAN_THU tt WHERE tt.RECORD_ID = kh.RECORD_ID AND tt.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR tt.NGAY_HO_TRO >= @EffFromDate) AND (@ToDate IS NULL OR tt.NGAY_HO_TRO <= @ToDate))
            OR EXISTS (SELECT 1 FROM dbo.CD45_HO_TRO_XH xh WHERE xh.RECORD_ID = kh.RECORD_ID AND xh.COMPLETE_STATUS = '2' AND (@EffFromDate IS NULL OR xh.NGAY_HO_TRO >= @EffFromDate) AND (@ToDate IS NULL OR xh.NGAY_HO_TRO <= @ToDate))
        )
        ORDER BY kh.RECORD_ID;
    END
    -- MTCT1.4_DEN
    ELSE IF @ChiTieuCode = 'MTCT1.4_DEN'
    BEGIN
        ;WITH CTE AS (
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, q.MA_TCV, kh.DOI_TUONG_TEXT, q.NGAY_SANG_LOC,
                   CONVERT(VARCHAR(10), q.NGAY_SANG_LOC, 103) AS NGAY_THUC_HIEN,
                   N'QST Dương tính - Điểm: ' + CAST(ISNULL(q.DIEM_QST, 0) AS VARCHAR) + N' (Mức ' + CAST(ISNULL(q.MUC_QST, 0) AS VARCHAR) + ')' AS CHI_TIET,
                   ROW_NUMBER() OVER (PARTITION BY kh.RECORD_ID ORDER BY q.MUC_QST ASC, q.NGAY_SANG_LOC DESC) AS rn
            FROM #TmpKH kh INNER JOIN dbo.CD45_QST q ON kh.RECORD_ID = q.RECORD_ID
            WHERE q.COMPLETE_STATUS = '2' AND q.MUC_QST IN (1, 2)
              AND (@EffFromDate IS NULL OR q.NGAY_SANG_LOC >= @EffFromDate)
              AND (@ToDate IS NULL OR q.NGAY_SANG_LOC <= @ToDate)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE WHERE rn = 1 ORDER BY NGAY_SANG_LOC DESC, RECORD_ID;
    END
    -- MTCT1.4 / MTCT1.4_NUM
    ELSE IF @ChiTieuCode IN ('MTCT1.4', 'MTCT1.4_NUM')
    BEGIN
        ;WITH CTE_Pos AS (
            SELECT DISTINCT RECORD_ID FROM dbo.CD45_QST
            WHERE COMPLETE_STATUS = '2' AND MUC_QST IN (1, 2)
              AND (@EffFromDate IS NULL OR NGAY_SANG_LOC >= @EffFromDate)
              AND (@ToDate IS NULL OR NGAY_SANG_LOC <= @ToDate)
        ),
        CTE AS (
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, c.MA_TCV, kh.DOI_TUONG_TEXT, c.NGAY_KHAM,
                   CONVERT(VARCHAR(10), c.NGAY_KHAM, 103) AS NGAY_THUC_HIEN,
                   N'Lần khám: ' + CAST(ISNULL(c.LAN_KHAM, 1) AS VARCHAR) + N' - Cơ sở: ' + ISNULL(c.CO_SO_Y_TE, '') + CASE WHEN c.CHAN_DOAN_CHINH IS NOT NULL AND c.CHAN_DOAN_CHINH <> '' THEN N' - Chẩn đoán: ' + c.CHAN_DOAN_CHINH ELSE N'' END AS CHI_TIET,
                   ROW_NUMBER() OVER (PARTITION BY kh.RECORD_ID ORDER BY c.NGAY_KHAM DESC) AS rn
            FROM #TmpKH kh
            INNER JOIN CTE_Pos p ON kh.RECORD_ID = p.RECORD_ID
            INNER JOIN dbo.CD45_CHAN_DOAN c ON kh.RECORD_ID = c.RECORD_ID
            WHERE c.COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE WHERE rn = 1 ORDER BY NGAY_KHAM DESC, RECORD_ID;
    END
    -- MTCT1.5 / MTCT1.5_NUM / MTCT1.5_DEN
    ELSE IF @ChiTieuCode IN ('MTCT1.5', 'MTCT1.5_NUM', 'MTCT1.5_DEN')
    BEGIN
        ;WITH CTE_F5 AS (
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, tt.MA_TCV, kh.DOI_TUONG_TEXT, tt.NGAY_HO_TRO, tt.TINH_TRANG_SKTT,
                   CONVERT(VARCHAR(10), tt.NGAY_HO_TRO, 103) AS NGAY_THUC_HIEN,
                   CASE tt.TINH_TRANG_SKTT WHEN 1 THEN N'SKTT hiện tại: Tốt hơn' WHEN 2 THEN N'SKTT hiện tại: Không thay đổi' WHEN 3 THEN N'SKTT hiện tại: Xấu hơn' ELSE N'Chưa có đánh giá f5_feeling' END AS CHI_TIET,
                   ROW_NUMBER() OVER (PARTITION BY kh.RECORD_ID ORDER BY tt.NGAY_HO_TRO DESC, ISNULL(tt.REPEAT_INSTANCE, 1) DESC, tt.ID DESC) AS rn
            FROM #TmpKH kh INNER JOIN dbo.CD45_TUAN_THU tt ON kh.RECORD_ID = tt.RECORD_ID
            WHERE tt.COMPLETE_STATUS = '2' AND ISNULL(tt.HINH_THUC_DIEU_TRI, 0) IN (1, 2, 3, 5)
              AND (@EffFromDate IS NULL OR tt.NGAY_HO_TRO >= @EffFromDate)
              AND (@ToDate IS NULL OR tt.NGAY_HO_TRO <= @ToDate)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE_F5
        WHERE rn = 1 AND (@ChiTieuCode = 'MTCT1.5_DEN' OR TINH_TRANG_SKTT = 1)
        ORDER BY NGAY_HO_TRO DESC, RECORD_ID;
    END
    -- KQ3.1_3_KH / KQ3.1_3_LUOT (Tư vấn cá nhân)
    ELSE IF @ChiTieuCode IN ('KQ3.1_3_KH', 'KQ3.1_3_LUOT')
    BEGIN
        ;WITH CTE_TV AS (
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV, N'Tư vấn lần 1 (F7)' AS LOAI FROM dbo.CD45_TU_VAN_L1 WHERE COMPLETE_STATUS = '2'
            UNION ALL
            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV, N'Tư vấn lần 2+ (F8)' AS LOAI FROM dbo.CD45_TU_VAN_L2 WHERE COMPLETE_STATUS = '2'
        ),
        CTE_R AS (
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, tv.MA_TCV, kh.DOI_TUONG_TEXT, tv.NGAY_TU_VAN,
                   CONVERT(VARCHAR(10), tv.NGAY_TU_VAN, 103) AS NGAY_THUC_HIEN, tv.LOAI AS CHI_TIET,
                   ROW_NUMBER() OVER (PARTITION BY kh.RECORD_ID ORDER BY tv.NGAY_TU_VAN DESC) AS rn
            FROM #TmpKH kh INNER JOIN CTE_TV tv ON kh.RECORD_ID = tv.RECORD_ID
            WHERE (@EffFromDate IS NULL OR tv.NGAY_TU_VAN >= @EffFromDate)
              AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE_R WHERE (@ChiTieuCode = 'KQ3.1_3_LUOT' OR rn = 1)
        ORDER BY NGAY_TU_VAN DESC, RECORD_ID;
    END
    -- KQ3.1_4_KH / KQ3.1_4_LUOT / MTCT-KQ4.2 / MTCT-KQ4.3 (Khám SKTT F6)
    ELSE IF @ChiTieuCode IN ('KQ3.1_4_KH', 'KQ3.1_4_LUOT', 'MTCT-KQ4.2', 'MTCT-KQ4.3')
    BEGIN
        ;WITH CTE_R AS (
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, c.MA_TCV, kh.DOI_TUONG_TEXT, c.NGAY_KHAM,
                   CONVERT(VARCHAR(10), c.NGAY_KHAM, 103) AS NGAY_THUC_HIEN,
                   N'Lần khám: ' + CAST(ISNULL(c.LAN_KHAM, 1) AS VARCHAR) + N' - Cơ sở: ' + ISNULL(c.CO_SO_Y_TE, '') + CASE WHEN c.CHAN_DOAN_CHINH IS NOT NULL AND c.CHAN_DOAN_CHINH <> '' THEN N' - Chẩn đoán: ' + c.CHAN_DOAN_CHINH ELSE N'' END AS CHI_TIET,
                   ROW_NUMBER() OVER (PARTITION BY kh.RECORD_ID ORDER BY c.NGAY_KHAM DESC, c.REPEAT_INSTANCE DESC) AS rn
            FROM #TmpKH kh INNER JOIN dbo.CD45_CHAN_DOAN c ON kh.RECORD_ID = c.RECORD_ID
            WHERE c.COMPLETE_STATUS = '2'
              AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate)
              AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE_R WHERE (@ChiTieuCode = 'KQ3.1_4_LUOT' OR rn = 1)
        ORDER BY NGAY_KHAM DESC, RECORD_ID;
    END
    -- KQ3.1_5_KH / KQ3.1_5_LUOT (Hỗ trợ tuân thủ F5)
    ELSE IF @ChiTieuCode IN ('KQ3.1_5_KH', 'KQ3.1_5_LUOT')
    BEGIN
        ;WITH CTE_R AS (
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, tt.MA_TCV, kh.DOI_TUONG_TEXT, tt.NGAY_HO_TRO,
                   CONVERT(VARCHAR(10), tt.NGAY_HO_TRO, 103) AS NGAY_THUC_HIEN,
                   N'Hỗ trợ tuân thủ điều trị (Lần ' + CAST(ISNULL(tt.REPEAT_INSTANCE, 1) AS VARCHAR) + ')' AS CHI_TIET,
                   ROW_NUMBER() OVER (PARTITION BY kh.RECORD_ID ORDER BY tt.NGAY_HO_TRO DESC, tt.REPEAT_INSTANCE DESC) AS rn
            FROM #TmpKH kh INNER JOIN dbo.CD45_TUAN_THU tt ON kh.RECORD_ID = tt.RECORD_ID
            WHERE tt.COMPLETE_STATUS = '2'
              AND (@EffFromDate IS NULL OR tt.NGAY_HO_TRO >= @EffFromDate)
              AND (@ToDate IS NULL OR tt.NGAY_HO_TRO <= @ToDate)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE_R WHERE (@ChiTieuCode = 'KQ3.1_5_LUOT' OR rn = 1)
        ORDER BY NGAY_HO_TRO DESC, RECORD_ID;
    END
    -- KQ3.2_1, KQ3.2_2, KQ3.2_3 (Chữa lành & Sinh hoạt nhóm F2)
    ELSE IF @ChiTieuCode IN ('KQ3.2_1_KH', 'KQ3.2_1_LUOT', 'KQ3.2_2_KH', 'KQ3.2_2_LUOT', 'KQ3.2_3_KH', 'KQ3.2_3_LUOT')
    BEGIN
        DECLARE @TargetLoaiDV INT = CASE
            WHEN @ChiTieuCode LIKE 'KQ3.2_1%' THEN 3
            WHEN @ChiTieuCode LIKE 'KQ3.2_2%' THEN 4
            ELSE 2
        END;
        ;WITH CTE_R AS (
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, h.MA_TCV, kh.DOI_TUONG_TEXT, h.NGAY_HOAT_DONG,
                   CONVERT(VARCHAR(10), h.NGAY_HOAT_DONG, 103) AS NGAY_THUC_HIEN,
                   CASE h.LOAI_DV WHEN 2 THEN N'Sinh hoạt nhóm' WHEN 3 THEN N'Vòng tròn chia sẻ' ELSE N'Trị liệu nghệ thuật' END + ISNULL(N' - ' + h.CHU_DE, '') AS CHI_TIET,
                   ROW_NUMBER() OVER (PARTITION BY kh.RECORD_ID ORDER BY h.NGAY_HOAT_DONG DESC, h.REPEAT_INSTANCE DESC) AS rn
            FROM #TmpKH kh INNER JOIN dbo.CD45_HOAT_DONG h ON kh.RECORD_ID = h.RECORD_ID
            WHERE h.COMPLETE_STATUS = '2' AND h.LOAI_DV = @TargetLoaiDV
              AND (@EffFromDate IS NULL OR h.NGAY_HOAT_DONG >= @EffFromDate)
              AND (@ToDate IS NULL OR h.NGAY_HOAT_DONG <= @ToDate)
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE_R WHERE (@ChiTieuCode LIKE '%_LUOT' OR rn = 1)
        ORDER BY NGAY_HOAT_DONG DESC, RECORD_ID;
    END
    -- MTCT-KQ4.4 / MTCT-KQ4.4_REF
    ELSE IF @ChiTieuCode IN ('MTCT-KQ4.4', 'MTCT-KQ4.4_REF')
    BEGIN
        ;WITH CTE_F6 AS (
            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, c.MA_TCV, kh.DOI_TUONG_TEXT, c.NGAY_KHAM,
                   CONVERT(VARCHAR(10), c.NGAY_KHAM, 103) AS NGAY_THUC_HIEN,
                   N'Lần khám: ' + CAST(ISNULL(c.LAN_KHAM, 1) AS VARCHAR) + N' - Cơ sở: ' + ISNULL(c.CO_SO_Y_TE, '') + CASE WHEN c.CHAN_DOAN_CHINH IS NOT NULL AND c.CHAN_DOAN_CHINH <> '' THEN N' - Chẩn đoán: ' + c.CHAN_DOAN_CHINH ELSE N'' END AS CHI_TIET,
                   ROW_NUMBER() OVER (PARTITION BY kh.RECORD_ID ORDER BY c.NGAY_KHAM DESC) AS rn
            FROM #TmpKH kh INNER JOIN dbo.CD45_CHAN_DOAN c ON kh.RECORD_ID = c.RECORD_ID
            WHERE c.COMPLETE_STATUS = '2'
              AND (c.HINH_THUC_DIEU_TRI LIKE '%1%' OR c.HINH_THUC_DIEU_TRI LIKE '%2%')
              AND (@EffFromDate IS NULL OR c.NGAY_KHAM >= @EffFromDate)
              AND (@ToDate IS NULL OR c.NGAY_KHAM <= @ToDate)
              AND (@ChiTieuCode = 'MTCT-KQ4.4_REF' OR EXISTS (
                  SELECT 1 FROM dbo.CD45_TUAN_THU tt WHERE tt.RECORD_ID = kh.RECORD_ID AND tt.COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR tt.NGAY_HO_TRO <= @ToDate)
              ))
        )
        SELECT RECORD_ID, CITY_CODE, MA_NHOM, MA_TCV, DOI_TUONG_TEXT, NGAY_THUC_HIEN, CHI_TIET
        FROM CTE_F6 WHERE rn = 1 ORDER BY NGAY_KHAM DESC, RECORD_ID;
    END

    DROP TABLE #TmpKH_Full, #TmpKH, #DD_Approach;
END
GO

-- ============================================================================
-- 6. THÊM MENU "Báo cáo Chỉ số CD45" VÀO NHÓM MENU "Dự án CD45 (DREAMH)"
-- ============================================================================
DECLARE @ParentID INT;
SELECT TOP 1 @ParentID = [ID]
FROM [dbo].[BVTL_QT_PAGE_MENU]
WHERE [ID] = 22 OR [NAME] = N'Báo cáo';

IF @ParentID IS NULL SET @ParentID = 22;

DECLARE @PageID INT;
SELECT @PageID = [ID]
FROM [dbo].[BVTL_QT_PAGE_MENU]
WHERE [HREF_URL] = '/BaoCaoChiSoCD45/Index';

IF @PageID IS NULL
BEGIN
    INSERT INTO [dbo].[BVTL_QT_PAGE_MENU] (
        [NAME], [DESCRIPTION], [IS_ACTIVE], [ORDER_BY], [CONTROLLER_NAME],
        [HREF_URL], [PARENT_PAGE_ID], [IS_SYSTEM_ROLE], [TEN_DU_AN], [TITLE_GROUP_MENU]
    )
    VALUES (
        N'Báo cáo Chỉ số CD45',
        N'Báo cáo tổng hợp kết quả thực hiện các chỉ số dự án DREAMH (CD45)',
        1,
        34,
        'BaoCaoChiSoCD45',
        '/BaoCaoChiSoCD45/Index',
        @ParentID,
        0,
        N'CD45',
        N'Dự án CD45 (DREAMH)'
    );
    SET @PageID = SCOPE_IDENTITY();
    PRINT N'Đã thêm mới menu Báo cáo Chỉ số CD45, PageID = ' + CAST(@PageID AS NVARCHAR(20));
END
ELSE
BEGIN
    UPDATE [dbo].[BVTL_QT_PAGE_MENU]
    SET [NAME] = N'Báo cáo Chỉ số CD45',
        [DESCRIPTION] = N'Báo cáo tổng hợp kết quả thực hiện các chỉ số dự án DREAMH (CD45)',
        [IS_ACTIVE] = 1,
        [ORDER_BY] = 34,
        [CONTROLLER_NAME] = 'BaoCaoChiSoCD45',
        [HREF_URL] = '/BaoCaoChiSoCD45/Index',
        [PARENT_PAGE_ID] = @ParentID,
        [IS_SYSTEM_ROLE] = 0,
        [TEN_DU_AN] = N'CD45',
        [TITLE_GROUP_MENU] = N'Dự án CD45 (DREAMH)'
    WHERE [ID] = @PageID;
    PRINT N'Đã cập nhật menu Báo cáo Chỉ số CD45, PageID = ' + CAST(@PageID AS NVARCHAR(20));
END

INSERT INTO [dbo].[BVTL_QT_QUYEN_PAGE] ([RoleID], [PageID], [IS_ACTIVE], [CONTROL_STRING])
SELECT r.[ID], @PageID, 1, ''
FROM [dbo].[BVTL_QT_QUYEN] r
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[BVTL_QT_QUYEN_PAGE] qp
    WHERE qp.[RoleID] = r.[ID] AND qp.[PageID] = @PageID
);

UPDATE [dbo].[BVTL_QT_QUYEN_PAGE]
SET [IS_ACTIVE] = 1
WHERE [PageID] = @PageID;
GO
