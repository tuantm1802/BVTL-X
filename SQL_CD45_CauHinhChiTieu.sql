-- =============================================================
-- Script: Tạo table CD45_BCTIEU_CAU_HINH + Seed data từ Excel
-- =============================================================

IF OBJECT_ID('dbo.CD45_BCTIEU_CAU_HINH', 'U') IS NOT NULL
    DROP TABLE dbo.CD45_BCTIEU_CAU_HINH;
GO

CREATE TABLE dbo.CD45_BCTIEU_CAU_HINH (
    ID            INT IDENTITY(1,1) PRIMARY KEY,
    ChiTieuCode   NVARCHAR(50)  NOT NULL,
    ChiTieuName   NVARCHAR(500) NOT NULL,
    SectionCode   NVARCHAR(10)  NOT NULL,
    IsSection     BIT           NOT NULL DEFAULT 0,
    HienThi_Thang BIT           NOT NULL DEFAULT 0,
    HienThi_Quy   BIT           NOT NULL DEFAULT 0,
    HienThi_6T    BIT           NOT NULL DEFAULT 0,
    HienThi_12T   BIT           NOT NULL DEFAULT 0,
    SortOrder     INT           NOT NULL DEFAULT 0,
    IsActive      BIT           NOT NULL DEFAULT 1,
    UpdatedAt     DATETIME      DEFAULT GETDATE(),
    UpdatedBy     NVARCHAR(100) NULL
);
GO

-- Seed data từ file Excel: CD45_Ma_Tran_Cau_hinh_Chi_Tieu_Bao_Cao.xlsx
INSERT INTO dbo.CD45_BCTIEU_CAU_HINH
    (ChiTieuCode, ChiTieuName, SectionCode, IsSection, HienThi_Thang, HienThi_Quy, HienThi_6T, HienThi_12T, SortOrder, IsActive)
VALUES
    (N'SEC_I', N'THÔNG TIN CHUNG', 'I', 1, 0, 0, 0, 0, 10, 1),
    (N'I_1', N'Tổng số KH được chăm sóc từ đầu dự án', 'I', 0, 0, 1, 1, 1, 20, 1),
    (N'I_2', N'Tổng số KH được chăm sóc trong kỳ báo cáo', 'I', 0, 1, 1, 1, 1, 30, 1),
    (N'I_3', N'Số KH mất dấu trong kỳ báo cáo', 'I', 0, 0, 1, 1, 1, 40, 1),
    (N'SEC_II', N'HOẠT ĐỘNG TRUYỀN THÔNG', 'II', 1, 0, 0, 0, 0, 50, 1),
    (N'II_1', N'Số KH được tham gia truyền thông lần 1', 'II', 0, 1, 1, 1, 1, 60, 1),
    (N'II_2', N'Số KH được tham gia truyền thông lần 2', 'II', 0, 1, 1, 1, 1, 70, 1),
    (N'II_3', N'Tổng số lượt KH tham gia truyền thông', 'II', 0, 1, 1, 1, 1, 80, 1),
    (N'SEC_III', N'SÀNG LỌC BẢNG HỎI QST VÀ CHUYỂN GỬI KHÁM, ĐIỀU TRỊ SKTT', 'III', 1, 0, 0, 0, 0, 90, 1),
    (N'III_1', N'Số KH được sàng lọc bảng hỏi QST lần 1', 'III', 0, 1, 1, 1, 1, 100, 1),
    (N'III_1_M1', N'Kết quả QST lần 1 - Mức 1 (>=8)', 'III', 0, 1, 1, 1, 1, 110, 1),
    (N'III_1_M2', N'Kết quả QST lần 1 - Mức 2 (6-7)', 'III', 0, 1, 1, 1, 1, 120, 1),
    (N'III_1_M3', N'Kết quả QST lần 1 - Mức 3 (4-5)', 'III', 0, 1, 1, 1, 1, 130, 1),
    (N'III_1_M4', N'Kết quả QST lần 1 - Mức 4 (<4)', 'III', 0, 1, 1, 1, 1, 140, 1),
    (N'III_2', N'Số KH được sàng lọc lại bảng hỏi QST (từ lần 2 trở đi)', 'III', 0, 1, 1, 1, 1, 150, 1),
    (N'III_2_1', N'Kết quả QST lần 2+ - Mức 1 (>=8)', 'III', 0, 1, 1, 1, 1, 160, 1),
    (N'III_2_2', N'Kết quả QST lần 2+ - Mức 2 (6-7)', 'III', 0, 1, 1, 1, 1, 170, 1),
    (N'III_2_3', N'Kết quả QST lần 2+ - Mức 3 (4-5)', 'III', 0, 1, 1, 1, 1, 180, 1),
    (N'III_2_4', N'Kết quả QST lần 2+ - Mức 4 (<4)', 'III', 0, 1, 1, 1, 1, 190, 1),
    (N'III_3', N'Số lượt KH được chuyển gửi khám SKTT', 'III', 0, 1, 1, 1, 1, 200, 1),
    (N'III_4', N'Số KH được chuyển gửi khám SKTT, trong đó:', 'III', 0, 1, 1, 1, 1, 210, 1),
    (N'III_4_1', N'Số KH được chuyển gửi khám SKTT lần 1', 'III', 0, 1, 1, 1, 1, 220, 1),
    (N'III_4_2', N'Số KH được tái khám SKTT', 'III', 0, 1, 1, 1, 1, 230, 1),
    (N'III_4_3', N'Số lượt KH được tái khám SKTT', 'III', 0, 1, 1, 1, 1, 240, 1),
    (N'III_5', N'Số KH được điều trị nội trú (nhập viện)', 'III', 0, 1, 1, 1, 1, 250, 1),
    (N'III_6', N'Số KH được hỗ trợ mua thẻ Bảo hiểm y tế', 'III', 0, 1, 1, 1, 1, 260, 1),
    (N'SEC_IV', N'CÁC CAN THIỆP CÁ NHÂN VÀ CAN THIỆP NHÓM', 'IV', 1, 0, 0, 0, 0, 270, 1),
    (N'IV_1', N'Số lượt KH được tư vấn cá nhân', 'IV', 0, 1, 1, 1, 1, 280, 1),
    (N'IV_2', N'Số KH được tư vấn cá nhân, trong đó:', 'IV', 0, 1, 1, 1, 1, 290, 1),
    (N'IV_2_1', N'Số KH được tư vấn 1 lần', 'IV', 0, 0, 1, 1, 1, 300, 1),
    (N'IV_2_2', N'Số KH được tư vấn 2 lần', 'IV', 0, 0, 1, 1, 1, 310, 1),
    (N'IV_2_3', N'Số KH được tư vấn từ 3 lần trở lên', 'IV', 0, 0, 1, 1, 1, 320, 1),
    (N'IV_3', N'Số lượt KH được tham gia sinh hoạt nhóm', 'IV', 0, 1, 1, 1, 1, 330, 1),
    (N'IV_4', N'Số KH được tham gia sinh hoạt nhóm', 'IV', 0, 1, 1, 1, 1, 340, 1),
    (N'IV_5', N'Số lượt KH được tham gia can thiệp chữa lành', 'IV', 0, 1, 1, 1, 1, 350, 1),
    (N'IV_6', N'Số KH được tham gia can thiệp chữa lành', 'IV', 0, 1, 1, 1, 1, 360, 1),
    (N'SEC_V', N'CAN THIỆP ONLINE', 'V', 1, 0, 0, 0, 0, 370, 1),
    (N'V_1', N'Số lượt KH được can thiệp online', 'V', 0, 1, 1, 1, 1, 380, 1),
    (N'V_2', N'Số KH được can thiệp online', 'V', 0, 1, 1, 1, 1, 390, 1),
    (N'SEC_VI', N'DỊCH VỤ CHUYỂN GỬI KHÁC', 'VI', 1, 0, 0, 0, 0, 400, 1),
    (N'VI_1_PREP', N'Số KH được chuyển gửi dịch vụ/xét nghiệm thành công - PREP', 'VI', 0, 1, 1, 1, 1, 410, 1),
    (N'VI_1_PEP', N'Số KH được chuyển gửi dịch vụ/xét nghiệm thành công - PEP', 'VI', 0, 1, 1, 1, 1, 420, 1),
    (N'VI_1_STIS', N'Số KH được chuyển gửi - Khám/điều trị STIs', 'VI', 0, 1, 1, 1, 1, 430, 1),
    (N'VI_1_HEPATITIS', N'Số KH được chuyển gửi - XN/Điều trị Viêm gan B,C', 'VI', 0, 1, 1, 1, 1, 440, 1),
    (N'VI_1_METHADONE', N'Số KH được chuyển gửi - Methadone', 'VI', 0, 1, 1, 1, 1, 450, 1),
    (N'VI_1_OTHER', N'Số KH được chuyển gửi - Dịch vụ y tế khác', 'VI', 0, 1, 1, 1, 1, 460, 1),
    (N'SEC_VII', N'PHÁT TÀI LIỆU TRUYỀN THÔNG', 'VII', 1, 0, 0, 0, 0, 470, 1),
    (N'VII_1', N'Số quyển tài liệu đã phát', 'VII', 0, 1, 1, 1, 1, 480, 1),
    (N'VII_2', N'Số KH nhận tài liệu', 'VII', 0, 1, 1, 1, 1, 490, 1);
GO

-- Tạo index
CREATE UNIQUE INDEX IX_CD45_BCTIEU_Code ON dbo.CD45_BCTIEU_CAU_HINH (ChiTieuCode);
GO

-- Verify
SELECT ChiTieuCode, ChiTieuName, IsSection, HienThi_Thang, HienThi_Quy, HienThi_6T, HienThi_12T, SortOrder FROM dbo.CD45_BCTIEU_CAU_HINH ORDER BY SortOrder;