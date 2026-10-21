using ClosedXML.Excel;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace WebApp.Services
{
    public static class ExcelTuVanHelper
    {
        /// <summary>
        /// Định dạng chuỗi tháng báo cáo: "Báo cáo tháng MM/yyyy"
        /// Ưu tiên lấy theo month/year nếu hợp lệ, sau đó parse từ toDate hoặc fromDate.
        /// </summary>
        public static string FormatReportMonth(string toDate, string fromDate = null, int? year = null, int? month = null)
        {
            if (year.HasValue && month.HasValue && year.Value > 1900 && month.Value >= 1 && month.Value <= 12)
            {
                return $"Báo cáo tháng {month.Value:D2}/{year.Value}";
            }

            string[] formats = new[] { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "yyyy/MM/dd", "dd.MM.yyyy" };
            DateTime dt;
            if (!string.IsNullOrWhiteSpace(toDate) && DateTime.TryParseExact(toDate.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return $"Báo cáo tháng {dt:MM/yyyy}";
            }
            if (!string.IsNullOrWhiteSpace(fromDate) && DateTime.TryParseExact(fromDate.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return $"Báo cáo tháng {dt:MM/yyyy}";
            }
            return $"Báo cáo tháng {DateTime.Now:MM/yyyy}";
        }

        public static void BuildTuVanL1Worksheet(
            IXLWorksheet ws,
            List<CD45BaoCaoTuVanL1Model> data,
            string fromDate,
            string toDate,
            string tenNhom,
            string tenTCV,
            string xungDanh = "Nhóm")
        {
            ws.Cell("A1").Value = "BÁO CÁO CHI TIẾT TƯ VẤN LẦN 1 (F7) - DỰ ÁN CD45";
            ws.Range("A1:N1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = FormatReportMonth(toDate, fromDate);
            ws.Range("A2:N2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 12;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A3").Value = $"Kỳ báo cáo: Từ {fromDate} đến {toDate}";
            ws.Range("A3:N3").Row(1).Merge();
            ws.Cell("A3").Style.Font.Italic = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            string prefixText = !string.IsNullOrWhiteSpace(xungDanh) ? xungDanh.Trim() : "Nhóm";
            string filterSubtitle = "";
            if (!string.IsNullOrWhiteSpace(tenNhom) || !string.IsNullOrWhiteSpace(tenTCV))
            {
                filterSubtitle = $"{prefixText}: {tenNhom ?? "Tất cả"} | Tiếp cận viên: {tenTCV ?? "Tất cả"}";
            }
            else
            {
                filterSubtitle = "Tất cả Nhóm & Tiếp cận viên";
            }
            ws.Cell("A4").Value = filterSubtitle;
            ws.Range("A4:N4").Row(1).Merge();
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Headers row 6 (14 cột: đã bỏ 3b f7_q3b và 3b.1 f7_q3b_1)
            string[] headers = new[]
            {
                "STT",
                "Record ID",
                "Tỉnh / Thành phố",
                "Nhóm",
                "TCV tư vấn",
                "Ngày tư vấn",
                "Địa điểm",
                "3a. Tình trạng SKTT hiện tại",
                "3a.1 Khác",
                "3c. Nhu cầu hỗ trợ chính (KH tự xác định)",
                "4.1. Các can thiệp đã được giới thiệu",
                "4.2. Nhu cầu tham gia của khách hàng",
                "4.2.1 Khác",
                "6.1 Ngày hẹn tiếp theo"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(6, i + 1).Value = headers[i];
            }

            var headerRange = ws.Range(6, 1, 6, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8ECEF");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Alignment.WrapText = true;
            ws.Row(6).Height = 30;

            int row = 7;
            if (data == null || data.Count == 0)
            {
                ws.Cell(row, 1).Value = "Không có ca tư vấn nào trong kỳ báo cáo";
                ws.Range(row, 1, row, headers.Length).Row(1).Merge();
                ws.Cell(row, 1).Style.Font.Italic = true;
                ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                row++;
            }
            else
            {
                int stt = 1;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = stt++;
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 2).Value = item.RECORD_ID ?? "";
                    ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 3).Value = item.CITY_NAME ?? item.CITY_CODE ?? "";
                    ws.Cell(row, 4).Value = item.TEN_NHOM ?? item.MA_NHOM ?? "";
                    ws.Cell(row, 5).Value = item.TEN_TCV ?? item.MA_TCV ?? "";

                    ws.Cell(row, 6).Value = item.NGAY_TU_VAN.HasValue ? item.NGAY_TU_VAN.Value.ToString("dd/MM/yyyy") : "";
                    ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 7).Value = item.DIA_DIEM_TEXT ?? "";

                    ws.Cell(row, 8).Value = item.Q3A_TEXT ?? "";
                    ws.Cell(row, 8).Style.Alignment.WrapText = true;

                    ws.Cell(row, 9).Value = item.Q3A_KHAC ?? "";
                    ws.Cell(row, 9).Style.Alignment.WrapText = true;

                    ws.Cell(row, 10).Value = item.Q3C_NHU_CAU ?? "";
                    ws.Cell(row, 10).Style.Alignment.WrapText = true;

                    ws.Cell(row, 11).Value = item.Q41_CAN_THIEP_GTH ?? "";
                    ws.Cell(row, 11).Style.Alignment.WrapText = true;

                    ws.Cell(row, 12).Value = item.Q42_NHU_CAU_TG ?? "";
                    ws.Cell(row, 12).Style.Alignment.WrapText = true;

                    ws.Cell(row, 13).Value = item.Q42_KHAC ?? "";
                    ws.Cell(row, 13).Style.Alignment.WrapText = true;

                    ws.Cell(row, 14).Value = item.NGAY_HEN_TIEP.HasValue ? item.NGAY_HEN_TIEP.Value.ToString("dd/MM/yyyy") : "";
                    ws.Cell(row, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (int c = 1; c <= headers.Length; c++)
                    {
                        ws.Cell(row, c).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }

                    row++;
                }
            }

            int lastRow = row - 1;
            var tableRange = ws.Range(6, 1, lastRow, headers.Length);
            tableRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Column widths (tối ưu để in vừa trang ngang A4)
            ws.Column(1).Width = 6;   // STT
            ws.Column(2).Width = 12;  // Record ID
            ws.Column(3).Width = 14;  // Tinh
            ws.Column(4).Width = 16;  // Nhom
            ws.Column(5).Width = 18;  // TCV
            ws.Column(6).Width = 13;  // Ngay TV
            ws.Column(7).Width = 18;  // Dia diem
            ws.Column(8).Width = 26;  // 3a
            ws.Column(9).Width = 18;  // 3a.1
            ws.Column(10).Width = 26; // 3c
            ws.Column(11).Width = 24; // 4.1
            ws.Column(12).Width = 24; // 4.2
            ws.Column(13).Width = 18; // 4.2.1
            ws.Column(14).Width = 13; // 6.1

            // Cấu hình in vừa 1 trang ngang
            ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
            ws.PageSetup.FitToPages(1, 0);
        }

        public static void BuildTuVanL2Worksheet(
            IXLWorksheet ws,
            List<CD45BaoCaoTuVanL2Model> data,
            string fromDate,
            string toDate,
            string tenNhom,
            string tenTCV,
            string xungDanh = "Nhóm")
        {
            ws.Cell("A1").Value = "BÁO CÁO CHI TIẾT TƯ VẤN TỪ LẦN 2 TRỞ ĐI (F8) - DỰ ÁN CD45";
            ws.Range("A1:N1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = FormatReportMonth(toDate, fromDate);
            ws.Range("A2:N2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 12;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A3").Value = $"Kỳ báo cáo: Từ {fromDate} đến {toDate}";
            ws.Range("A3:N3").Row(1).Merge();
            ws.Cell("A3").Style.Font.Italic = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            string prefixText = !string.IsNullOrWhiteSpace(xungDanh) ? xungDanh.Trim() : "Nhóm";
            string filterSubtitle = "";
            if (!string.IsNullOrWhiteSpace(tenNhom) || !string.IsNullOrWhiteSpace(tenTCV))
            {
                filterSubtitle = $"{prefixText}: {tenNhom ?? "Tất cả"} | Tiếp cận viên: {tenTCV ?? "Tất cả"}";
            }
            else
            {
                filterSubtitle = "Tất cả Nhóm & Tiếp cận viên";
            }
            ws.Cell("A4").Value = filterSubtitle;
            ws.Range("A4:N4").Row(1).Merge();
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Headers row 6 (14 cột: đã bỏ 2.1.a.1, 3.1.a..e1, 4.2.1, 4.2.2)
            string[] headers = new[]
            {
                "STT",
                "Record ID",
                "Lần tư vấn",
                "Tỉnh / Thành phố",
                "Nhóm",
                "TCV tư vấn",
                "Ngày tư vấn",
                "Địa điểm",
                "2.1.a Dạo gần đây KH cảm thấy:",
                "2.1.b KH chia sẻ ngắn",
                "2.2. Vấn đề KH muốn tập trung",
                "3.2. Nội dung can thiệp chính (ghi ngắn)",
                "4.1. Can thiệp KH đồng ý áp dụng",
                "Ngày hẹn tiếp theo"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(6, i + 1).Value = headers[i];
            }

            var headerRange = ws.Range(6, 1, 6, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8ECEF");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Alignment.WrapText = true;
            ws.Row(6).Height = 30;

            int row = 7;
            if (data == null || data.Count == 0)
            {
                ws.Cell(row, 1).Value = "Không có ca tư vấn nào trong kỳ báo cáo";
                ws.Range(row, 1, row, headers.Length).Row(1).Merge();
                ws.Cell(row, 1).Style.Font.Italic = true;
                ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                row++;
            }
            else
            {
                int stt = 1;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = stt++;
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 2).Value = item.RECORD_ID ?? "";
                    ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 3).Value = item.LAN_TU_VAN ?? (item.REPEAT_INSTANCE.HasValue ? $"Lần {item.REPEAT_INSTANCE.Value + 1}" : "Lần 2");
                    ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 4).Value = item.CITY_NAME ?? item.CITY_CODE ?? "";
                    ws.Cell(row, 5).Value = item.TEN_NHOM ?? item.MA_NHOM ?? "";
                    ws.Cell(row, 6).Value = item.TEN_TCV ?? item.MA_TCV ?? "";

                    ws.Cell(row, 7).Value = item.NGAY_TU_VAN.HasValue ? item.NGAY_TU_VAN.Value.ToString("dd/MM/yyyy") : "";
                    ws.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 8).Value = item.DIA_DIEM_TEXT ?? "";

                    ws.Cell(row, 9).Value = item.Q21A_CAM_THAY ?? "";
                    ws.Cell(row, 9).Style.Alignment.WrapText = true;

                    ws.Cell(row, 10).Value = item.Q21B_CHIA_SE ?? "";
                    ws.Cell(row, 10).Style.Alignment.WrapText = true;

                    ws.Cell(row, 11).Value = item.Q22_VAN_DE ?? "";
                    ws.Cell(row, 11).Style.Alignment.WrapText = true;

                    ws.Cell(row, 12).Value = item.Q32_CAN_THIEP_CHINH ?? "";
                    ws.Cell(row, 12).Style.Alignment.WrapText = true;

                    ws.Cell(row, 13).Value = item.Q41_DONG_Y ?? "";
                    ws.Cell(row, 13).Style.Alignment.WrapText = true;

                    ws.Cell(row, 14).Value = item.NGAY_HEN_TIEP.HasValue ? item.NGAY_HEN_TIEP.Value.ToString("dd/MM/yyyy") : "";
                    ws.Cell(row, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (int c = 1; c <= headers.Length; c++)
                    {
                        ws.Cell(row, c).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }

                    row++;
                }
            }

            int lastRow = row - 1;
            var tableRange = ws.Range(6, 1, lastRow, headers.Length);
            tableRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Column widths (tối ưu để in vừa trang ngang A4)
            ws.Column(1).Width = 6;   // STT
            ws.Column(2).Width = 12;  // Record ID
            ws.Column(3).Width = 11;  // Lan TV
            ws.Column(4).Width = 14;  // Tinh
            ws.Column(5).Width = 16;  // Nhom
            ws.Column(6).Width = 18;  // TCV
            ws.Column(7).Width = 13;  // Ngay TV
            ws.Column(8).Width = 18;  // Dia diem
            ws.Column(9).Width = 26;  // 2.1.a
            ws.Column(10).Width = 26; // 2.1.b
            ws.Column(11).Width = 26; // 2.2
            ws.Column(12).Width = 26; // 3.2
            ws.Column(13).Width = 26; // 4.1
            ws.Column(14).Width = 13; // Ngay hen tiep

            // Cấu hình in vừa 1 trang ngang
            ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
            ws.PageSetup.FitToPages(1, 0);
        }
    }
}