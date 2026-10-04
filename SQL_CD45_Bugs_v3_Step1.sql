-- ==============================================================================
-- BƯỚC 1: CẤU HÌNH API F9, CHỈ TIÊU HIV MỤC VI, DỌN DẸP LOG F5 (LIST_BUGS_V3)
-- ==============================================================================

-- 1. Cấu hình API F9 vào bảng BVTL_API cho dự án CD45
IF NOT EXISTS (SELECT 1 FROM dbo.BVTL_API WHERE Api_Code = 'API_CD45_F9' AND maduan = 'CD45')
BEGIN
    INSERT INTO dbo.BVTL_API (
        Api_Code, NameSyncdata, HrefApi, TypeApi, TokenApi, TableNameSaveData, IsActive, ReportId, TimeReCall, maduan, RawOrLabel
    )
    VALUES (
        'API_CD45_F9', 
        N'F9 - Theo dấu khách hàng', 
        'https://rcap.scdi.org.vn/api/', 
        'json', 
        '7C4550FC8BD8032D06E99A32F173D1A8', 
        'CD45_THEO_DAU', 
        1, 
        '2508', 
        3600, 
        'CD45', 
        'raw'
    );
    PRINT N'Đã thêm cấu hình API_CD45_F9 vào bảng BVTL_API.';
END
ELSE
BEGIN
    UPDATE dbo.BVTL_API
    SET IsActive = 1, ReportId = '2508', TableNameSaveData = 'CD45_THEO_DAU'
    WHERE Api_Code = 'API_CD45_F9' AND maduan = 'CD45';
    PRINT N'Đã cập nhật cấu hình API_CD45_F9 trong bảng BVTL_API.';
END

-- 2. Thêm chỉ tiêu VI_1_HIV vào bảng CD45_BCTIEU_CAU_HINH (STT 45)
IF NOT EXISTS (SELECT 1 FROM dbo.CD45_BCTIEU_CAU_HINH WHERE ChiTieuCode = 'VI_1_HIV')
BEGIN
    INSERT INTO dbo.CD45_BCTIEU_CAU_HINH (
        ChiTieuCode, ChiTieuName, SectionCode, IsSection, 
        HienThi_Thang, HienThi_Quy, HienThi_6T, HienThi_12T, 
        Default_Thang, Default_Quy, Default_6T, Default_12T, 
        SortOrder, IsActive, UpdatedAt, UpdatedBy
    )
    VALUES (
        'VI_1_HIV', 
        N'Số KH được chuyển gửi - Tư vấn và xét nghiệm HIV', 
        'VI', 
        0, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        405, 1, GETDATE(), 'system'
    );
    PRINT N'Đã thêm chỉ tiêu VI_1_HIV vào bảng CD45_BCTIEU_CAU_HINH.';
END
ELSE
BEGIN
    UPDATE dbo.CD45_BCTIEU_CAU_HINH
    SET IsActive = 1, HienThi_Thang = 1, HienThi_Quy = 1, HienThi_6T = 1, HienThi_12T = 1,
        Default_Thang = 1, Default_Quy = 1, Default_6T = 1, Default_12T = 1
    WHERE ChiTieuCode = 'VI_1_HIV';
    PRINT N'Đã kích hoạt chỉ tiêu VI_1_HIV trong bảng CD45_BCTIEU_CAU_HINH.';
END

-- 3. Xóa bỏ các cảnh báo nghịch đảo thời gian đối với F5 (CD45_TUAN_THU) do quy tắc đã được bãi bỏ (Row 43)
DELETE FROM dbo.BVTL_DATA_STANDARDIZATION_LOG 
WHERE TABLE_NAME = 'CD45_TUAN_THU' AND RULE_CODE = 'ERR_CHRONOLOGICAL_INVERSION';
PRINT N'Đã dọn dẹp log ERR_CHRONOLOGICAL_INVERSION cho CD45_TUAN_THU.';
