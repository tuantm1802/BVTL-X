using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
    public class ExportExcel
    {
        public Stream CreateExcelFile<T>(string linkFileTemplate, List<T> lsData, int rowStart, int columnStart)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Template File
            string templateDocument = HttpContext.Current.Server.MapPath(linkFileTemplate);

            // Results Output
            MemoryStream outputStream = new MemoryStream();

            // Read Template
            using (FileStream templateDocumentStream = File.OpenRead(templateDocument))
            {
                using (var excelPackage = new ExcelPackage(templateDocumentStream))
                {
                    // Tạo author cho file Excel
                    excelPackage.Workbook.Properties.Author = "APP";
                    // Tạo title cho file Excel
                    excelPackage.Workbook.Properties.Title = "Data App";
                    // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                    var workSheet = excelPackage.Workbook.Worksheets[0];

                    // Đổ data vào Excel file
                    workSheet.Cells[rowStart, columnStart].LoadFromCollection(lsData, false);

                    // BindingFormatForExcel(workSheet, list);
                    excelPackage.SaveAs(outputStream);
                    return outputStream;
                }
            }
        }
    }
}
