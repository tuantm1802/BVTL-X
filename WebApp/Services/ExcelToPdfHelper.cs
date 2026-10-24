using ClosedXML.Excel;
using Spire.Xls;
using System;
using System.IO;

namespace WebApp.Services
{
    /// <summary>
    /// Helper chuyển đổi bảng tính Excel (ClosedXML hoặc Stream) sang định dạng PDF chuẩn A4
    /// </summary>
    public static class ExcelToPdfHelper
    {
        /// <summary>
        /// Chuyển đổi trực tiếp một đối tượng XLWorkbook của ClosedXML sang mảng byte PDF
        /// Giữ nguyên toàn bộ cấu hình trang in A4 (Portrait/Landscape, Margins, FitToPages)
        /// </summary>
        public static byte[] ConvertClosedXmlToPdf(XLWorkbook workbook)
        {
            if (workbook == null) return null;

            using (var xlsxStream = new MemoryStream())
            {
                workbook.SaveAs(xlsxStream);
                xlsxStream.Position = 0;
                return ConvertXlsxStreamToPdf(xlsxStream);
            }
        }

        /// <summary>
        /// Chuyển đổi từ luồng (Stream) chứa dữ liệu XLSX sang mảng byte PDF
        /// </summary>
        public static byte[] ConvertXlsxStreamToPdf(Stream xlsxStream)
        {
            if (xlsxStream == null) return null;

            var spireWorkbook = new Workbook();
            spireWorkbook.LoadFromStream(xlsxStream);

            using (var pdfStream = new MemoryStream())
            {
                spireWorkbook.SaveToStream(pdfStream, FileFormat.PDF);
                return pdfStream.ToArray();
            }
        }

        /// <summary>
        /// Chuyển đổi file XLSX trên đĩa sang file PDF
        /// </summary>
        public static void ConvertXlsxFileToPdf(string xlsxPath, string pdfPath)
        {
            if (string.IsNullOrWhiteSpace(xlsxPath) || !File.Exists(xlsxPath))
            {
                throw new FileNotFoundException("Tệp Excel không tồn tại: " + xlsxPath);
            }

            var spireWorkbook = new Workbook();
            spireWorkbook.LoadFromFile(xlsxPath);

            string directory = Path.GetDirectoryName(pdfPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            spireWorkbook.SaveToFile(pdfPath, FileFormat.PDF);
        }
    }
}
