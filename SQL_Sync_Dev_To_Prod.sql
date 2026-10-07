-- ============================================================

-- SYNC SCRIPT: DEV -> PROD (BVTL_REPORTING)

-- Generated at: 2026-10-06 10:20:00

-- ============================================================

USE [BVTL_REPORTING];

GO



-- 1. ADD MISSING COLUMNS TO EXISTING TABLES

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('BVTL_NHOM_TBH') AND name = 'IS_ACTIVE')

    ALTER TABLE [BVTL_NHOM_TBH] ADD [IS_ACTIVE] bit NOT NULL CONSTRAINT [DF_BVTL_NHOM_TBH_IS_ACTIVE] DEFAULT (1);

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('BVTL_NHOM_TBH') AND name = 'CHUC_DANH')

    ALTER TABLE [BVTL_NHOM_TBH] ADD [CHUC_DANH] nvarchar(50) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('BVTL_NHOM_TBH') AND name = 'SHORT_PREFIX')

    ALTER TABLE [BVTL_NHOM_TBH] ADD [SHORT_PREFIX] nvarchar(20) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('BVTL_NHOM_TBH') AND name = 'PREFIX')

    ALTER TABLE [BVTL_NHOM_TBH] ADD [PREFIX] nvarchar(50) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('BVTL_DU_AN') AND name = 'IsActive')

    ALTER TABLE [BVTL_DU_AN] ADD [IsActive] bit NOT NULL CONSTRAINT [DF_BVTL_DU_AN_IsActive] DEFAULT (1);

GO



-- 2. CREATE MISSING TABLES

IF OBJECT_ID('BVTL_DATA_STANDARDIZATION_LOG', 'U') IS NULL

BEGIN

    CREATE TABLE [BVTL_DATA_STANDARDIZATION_LOG] (

    [ID] bigint IDENTITY(1,1) NOT NULL,

    [MADUAN] varchar(50) NOT NULL,

    [REPORT_ID] varchar(50) NOT NULL,

    [API_CODE] varchar(50) NOT NULL,

    [TABLE_NAME] varchar(100) NOT NULL,

    [RECORD_ID] varchar(50) NULL,

    [FIELD_NAME] varchar(100) NOT NULL,

    [OLD_VALUE] nvarchar(500) NULL,

    [NEW_VALUE] nvarchar(500) NULL,

    [RULE_CODE] varchar(50) NOT NULL,

    [SEVERITY] varchar(20) NOT NULL,

    [ACTION_TAKEN] nvarchar(250) NOT NULL,

    [MESSAGE] nvarchar(1000) NULL,

    [CREATED_DATE] datetime NULL DEFAULT (getdate()),

    [IS_RESOLVED] bit NULL DEFAULT ((0)),

    [RESOLVED_NOTE] nvarchar(500) NULL,

    [MA_NHOM] varchar(50) NULL,

    [CITY_CODE] varchar(50) NULL,

    CONSTRAINT [PK_BVTL_DATA_STANDARDIZATION_LOG] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



IF OBJECT_ID('BVTL_DM_TINH_MOI', 'U') IS NULL

BEGIN

    CREATE TABLE [BVTL_DM_TINH_MOI] (

    [Code] varchar(10) NOT NULL,

    [Name] nvarchar(150) NOT NULL,

    [Code_Map] varchar(10) NULL,

    [IsKeyProvince] bit NOT NULL DEFAULT ((0)),

    [OldCount] int NOT NULL DEFAULT ((1)),

    [OldNamesSummary] nvarchar(250) NULL,

    [DisplayOrder] int NOT NULL DEFAULT ((99)),

    [IsActive] bit NOT NULL DEFAULT ((1)),

    [CreatedDate] datetime NOT NULL DEFAULT (getdate()),

    CONSTRAINT [PK_BVTL_DM_TINH_MOI] PRIMARY KEY CLUSTERED ([Code])

    );

END

GO



IF OBJECT_ID('BVTL_EXPORTED_REPORT_LOG', 'U') IS NULL

BEGIN

    CREATE TABLE [BVTL_EXPORTED_REPORT_LOG] (

    [Id] bigint IDENTITY(1,1) NOT NULL,

    [ReportType] varchar(50) NOT NULL,

    [ReportName] nvarchar(250) NOT NULL,

    [PeriodType] varchar(20) NOT NULL,

    [PeriodValue] varchar(50) NOT NULL,

    [Year] int NOT NULL,

    [Month] int NULL,

    [MaDuAn] varchar(50) NULL,

    [CityCode] varchar(50) NULL,

    [MaNhom] varchar(50) NULL,

    [FileName] nvarchar(250) NOT NULL,

    [FilePath] nvarchar(500) NOT NULL,

    [FileSizeKb] bigint NOT NULL DEFAULT ((0)),

    [TotalRecords] int NOT NULL DEFAULT ((0)),

    [Status] varchar(30) NOT NULL DEFAULT ('Success'),

    [ErrorMessage] nvarchar(MAX) NULL,

    [ExecutionTimeMs] int NOT NULL DEFAULT ((0)),

    [TelegramSent] bit NOT NULL DEFAULT ((0)),

    [TriggerType] varchar(30) NOT NULL DEFAULT ('AutoSchedule'),

    [CreatedBy] nvarchar(100) NOT NULL DEFAULT ('QuartzScheduler'),

    [CreatedDate] datetime NOT NULL DEFAULT (getdate()),

    CONSTRAINT [PK_BVTL_EXPORTED_REPORT_LOG] PRIMARY KEY CLUSTERED ([Id])

    );

END

GO



IF OBJECT_ID('BVTL_FEATURE_USAGE_LOG', 'U') IS NULL

BEGIN

    CREATE TABLE [BVTL_FEATURE_USAGE_LOG] (

    [Id] bigint IDENTITY(1,1) NOT NULL,

    [UserId] bigint NULL,

    [UserName] varchar(50) NULL,

    [FullName] nvarchar(100) NULL,

    [ControllerName] varchar(100) NOT NULL,

    [ActionName] varchar(100) NOT NULL,

    [ActionType] varchar(30) NOT NULL,

    [ExecutionTimeMs] int NULL,

    [IsError] bit NOT NULL DEFAULT ((0)),

    [ErrorMessage] nvarchar(1000) NULL,

    [AccessDate] date NOT NULL DEFAULT (CONVERT([date],getdate())),

    [CreatedDate] datetime NOT NULL DEFAULT (getdate()),

    CONSTRAINT [PK_BVTL_FEATURE_USAGE_LOG] PRIMARY KEY CLUSTERED ([Id])

    );

END

GO



IF OBJECT_ID('BVTL_LOGIN_HISTORY', 'U') IS NULL

BEGIN

    CREATE TABLE [BVTL_LOGIN_HISTORY] (

    [Id] bigint IDENTITY(1,1) NOT NULL,

    [UserId] bigint NULL,

    [UserName] varchar(50) NOT NULL,

    [FullName] nvarchar(100) NULL,

    [LoginTime] datetime NOT NULL DEFAULT (getdate()),

    [LogoutTime] datetime NULL,

    [IpAddress] varchar(50) NULL,

    [UserAgent] nvarchar(500) NULL,

    [Browser] nvarchar(50) NULL,

    [OperatingSystem] nvarchar(50) NULL,

    [DeviceType] nvarchar(30) NULL,

    [Status] nvarchar(50) NOT NULL,

    [SessionId] varchar(100) NULL,

    CONSTRAINT [PK_BVTL_LOGIN_HISTORY] PRIMARY KEY CLUSTERED ([Id])

    );

END

GO



IF OBJECT_ID('BVTL_MAP_TINH_CU_MOI', 'U') IS NULL

BEGIN

    CREATE TABLE [BVTL_MAP_TINH_CU_MOI] (

    [OldCityCode] varchar(10) NOT NULL,

    [NewCityCode] varchar(10) NOT NULL,

    [OldCityName] nvarchar(150) NOT NULL,

    [NewCityName] nvarchar(150) NOT NULL,

    [EffectiveDate] datetime NOT NULL DEFAULT ('2025-07-01'),

    CONSTRAINT [PK_BVTL_MAP_TINH_CU_MOI] PRIMARY KEY CLUSTERED ([OldCityCode])

    );

END

GO



IF OBJECT_ID('BVTL_QT_NGUOI_DUNG_CITY', 'U') IS NULL

BEGIN

    CREATE TABLE [BVTL_QT_NGUOI_DUNG_CITY] (

    [Id] int IDENTITY(1,1) NOT NULL,

    [NguoiDungId] bigint NOT NULL,

    [CityCode] varchar(10) NOT NULL,

    [IsActive] bit NOT NULL DEFAULT ((1)),

    [CreatedDate] datetime NULL DEFAULT (getdate()),

    CONSTRAINT [PK_BVTL_QT_NGUOI_DUNG_CITY] PRIMARY KEY CLUSTERED ([Id])

    );

END

GO



IF OBJECT_ID('BVTL_USER_ONLINE', 'U') IS NULL

BEGIN

    CREATE TABLE [BVTL_USER_ONLINE] (

    [SessionId] varchar(100) NOT NULL,

    [UserId] bigint NOT NULL,

    [UserName] varchar(50) NOT NULL,

    [FullName] nvarchar(100) NULL,

    [RoleName] nvarchar(100) NULL,

    [CityCodes] varchar(200) NULL,

    [MaDuAn] varchar(200) NULL,

    [IpAddress] varchar(50) NULL,

    [Browser] nvarchar(50) NULL,

    [OperatingSystem] nvarchar(50) NULL,

    [DeviceType] nvarchar(30) NULL,

    [LoginTime] datetime NOT NULL DEFAULT (getdate()),

    [LastActiveTime] datetime NOT NULL DEFAULT (getdate()),

    [CurrentController] varchar(100) NULL,

    [CurrentAction] varchar(100) NULL,

    [CurrentUrl] nvarchar(500) NULL,

    [Status] varchar(20) NOT NULL DEFAULT ('Online'),

    CONSTRAINT [PK_BVTL_USER_ONLINE] PRIMARY KEY CLUSTERED ([SessionId])

    );

END

GO



IF OBJECT_ID('CD45_BCTIEU_CAU_HINH', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_BCTIEU_CAU_HINH] (

    [ID] int IDENTITY(1,1) NOT NULL,

    [ChiTieuCode] nvarchar(50) NOT NULL,

    [ChiTieuName] nvarchar(500) NOT NULL,

    [SectionCode] nvarchar(10) NOT NULL,

    [IsSection] bit NOT NULL DEFAULT ((0)),

    [HienThi_Thang] bit NOT NULL DEFAULT ((0)),

    [HienThi_Quy] bit NOT NULL DEFAULT ((0)),

    [HienThi_6T] bit NOT NULL DEFAULT ((0)),

    [HienThi_12T] bit NOT NULL DEFAULT ((0)),

    [SortOrder] int NOT NULL DEFAULT ((0)),

    [IsActive] bit NOT NULL DEFAULT ((1)),

    [UpdatedAt] datetime NULL DEFAULT (getdate()),

    [UpdatedBy] nvarchar(100) NULL,

    [Default_Thang] bit NOT NULL DEFAULT ((0)),

    [Default_Quy] bit NOT NULL DEFAULT ((1)),

    [Default_6T] bit NOT NULL DEFAULT ((1)),

    [Default_12T] bit NOT NULL DEFAULT ((1)),

    CONSTRAINT [PK_CD45_BCTIEU_CAU_HINH] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



IF OBJECT_ID('CD45_CHAN_DOAN', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_CHAN_DOAN] (

    [ID] bigint IDENTITY(1,1) NOT NULL,

    [RECORD_ID] varchar(20) NOT NULL,

    [REPEAT_INSTANCE] int NULL,

    [CITY_CODE] varchar(10) NOT NULL,

    [MADUAN] varchar(10) NOT NULL DEFAULT ('CD45'),

    [NGAY_KHAM] date NULL,

    [MA_NHOM] varchar(10) NULL,

    [MA_TCV] varchar(5) NULL,

    [CO_SO_Y_TE] nvarchar(500) NULL,

    [BAC_SI] nvarchar(200) NULL,

    [LAN_KHAM] tinyint NULL,

    [TRIEU_CHUNG] nvarchar(MAX) NULL,

    [CHAN_DOAN_CHINH] nvarchar(500) NULL,

    [NGUY_CO] nvarchar(500) NULL,

    [HINH_THUC_DIEU_TRI] nvarchar(500) NULL,

    [NGAY_HEN_TAI_KHAM] date NULL,

    [COMPLETE_STATUS] varchar(20) NULL,

    [NGAY_SYNC] datetime NOT NULL DEFAULT (getdate()),

    [REDCAP_DAG] varchar(50) NULL,

    CONSTRAINT [PK_CD45_CHAN_DOAN] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



IF OBJECT_ID('CD45_DM_BAC_SI', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_DM_BAC_SI] (

    [MA_BAC_SI] varchar(10) NOT NULL,

    [TEN_BAC_SI] nvarchar(250) NOT NULL,

    [TINH_THANH] nvarchar(100) NULL,

    [CITY_CODE] varchar(10) NULL,

    [IS_ACTIVE] bit NULL DEFAULT ((1)),

    CONSTRAINT [PK_CD45_DM_BAC_SI] PRIMARY KEY CLUSTERED ([MA_BAC_SI])

    );

END

GO

-- Khởi tạo danh mục bác sĩ dự án CD45
MERGE CD45_DM_BAC_SI AS target
USING (VALUES
    ('1', N'BS. Phạm Thị Phương', N'Hà Nội', 'HNO', 1),
    ('2', N'BS. Nguyễn Thị Hòa', N'Hà Nội', 'HNO', 1),
    ('3', N'BS. Đặng Thu Thảo', N'Hưng Yên', 'HYE', 1),
    ('4', N'BS. Nguyễn Văn Phi', N'Hà Nội', 'HNO', 1),
    ('5', N'BS. Đỗ Thị Hằng', N'Ninh Bình', 'NBI', 1),
    ('6', N'BS. Phạm Đặng Duy', N'Ninh Bình', 'NBI', 1),
    ('7', N'BS. Bùi Thị Hè', N'Hưng Yên', 'HYE', 1),
    ('8', N'BS. Phạm Thị Anh', N'Nghệ An', 'NAN', 1),
    ('9', N'BS. Nguyễn Thị Lan Anh', N'Hải Phòng', 'HPG', 1),
    ('10', N'BS. Hồ Thu Nga', N'Hải Phòng', 'HPG', 1),
    ('11', N'BS. Phạm Thị Huyền', N'Hải Phòng', 'HPG', 1),
    ('12', N'BS. Đặng Tiến Thành', N'Hải Phòng', 'HPG', 1),
    ('13', N'BS. Đỗ Phương Linh', N'Hải Phòng', 'HPG', 1),
    ('14', N'BS. Nguyễn Văn Cảnh', N'Hưng Yên', 'HYE', 1),
    ('15', N'BS. Lê Minh Ngọc', N'Hưng Yên', 'HYE', 1),
    ('16', N'BS. Phạm Thị Mừng', N'Hải Phòng', 'HPG', 1),
    ('17', N'BS. Diệp Lê Tuấn', N'Hồ Chí Minh', 'HCM', 1),
    ('18', N'BS. Nguyễn Phi Yến', N'Hồ Chí Minh', 'HCM', 1),
    ('19', N'BS. Trương Đặng Anh Vân', N'Hải Phòng', 'HPG', 1),
    ('20', N'BS. Nguyễn Trọng Hiến', N'Hà Nội', 'HNO', 1),
    ('21', N'BS. Nguyễn Thu Hà', N'Hà Nội', 'HNO', 1),
    ('22', N'BS. Bùi Xuân Đạt', N'Nghệ An', 'NAN', 1),
    ('23', N'BS. Nguyễn Thị Hà Trang', N'Nghệ An', 'NAN', 1),
    ('24', N'BS. Trần Thi Tuyết', N'Nghệ An', 'NAN', 1),
    ('25', N'BS. Trương Thị Nụ', N'Nghệ An', 'NAN', 1),
    ('26', N'BS. Nguyễn Đức Tài', N'Nghệ An', 'NAN', 1),
    ('27', N'BS. Trần Nhật Tân', N'Nghệ An', 'NAN', 1),
    ('28', N'BS. Trần Thị Hiệp', N'Nghệ An', 'NAN', 1),
    ('29', N'BS. Trần Đình Ngọc', N'Nghệ An', 'NAN', 1),
    ('30', N'BS. Nguyễn Thị Minh Châu', N'Nghệ An', 'NAN', 1)
) AS source (MA_BAC_SI, TEN_BAC_SI, TINH_THANH, CITY_CODE, IS_ACTIVE)
ON target.MA_BAC_SI = source.MA_BAC_SI
WHEN MATCHED THEN
    UPDATE SET target.TEN_BAC_SI = source.TEN_BAC_SI, target.TINH_THANH = source.TINH_THANH, target.CITY_CODE = source.CITY_CODE
WHEN NOT MATCHED THEN
    INSERT (MA_BAC_SI, TEN_BAC_SI, TINH_THANH, CITY_CODE, IS_ACTIVE)
    VALUES (source.MA_BAC_SI, source.TEN_BAC_SI, source.TINH_THANH, source.CITY_CODE, source.IS_ACTIVE);
GO




IF OBJECT_ID('CD45_HO_TRO_XH', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_HO_TRO_XH] (

    [ID] bigint IDENTITY(1,1) NOT NULL,

    [RECORD_ID] varchar(20) NOT NULL,

    [REPEAT_INSTANCE] int NULL,

    [CITY_CODE] varchar(10) NOT NULL,

    [MADUAN] varchar(10) NOT NULL DEFAULT ('CD45'),

    [NGAY_HO_TRO] date NULL,

    [MA_NHOM] varchar(10) NULL,

    [MA_TCV] varchar(5) NULL,

    [DICH_VU] nvarchar(500) NULL,

    [KET_QUA_HIV] tinyint NULL,

    [COMPLETE_STATUS] varchar(20) NULL,

    [NGAY_SYNC] datetime NOT NULL DEFAULT (getdate()),

    [REDCAP_DAG] varchar(50) NULL,

    CONSTRAINT [PK_CD45_HO_TRO_XH] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



IF OBJECT_ID('CD45_HOAT_DONG', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_HOAT_DONG] (

    [ID] bigint IDENTITY(1,1) NOT NULL,

    [RECORD_ID] varchar(20) NOT NULL,

    [REPEAT_INSTANCE] int NULL,

    [CITY_CODE] varchar(10) NOT NULL,

    [MADUAN] varchar(10) NOT NULL DEFAULT ('CD45'),

    [LOAI_DV] tinyint NULL,

    [NGAY_HOAT_DONG] date NULL,

    [MA_NHOM] varchar(10) NULL,

    [MA_TCV] varchar(5) NULL,

    [DIA_DIEM] tinyint NULL,

    [DIA_DIEM_NGOAI] nvarchar(200) NULL,

    [DIA_DIEM_KHAC] nvarchar(200) NULL,

    [CHU_DE] nvarchar(500) NULL,

    [SO_TAI_LIEU] int NULL DEFAULT ((0)),

    [DONG_Y_QST] bit NULL,

    [SO_BAO_CAO_SU] int NULL DEFAULT ((0)),

    [SO_CHAT_BOI_TRAN] int NULL DEFAULT ((0)),

    [SO_BOM_KIM] int NULL DEFAULT ((0)),

    [GHI_CHU] nvarchar(MAX) NULL,

    [COMPLETE_STATUS] varchar(20) NULL,

    [NGAY_SYNC] datetime NOT NULL DEFAULT (getdate()),

    [REDCAP_DAG] varchar(50) NULL,

    CONSTRAINT [PK_CD45_HOAT_DONG] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



IF OBJECT_ID('CD45_KH', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_KH] (

    [ID] bigint IDENTITY(1,1) NOT NULL,

    [RECORD_ID] varchar(20) NOT NULL,

    [CITY_CODE] varchar(10) NOT NULL,

    [MADUAN] varchar(10) NOT NULL DEFAULT ('CD45'),

    [NGAY_THAM_GIA] date NULL,

    [THAM_GIA_NGHIEN_CUU] bit NULL,

    [MA_KH_NGHIEN_CUU] varchar(50) NULL,

    [GIOI_TINH_TU_XD] tinyint NULL,

    [GIOI_TINH_KHAI_SINH] tinyint NULL,

    [NAM_SINH] smallint NULL,

    [DOI_TUONG] tinyint NULL,

    [DOI_TUONG_KHAC] varchar(20) NULL,

    [CO_CCCD] bit NULL,

    [CO_THUONG_TRU] bit NULL,

    [CO_BHYT] bit NULL,

    [VO_GIA_CU_6T] bit NULL,

    [DANG_VO_GIA_CU] bit NULL,

    [BI_TAM_GIU_6T] bit NULL,

    [HON_NHAN] tinyint NULL,

    [SO_CON] tinyint NULL,

    [COMPLETE_STATUS] varchar(20) NULL,

    [NGAY_SYNC] datetime NOT NULL DEFAULT (getdate()),

    [MA_NHOM] varchar(20) NULL,

    [REDCAP_DAG] varchar(50) NULL,

    CONSTRAINT [PK_CD45_KH] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



IF OBJECT_ID('CD45_NHOM_TCV', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_NHOM_TCV] (

    [ID] int IDENTITY(1,1) NOT NULL,

    [MA_NHOM] varchar(20) NOT NULL,

    [TEN_NHOM] nvarchar(100) NOT NULL,

    [CITY_CODE] varchar(10) NOT NULL,

    [MA_TCV] varchar(5) NOT NULL,

    [TEN_TCV] nvarchar(200) NOT NULL,

    [MADUAN] varchar(10) NOT NULL DEFAULT ('CD45'),

    [IsActive] bit NOT NULL DEFAULT ((1)),

    [CreatedDate] datetime NOT NULL DEFAULT (getdate()),

    [PREFIX] nvarchar(50) NULL DEFAULT (N'Nhóm'),

    [SHORT_PREFIX] nvarchar(20) NULL DEFAULT (N'Nhóm'),

    [CHUC_DANH] nvarchar(50) NULL DEFAULT (N'Trưởng nhóm'),

    CONSTRAINT [PK_CD45_NHOM_TCV] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



IF OBJECT_ID('CD45_QST', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_QST] (

    [ID] bigint IDENTITY(1,1) NOT NULL,

    [RECORD_ID] varchar(20) NOT NULL,

    [REPEAT_INSTANCE] int NULL,

    [CITY_CODE] varchar(10) NOT NULL,

    [MADUAN] varchar(10) NOT NULL DEFAULT ('CD45'),

    [LY_DO_DANH_GIA_LAI] tinyint NULL,

    [NGAY_SANG_LOC] date NULL,

    [MA_NHOM] varchar(10) NULL,

    [MA_TCV] varchar(5) NULL,

    [Q1A] tinyint NULL,

    [Q1B] tinyint NULL,

    [Q1C] tinyint NULL,

    [Q1D] tinyint NULL,

    [Q1E] tinyint NULL,

    [Q1F] tinyint NULL,

    [Q2_TU_HARM] tinyint NULL,

    [Q3_NGHE] tinyint NULL,

    [DIEM_QST] int NULL,

    [MUC_QST] tinyint NULL,

    [COMPLETE_STATUS] varchar(20) NULL,

    [NGAY_SYNC] datetime NOT NULL DEFAULT (getdate()),

    [REDCAP_DAG] varchar(50) NULL,

    CONSTRAINT [PK_CD45_QST] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



IF OBJECT_ID('CD45_THEO_DAU', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_THEO_DAU] (

    [ID] bigint IDENTITY(1,1) NOT NULL,

    [RECORD_ID] varchar(20) NOT NULL,

    [REPEAT_INSTANCE] int NULL,

    [CITY_CODE] varchar(10) NOT NULL,

    [MADUAN] varchar(10) NOT NULL DEFAULT ('CD45'),

    [NGAY_THEO_DAU] date NULL,

    [HINH_THUC_LIEN_HE] tinyint NULL,

    [KET_QUA] tinyint NULL,

    [MAT_DAU] bit NULL,

    [COMPLETE_STATUS] varchar(20) NULL,

    [NGAY_SYNC] datetime NOT NULL DEFAULT (getdate()),

    [MA_NHOM] varchar(20) NULL,

    [REDCAP_DAG] varchar(50) NULL,

    CONSTRAINT [PK_CD45_THEO_DAU] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



IF OBJECT_ID('CD45_TU_VAN_L1', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_TU_VAN_L1] (

    [ID] bigint IDENTITY(1,1) NOT NULL,

    [RECORD_ID] varchar(20) NOT NULL,

    [CITY_CODE] varchar(10) NOT NULL,

    [MADUAN] varchar(10) NOT NULL DEFAULT ('CD45'),

    [NGAY_TU_VAN] date NULL,

    [MA_NHOM] varchar(10) NULL,

    [MA_TCV] varchar(5) NULL,

    [DIA_DIEM] tinyint NULL,

    [AUDIT_C_SCORE] int NULL,

    [PCL5_SCORE] int NULL,

    [PCL5_POSITIVE] bit NULL,

    [STIGMA_SCORE] int NULL,

    [TINH_TRANG_SKTT] tinyint NULL,

    [NHU_CAU_HO_TRO] nvarchar(500) NULL,

    [LICH_HEN_TIEP] date NULL,

    [COMPLETE_STATUS] varchar(20) NULL,

    [NGAY_SYNC] datetime NOT NULL DEFAULT (getdate()),

    [REDCAP_DAG] varchar(50) NULL,

    CONSTRAINT [PK_CD45_TU_VAN_L1] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD45_TU_VAN_L1' AND COLUMN_NAME = 'QA2')
BEGIN
    ALTER TABLE [dbo].[CD45_TU_VAN_L1] ADD [QA2] tinyint NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD45_TU_VAN_L1' AND COLUMN_NAME = 'QA5')
BEGIN
    ALTER TABLE [dbo].[CD45_TU_VAN_L1] ADD [QA5] tinyint NULL;
END
GO




IF OBJECT_ID('CD45_TU_VAN_L2', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_TU_VAN_L2] (

    [ID] bigint IDENTITY(1,1) NOT NULL,

    [RECORD_ID] varchar(20) NOT NULL,

    [REPEAT_INSTANCE] int NULL,

    [CITY_CODE] varchar(10) NOT NULL,

    [MADUAN] varchar(10) NOT NULL DEFAULT ('CD45'),

    [NGAY_TU_VAN] date NULL,

    [MA_NHOM] varchar(10) NULL,

    [MA_TCV] varchar(5) NULL,

    [DIA_DIEM] tinyint NULL,

    [DANH_GIA_HIEN_TAI] nvarchar(MAX) NULL,

    [CAN_THIEP_AP_DUNG] nvarchar(MAX) NULL,

    [NGAY_HEN_TIEP] date NULL,

    [COMPLETE_STATUS] varchar(20) NULL,

    [NGAY_SYNC] datetime NOT NULL DEFAULT (getdate()),

    [REDCAP_DAG] varchar(50) NULL,

    CONSTRAINT [PK_CD45_TU_VAN_L2] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



IF OBJECT_ID('CD45_TUAN_THU', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_TUAN_THU] (

    [ID] bigint IDENTITY(1,1) NOT NULL,

    [RECORD_ID] varchar(20) NOT NULL,

    [REPEAT_INSTANCE] int NULL,

    [CITY_CODE] varchar(10) NOT NULL,

    [MADUAN] varchar(10) NOT NULL DEFAULT ('CD45'),

    [NGAY_HO_TRO] date NULL,

    [MA_NHOM] varchar(10) NULL,

    [MA_TCV] varchar(5) NULL,

    [HINH_THUC_DIEU_TRI] tinyint NULL,

    [CO_KE_DON_THUOC] bit NULL,

    [TUAN_THU] tinyint NULL,

    [COMPLETE_STATUS] varchar(20) NULL,

    [NGAY_SYNC] datetime NOT NULL DEFAULT (getdate()),

    [REDCAP_DAG] varchar(50) NULL,

    CONSTRAINT [PK_CD45_TUAN_THU] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



IF OBJECT_ID('CD45_VAN_TAY', 'U') IS NULL

BEGIN

    CREATE TABLE [CD45_VAN_TAY] (

    [ID] bigint IDENTITY(1,1) NOT NULL,

    [RECORD_ID] varchar(20) NOT NULL,

    [REPEAT_INSTANCE] int NULL,

    [CITY_CODE] varchar(10) NOT NULL,

    [MADUAN] varchar(10) NOT NULL DEFAULT ('CD45'),

    [NGAY_GHI_NHAN] date NULL,

    [DICH_VU] tinyint NULL,

    [VAN_DE] tinyint NULL,

    [PHUONG_AN] tinyint NULL,

    [COMPLETE_STATUS] varchar(20) NULL,

    [NGAY_SYNC] datetime NOT NULL DEFAULT (getdate()),

    [MA_NHOM] varchar(20) NULL,

    [REDCAP_DAG] varchar(50) NULL,

    CONSTRAINT [PK_CD45_VAN_TAY] PRIMARY KEY CLUSTERED ([ID])

    );

END

GO



-- 3. SCRIPT MISSING STORED PROCEDURES

IF OBJECT_ID('SP_CD45_AutoProvisionNhom', 'P') IS NOT NULL DROP PROCEDURE [SP_CD45_AutoProvisionNhom];

GO

-- ==============================================================================

-- STORED PROCEDURE TỰ ĐỘNG KHỞI TẠO & ĐỒNG BỘ NHÓM DỰ PHÒNG TỪ REDCAP (CD45)

-- ==============================================================================

CREATE   PROCEDURE [dbo].[SP_CD45_AutoProvisionNhom]

AS

BEGIN

    SET NOCOUNT ON;



    -- 1. Tự động liên kết các nhóm có tên khớp nhau nhưng manhom_tbh_map chưa đúng

    UPDATE n

    SET n.manhom_tbh_map = t.MA_NHOM

    FROM dbo.BVTL_NHOM_TBH n

    INNER JOIN (

        SELECT DISTINCT MA_NHOM, TEN_NHOM, CITY_CODE 

        FROM dbo.CD45_NHOM_TCV 

        WHERE MADUAN = 'CD45' AND IsActive = 1

    ) t ON (n.city_code = t.CITY_CODE AND LOWER(LTRIM(RTRIM(n.tennhom_tbh))) = LOWER(LTRIM(RTRIM(t.TEN_NHOM))))

    WHERE (n.manhom_tbh_map IS NULL OR n.manhom_tbh_map != t.MA_NHOM);



    -- 2. Tự động liên kết nếu manhom_tbh_map khớp với hậu tố của manhom_tbh (ví dụ HN_ITT -> itt)

    UPDATE n

    SET n.manhom_tbh_map = t.MA_NHOM

    FROM dbo.BVTL_NHOM_TBH n

    INNER JOIN (

        SELECT DISTINCT MA_NHOM, CITY_CODE 

        FROM dbo.CD45_NHOM_TCV 

        WHERE MADUAN = 'CD45' AND IsActive = 1

    ) t ON (n.city_code = t.CITY_CODE AND LOWER(SUBSTRING(n.manhom_tbh, CHARINDEX('_', n.manhom_tbh) + 1, 10)) = LOWER(t.MA_NHOM))

    WHERE (n.manhom_tbh_map IS NULL OR n.manhom_tbh_map != t.MA_NHOM);



    -- 3. Tìm các nhóm mới trên REDCap chưa từng có trong BVTL_NHOM_TBH

    DECLARE @UnmappedGroups TABLE (

        MA_NHOM VARCHAR(20),

        TEN_NHOM NVARCHAR(100),

        CITY_CODE VARCHAR(10)

    );



    INSERT INTO @UnmappedGroups (MA_NHOM, TEN_NHOM, CITY_CODE)

    SELECT DISTINCT t.MA_NHOM, t.TEN_NHOM, t.CITY_CODE

    FROM dbo.CD45_NHOM_TCV t

    WHERE t.MADUAN = 'CD45' AND t.IsActive = 1

      AND NOT EXISTS (

          SELECT 1 FROM dbo.BVTL_NHOM_TBH n

          WHERE n.manhom_tbh = t.MA_NHOM 

             OR (n.manhom_tbh_map IS NOT NULL AND n.manhom_tbh_map = t.MA_NHOM)

      );



    -- Duyệt từng nhóm unmapped để tạo nhóm dự phòng

    DECLARE @curMaNhom VARCHAR(20), @curTenNhom NVARCHAR(100), @curCityCode VARCHAR(10);

    DECLARE @newMaNhom VARCHAR(6), @provPrefix VARCHAR(3), @suffix VARCHAR(4);

    DECLARE @counter INT;



    DECLARE group_cursor CURSOR LOCAL FAST_FORWARD FOR

    SELECT MA_NHOM, TEN_NHOM, CITY_CODE FROM @UnmappedGroups;



    OPEN group_cursor;

    FETCH NEXT FROM group_cursor INTO @curMaNhom, @curTenNhom, @curCityCode;



    WHILE @@FETCH_STATUS = 0

    BEGIN

        -- Xác định tiền tố tỉnh 2 ký tự (HN, HC, HP, NA, NB, HY...)

        SET @provPrefix = UPPER(SUBSTRING(ISNULL(@curCityCode, 'XX'), 1, 2));

        SET @suffix = UPPER(LEFT(@curMaNhom, 3));

        SET @newMaNhom = @provPrefix + '_' + @suffix;

        IF LEN(@newMaNhom) > 6 SET @newMaNhom = LEFT(@newMaNhom, 6);



        -- Đảm bảo mã nhóm không bị trùng lặp trong BVTL_NHOM_TBH (tối đa 6 ký tự)

        SET @counter = 1;

        WHILE EXISTS (SELECT 1 FROM dbo.BVTL_NHOM_TBH WHERE manhom_tbh = @newMaNhom)

        BEGIN

            SET @newMaNhom = LEFT(@provPrefix + '_' + LEFT(@suffix, 2) + CAST(@counter AS VARCHAR(2)), 6);

            SET @counter = @counter + 1;

        END



        -- Thêm bản ghi nhóm mới vào BVTL_NHOM_TBH

        INSERT INTO dbo.BVTL_NHOM_TBH (

            manhom_tbh, tennhom_tbh, city_code, manhom_tbh_map, maduan, PREFIX, SHORT_PREFIX, CHUC_DANH

        )

        VALUES (

            @newMaNhom, @curTenNhom, @curCityCode, @curMaNhom, 'CD45', N'Nhóm', N'Nhóm', N'Trưởng nhóm'

        );



        -- Ghi log hệ thống

        INSERT INTO dbo.BVTL_QT_LOG (ControllerName, UserName, DateLog, Content)

        VALUES (

            'AutoProvisionNhom',

            'SYSTEM',

            GETDATE(),

            N'Tự động khởi tạo nhóm dự phòng từ REDCap: ' + @newMaNhom + N' - ' + @curTenNhom + N' (Mã REDCap: ' + @curMaNhom + N')'

        );



        FETCH NEXT FROM group_cursor INTO @curMaNhom, @curTenNhom, @curCityCode;

    END



    CLOSE group_cursor;

    DEALLOCATE group_cursor;



    SELECT COUNT(*) AS NewGroupsCreated FROM @UnmappedGroups;

END

GO



IF OBJECT_ID('SP_CD45_Dashboard', 'P') IS NOT NULL DROP PROCEDURE [SP_CD45_Dashboard];

GO

CREATE   PROC SP_CD45_Dashboard

    @CityCode VARCHAR(50) = NULL,

    @MaNhom VARCHAR(50) = NULL,

    @FromDate DATE = NULL,

    @ToDate DATE = NULL,

    @NhomTuoiTable1 VARCHAR(50) = NULL,

    @CityMode VARCHAR(10) = 'NEW34',

    @GioiTinhFilter INT = NULL,          -- Option B: 1=Nam, 2=Nữ, 3=Khác (NULL=Tất cả)

    @NhomTuoiFilter VARCHAR(50) = NULL,  -- Option B: '< 18', '18 - 25', '26 - 35', '>= 36' (NULL=Tất cả)

    @DoiTuongFilter INT = NULL,          -- Option B: 1=PUD, 2=PLHIV, 3=TG, 4=MSM, 5=SW (NULL=Tất cả)

    @DimensionMode INT = 1               -- Option A: 1=Quần thể, 2=Giới tính, 3=Nhóm tuổi

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



    -- Resolve @CityCode mapping (hỗ trợ cả 34 tỉnh mới, 63 tỉnh cũ và chuỗi phân cách dấu phẩy)

    DECLARE @MappedCityCodes TABLE (Code VARCHAR(10));

    IF @CityCode IS NOT NULL AND @CityCode <> ''

    BEGIN

        INSERT INTO @MappedCityCodes(Code)

        SELECT DISTINCT m.OldCityCode 

        FROM STRING_SPLIT(@CityCode, ',') s

        JOIN BVTL_MAP_TINH_CU_MOI m ON LTRIM(RTRIM(s.value)) = m.NewCityCode

        UNION

        SELECT DISTINCT LTRIM(RTRIM(s.value))

        FROM STRING_SPLIT(@CityCode, ',') s

        WHERE NOT EXISTS (SELECT 1 FROM BVTL_MAP_TINH_CU_MOI m WHERE m.NewCityCode = LTRIM(RTRIM(s.value)))

          AND LTRIM(RTRIM(s.value)) <> '';

    END



    -- Lọc danh sách KH cơ bản theo Tỉnh, Nhóm, Thời gian và các bộ lọc phân tầng (Option B)

    SELECT 

        kh.RECORD_ID,

        kh.CITY_CODE,

        kh.MA_NHOM,

        kh.REDCAP_DAG,

        kh.DOI_TUONG,

        kh.NAM_SINH,

        kh.GIOI_TINH_TU_XD,

        kh.GIOI_TINH_KHAI_SINH,

        kh.CO_BHYT,

        kh.CO_CCCD,

        kh.NGAY_THAM_GIA,

        CASE 

            WHEN kh.GIOI_TINH_TU_XD IN (3, 4) OR kh.GIOI_TINH_KHAI_SINH = 3 THEN 3

            WHEN kh.GIOI_TINH_TU_XD = 1 OR (kh.GIOI_TINH_TU_XD IS NULL AND kh.GIOI_TINH_KHAI_SINH = 1) THEN 1

            WHEN kh.GIOI_TINH_TU_XD = 2 OR (kh.GIOI_TINH_TU_XD IS NULL AND kh.GIOI_TINH_KHAI_SINH = 2) THEN 2

            ELSE 3

        END AS GIOI_TINH,

        CASE 

            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) < 18 THEN '< 18'

            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 18 AND 25 THEN '18 - 25'

            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 26 AND 35 THEN '26 - 35'

            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) >= 36 THEN '>= 36'

            ELSE N'Chưa xác định'

        END AS NHOM_TUOI

    INTO #TmpKH

    FROM CD45_KH kh

    WHERE (@CityCode IS NULL OR @CityCode = '' OR kh.CITY_CODE IN (SELECT Code FROM @MappedCityCodes))

      AND (

          @MaNhom IS NULL OR @MaNhom = '' 

          OR kh.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom)

          OR kh.REDCAP_DAG IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom)

      )

      AND (@FromDate IS NULL OR kh.NGAY_THAM_GIA >= @FromDate)

      AND (@ToDate IS NULL OR kh.NGAY_THAM_GIA <= @ToDate)

      AND (@GioiTinhFilter IS NULL OR @GioiTinhFilter = 0 OR (

          CASE 

              WHEN kh.GIOI_TINH_TU_XD IN (3, 4) OR kh.GIOI_TINH_KHAI_SINH = 3 THEN 3

              WHEN kh.GIOI_TINH_TU_XD = 1 OR (kh.GIOI_TINH_TU_XD IS NULL AND kh.GIOI_TINH_KHAI_SINH = 1) THEN 1

              WHEN kh.GIOI_TINH_TU_XD = 2 OR (kh.GIOI_TINH_TU_XD IS NULL AND kh.GIOI_TINH_KHAI_SINH = 2) THEN 2

              ELSE 3

          END = @GioiTinhFilter

      ))

      AND (@NhomTuoiFilter IS NULL OR @NhomTuoiFilter = '' OR (

          CASE 

              WHEN (YEAR(GETDATE()) - kh.NAM_SINH) < 18 THEN '< 18'

              WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 18 AND 25 THEN '18 - 25'

              WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 26 AND 35 THEN '26 - 35'

              WHEN (YEAR(GETDATE()) - kh.NAM_SINH) >= 36 THEN '>= 36'

              ELSE N'Chưa xác định'

          END = @NhomTuoiFilter

      ))

      AND (@DoiTuongFilter IS NULL OR @DoiTuongFilter = 0 OR kh.DOI_TUONG = @DoiTuongFilter);



    CREATE CLUSTERED INDEX IX_TmpKH_Rec ON #TmpKH(RECORD_ID);



    -- 1. TỔNG QUAN KPI CARDS

    SELECT 

        (SELECT COUNT(DISTINCT RECORD_ID) FROM #TmpKH) AS TongKhachHang,

        ISNULL((SELECT COUNT(DISTINCT qst.RECORD_ID) FROM CD45_QST qst INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID WHERE qst.COMPLETE_STATUS = '2'), 0) AS TongSangLocQST,

        ISNULL((SELECT COUNT(DISTINCT qst.RECORD_ID) FROM CD45_QST qst INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID WHERE qst.COMPLETE_STATUS = '2' AND qst.MUC_QST IN (1, 2)), 0) AS QSTNguyCoCao,

        ISNULL((SELECT COUNT(DISTINCT cd.RECORD_ID) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID WHERE cd.COMPLETE_STATUS = '2'), 0) AS TongKhamSKTT,

        ISNULL((SELECT COUNT(*) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID WHERE cd.COMPLETE_STATUS = '2'), 0) AS TongLuotKhamSKTT,

        ISNULL((SELECT COUNT(DISTINCT tv.RECORD_ID) FROM CD45_TU_VAN_L1 tv INNER JOIN #TmpKH k ON tv.RECORD_ID = k.RECORD_ID WHERE tv.COMPLETE_STATUS = '2'), 0) AS TongTuVanL1,

        ISNULL((SELECT COUNT(DISTINCT htxh.RECORD_ID) FROM CD45_HO_TRO_XH htxh INNER JOIN #TmpKH k ON htxh.RECORD_ID = k.RECORD_ID WHERE htxh.COMPLETE_STATUS = '2'), 0) AS TongHoTroXH,

        ISNULL((SELECT SUM(hd.SO_TAI_LIEU) FROM CD45_HOAT_DONG hd INNER JOIN #TmpKH k ON hd.RECORD_ID = k.RECORD_ID WHERE hd.COMPLETE_STATUS = '2'), 0) AS TongTaiLieuPhat,

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

        WHERE qst.COMPLETE_STATUS = '2'

    )

    -- 2. PHÂN TÍCH QST THEO CHIỀU PHÂN TÍCH (@DimensionMode: 1=Quần thể, 2=Giới tính, 3=Nhóm tuổi)

    SELECT 

        d.DoiTuongId,

        d.TenDoiTuong,

        COUNT(DISTINCT kh.RECORD_ID) AS TongKH,

        COUNT(DISTINCT qst.RECORD_ID) AS SoKHSangLoc,

        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 1 THEN qst.RECORD_ID END) AS Muc1_RatCao,

        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 2 THEN qst.RECORD_ID END) AS Muc2_Cao,

        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 3 THEN qst.RECORD_ID END) AS Muc3_TrungBinh,

        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 4 THEN qst.RECORD_ID END) AS Muc4_Thap

    FROM (

        SELECT 1 AS DoiTuongId, N'PUD (Sử dụng ma túy)' AS TenDoiTuong, 1 AS SortOrder WHERE ISNULL(@DimensionMode, 1) = 1

        UNION ALL SELECT 2, N'PLHIV (Sống với HIV)', 2 WHERE ISNULL(@DimensionMode, 1) = 1

        UNION ALL SELECT 3, N'TG (Người chuyển giới)', 3 WHERE ISNULL(@DimensionMode, 1) = 1

        UNION ALL SELECT 4, N'MSM (Nam QHTD đồng giới)', 4 WHERE ISNULL(@DimensionMode, 1) = 1

        UNION ALL SELECT 5, N'SW (Người bán dâm)', 5 WHERE ISNULL(@DimensionMode, 1) = 1

        

        UNION ALL SELECT 1, N'Nam', 1 WHERE @DimensionMode = 2

        UNION ALL SELECT 2, N'Nữ', 2 WHERE @DimensionMode = 2

        UNION ALL SELECT 3, N'Khác', 3 WHERE @DimensionMode = 2

        

        UNION ALL SELECT 1, N'< 18', 1 WHERE @DimensionMode = 3

        UNION ALL SELECT 2, N'18 - 25', 2 WHERE @DimensionMode = 3

        UNION ALL SELECT 3, N'26 - 35', 3 WHERE @DimensionMode = 3

        UNION ALL SELECT 4, N'>= 36', 4 WHERE @DimensionMode = 3

        UNION ALL SELECT 5, N'Chưa xác định', 5 WHERE @DimensionMode = 3

    ) d

    LEFT JOIN #TmpKH kh ON (

        (ISNULL(@DimensionMode, 1) = 1 AND kh.DOI_TUONG = d.DoiTuongId)

        OR (@DimensionMode = 2 AND kh.GIOI_TINH = d.DoiTuongId)

        OR (@DimensionMode = 3 AND kh.NHOM_TUOI = d.TenDoiTuong)

    ) AND (@NhomTuoiTable1 IS NULL OR @NhomTuoiTable1 = '' OR kh.NHOM_TUOI = @NhomTuoiTable1)

    LEFT JOIN CTE_LatestQST qst ON kh.RECORD_ID = qst.RECORD_ID AND qst.rn = 1

    GROUP BY d.DoiTuongId, d.TenDoiTuong, d.SortOrder

    ORDER BY d.SortOrder;



    -- 3. PHÂN TÍCH QST THEO NHÓM ĐỘ TUỔI - BẢNG 2 & BIỂU ĐỒ TUỔI

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

        WHERE qst.COMPLETE_STATUS = '2'

    )

    SELECT 

        ao.NhomTuoi,

        COUNT(DISTINCT kh.RECORD_ID) AS TongKH,

        COUNT(DISTINCT qst.RECORD_ID) AS SoKHSangLoc,

        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 1 THEN qst.RECORD_ID END) AS Muc1,

        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 2 THEN qst.RECORD_ID END) AS Muc2,

        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 3 THEN qst.RECORD_ID END) AS Muc3,

        COUNT(DISTINCT CASE WHEN qst.MUC_QST = 4 THEN qst.RECORD_ID END) AS Muc4

    FROM (

        SELECT 1 AS AId, N'< 18' AS NhomTuoi

        UNION ALL SELECT 2, N'18 - 25'

        UNION ALL SELECT 3, N'26 - 35'

        UNION ALL SELECT 4, N'>= 36'

        UNION ALL SELECT 5, N'Chưa xác định'

    ) ao

    LEFT JOIN #TmpKH kh ON ao.NhomTuoi = kh.NHOM_TUOI

    LEFT JOIN CTE_LatestQST qst ON kh.RECORD_ID = qst.RECORD_ID AND qst.rn = 1

    GROUP BY ao.AId, ao.NhomTuoi

    ORDER BY ao.AId;



    -- 4. ĐÁNH GIÁ SANG CHẤN PTSD (PCL-5), RƯỢU (AUDIT-C) & KỲ THỊ (STIGMA)

    SELECT 

        d.DoiTuongId,

        d.TenDoiTuong,

        COUNT(tv.RECORD_ID) AS SoCaTuVan,

        SUM(CASE WHEN tv.PCL5_POSITIVE = 1 THEN 1 ELSE 0 END) AS PCL5_DuongTinh,

        SUM(CASE WHEN tv.PCL5_POSITIVE = 0 THEN 1 ELSE 0 END) AS PCL5_AmTinh,

        ROUND(ISNULL(AVG(CAST(tv.AUDIT_C_SCORE AS FLOAT)), 0), 1) AS DiemAuditCTB,

        ROUND(ISNULL(AVG(CAST(tv.STIGMA_SCORE AS FLOAT)), 0), 1) AS DiemKyThiTB

    FROM (

        SELECT 1 AS DoiTuongId, N'PUD' AS TenDoiTuong, 1 AS SortOrder WHERE ISNULL(@DimensionMode, 1) = 1

        UNION ALL SELECT 2, N'PLHIV', 2 WHERE ISNULL(@DimensionMode, 1) = 1

        UNION ALL SELECT 3, N'TG', 3 WHERE ISNULL(@DimensionMode, 1) = 1

        UNION ALL SELECT 4, N'MSM', 4 WHERE ISNULL(@DimensionMode, 1) = 1

        UNION ALL SELECT 5, N'SW', 5 WHERE ISNULL(@DimensionMode, 1) = 1

        

        UNION ALL SELECT 1, N'Nam', 1 WHERE @DimensionMode = 2

        UNION ALL SELECT 2, N'Nữ', 2 WHERE @DimensionMode = 2

        UNION ALL SELECT 3, N'Khác', 3 WHERE @DimensionMode = 2

        

        UNION ALL SELECT 1, N'< 18', 1 WHERE @DimensionMode = 3

        UNION ALL SELECT 2, N'18 - 25', 2 WHERE @DimensionMode = 3

        UNION ALL SELECT 3, N'26 - 35', 3 WHERE @DimensionMode = 3

        UNION ALL SELECT 4, N'>= 36', 4 WHERE @DimensionMode = 3

        UNION ALL SELECT 5, N'Chưa xác định', 5 WHERE @DimensionMode = 3

    ) d

    LEFT JOIN #TmpKH kh ON (

        (ISNULL(@DimensionMode, 1) = 1 AND kh.DOI_TUONG = d.DoiTuongId)

        OR (@DimensionMode = 2 AND kh.GIOI_TINH = d.DoiTuongId)

        OR (@DimensionMode = 3 AND kh.NHOM_TUOI = d.TenDoiTuong)

    )

    LEFT JOIN CD45_TU_VAN_L1 tv ON kh.RECORD_ID = tv.RECORD_ID AND tv.COMPLETE_STATUS = '2'

    GROUP BY d.DoiTuongId, d.TenDoiTuong, d.SortOrder

    ORDER BY d.SortOrder;



    -- 5. PHÂN BỐ THEO TỈNH THÀNH (HỖ TRỢ 34 TỈNH MỚI HOẶC 63 TỈNH CŨ)

    IF @CityMode = 'OLD63'

    BEGIN

        SELECT 

            kh.CITY_CODE AS CityCode,

            ISNULL(c.Name, kh.CITY_CODE) AS CityName,

            COUNT(DISTINCT kh.RECORD_ID) AS TongKH,

            ISNULL((SELECT COUNT(DISTINCT qst.RECORD_ID) FROM CD45_QST qst INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID WHERE k.CITY_CODE = kh.CITY_CODE AND qst.COMPLETE_STATUS = '2'), 0) AS SangLocQST,

            ISNULL((SELECT COUNT(DISTINCT cd.RECORD_ID) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID WHERE k.CITY_CODE = kh.CITY_CODE AND cd.COMPLETE_STATUS = '2'), 0) AS KhamSKTT,

            ISNULL((SELECT COUNT(DISTINCT tv.RECORD_ID) FROM CD45_TU_VAN_L1 tv INNER JOIN #TmpKH k ON tv.RECORD_ID = k.RECORD_ID WHERE k.CITY_CODE = kh.CITY_CODE AND tv.COMPLETE_STATUS = '2'), 0) AS TuVanL1

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

                    WHERE ISNULL(m2.NewCityCode, k.CITY_CODE) = ISNULL(map.NewCityCode, kh.CITY_CODE) AND qst.COMPLETE_STATUS = '2'), 0) AS SangLocQST,

            ISNULL((SELECT COUNT(DISTINCT cd.RECORD_ID) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID 

                    LEFT JOIN BVTL_MAP_TINH_CU_MOI m2 ON k.CITY_CODE = m2.OldCityCode 

                    WHERE ISNULL(m2.NewCityCode, k.CITY_CODE) = ISNULL(map.NewCityCode, kh.CITY_CODE) AND cd.COMPLETE_STATUS = '2'), 0) AS KhamSKTT,

            ISNULL((SELECT COUNT(DISTINCT tv.RECORD_ID) FROM CD45_TU_VAN_L1 tv INNER JOIN #TmpKH k ON tv.RECORD_ID = k.RECORD_ID 

                    LEFT JOIN BVTL_MAP_TINH_CU_MOI m2 ON k.CITY_CODE = m2.OldCityCode 

                    WHERE ISNULL(m2.NewCityCode, k.CITY_CODE) = ISNULL(map.NewCityCode, kh.CITY_CODE) AND tv.COMPLETE_STATUS = '2'), 0) AS TuVanL1

        FROM #TmpKH kh

        LEFT JOIN BVTL_MAP_TINH_CU_MOI map ON kh.CITY_CODE = map.OldCityCode

        LEFT JOIN BVTL_DM_TINH_MOI newc ON map.NewCityCode = newc.Code

        GROUP BY ISNULL(map.NewCityCode, kh.CITY_CODE), ISNULL(newc.Name, ISNULL(map.NewCityName, kh.CITY_CODE))

        ORDER BY TongKH DESC;

    END



    -- 6. PHỄU DỊCH VỤ CHĂM SÓC SKTT (CASCADE FUNNEL)

    SELECT 

        (SELECT COUNT(DISTINCT hd.RECORD_ID) FROM CD45_HOAT_DONG hd INNER JOIN #TmpKH k ON hd.RECORD_ID = k.RECORD_ID WHERE hd.COMPLETE_STATUS = '2') AS Step1_TiepCanTruyenThong,

        (SELECT COUNT(DISTINCT qst.RECORD_ID) FROM CD45_QST qst INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID WHERE qst.COMPLETE_STATUS = '2') AS Step2_SangLocQST,

        (SELECT COUNT(DISTINCT qst.RECORD_ID) FROM CD45_QST qst INNER JOIN #TmpKH k ON qst.RECORD_ID = k.RECORD_ID WHERE qst.COMPLETE_STATUS = '2' AND qst.MUC_QST IN (1, 2)) AS Step3_NguyCoCaoQST,

        (SELECT COUNT(DISTINCT tv.RECORD_ID) FROM CD45_TU_VAN_L1 tv INNER JOIN #TmpKH k ON tv.RECORD_ID = k.RECORD_ID WHERE tv.COMPLETE_STATUS = '2') AS Step4_TuVanTamLy,

        (SELECT COUNT(DISTINCT cd.RECORD_ID) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID WHERE cd.COMPLETE_STATUS = '2') AS Step5_KhamChuyenKhoa,

        (SELECT COUNT(DISTINCT cd.RECORD_ID) FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH k ON cd.RECORD_ID = k.RECORD_ID WHERE cd.COMPLETE_STATUS = '2' AND cd.LAN_KHAM > 1) AS Step6_TaiKhamSKTT;



    -- 7. DỊCH VỤ HỖ TRỢ CHUYỂN GỬI XÃ HỘI (CD45_HO_TRO_XH)

    SELECT 

        COUNT(DISTINCT htxh.RECORD_ID) AS TongNhanHoTro,

        COUNT(DISTINCT CASE WHEN (CHARINDEX(',1,', ',' + REPLACE(ISNULL(htxh.DICH_VU, ''), ' ', '') + ',') > 0 OR htxh.DICH_VU LIKE N'%BHYT%' OR htxh.DICH_VU LIKE N'%bảo hiểm%') THEN htxh.RECORD_ID END) AS HoTroBHYT,

        COUNT(DISTINCT CASE WHEN (CHARINDEX(',2,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%Methadone%') THEN htxh.RECORD_ID END) AS HoTroMethadone,

        COUNT(DISTINCT CASE WHEN (CHARINDEX(',10,', ',' + ISNULL(htxh.DICH_VU, '') + ',') > 0 OR htxh.DICH_VU LIKE N'%HIV%') THEN htxh.RECORD_ID END) AS XetNghiemHIV,

        COUNT(DISTINCT CASE WHEN htxh.DICH_VU LIKE N'%STIs%' THEN htxh.RECORD_ID END) AS STIs,

        COUNT(DISTINCT CASE WHEN htxh.DICH_VU LIKE N'%gan%' THEN htxh.RECORD_ID END) AS ViemGan

    FROM CD45_HO_TRO_XH htxh

    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID

    WHERE htxh.COMPLETE_STATUS = '2';



    DROP TABLE #TmpKH;

END

GO



IF OBJECT_ID('SP_CD45_GetBaoCao', 'P') IS NOT NULL DROP PROCEDURE [SP_CD45_GetBaoCao];

GO

CREATE   PROC SP_CD45_GetBaoCao

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

        Nam INT DEFAULT 0,

        Nu INT DEFAULT 0,

        Khac INT DEFAULT 0,

        Tuoi_18_25 INT DEFAULT 0,

        Tuoi_26_35 INT DEFAULT 0,

        Tuoi_Tren35 INT DEFAULT 0,

        IsBold BIT DEFAULT 0,

        IndentLevel INT DEFAULT 0,

        Code NVARCHAR(50) DEFAULT NULL

    );



    -- Resolve @CityCode mapping (hỗ trợ cả 34 tỉnh mới, 63 tỉnh cũ và chuỗi phân cách dấu phẩy)

    DECLARE @MappedCityCodes TABLE (Code VARCHAR(10));

    IF @CityCode IS NOT NULL AND @CityCode <> ''

    BEGIN

        INSERT INTO @MappedCityCodes(Code)

        SELECT DISTINCT m.OldCityCode 

        FROM STRING_SPLIT(@CityCode, ',') s

        JOIN BVTL_MAP_TINH_CU_MOI m ON LTRIM(RTRIM(s.value)) = m.NewCityCode

        UNION

        SELECT DISTINCT LTRIM(RTRIM(s.value))

        FROM STRING_SPLIT(@CityCode, ',') s

        WHERE NOT EXISTS (SELECT 1 FROM BVTL_MAP_TINH_CU_MOI m WHERE m.NewCityCode = LTRIM(RTRIM(s.value)))

          AND LTRIM(RTRIM(s.value)) <> '';

    END



    -- 2. Lọc danh sách Khách hàng cơ sở theo Tỉnh và Nhóm

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

            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 18 AND 25 THEN 1

            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) BETWEEN 26 AND 35 THEN 2

            WHEN (YEAR(GETDATE()) - kh.NAM_SINH) >= 36 THEN 3

            ELSE 2

        END AS NHOM_TUOI

    INTO #TmpKH

    FROM CD45_KH kh

    WHERE (@CityCode IS NULL OR @CityCode = '' OR kh.CITY_CODE IN (SELECT Code FROM @MappedCityCodes))

      AND (@MaNhom IS NULL OR @MaNhom = '' OR kh.MA_NHOM IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom) OR kh.REDCAP_DAG IN (@Var_MaNhomStd, @Var_MaNhomMap, @MaNhom))

      AND kh.COMPLETE_STATUS = '2';



    CREATE CLUSTERED INDEX IX_TmpKH_RecId ON #TmpKH(RECORD_ID);



    -- =========================================================================

    -- SECTION I: THÔNG TIN CHUNG

    -- =========================================================================

    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)

    VALUES ('I', N'THÔNG TIN CHUNG', 1, 0, 'SEC_I');



    -- 1. Tổng số KH được chăm sóc từ đầu dự án (F2 ∪ F3 ∪ F6 ∪ F7 ∪ F8 tính đến @ToDate)

    ;WITH CTE_AllTimeCare AS (

        SELECT RECORD_ID FROM CD45_HOAT_DONG 

        WHERE COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR NGAY_HOAT_DONG <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)

        UNION

        SELECT RECORD_ID FROM CD45_QST 

        WHERE COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR NGAY_SANG_LOC <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)

        UNION

        SELECT RECORD_ID FROM CD45_CHAN_DOAN 

        WHERE COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR NGAY_KHAM <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)

        UNION

        SELECT RECORD_ID FROM CD45_TU_VAN_L1 

        WHERE COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR NGAY_TU_VAN <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)

        UNION

        SELECT RECORD_ID FROM CD45_TU_VAN_L2 

        WHERE COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR NGAY_TU_VAN <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)

    )

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '1', N'Tổng số KH được chăm sóc từ đầu dự án',

        COUNT(DISTINCT c.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN c.RECORD_ID END),

        0, 0, 'I_1'

    FROM CTE_AllTimeCare c

    INNER JOIN #TmpKH kh ON c.RECORD_ID = kh.RECORD_ID;



    -- 2. Tổng số KH được chăm sóc trong kỳ báo cáo (F2 ∪ F3 ∪ F6 ∪ F7 ∪ F8 có ngày trong kỳ)

    ;WITH CTE_KyCare AS (

        SELECT RECORD_ID FROM CD45_HOAT_DONG 

        WHERE COMPLETE_STATUS = '2' AND (@FromDate IS NULL OR NGAY_HOAT_DONG >= @FromDate) AND (@ToDate IS NULL OR NGAY_HOAT_DONG <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)

        UNION

        SELECT RECORD_ID FROM CD45_QST 

        WHERE COMPLETE_STATUS = '2' AND (@FromDate IS NULL OR NGAY_SANG_LOC >= @FromDate) AND (@ToDate IS NULL OR NGAY_SANG_LOC <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)

        UNION

        SELECT RECORD_ID FROM CD45_CHAN_DOAN 

        WHERE COMPLETE_STATUS = '2' AND (@FromDate IS NULL OR NGAY_KHAM >= @FromDate) AND (@ToDate IS NULL OR NGAY_KHAM <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)

        UNION

        SELECT RECORD_ID FROM CD45_TU_VAN_L1 

        WHERE COMPLETE_STATUS = '2' AND (@FromDate IS NULL OR NGAY_TU_VAN >= @FromDate) AND (@ToDate IS NULL OR NGAY_TU_VAN <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)

        UNION

        SELECT RECORD_ID FROM CD45_TU_VAN_L2 

        WHERE COMPLETE_STATUS = '2' AND (@FromDate IS NULL OR NGAY_TU_VAN >= @FromDate) AND (@ToDate IS NULL OR NGAY_TU_VAN <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR MA_TCV = @MaTCV)

    )

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '2', N'Tổng số KH được chăm sóc trong kỳ báo cáo',

        COUNT(DISTINCT c.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN c.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN c.RECORD_ID END),

        0, 0, 'I_2'

    FROM CTE_KyCare c

    INNER JOIN #TmpKH kh ON c.RECORD_ID = kh.RECORD_ID;



    -- 3. Số KH mất dấu trong kỳ báo cáo

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '3', N'Số KH mất dấu trong kỳ báo cáo',

        COUNT(DISTINCT td.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN td.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN td.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN td.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN td.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN td.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN td.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN td.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN td.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN td.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN td.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN td.RECORD_ID END),

        0, 0, 'I_3'

    FROM CD45_THEO_DAU td

    INNER JOIN #TmpKH kh ON td.RECORD_ID = kh.RECORD_ID

    WHERE td.MAT_DAU = 1

      AND ISNULL(td.COMPLETE_STATUS, '2') = '2'

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

          AND hd.COMPLETE_STATUS = '2'

    )

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '1', N'Số KH được tham gia truyền thông lần 1',

        COUNT(DISTINCT tt.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN tt.RECORD_ID END),

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

          AND hd.COMPLETE_STATUS = '2'

    )

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '2', N'Số KH được tham gia truyền thông lần 2',

        COUNT(DISTINCT tt.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN tt.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN tt.RECORD_ID END),

        0, 0, 'II_2'

    FROM CTE_TruyenThong tt

    INNER JOIN #TmpKH kh ON tt.RECORD_ID = kh.RECORD_ID

    WHERE tt.TT_Order > 1

      AND (@FromDate IS NULL OR tt.NGAY_HOAT_DONG >= @FromDate)

      AND (@ToDate IS NULL OR tt.NGAY_HOAT_DONG <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR tt.MA_TCV = @MaTCV);



    -- 3. Tổng lượt

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '3', N'Tổng số lượt KH tham gia truyền thông',

        COUNT(*),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 3 THEN 1 ELSE 0 END), 0),

        0, 0, 'II_3'

    FROM CD45_HOAT_DONG hd

    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID

    WHERE hd.LOAI_DV = 1

      AND hd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)

      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);



    -- =========================================================================

    -- SECTION III: SÀNG LỌC BẰNG HỎI QST VÀ CHUYỂN GỬI KHÁM, ĐIỀU TRỊ SKTT

    -- =========================================================================

    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)

    VALUES ('III', N'SÀNG LỌC BẰNG HỎI QST VÀ CHUYỂN GỬI KHÁM, ĐIỀU TRỊ SKTT', 1, 0, 'SEC_III');



    -- 1. Sàng lọc QST Lần 1

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '1', N'Số KH được sàng lọc bằng hỏi QST lần 1',

        COUNT(DISTINCT qst.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN qst.RECORD_ID END),

        0, 0, 'III_1'

    FROM CD45_QST qst

    INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID

    WHERE qst.REPEAT_INSTANCE = 1

      AND qst.COMPLETE_STATUS = '2'

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



    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '', m.Ten,

        COUNT(DISTINCT qst.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN qst.RECORD_ID END),

        0, 1, m.Code

    FROM @MucQst m

    LEFT JOIN (

        CD45_QST qst INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID

    ) ON qst.MUC_QST = m.Muc AND qst.REPEAT_INSTANCE = 1

        AND qst.COMPLETE_STATUS = '2'

        AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)

        AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)

        AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)

    GROUP BY m.Muc, m.Ten, m.Code

    ORDER BY m.Muc;



    -- 2. Sàng lọc QST Lần 2+

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '2', N'Số KH được sàng lọc lại bảng hỏi QST (từ lần 2 trở đi)',

        COUNT(DISTINCT qst.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN qst.RECORD_ID END),

        0, 0, 'III_2'

    FROM CD45_QST qst

    INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID

    WHERE qst.REPEAT_INSTANCE > 1

      AND qst.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)

      AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV);



    -- 2.1 - 2.4: Mức điểm QST Lần 2+

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '', m.Ten,

        COUNT(DISTINCT qst.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN qst.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN qst.RECORD_ID END),

        0, 1, 'III_2_' + CAST(m.Muc AS VARCHAR)

    FROM @MucQst m

    LEFT JOIN (

        CD45_QST qst INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID

    ) ON qst.MUC_QST = m.Muc AND qst.REPEAT_INSTANCE > 1

        AND qst.COMPLETE_STATUS = '2'

        AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate)

        AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)

        AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)

    GROUP BY m.Muc, m.Ten

    ORDER BY m.Muc;



    -- 3. Số lượt KH được chuyển gửi khám SKTT

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '3', N'Số lượt KH được chuyển gửi khám SKTT',

        COUNT(*),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 3 THEN 1 ELSE 0 END), 0),

        0, 0, 'III_3'

    FROM CD45_CHAN_DOAN cd

    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID

    WHERE cd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)

      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);



    -- 4. Số KH được chuyển gửi khám SKTT, trong đó:

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '4', N'Số KH được chuyển gửi khám SKTT, trong đó:',

        COUNT(DISTINCT cd.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN cd.RECORD_ID END),

        0, 0, 'III_4'

    FROM CD45_CHAN_DOAN cd

    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID

    WHERE cd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)

      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);



    -- 4.1 Khám lần 1

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '', N'Số KH được chuyển gửi khám SKTT lần 1',

        COUNT(DISTINCT cd.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN cd.RECORD_ID END),

        0, 1, 'III_4_1'

    FROM CD45_CHAN_DOAN cd

    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID

    WHERE cd.LAN_KHAM = 1

      AND cd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)

      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);



    -- 4.2 Tái khám SKTT (Số KH)

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '', N'Số KH được tái khám SKTT',

        COUNT(DISTINCT cd.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN cd.RECORD_ID END),

        0, 1, 'III_4_2'

    FROM CD45_CHAN_DOAN cd

    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID

    WHERE cd.LAN_KHAM > 1

      AND cd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)

      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);



    -- 4.3 Tái khám SKTT (Số lượt)

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '', N'Số lượt KH được tái khám SKTT',

        COUNT(*),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 3 THEN 1 ELSE 0 END), 0),

        0, 1, 'III_4_3'

    FROM CD45_CHAN_DOAN cd

    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID

    WHERE cd.LAN_KHAM > 1

      AND cd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)

      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);



    -- 5. Điều trị nội trú (nhập viện) - Mã '2' trong f6_treatment

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '5', N'Số KH được điều trị nội trú (nhập viện)',

        COUNT(DISTINCT cd.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN cd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN cd.RECORD_ID END),

        0, 0, 'III_5'

    FROM CD45_CHAN_DOAN cd

    INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID

    WHERE cd.COMPLETE_STATUS = '2'

      AND (CHARINDEX(',2,', ',' + ISNULL(cd.HINH_THUC_DIEU_TRI, '') + ',') > 0 

           OR cd.HINH_THUC_DIEU_TRI LIKE '%2%' 

           OR cd.HINH_THUC_DIEU_TRI LIKE N'%nội trú%')

      AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)

      AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV);



    -- 6. Hỗ trợ mua thẻ BHYT - Mã '1' trong f4_services

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '6', N'Số KH được hỗ trợ mua thẻ Bảo hiểm y tế',

        COUNT(DISTINCT htxh.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN htxh.RECORD_ID END),

        0, 0, 'III_6'

    FROM CD45_HO_TRO_XH htxh

    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID

    WHERE htxh.COMPLETE_STATUS = '2'

      AND (CHARINDEX(',1,', ',' + REPLACE(ISNULL(htxh.DICH_VU, ''), ' ', '') + ',') > 0 

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

        SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1 WHERE COMPLETE_STATUS = '2'

        UNION ALL

        SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2 WHERE COMPLETE_STATUS = '2'

    )

    -- 1. Số lượt KH được tư vấn cá nhân

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '1', N'Số lượt KH được tư vấn cá nhân',

        COUNT(*),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 3 THEN 1 ELSE 0 END), 0),

        0, 0, 'IV_1'

    FROM CTE_TuVan tv

    INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID

    WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)

      AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV);



    -- 2. Số KH được tư vấn cá nhân, trong đó:

    ;WITH CTE_TuVanCount AS (

        SELECT tv.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI, COUNT(*) AS SoLan

        FROM (

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1 WHERE COMPLETE_STATUS = '2'

            UNION ALL

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2 WHERE COMPLETE_STATUS = '2'

        ) tv

        INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID

        WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)

          AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV)

        GROUP BY tv.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI

    )

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '2', N'Số KH được tư vấn cá nhân, trong đó:',

        COUNT(DISTINCT RECORD_ID),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 5 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 4 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 3 THEN RECORD_ID END),

        0, 0, 'IV_2'

    FROM CTE_TuVanCount;



    -- 2.1 Tư vấn 1 lần

    ;WITH CTE_TuVanCount AS (

        SELECT tv.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI, COUNT(*) AS SoLan

        FROM (

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1 WHERE COMPLETE_STATUS = '2'

            UNION ALL

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2 WHERE COMPLETE_STATUS = '2'

        ) tv

        INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID

        WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)

          AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV)

        GROUP BY tv.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI

    )

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '', N'Số KH được tư vấn 1 lần',

        COUNT(DISTINCT CASE WHEN SoLan = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 1 AND SoLan = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 2 AND SoLan = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 3 AND SoLan = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 5 AND SoLan = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 4 AND SoLan = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 1 AND SoLan = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 2 AND SoLan = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 3 AND SoLan = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 1 AND SoLan = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 2 AND SoLan = 1 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 3 AND SoLan = 1 THEN RECORD_ID END),

        0, 1, 'IV_2_1'

    FROM CTE_TuVanCount;



    -- 2.2 Tư vấn 2 lần

    ;WITH CTE_TuVanCount AS (

        SELECT tv.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI, COUNT(*) AS SoLan

        FROM (

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1 WHERE COMPLETE_STATUS = '2'

            UNION ALL

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2 WHERE COMPLETE_STATUS = '2'

        ) tv

        INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID

        WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)

          AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV)

        GROUP BY tv.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI

    )

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '', N'Số KH được tư vấn 2 lần',

        COUNT(DISTINCT CASE WHEN SoLan = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 1 AND SoLan = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 2 AND SoLan = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 3 AND SoLan = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 5 AND SoLan = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 4 AND SoLan = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 1 AND SoLan = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 2 AND SoLan = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 3 AND SoLan = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 1 AND SoLan = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 2 AND SoLan = 2 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 3 AND SoLan = 2 THEN RECORD_ID END),

        0, 1, 'IV_2_2'

    FROM CTE_TuVanCount;



    -- 2.3 Tư vấn từ 3 lần trở lên

    ;WITH CTE_TuVanCount AS (

        SELECT tv.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI, COUNT(*) AS SoLan

        FROM (

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1 WHERE COMPLETE_STATUS = '2'

            UNION ALL

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2 WHERE COMPLETE_STATUS = '2'

        ) tv

        INNER JOIN #TmpKH kh ON tv.RECORD_ID = kh.RECORD_ID

        WHERE (@FromDate IS NULL OR tv.NGAY_TU_VAN >= @FromDate)

          AND (@ToDate IS NULL OR tv.NGAY_TU_VAN <= @ToDate)

          AND (@MaTCV IS NULL OR @MaTCV = '' OR tv.MA_TCV = @MaTCV)

        GROUP BY tv.RECORD_ID, kh.DOI_TUONG, kh.GIOI_TINH, kh.NHOM_TUOI

    )

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '', N'Số KH được tư vấn từ 3 lần trở lên',

        COUNT(DISTINCT CASE WHEN SoLan >= 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 1 AND SoLan >= 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 2 AND SoLan >= 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 3 AND SoLan >= 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 5 AND SoLan >= 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN DOI_TUONG = 4 AND SoLan >= 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 1 AND SoLan >= 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 2 AND SoLan >= 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN GIOI_TINH = 3 AND SoLan >= 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 1 AND SoLan >= 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 2 AND SoLan >= 3 THEN RECORD_ID END),

        COUNT(DISTINCT CASE WHEN NHOM_TUOI = 3 AND SoLan >= 3 THEN RECORD_ID END),

        0, 1, 'IV_2_3'

    FROM CTE_TuVanCount;



    -- 3. Số lượt KH tham gia sinh hoạt nhóm (LOAI_DV = 2)

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '3', N'Số lượt KH được tham gia sinh hoạt nhóm',

        COUNT(*),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 3 THEN 1 ELSE 0 END), 0),

        0, 0, 'IV_3'

    FROM CD45_HOAT_DONG hd

    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID

    WHERE hd.LOAI_DV = 2

      AND hd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)

      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);



    -- 4. Số KH tham gia sinh hoạt nhóm

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '4', N'Số KH được tham gia sinh hoạt nhóm',

        COUNT(DISTINCT hd.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN hd.RECORD_ID END),

        0, 0, 'IV_4'

    FROM CD45_HOAT_DONG hd

    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID

    WHERE hd.LOAI_DV = 2

      AND hd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)

      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);



    -- 5. Số lượt tham gia can thiệp chữa lành (LOAI_DV = 3 hoặc 4: Vòng tròn chia sẻ & Trị liệu nghệ thuật)

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '5', N'Số lượt KH được tham gia can thiệp chữa lành',

        COUNT(*),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 3 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 1 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 2 THEN 1 ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 3 THEN 1 ELSE 0 END), 0),

        0, 0, 'IV_5'

    FROM CD45_HOAT_DONG hd

    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID

    WHERE hd.LOAI_DV IN (3, 4)

      AND hd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)

      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);



    -- 6. Số KH tham gia can thiệp chữa lành

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '6', N'Số KH được tham gia can thiệp chữa lành',

        COUNT(DISTINCT hd.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN hd.RECORD_ID END),

        0, 0, 'IV_6'

    FROM CD45_HOAT_DONG hd

    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID

    WHERE hd.LOAI_DV IN (3, 4)

      AND hd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)

      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);



    -- =========================================================================

    -- SECTION V: CAN THIỆP ONLINE

    -- =========================================================================

    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)

    VALUES ('V', N'CAN THIỆP ONLINE', 1, 0, 'SEC_V');



    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '1', N'Số lượt KH được can thiệp online',

        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 'V_1';



    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '2', N'Số KH được can thiệp online',

        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 'V_2';



    -- =========================================================================

    -- SECTION VI: DỊCH VỤ CHUYỂN GỬI KHÁC

    -- =========================================================================

    INSERT INTO @TmpResult (STT, ChiTieu, IsBold, IndentLevel, Code)

    VALUES ('VI', N'DỊCH VỤ CHUYỂN GỬI KHÁC', 1, 0, 'SEC_VI');



    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '1', N'Số KH được chuyển gửi dịch vụ/xét nghiệm thành công',

        COUNT(DISTINCT htxh.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN htxh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN htxh.RECORD_ID END),

        0, 0, 'VI_1'

    FROM CD45_HO_TRO_XH htxh

    INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID

    WHERE htxh.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)

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



    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '', dv.Ten,

        COUNT(DISTINCT kh.RECORD_ID),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 THEN kh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 THEN kh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 THEN kh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 THEN kh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 THEN kh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 THEN kh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 THEN kh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 THEN kh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 THEN kh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 THEN kh.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 THEN kh.RECORD_ID END),

        0, 1, dv.SubCode

    FROM @DvList dv

    LEFT JOIN (

        CD45_HO_TRO_XH htxh

        INNER JOIN #TmpKH kh ON htxh.RECORD_ID = kh.RECORD_ID

    ) ON (

            htxh.COMPLETE_STATUS = '2'

            AND (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)

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

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '1', N'Số quyển tài liệu đã phát',

        ISNULL(SUM(hd.SO_TAI_LIEU), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 1 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 2 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 3 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 5 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.DOI_TUONG = 4 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 1 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 2 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.GIOI_TINH = 3 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 1 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 2 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),

        ISNULL(SUM(CASE WHEN kh.NHOM_TUOI = 3 THEN hd.SO_TAI_LIEU ELSE 0 END), 0),

        0, 0, 'VII_1'

    FROM CD45_HOAT_DONG hd

    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID

    WHERE hd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)

      AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)

      AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV);



    -- 2. Số KH nhận tài liệu

    INSERT INTO @TmpResult (STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, Nam, Nu, Khac, Tuoi_18_25, Tuoi_26_35, Tuoi_Tren35, IsBold, IndentLevel, Code)

    SELECT 

        '2', N'Số KH nhận tài liệu',

        COUNT(DISTINCT CASE WHEN ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 1 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 2 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 3 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 5 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.DOI_TUONG = 4 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 1 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 2 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.GIOI_TINH = 3 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 1 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 2 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        COUNT(DISTINCT CASE WHEN kh.NHOM_TUOI = 3 AND ISNULL(hd.SO_TAI_LIEU, 0) > 0 THEN hd.RECORD_ID END),

        0, 0, 'VII_2'

    FROM CD45_HOAT_DONG hd

    INNER JOIN #TmpKH kh ON hd.RECORD_ID = kh.RECORD_ID

    WHERE hd.COMPLETE_STATUS = '2'

      AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)

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

        ISNULL(Nam, 0) AS Nam, 

        ISNULL(Nu, 0) AS Nu, 

        ISNULL(Khac, 0) AS Khac, 

        ISNULL(Tuoi_18_25, 0) AS Tuoi_18_25, 

        ISNULL(Tuoi_26_35, 0) AS Tuoi_26_35, 

        ISNULL(Tuoi_Tren35, 0) AS Tuoi_Tren35, 

        ISNULL(IsBold, 0) AS IsBold, 

        ISNULL(IndentLevel, 0) AS IndentLevel,

        Code

    FROM @TmpResult

    ORDER BY STT_Sort;



    DROP TABLE #TmpKH;

END

GO



IF OBJECT_ID('SP_CD45_GetBaoCaoBacSi', 'P') IS NOT NULL DROP PROCEDURE [SP_CD45_GetBaoCaoBacSi];

GO

-- =========================================================================

-- BÁO CÁO BÁC SĨ (DỰ ÁN CD45 - DREAMH) - STT 42

-- =========================================================================



CREATE   PROC SP_CD45_GetBaoCaoBacSi

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



IF OBJECT_ID('SP_CD45_GetBaoCaoBacSi_TongHop', 'P') IS NOT NULL DROP PROCEDURE [SP_CD45_GetBaoCaoBacSi_TongHop];

GO

CREATE   PROC SP_CD45_GetBaoCaoBacSi_TongHop

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



IF OBJECT_ID('SP_CD45_GetBaoCaoTCV', 'P') IS NOT NULL DROP PROCEDURE [SP_CD45_GetBaoCaoTCV];

GO



CREATE   PROC SP_CD45_GetBaoCaoTCV

    @FromDate DATE = NULL,

    @ToDate DATE = NULL,

    @MaNhom VARCHAR(100) = NULL,

    @MaTCV VARCHAR(100) = NULL

AS

BEGIN

    SET NOCOUNT ON;

    EXEC SP_CD45_GetBaoCao @FromDate = @FromDate, @ToDate = @ToDate, @CityCode = NULL, @MaNhom = @MaNhom, @MaTCV = @MaTCV;

END



GO



IF OBJECT_ID('SP_CD45_GetDrillDown', 'P') IS NOT NULL DROP PROCEDURE [SP_CD45_GetDrillDown];

GO

-- =========================================================================



-- SP_CD45_GetDrillDown: Phục vụ Drill Down Chi tiết Báo cáo CD45



-- =========================================================================



CREATE   PROC SP_CD45_GetDrillDown

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



    -- Resolve @CityCode mapping (hỗ trợ cả 34 tỉnh mới, 63 tỉnh cũ và chuỗi phân cách dấu phẩy)

    DECLARE @MappedCityCodes TABLE (Code VARCHAR(10));

    IF @CityCode IS NOT NULL AND @CityCode <> ''

    BEGIN

        INSERT INTO @MappedCityCodes(Code)

        SELECT DISTINCT m.OldCityCode 

        FROM STRING_SPLIT(@CityCode, ',') s

        JOIN BVTL_MAP_TINH_CU_MOI m ON LTRIM(RTRIM(s.value)) = m.NewCityCode

        UNION

        SELECT DISTINCT LTRIM(RTRIM(s.value))

        FROM STRING_SPLIT(@CityCode, ',') s

        WHERE NOT EXISTS (SELECT 1 FROM BVTL_MAP_TINH_CU_MOI m WHERE m.NewCityCode = LTRIM(RTRIM(s.value)))

          AND LTRIM(RTRIM(s.value)) <> '';

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

      AND kh.COMPLETE_STATUS = '2'

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

            WHERE hd.COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)

              AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)

            UNION ALL

            SELECT qst.RECORD_ID, qst.MA_TCV, qst.NGAY_SANG_LOC AS ACT_DATE, N'Sàng lọc QST' AS CHI_TIET

            FROM CD45_QST qst

            WHERE qst.COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)

              AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)

            UNION ALL

            SELECT cd.RECORD_ID, cd.MA_TCV, cd.NGAY_KHAM AS ACT_DATE, N'Khám SKTT' AS CHI_TIET

            FROM CD45_CHAN_DOAN cd

            WHERE cd.COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)

              AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV)

            UNION ALL

            SELECT tv1.RECORD_ID, tv1.MA_TCV, tv1.NGAY_TU_VAN AS ACT_DATE, N'Tư vấn lần 1' AS CHI_TIET

            FROM CD45_TU_VAN_L1 tv1

            WHERE tv1.COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR tv1.NGAY_TU_VAN <= @ToDate)

              AND (@MaTCV IS NULL OR @MaTCV = '' OR tv1.MA_TCV = @MaTCV)

            UNION ALL

            SELECT tv2.RECORD_ID, tv2.MA_TCV, tv2.NGAY_TU_VAN AS ACT_DATE, N'Tư vấn lần 2+' AS CHI_TIET

            FROM CD45_TU_VAN_L2 tv2

            WHERE tv2.COMPLETE_STATUS = '2' AND (@ToDate IS NULL OR tv2.NGAY_TU_VAN <= @ToDate)

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

            WHERE hd.COMPLETE_STATUS = '2' AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate) AND (@ToDate IS NULL OR hd.NGAY_HOAT_DONG <= @ToDate)

              AND (@MaTCV IS NULL OR @MaTCV = '' OR hd.MA_TCV = @MaTCV)

            UNION ALL

            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, qst.MA_TCV, kh.DOI_TUONG_TEXT,

                   qst.NGAY_SANG_LOC AS ACT_DATE, N'Sàng lọc QST' AS CHI_TIET

            FROM CD45_QST qst INNER JOIN #TmpKH kh ON qst.RECORD_ID = kh.RECORD_ID

            WHERE qst.COMPLETE_STATUS = '2' AND (@FromDate IS NULL OR qst.NGAY_SANG_LOC >= @FromDate) AND (@ToDate IS NULL OR qst.NGAY_SANG_LOC <= @ToDate)

              AND (@MaTCV IS NULL OR @MaTCV = '' OR qst.MA_TCV = @MaTCV)

            UNION ALL

            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, cd.MA_TCV, kh.DOI_TUONG_TEXT,

                   cd.NGAY_KHAM AS ACT_DATE, N'Khám SKTT' AS CHI_TIET

            FROM CD45_CHAN_DOAN cd INNER JOIN #TmpKH kh ON cd.RECORD_ID = kh.RECORD_ID

            WHERE cd.COMPLETE_STATUS = '2' AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate) AND (@ToDate IS NULL OR cd.NGAY_KHAM <= @ToDate)

              AND (@MaTCV IS NULL OR @MaTCV = '' OR cd.MA_TCV = @MaTCV)

            UNION ALL

            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, tv1.MA_TCV, kh.DOI_TUONG_TEXT,

                   tv1.NGAY_TU_VAN AS ACT_DATE, N'Tư vấn lần 1' AS CHI_TIET

            FROM CD45_TU_VAN_L1 tv1 INNER JOIN #TmpKH kh ON tv1.RECORD_ID = kh.RECORD_ID

            WHERE tv1.COMPLETE_STATUS = '2' AND (@FromDate IS NULL OR tv1.NGAY_TU_VAN >= @FromDate) AND (@ToDate IS NULL OR tv1.NGAY_TU_VAN <= @ToDate)

              AND (@MaTCV IS NULL OR @MaTCV = '' OR tv1.MA_TCV = @MaTCV)

            UNION ALL

            SELECT kh.RECORD_ID, kh.CITY_CODE, kh.MA_NHOM, tv2.MA_TCV, kh.DOI_TUONG_TEXT,

                   tv2.NGAY_TU_VAN AS ACT_DATE, N'Tư vấn lần 2+' AS CHI_TIET

            FROM CD45_TU_VAN_L2 tv2 INNER JOIN #TmpKH kh ON tv2.RECORD_ID = kh.RECORD_ID

            WHERE tv2.COMPLETE_STATUS = '2' AND (@FromDate IS NULL OR tv2.NGAY_TU_VAN >= @FromDate) AND (@ToDate IS NULL OR tv2.NGAY_TU_VAN <= @ToDate)

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

          AND hd.COMPLETE_STATUS = '2'

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

              AND qst.COMPLETE_STATUS = '2'

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

              AND qst.COMPLETE_STATUS = '2'

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

            WHERE cd.COMPLETE_STATUS = '2'

              AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)

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

                WHERE cd.COMPLETE_STATUS = '2'

                  AND (@FromDate IS NULL OR cd.NGAY_KHAM >= @FromDate)

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

            WHERE htxh.COMPLETE_STATUS = '2'

              AND (CHARINDEX(',1,', ',' + REPLACE(ISNULL(htxh.DICH_VU, ''), ' ', '') + ',') > 0 OR htxh.DICH_VU LIKE N'%BHYT%' OR htxh.DICH_VU LIKE N'%bảo hiểm%')

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

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV, N'Tư vấn lần 1' AS LOAI FROM CD45_TU_VAN_L1 WHERE COMPLETE_STATUS = '2'

            UNION ALL

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV, N'Tư vấn lần 2+' AS LOAI FROM CD45_TU_VAN_L2 WHERE COMPLETE_STATUS = '2'

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

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L1 WHERE COMPLETE_STATUS = '2'

            UNION ALL

            SELECT RECORD_ID, NGAY_TU_VAN, MA_TCV FROM CD45_TU_VAN_L2 WHERE COMPLETE_STATUS = '2'

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

            WHERE hd.COMPLETE_STATUS = '2'

              AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)

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

                WHERE hd.COMPLETE_STATUS = '2'

                  AND (@FromDate IS NULL OR hd.NGAY_HOAT_DONG >= @FromDate)

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

            WHERE htxh.COMPLETE_STATUS = '2'

              AND (@FromDate IS NULL OR htxh.NGAY_HO_TRO >= @FromDate)

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

        WHERE hd.COMPLETE_STATUS = '2'

          AND ISNULL(hd.SO_TAI_LIEU, 0) > 0

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

            WHERE hd.COMPLETE_STATUS = '2'

              AND ISNULL(hd.SO_TAI_LIEU, 0) > 0

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



IF OBJECT_ID('SP_CD45_ResetCauHinhChiTieuMacDinh', 'P') IS NOT NULL DROP PROCEDURE [SP_CD45_ResetCauHinhChiTieuMacDinh];

GO

CREATE   PROC dbo.SP_CD45_ResetCauHinhChiTieuMacDinh

    @UpdatedBy NVARCHAR(100) = NULL

AS

BEGIN

    SET NOCOUNT ON;

    UPDATE dbo.CD45_BCTIEU_CAU_HINH

    SET HienThi_Thang = Default_Thang,

        HienThi_Quy   = Default_Quy,

        HienThi_6T    = Default_6T,

        HienThi_12T   = Default_12T,

        UpdatedAt     = GETDATE(),

        UpdatedBy     = ISNULL(@UpdatedBy, 'System')

    WHERE IsSection = 0;

END

GO



IF OBJECT_ID('SP_CD45_UpdateNhomPrefix', 'P') IS NOT NULL DROP PROCEDURE [SP_CD45_UpdateNhomPrefix];

GO

-- 4. Cập nhật Stored Procedure SP_CD45_UpdateNhomPrefix để hỗ trợ lưu thêm Chức danh

CREATE   PROCEDURE [dbo].[SP_CD45_UpdateNhomPrefix]

    @MaNhom VARCHAR(20),

    @Prefix NVARCHAR(50),

    @ShortPrefix NVARCHAR(20),

    @ChucDanh NVARCHAR(50) = NULL

AS

BEGIN

    SET NOCOUNT ON;

    

    SET @Prefix = ISNULL(NULLIF(LTRIM(RTRIM(@Prefix)), ''), N'Nhóm');

    SET @ShortPrefix = ISNULL(NULLIF(LTRIM(RTRIM(@ShortPrefix)), ''), @Prefix);

    SET @ChucDanh = ISNULL(NULLIF(LTRIM(RTRIM(@ChucDanh)), ''), N'Trưởng nhóm');

    

    UPDATE dbo.BVTL_NHOM_TBH

    SET PREFIX = @Prefix,

        SHORT_PREFIX = @ShortPrefix,

        CHUC_DANH = @ChucDanh

    WHERE manhom_tbh = @MaNhom OR manhom_tbh_map = @MaNhom;



    UPDATE dbo.CD45_NHOM_TCV

    SET PREFIX = @Prefix,

        SHORT_PREFIX = @ShortPrefix,

        CHUC_DANH = @ChucDanh

    WHERE MA_NHOM = @MaNhom;

    

    SELECT @@ROWCOUNT AS RowsAffected;

END

GO





-- ============================================================

-- 4. UPGRADE EXISTING STORED PROCEDURES (City, User, NhomTBH)

-- ============================================================

IF OBJECT_ID('City_Get_By_Page', 'P') IS NOT NULL DROP PROCEDURE [City_Get_By_Page];

GO

-- 6. NÂNG CẤP STORED PROCEDURE City_Get_By_Page HỖ TRỢ @CityMode ('NEW34' | 'OLD63')

CREATE   PROCEDURE [dbo].[City_Get_By_Page] 

	@Keyword NVARCHAR(250) = null,

	@OrderByName VARCHAR(100) = 'KeyFirst',

	@Page int = 1,

	@PageSize int = 20,

	@IsKeyOnly bit = 0,

	@CityMode VARCHAR(10) = 'NEW34'

AS

BEGIN

    SET NOCOUNT ON;



    IF @CityMode = 'OLD63'

    BEGIN

        SELECT 

            c.Code,

            c.[Name],

            c.Code_Map,

            c.IsActive,

            c.CreatedDate,

            c.CreatedBy,

            c.LastUpdateDate,

            c.LastUpdateBy,

            CAST(CASE WHEN c.Code_Map IS NOT NULL AND RTRIM(c.Code_Map) <> '' THEN 1 ELSE 0 END AS BIT) AS IsKeyProvince,

            1 AS OldCount,

            c.[Name] AS OldNamesSummary,

            count(c.Code) over() as TotalRow  

        FROM [dbo].[BVTL_CITES] c

        WHERE (@Keyword IS NULL OR (c.Code LIKE '%'+@Keyword+'%' OR c.[Name] LIKE N'%'+@Keyword+'%' OR c.Code_Map LIKE '%'+@Keyword+'%'))

          AND (@IsKeyOnly = 0 OR (@IsKeyOnly = 1 AND c.Code_Map IS NOT NULL AND RTRIM(c.Code_Map) <> ''))

        ORDER BY 

            CASE 

                WHEN @OrderByName = 'KeyFirst' AND c.Code_Map IS NOT NULL AND RTRIM(c.Code_Map) <> '' THEN 0

                WHEN @OrderByName = 'KeyFirst' THEN 1

                ELSE 0 

            END,

            CASE @OrderByName

                WHEN 'Name' THEN c.[Name]

                ELSE c.Code

            END ASC

        OFFSET ((@Page - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;

    END

    ELSE

    BEGIN

        -- Chế độ 34 Tỉnh mới (NQ 202/2025/QH15)

        SELECT 

            m.Code,

            m.[Name],

            m.Code_Map,

            m.IsActive,

            m.CreatedDate,

            1 AS CreatedBy,

            NULL AS LastUpdateDate,

            NULL AS LastUpdateBy,

            m.IsKeyProvince,

            m.OldCount,

            m.OldNamesSummary,

            count(m.Code) over() as TotalRow  

        FROM [dbo].[BVTL_DM_TINH_MOI] m

        WHERE (@Keyword IS NULL OR (m.Code LIKE '%'+@Keyword+'%' OR m.[Name] LIKE N'%'+@Keyword+'%' OR m.Code_Map LIKE '%'+@Keyword+'%' OR m.OldNamesSummary LIKE N'%'+@Keyword+'%'))

          AND (@IsKeyOnly = 0 OR (@IsKeyOnly = 1 AND m.IsKeyProvince = 1))

        ORDER BY 

            CASE 

                WHEN @OrderByName = 'KeyFirst' AND m.IsKeyProvince = 1 THEN 0

                WHEN @OrderByName = 'KeyFirst' THEN 1

                ELSE 0 

            END,

            m.DisplayOrder ASC,

            CASE @OrderByName

                WHEN 'Name' THEN m.[Name]

                ELSE m.Code

            END ASC

        OFFSET ((@Page - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;

    END

END

GO



IF OBJECT_ID('User_Get_By_Page', 'P') IS NOT NULL DROP PROCEDURE [User_Get_By_Page];

GO

-- =============================================

-- Author: BVTL Team

-- Create date: 2026-10-05

-- Description: Cập nhật Stored Procedure User_Get_By_Page

-- Hỗ trợ phân trang, tìm kiếm từ khóa, và lọc tài khoản đã xóa mềm (@IsDeleted)

-- =============================================



CREATE   PROCEDURE [dbo].[User_Get_By_Page] 

	@Keyword NVARCHAR(250) = NULL,

	@OrderByName VARCHAR(100) = 'UserName',

	@Page INT = 1,

	@PageSize INT = 10,

	@IsDeleted BIT = 0

AS

BEGIN

	SET NOCOUNT ON;



	SELECT u.*,

	       q.Name AS RoleName,

	       COUNT(u.ID) OVER() AS TotalRow 

	FROM [dbo].[BVTL_QT_NGUOI_DUNG] u

	INNER JOIN [dbo].[BVTL_QT_QUYEN] q ON u.GroupID = q.ID

	WHERE (u.IsAdmin = 0 OR u.IsAdmin IS NULL)

	  AND (

	      (@IsDeleted = 1 AND u.IsActive = 0)

	      OR ((@IsDeleted = 0 OR @IsDeleted IS NULL) AND (u.IsActive = 1 OR u.IsActive IS NULL))

	  )

	  AND (

	      @Keyword IS NULL 

	      OR @Keyword = '' 

	      OR (

	          u.UserName LIKE '%' + @Keyword + '%' 

	          OR u.Name LIKE N'%' + @Keyword + '%' 

	          OR u.Phone LIKE N'%' + @Keyword + '%'

	          OR u.IdNumber LIKE '%' + @Keyword + '%'

	          OR q.Name LIKE N'%' + @Keyword + '%'

	      )

	  )

	ORDER BY CASE @OrderByName

		WHEN 'UserName' THEN u.UserName

		WHEN 'Name' THEN u.Name

		WHEN 'Phone' THEN u.Phone

		ELSE u.UserName

		END 

	OFFSET ((@Page - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;

END



GO



IF OBJECT_ID('NhomTBH_Get_By_Page', 'P') IS NOT NULL DROP PROCEDURE [NhomTBH_Get_By_Page];

GO



CREATE PROCEDURE [dbo].[NhomTBH_Get_By_Page] 

	@Keyword NVARCHAR(250) = NULL,

	@OrderByName VARCHAR(100) = 'manhom_tbh',

	@Page INT = 1,

	@PageSize INT = 10,

	@CityCode VARCHAR(100) = NULL

AS

BEGIN

	SET NOCOUNT ON;



	SELECT ace.*,

	       c.Name AS CityName,

	       COUNT(ace.manhom_tbh) OVER() AS TotalRow 

	FROM [dbo].[BVTL_NHOM_TBH] ace

	INNER JOIN [dbo].[BVTL_CITES] c ON c.Code = ace.city_code

	WHERE (ace.maduan = 'CD45' OR ace.maduan IS NULL)

	  AND (ace.maduan IS NULL OR ace.maduan NOT IN ('CH07'))

	  AND ISNULL(ace.IS_ACTIVE, 1) = 1

	  AND (

	      @CityCode IS NULL 

	      OR @CityCode = '' 

	      OR ace.city_code IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@CityCode, ','))

	  )

	  AND (

	      @Keyword IS NULL 

	      OR @Keyword = '' 

	      OR (

	          ace.manhom_tbh LIKE '%' + @Keyword + '%' 

	          OR ace.tennhom_tbh LIKE N'%' + @Keyword + '%' 

	          OR ace.PREFIX LIKE N'%' + @Keyword + '%'

	          OR ace.SHORT_PREFIX LIKE N'%' + @Keyword + '%'

	          OR c.Name LIKE N'%' + @Keyword + '%'

	      )

	  )

	ORDER BY CASE @OrderByName

		WHEN 'manhom_tbh' THEN ace.manhom_tbh

		WHEN 'tennhom_tbh' THEN ace.tennhom_tbh

		WHEN 'city_code' THEN ace.city_code

		WHEN 'PREFIX' THEN ace.PREFIX

		ELSE ace.manhom_tbh

		END 

	OFFSET ((@Page - 1) * @PageSize) ROWS FETCH NEXT @PageSize ROWS ONLY;

END



GO



