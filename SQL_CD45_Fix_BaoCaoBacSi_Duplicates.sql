-- =========================================================================
-- FIX: LOẠI BỎ DUPLICATES VÀ ĐỒNG BỘ 100% BÁO CÁO BÁC SĨ (DỰ ÁN CD45 - DREAMH)
-- =========================================================================

-- 1. Cập nhật danh mục bác sĩ CD45_DM_BAC_SI từ RedCap
IF NOT EXISTS (SELECT 1 FROM CD45_DM_BAC_SI WHERE MA_BAC_SI = '24')
    INSERT INTO CD45_DM_BAC_SI (MA_BAC_SI, TEN_BAC_SI, TINH_THANH, CITY_CODE, IS_ACTIVE)
    VALUES ('24', N'BS. Trần Thi Tuyết', N'Nghệ An', 'NAN', 1);

IF NOT EXISTS (SELECT 1 FROM CD45_DM_BAC_SI WHERE MA_BAC_SI = '25')
    INSERT INTO CD45_DM_BAC_SI (MA_BAC_SI, TEN_BAC_SI, TINH_THANH, CITY_CODE, IS_ACTIVE)
    VALUES ('25', N'BS. Trương Thị Nụ', N'Nghệ An', 'NAN', 1);

IF NOT EXISTS (SELECT 1 FROM CD45_DM_BAC_SI WHERE MA_BAC_SI = '26')
    INSERT INTO CD45_DM_BAC_SI (MA_BAC_SI, TEN_BAC_SI, TINH_THANH, CITY_CODE, IS_ACTIVE)
    VALUES ('26', N'BS. Nguyễn Đức Tài', N'Nghệ An', 'NAN', 1);

IF NOT EXISTS (SELECT 1 FROM CD45_DM_BAC_SI WHERE MA_BAC_SI = '27')
    INSERT INTO CD45_DM_BAC_SI (MA_BAC_SI, TEN_BAC_SI, TINH_THANH, CITY_CODE, IS_ACTIVE)
    VALUES ('27', N'BS. Trần Nhật Tân', N'Nghệ An', 'NAN', 1);

IF NOT EXISTS (SELECT 1 FROM CD45_DM_BAC_SI WHERE MA_BAC_SI = '28')
    INSERT INTO CD45_DM_BAC_SI (MA_BAC_SI, TEN_BAC_SI, TINH_THANH, CITY_CODE, IS_ACTIVE)
    VALUES ('28', N'BS. Trần Thị Hiệp', N'Nghệ An', 'NAN', 1);

IF NOT EXISTS (SELECT 1 FROM CD45_DM_BAC_SI WHERE MA_BAC_SI = '29')
    INSERT INTO CD45_DM_BAC_SI (MA_BAC_SI, TEN_BAC_SI, TINH_THANH, CITY_CODE, IS_ACTIVE)
    VALUES ('29', N'BS. Trần Đình Ngọc', N'Nghệ An', 'NAN', 1);

IF NOT EXISTS (SELECT 1 FROM CD45_DM_BAC_SI WHERE MA_BAC_SI = '30')
    INSERT INTO CD45_DM_BAC_SI (MA_BAC_SI, TEN_BAC_SI, TINH_THANH, CITY_CODE, IS_ACTIVE)
    VALUES ('30', N'BS. Nguyễn Thị Minh Châu', N'Nghệ An', 'NAN', 1);
GO

-- 2. Stored Procedure Báo cáo Chi tiết: Khắc phục duplicate do join nhóm 'vn'
CREATE OR ALTER PROC SP_CD45_GetBaoCaoBacSi
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @CityCode VARCHAR(50) = NULL,
    @DoctorId VARCHAR(10) = NULL,
    @MaNhom VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        f6.ID,
        f6.RECORD_ID,
        f6.CITY_CODE,
        ISNULL(cty.Name, f6.CITY_CODE) AS TEN_TINH,
        f6.MA_NHOM,
        ISNULL(nhom.tennhom_tbh, f6.MA_NHOM) AS TEN_NHOM,
        f6.MA_TCV,
        ISNULL(tcv.TEN_TCV, f6.MA_TCV) AS TEN_TCV,
        f6.NGAY_KHAM,
        f6.BAC_SI,
        ISNULL(bs.TEN_BAC_SI, N'Bác sĩ ' + f6.BAC_SI) AS TEN_BAC_SI,
        f6.CO_SO_Y_TE,
        f6.LAN_KHAM,
        f6.CHAN_DOAN_CHINH,
        f6.HINH_THUC_DIEU_TRI,
        f6.COMPLETE_STATUS
    FROM CD45_CHAN_DOAN f6
    LEFT JOIN CD45_DM_BAC_SI bs ON f6.BAC_SI = bs.MA_BAC_SI
    LEFT JOIN BVTL_CITES cty ON f6.CITY_CODE = cty.Code
    LEFT JOIN BVTL_NHOM_TBH nhom ON nhom.maduan = 'CD45' AND (f6.MA_NHOM = nhom.manhom_tbh OR (nhom.manhom_tbh_map IS NOT NULL AND f6.MA_NHOM = nhom.manhom_tbh_map))
    LEFT JOIN CD45_NHOM_TCV tcv ON (f6.MA_NHOM = tcv.MA_NHOM AND f6.MA_TCV = tcv.MA_TCV)
    WHERE f6.COMPLETE_STATUS = '2'
      AND (@FromDate IS NULL OR f6.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR f6.NGAY_KHAM <= @ToDate)
      AND (@CityCode IS NULL OR @CityCode = '' OR f6.CITY_CODE = @CityCode)
      AND (@DoctorId IS NULL OR @DoctorId = '' OR f6.BAC_SI = @DoctorId)
      AND (@MaNhom IS NULL OR @MaNhom = '' OR f6.MA_NHOM = @MaNhom)
    ORDER BY ISNULL(bs.TEN_BAC_SI, f6.BAC_SI), f6.NGAY_KHAM, f6.RECORD_ID;
END
GO

-- 3. Stored Procedure Báo cáo Tổng hợp: Group theo dữ liệu thực tế và LEFT JOIN danh mục
CREATE OR ALTER PROC SP_CD45_GetBaoCaoBacSi_TongHop
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @CityCode VARCHAR(50) = NULL,
    @DoctorId VARCHAR(10) = NULL,
    @MaNhom VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ISNULL(bs.MA_BAC_SI, f6.BAC_SI) AS MA_BAC_SI,
        ISNULL(bs.TEN_BAC_SI, N'Bác sĩ ' + f6.BAC_SI) AS TEN_BAC_SI,
        ISNULL(bs.TINH_THANH, ISNULL(cty.Name, f6.CITY_CODE)) AS TINH_THANH,
        ISNULL(bs.CITY_CODE, f6.CITY_CODE) AS CITY_CODE,
        COUNT(f6.ID) AS TongSoCa,
        COUNT(CASE WHEN f6.LAN_KHAM = 1 THEN 1 END) AS KhamLan1,
        COUNT(CASE WHEN f6.LAN_KHAM = 2 THEN 1 END) AS TaiKham,
        COUNT(CASE WHEN f6.LAN_KHAM > 2 OR f6.LAN_KHAM IS NULL THEN 1 END) AS KhamKhac,
        COUNT(CASE WHEN f6.HINH_THUC_DIEU_TRI = '1' THEN 1 END) AS NgoaiTru,
        COUNT(CASE WHEN f6.HINH_THUC_DIEU_TRI = '2' THEN 1 END) AS NoiTru
    FROM CD45_CHAN_DOAN f6
    LEFT JOIN CD45_DM_BAC_SI bs ON f6.BAC_SI = bs.MA_BAC_SI
    LEFT JOIN BVTL_CITES cty ON f6.CITY_CODE = cty.Code
    WHERE f6.COMPLETE_STATUS = '2'
      AND (@FromDate IS NULL OR f6.NGAY_KHAM >= @FromDate)
      AND (@ToDate IS NULL OR f6.NGAY_KHAM <= @ToDate)
      AND (@CityCode IS NULL OR @CityCode = '' OR f6.CITY_CODE = @CityCode)
      AND (@DoctorId IS NULL OR @DoctorId = '' OR f6.BAC_SI = @DoctorId)
      AND (@MaNhom IS NULL OR @MaNhom = '' OR f6.MA_NHOM = @MaNhom)
    GROUP BY ISNULL(bs.MA_BAC_SI, f6.BAC_SI), ISNULL(bs.TEN_BAC_SI, N'Bác sĩ ' + f6.BAC_SI), ISNULL(bs.TINH_THANH, ISNULL(cty.Name, f6.CITY_CODE)), ISNULL(bs.CITY_CODE, f6.CITY_CODE)
    ORDER BY TINH_THANH, TEN_BAC_SI;
END
GO
