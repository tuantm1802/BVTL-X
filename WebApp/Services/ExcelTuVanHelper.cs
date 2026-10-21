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
            ws.Range("A1:P1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = FormatReportMonth(toDate, fromDate);
            ws.Range("A2:P2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 12;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A3").Value = $"Kỳ báo cáo: Từ {fromDate} đến {toDate}";
            ws.Range("A3:P3").Row(1).Merge();
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
            ws.Range("A4:P4").Row(1).Merge();
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Headers row 6 (16 cột, đã bỏ cột Mã KH)
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
                "3b. Các yếu tố liên quan",
                "3b.1 Khác",
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

                    ws.Cell(row, 10).Value = item.Q3B_TEXT ?? "";
                    ws.Cell(row, 10).Style.Alignment.WrapText = true;

                    ws.Cell(row, 11).Value = item.Q3B_KHAC ?? "";
                    ws.Cell(row, 11).Style.Alignment.WrapText = true;

                    ws.Cell(row, 12).Value = item.Q3C_NHU_CAU ?? "";
                    ws.Cell(row, 12).Style.Alignment.WrapText = true;

                    ws.Cell(row, 13).Value = item.Q41_CAN_THIEP_GTH ?? "";
                    ws.Cell(row, 13).Style.Alignment.WrapText = true;

                    ws.Cell(row, 14).Value = item.Q42_NHU_CAU_TG ?? "";
                    ws.Cell(row, 14).Style.Alignment.WrapText = true;

                    ws.Cell(row, 15).Value = item.Q42_KHAC ?? "";
                    ws.Cell(row, 15).Style.Alignment.WrapText = true;

                    ws.Cell(row, 16).Value = item.NGAY_HEN_TIEP.HasValue ? item.NGAY_HEN_TIEP.Value.ToString("dd/MM/yyyy") : "";
                    ws.Cell(row, 16).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

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

            // Column widths
            ws.Column(1).Width = 7;   // STT
            ws.Column(2).Width = 13;  // Record ID
            ws.Column(3).Width = 16;  // Tinh
            ws.Column(4).Width = 18;  // Nhom
            ws.Column(5).Width = 20;  // TCV
            ws.Column(6).Width = 14;  // Ngay TV
            ws.Column(7).Width = 22;  // Dia diem
            ws.Column(8).Width = 28;  // 3a
            ws.Column(9).Width = 20;  // 3a.1
            ws.Column(10).Width = 28; // 3b
            ws.Column(11).Width = 20; // 3b.1
            ws.Column(12).Width = 30; // 3c
            ws.Column(13).Width = 28; // 4.1
            ws.Column(14).Width = 28; // 4.2
            ws.Column(15).Width = 20; // 4.2.1
            ws.Column(16).Width = 14; // 6.1
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
            ws.Range("A1:W1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = FormatReportMonth(toDate, fromDate);
            ws.Range("A2:W2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 12;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A3").Value = $"Kỳ báo cáo: Từ {fromDate} đến {toDate}";
            ws.Range("A3:W3").Row(1).Merge();
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
            ws.Range("A4:W4").Row(1).Merge();
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Headers row 6 (23 cột, đã bỏ cột Mã KH)
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
                "2.1.a.1 Khác",
                "2.1.b KH chia sẻ ngắn",
                "2.2. Vấn đề KH muốn tập trung",
                "3.1.a Nhận diện cảm xúc",
                "3.1.b Quản lý căng thẳng ngắn hạn",
                "3.1.c Quản lý căng thẳng dài hạn",
                "3.1.d Thói quen sinh hoạt",
                "3.1.e Nội dung khác",
                "3.1.e.1 Khác",
                "3.2. Nội dung can thiệp chính (ghi ngắn)",
                "4.1. Can thiệp KH đồng ý áp dụng",
                "4.2.1 Việc sẽ làm",
                "4.2.2 Tần suất",
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

                    ws.Cell(row, 10).Value = item.Q21A_KHAC ?? "";
                    ws.Cell(row, 10).Style.Alignment.WrapText = true;

                    ws.Cell(row, 11).Value = item.Q21B_CHIA_SE ?? "";
                    ws.Cell(row, 11).Style.Alignment.WrapText = true;

                    ws.Cell(row, 12).Value = item.Q22_VAN_DE ?? "";
                    ws.Cell(row, 12).Style.Alignment.WrapText = true;

                    ws.Cell(row, 13).Value = item.Q31A_CAM_XUC ?? "";
                    ws.Cell(row, 13).Style.Alignment.WrapText = true;

                    ws.Cell(row, 14).Value = item.Q31B_CANG_THANG_NGAN ?? "";
                    ws.Cell(row, 14).Style.Alignment.WrapText = true;

                    ws.Cell(row, 15).Value = item.Q31C_CANG_THANG_DAI ?? "";
                    ws.Cell(row, 15).Style.Alignment.WrapText = true;

                    ws.Cell(row, 16).Value = item.Q31D_THOI_QUEN ?? "";
                    ws.Cell(row, 16).Style.Alignment.WrapText = true;

                    ws.Cell(row, 17).Value = item.Q31E_NOI_DUNG_KHAC ?? "";
                    ws.Cell(row, 17).Style.Alignment.WrapText = true;

                    ws.Cell(row, 18).Value = item.Q31E1_KHAC ?? "";
                    ws.Cell(row, 18).Style.Alignment.WrapText = true;

                    ws.Cell(row, 19).Value = item.Q32_CAN_THIEP_CHINH ?? "";
                    ws.Cell(row, 19).Style.Alignment.WrapText = true;

                    ws.Cell(row, 20).Value = item.Q41_DONG_Y ?? "";
                    ws.Cell(row, 20).Style.Alignment.WrapText = true;

                    ws.Cell(row, 21).Value = item.Q421_VIEC_LAM ?? "";
                    ws.Cell(row, 21).Style.Alignment.WrapText = true;

                    ws.Cell(row, 22).Value = item.Q422_TAN_SUAT ?? "";
                    ws.Cell(row, 22).Style.Alignment.WrapText = true;

                    ws.Cell(row, 23).Value = item.NGAY_HEN_TIEP.HasValue ? item.NGAY_HEN_TIEP.Value.ToString("dd/MM/yyyy") : "";
                    ws.Cell(row, 23).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

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

            // Column widths
            ws.Column(1).Width = 7;   // STT
            ws.Column(2).Width = 13;  // Record ID
            ws.Column(3).Width = 13;  // Lan TV
            ws.Column(4).Width = 16;  // Tinh
            ws.Column(5).Width = 18;  // Nhom
            ws.Column(6).Width = 20;  // TCV
            ws.Column(7).Width = 14;  // Ngay TV
            ws.Column(8).Width = 22;  // Dia diem
            ws.Column(9).Width = 25;  // 2.1.a
            ws.Column(10).Width = 20; // 2.1.a.1
            ws.Column(11).Width = 30; // 2.1.b
            ws.Column(12).Width = 30; // 2.2
            ws.Column(13).Width = 25; // 3.1.a
            ws.Column(14).Width = 25; // 3.1.b
            ws.Column(15).Width = 25; // 3.1.c
            ws.Column(16).Width = 25; // 3.1.d
            ws.Column(17).Width = 25; // 3.1.e
            ws.Column(18).Width = 20; // 3.1.e.1
            ws.Column(19).Width = 30; // 3.2
            ws.Column(20).Width = 28; // 4.1
            ws.Column(21).Width = 25; // 4.2.1
            ws.Column(22).Width = 20; // 4.2.2
            ws.Column(23).Width = 14; // Ngay hen
        }
    }
}
