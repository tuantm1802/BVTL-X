-- ============================================================================
-- Script: SQL_Add_FileFormat_To_ExportedReportLog.sql
-- Mục đích: Bổ sung cột FileFormat vào bảng BVTL_EXPORTED_REPORT_LOG
--           để phân biệt rõ ràng giữa định dạng EXCEL (.xlsx / .zip) và PDF (.pdf / .zip).
--           Giúp lưu trữ và quản lý độc lập 2 định dạng cùng kỳ/loại báo cáo mà không bị ghi đè.
-- ============================================================================

IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[BVTL_EXPORTED_REPORT_LOG]') 
      AND name = 'FileFormat'
)
BEGIN
    ALTER TABLE [dbo].[BVTL_EXPORTED_REPORT_LOG] 
    ADD [FileFormat] VARCHAR(20) NULL CONSTRAINT DF_BVTL_EXPORTED_REPORT_LOG_FileFormat DEFAULT ('EXCEL');
    
    PRINT N'Đã bổ sung cột FileFormat vào bảng BVTL_EXPORTED_REPORT_LOG thành công!';
END
ELSE
BEGIN
    PRINT N'Cột FileFormat đã tồn tại trong bảng BVTL_EXPORTED_REPORT_LOG.';
END
GO

-- Chuẩn hóa dữ liệu lịch sử dựa trên phần mở rộng hoặc hậu tố tên file
UPDATE [dbo].[BVTL_EXPORTED_REPORT_LOG]
SET [FileFormat] = CASE 
    WHEN [FileName] LIKE '%_PDF.zip' OR [FileName] LIKE '%.pdf' THEN 'PDF' 
    ELSE 'EXCEL' 
END
WHERE [FileFormat] IS NULL OR [FileFormat] = '';

PRINT N'Đã cập nhật chuẩn hóa dữ liệu FileFormat cho các bản ghi lịch sử thành công!';
GO
