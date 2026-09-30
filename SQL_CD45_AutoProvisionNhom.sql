-- ==============================================================================
-- STORED PROCEDURE TỰ ĐỘNG KHỞI TẠO & ĐỒNG BỘ NHÓM DỰ PHÒNG TỪ REDCAP (CD45)
-- ==============================================================================
CREATE OR ALTER PROCEDURE [dbo].[SP_CD45_AutoProvisionNhom]
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
