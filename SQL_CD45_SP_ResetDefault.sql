CREATE OR ALTER PROC dbo.SP_CD45_ResetCauHinhChiTieuMacDinh
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
