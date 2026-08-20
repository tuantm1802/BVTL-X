USE BVTL_REPORTING_DEV;
GO

IF OBJECT_ID('SP_CD45_GetBaoCaoTCV', 'P') IS NOT NULL DROP PROC SP_CD45_GetBaoCaoTCV;
GO
CREATE PROC SP_CD45_GetBaoCaoTCV
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @MaTCV VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Tương tự SP_CD45_GetBaoCao nhưng lọc thêm theo Mã TCV (dựa vào biến MA_TCV trong bảng CD45_NHOM_TCV nếu cần, hoặc mã TCV được ghi nhận từ F2_PERSON_*)
    -- Tạm thời trả về format mẫu để Controller có thể bind dữ liệu ra Excel
    DECLARE @TmpResult TABLE (
        STT_Sort INT IDENTITY(1,1), STT VARCHAR(10), ChiTieu NVARCHAR(500),
        Tong INT DEFAULT 0, PUD INT DEFAULT 0, PLHIV INT DEFAULT 0, TG INT DEFAULT 0, SW INT DEFAULT 0, MSM INT DEFAULT 0, IsBold BIT DEFAULT 0
    );

    INSERT INTO @TmpResult(STT, ChiTieu, IsBold) VALUES ('I', N'THÔNG TIN CHUNG', 1);
    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD) VALUES ('1', N'Tổng số KH được chăm sóc từ đầu dự án', 0, 0);
    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD) VALUES ('2', N'Tổng số KH được chăm sóc trong kỳ báo cáo', 0, 0);

    INSERT INTO @TmpResult(STT, ChiTieu, IsBold) VALUES ('II', N'HOẠT ĐỘNG TRUYỀN THÔNG', 1);
    INSERT INTO @TmpResult(STT, ChiTieu, Tong, PUD) VALUES ('1', N'Số KH được tham gia truyền thông lần 1', 0, 0);
    
    SELECT STT, ChiTieu, Tong, PUD, PLHIV, TG, SW, MSM, IsBold FROM @TmpResult ORDER BY STT_Sort;
END
GO
