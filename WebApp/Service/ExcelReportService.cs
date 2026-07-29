using ClosedXML.Excel;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebApp.Service
{
    public class ExcelReportService : IExcelReportService
    {
        public byte[] ExportReport(
            List<BaoCaoModel> data, 
            string titleReport, 
            string sheetName, 
            UserLogin user, 
            string tenNhomTBHs, 
            string baseFileName,
            out string fileName
        )
        {
            fileName = baseFileName + ".xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(sheetName);
                CreateHeader(ws, titleReport, user, tenNhomTBHs);

                var columnName = "";
                var columnNumber = 0;
                var row = 6;

                if (data != null && data.Any())
                {
                    foreach (var rowReport in data)
                    {
                        if (rowReport.IsShow == "Y")
                        {
                            // STT Column
                            InsertDataCell(ws, "A", row, rowReport.STT, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                            if (!string.IsNullOrEmpty(rowReport.ThongTinBC))
                            {
                                // ThongTinBC Column
                                InsertDataCell(ws, "B", row, rowReport.ThongTinBC, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                if (rowReport.Colpan > 1)
                                {
                                    columnNumber = ExcelColumnNameToNumber("B");
                                    columnName = GetExcelColumnName(columnNumber + (int)rowReport.Colpan);
                                    ws.Range("B" + row + ":" + columnName + row).Column(1).Merge();
                                }
                                else
                                {
                                    // ThongTinBC_Them Column
                                    InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                }

                                if (rowReport.Rowpan > 1)
                                {
                                    ws.Range("B" + row + ":" + "B" + (row + rowReport.Rowpan - 1)).Merge();
                                }
                            }
                            else
                            {
                                // ThongTinBC_Them Column
                                InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                            }

                            // Tong Column
                            InsertDataCell(ws, "D", row, rowReport.Tong > 0 ? rowReport.Tong.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Tong > 0 ? true : false);

                            // MSM Column
                            InsertDataCell(ws, "E", row, rowReport.MSM > 0 ? rowReport.MSM.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.MSM > 0 ? true : false);

                            // PUD Column
                            InsertDataCell(ws, "F", row, rowReport.PUD > 0 ? rowReport.PUD.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.PUD > 0 ? true : false);

                            // SW Column
                            InsertDataCell(ws, "G", row, rowReport.SW > 0 ? rowReport.SW.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.SW > 0 ? true : false);

                            // Nam Column
                            InsertDataCell(ws, "H", row, rowReport.Nam > 0 ? rowReport.Nam.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nam > 0 ? true : false);

                            // Nu Column
                            InsertDataCell(ws, "I", row, rowReport.Nu > 0 ? rowReport.Nu.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nu > 0 ? true : false);

                            // ChuyenGioi Column
                            InsertDataCell(ws, "J", row, rowReport.ChuyenGioi > 0 ? rowReport.ChuyenGioi.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.ChuyenGioi > 0 ? true : false);

                            row++;
                        }
                    }
                }

                CreateFooter(ws, titleReport, user, tenNhomTBHs);

                ws.Range("A5:J" + row).Style.Font.FontName = "Times New Roman";
                ws.Range("A5:J" + row).Style.Font.FontSize = 13;
                ws.Range("A5:J" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A5:J" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A5:J" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A5:J" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                using (MemoryStream stream = new MemoryStream())
                {
                    ws.Columns(1, 30).AdjustToContents();
                    ws.Column("B").Width = 50;
                    ws.Column("C").Width = 20;
                    ws.Column("D").Width = 20;
                    ws.Column("E").Width = 20;
                    ws.Column("F").Width = 20;
                    ws.Column("G").Width = 20;
                    ws.Column("H").Width = 20;
                    ws.Column("I").Width = 20;
                    ws.Column("J").Width = 20;
                    ws.Column("A").Style.Alignment.SetWrapText(true);
                    ws.Column("B").Style.Alignment.SetWrapText(true);
                    ws.Column("C").Style.Alignment.SetWrapText(true);
                    ws.Column("D").Style.Alignment.SetWrapText(true);
                    ws.Column("E").Style.Alignment.SetWrapText(true);
                    ws.Column("F").Style.Alignment.SetWrapText(true);
                    ws.Column("G").Style.Alignment.SetWrapText(true);
                    ws.Column("H").Style.Alignment.SetWrapText(true);
                    ws.Column("I").Style.Alignment.SetWrapText(true);
                    ws.Column("J").Style.Alignment.SetWrapText(true);
                    wb.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }

        private int ExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");
            columnName = columnName.ToUpperInvariant();
            int sum = 0;
            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }
            return sum;
        }

        private void InsertDataCell(
            IXLWorksheet ws, 
            string cellName, 
            int row, 
            string value, 
            bool bold,
            XLAlignmentHorizontalValues horizontal, 
            XLAlignmentVerticalValues vertical, 
            bool isNumber = true
        )
        {
            if (isNumber == true && string.IsNullOrEmpty(value))
                value = "0";

            ws.Cell(cellName + row).Value = isNumber == false ? (object)value : Convert.ToDecimal(value);
            ws.Cell(cellName + row).Style.Font.Bold = bold;
            ws.Cell(cellName + row).Style.Alignment.Horizontal = horizontal;
            ws.Cell(cellName + row).Style.Alignment.Vertical = vertical;
            ws.Cell(cellName + row).Style.Alignment.WrapText = true;
            if (isNumber)
                ws.Cell(cellName + row).Style.NumberFormat.Format = "#,##0";
        }

        private void CreateHeader(IXLWorksheet ws, string tileReport, UserLogin user, string tenNhomTBHs)
        {
            ws.Cell("A1").Value = "BÁO CÁO HOẠT ĐỘNG";
            ws.Range("A1:J1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A1").Style.Font.FontName = "Times New Roman";
            ws.Cell("A1").Style.Font.FontSize = 13;

            ws.Cell("A2").Value = tileReport;
            ws.Range("A2:J2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A2").Style.Font.FontName = "Times New Roman";
            ws.Cell("A2").Style.Font.FontSize = 13;

            ws.Cell("A3").Value = tenNhomTBHs;
            ws.Range("A3:J3").Row(1).Merge();
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A3").Style.Font.FontName = "Times New Roman";
            ws.Cell("A3").Style.Font.FontSize = 13;

            var row = 5;

            ws.Cell("A" + row).Value = "#";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Thông tin báo cáo";
            ws.Range("B" + row + ":C" + row).Merge();
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Tổng";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "MSM";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.WrapText = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "PUD";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.WrapText = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("G" + row).Value = "SW";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Style.Alignment.WrapText = true;
            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("H" + row).Value = "Nam";
            ws.Cell("H" + row).Style.Font.Bold = true;
            ws.Cell("H" + row).Style.Alignment.WrapText = true;
            ws.Cell("H" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("I" + row).Value = "Nữ";
            ws.Cell("I" + row).Style.Font.Bold = true;
            ws.Cell("I" + row).Style.Alignment.WrapText = true;
            ws.Cell("I" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("I" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("J" + row).Value = "Chuyển giới";
            ws.Cell("J" + row).Style.Font.Bold = true;
            ws.Cell("J" + row).Style.Alignment.WrapText = true;
            ws.Cell("J" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("J" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private void CreateFooter(IXLWorksheet ws, string tileReport, UserLogin user, string tenNhomTBHs)
        {
            ws.Cell("B60").Value = "Trưởng nhóm";
            ws.Cell("B60").Style.Font.Bold = true;
            ws.Cell("B60").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B60").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("B60").Style.Font.FontName = "Times New Roman";
            ws.Cell("B60").Style.Font.FontSize = 13;

            ws.Cell("C60").Value = "Cán bộ dự án";
            ws.Range("C60:D60").Row(1).Merge();
            ws.Cell("C60").Style.Font.Bold = true;
            ws.Cell("C60").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C60").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("C60").Style.Font.FontName = "Times New Roman";
            ws.Cell("C60").Style.Font.FontSize = 13;

            ws.Cell("F60").Value = "Quản lý chương trình";
            ws.Range("F60:I60").Row(1).Merge();
            ws.Cell("F60").Style.Font.Bold = true;
            ws.Cell("F60").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F60").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("F60").Style.Font.FontName = "Times New Roman";
            ws.Cell("F60").Style.Font.FontSize = 13;
        }
    }
}
