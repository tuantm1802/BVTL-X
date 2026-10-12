-- =========================================================================================
-- BỔ SUNG 17 TIẾP CẬN VIÊN (TCV) CỦA 3 NHÓM/DNXH TP. HỒ CHÍ MINH VÀO CD45_NHOM_TCV
-- Dự án: CD45 (DREAMH)
-- Ngày tạo: 12/10/2026
-- File encoding: UTF-8 with BOM (utf-8-sig)
-- =========================================================================================

USE BVTL_REPORTING_DEV;
GO

SET NOCOUNT ON;

-- 1. THE GATE (Mã nhóm: tg, Thành phố: HCM)
-- Tương ứng trường f2_person_tg trên REDCap
IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'tg' AND MA_TCV = '1')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('tg', N'The Gate', 'HCM', '1', N'Nguyễn Công Hậu', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'tg' AND MA_TCV = '2')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('tg', N'The Gate', 'HCM', '2', N'Lê Quốc Thắng', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'tg' AND MA_TCV = '3')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('tg', N'The Gate', 'HCM', '3', N'Ngô Tấn Huỳnh', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'tg' AND MA_TCV = '4')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('tg', N'The Gate', 'HCM', '4', N'Trần Đức Trọng', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

-- 2. G3VN (Mã nhóm: g3vn, Thành phố: HCM)
-- Tương ứng trường f2_person_g3 trên REDCap
IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'g3vn' AND MA_TCV = '1')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('g3vn', N'G3VN', 'HCM', '1', N'Phạm Hồng Sơn', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'g3vn' AND MA_TCV = '2')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('g3vn', N'G3VN', 'HCM', '2', N'Đỗ Thụy An My', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'g3vn' AND MA_TCV = '3')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('g3vn', N'G3VN', 'HCM', '3', N'Cao Trần Quốc Thái', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'g3vn' AND MA_TCV = '4')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('g3vn', N'G3VN', 'HCM', '4', N'Cao Trần Quốc Bình', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'g3vn' AND MA_TCV = '5')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('g3vn', N'G3VN', 'HCM', '5', N'Nguyễn Thành Lộc', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

-- 3. ALOCARE (Mã nhóm: alo, Thành phố: HCM)
-- Tương ứng trường f2_person_ac trên REDCap
IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'alo' AND MA_TCV = '1')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('alo', N'Alocare', 'HCM', '1', N'Nguyễn Minh Thuận', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'alo' AND MA_TCV = '2')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('alo', N'Alocare', 'HCM', '2', N'Lê Đặng Nghĩa', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'alo' AND MA_TCV = '3')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('alo', N'Alocare', 'HCM', '3', N'Đỗ Hoàng Duy', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'alo' AND MA_TCV = '4')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('alo', N'Alocare', 'HCM', '4', N'Trần Công Minh Đức', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'alo' AND MA_TCV = '5')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('alo', N'Alocare', 'HCM', '5', N'Chiêm Minh Tân', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'alo' AND MA_TCV = '6')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('alo', N'Alocare', 'HCM', '6', N'Nguyễn Thanh Khoa', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'alo' AND MA_TCV = '7')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('alo', N'Alocare', 'HCM', '7', N'Hồ Nguyễn Hoàng Kha', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

IF NOT EXISTS (SELECT 1 FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45' AND MA_NHOM = 'alo' AND MA_TCV = '8')
    INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
    VALUES ('alo', N'Alocare', 'HCM', '8', N'Phạm Ngọc Kỳ', 'CD45', 1, GETDATE(), N'Doanh nghiệp xã hội', N'DNXH', N'Tiếp cận viên');

PRINT N'Hoàn tất bổ sung 17 TCV của 3 nhóm TP.HCM vào bảng CD45_NHOM_TCV!';
GO
