using ClosedXML.Excel;
using Model.ModelExtend;
using System.Collections.Generic;

namespace WebApp.Services
{
    public static class ExcelTuVanHelper
    {
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
            ws.Range("A1:Q1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = $"Kỳ báo cáo: Từ {fromDate} đến {toDate}";
            ws.Range("A2:Q2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Italic = true;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

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
            ws.Cell("A3").Value = filterSubtitle;
            ws.Range("A3:Q3").Row(1).Merge();
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Headers row 5
            string[] headers = new[]
            {
                "STT",
                "Record ID",
                "Mã KH nghiên cứu",
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
                ws.Cell(5, i + 1).Value = headers[i];
            }

            var headerRange = ws.Range(5, 1, 5, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8ECEF");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Alignment.WrapText = true;
            ws.Row(5).Height = 30;

            int row = 6;
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

                    ws.Cell(row, 3).Value = item.MA_KH_NGHIEN_CUU ?? "";
                    ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 4).Value = item.CITY_NAME ?? item.CITY_CODE ?? "";
                    ws.Cell(row, 5).Value = item.TEN_NHOM ?? item.MA_NHOM ?? "";
                    ws.Cell(row, 6).Value = item.TEN_TCV ?? item.MA_TCV ?? "";

                    ws.Cell(row, 7).Value = item.NGAY_TU_VAN.HasValue ? item.NGAY_TU_VAN.Value.ToString("dd/MM/yyyy") : "";
                    ws.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 8).Value = item.DIA_DIEM_TEXT ?? "";

                    ws.Cell(row, 9).Value = item.Q3A_TEXT ?? "";
                    ws.Cell(row, 9).Style.Alignment.WrapText = true;

                    ws.Cell(row, 10).Value = item.Q3A_KHAC ?? "";
                    ws.Cell(row, 10).Style.Alignment.WrapText = true;

                    ws.Cell(row, 11).Value = item.Q3B_TEXT ?? "";
                    ws.Cell(row, 11).Style.Alignment.WrapText = true;

                    ws.Cell(row, 12).Value = item.Q3B_KHAC ?? "";
                    ws.Cell(row, 12).Style.Alignment.WrapText = true;

                    ws.Cell(row, 13).Value = item.Q3C_NHU_CAU ?? "";
                    ws.Cell(row, 13).Style.Alignment.WrapText = true;

                    ws.Cell(row, 14).Value = item.Q41_CAN_THIEP_GTH ?? "";
                    ws.Cell(row, 14).Style.Alignment.WrapText = true;

                    ws.Cell(row, 15).Value = item.Q42_NHU_CAU_TG ?? "";
                    ws.Cell(row, 15).Style.Alignment.WrapText = true;

                    ws.Cell(row, 16).Value = item.Q42_KHAC ?? "";
                    ws.Cell(row, 16).Style.Alignment.WrapText = true;

                    ws.Cell(row, 17).Value = item.NGAY_HEN_TIEP.HasValue ? item.NGAY_HEN_TIEP.Value.ToString("dd/MM/yyyy") : "";
                    ws.Cell(row, 17).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (int c = 1; c <= headers.Length; c++)
                    {
                        ws.Cell(row, c).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }

                    row++;
                }
            }

            int lastRow = row - 1;
            var tableRange = ws.Range(5, 1, lastRow, headers.Length);
            tableRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Column widths
            ws.Column(1).Width = 7;   // STT
            ws.Column(2).Width = 13;  // Record ID
            ws.Column(3).Width = 16;  // Ma KH
            ws.Column(4).Width = 16;  // Tinh
            ws.Column(5).Width = 18;  // Nhom
            ws.Column(6).Width = 20;  // TCV
            ws.Column(7).Width = 14;  // Ngay TV
            ws.Column(8).Width = 22;  // Dia diem
            ws.Column(9).Width = 28;  // 3a
            ws.Column(10).Width = 20; // 3a.1
            ws.Column(11).Width = 28; // 3b
            ws.Column(12).Width = 20; // 3b.1
            ws.Column(13).Width = 30; // 3c
            ws.Column(14).Width = 28; // 4.1
            ws.Column(15).Width = 28; // 4.2
            ws.Column(16).Width = 20; // 4.2.1
            ws.Column(17).Width = 14; // 6.1
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
            ws.Range("A1:X1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = $"Kỳ báo cáo: Từ {fromDate} đến {toDate}";
            ws.Range("A2:X2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Italic = true;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

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
            ws.Cell("A3").Value = filterSubtitle;
            ws.Range("A3:X3").Row(1).Merge();
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Headers row 5
            string[] headers = new[]
            {
                "STT",
                "Record ID",
                "Mã KH nghiên cứu",
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
                ws.Cell(5, i + 1).Value = headers[i];
            }

            var headerRange = ws.Range(5, 1, 5, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8ECEF");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Alignment.WrapText = true;
            ws.Row(5).Height = 30;

            int row = 6;
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

                    ws.Cell(row, 3).Value = item.MA_KH_NGHIEN_CUU ?? "";
                    ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 4).Value = item.LAN_TU_VAN ?? (item.REPEAT_INSTANCE.HasValue ? $"Lần {item.REPEAT_INSTANCE.Value + 1}" : "Lần 2");
                    ws.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 5).Value = item.CITY_NAME ?? item.CITY_CODE ?? "";
                    ws.Cell(row, 6).Value = item.TEN_NHOM ?? item.MA_NHOM ?? "";
                    ws.Cell(row, 7).Value = item.TEN_TCV ?? item.MA_TCV ?? "";

                    ws.Cell(row, 8).Value = item.NGAY_TU_VAN.HasValue ? item.NGAY_TU_VAN.Value.ToString("dd/MM/yyyy") : "";
                    ws.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(row, 9).Value = item.DIA_DIEM_TEXT ?? "";

                    ws.Cell(row, 10).Value = item.Q21A_CAM_THAY ?? "";
                    ws.Cell(row, 10).Style.Alignment.WrapText = true;

                    ws.Cell(row, 11).Value = item.Q21A_KHAC ?? "";
                    ws.Cell(row, 11).Style.Alignment.WrapText = true;

                    ws.Cell(row, 12).Value = item.Q21B_CHIA_SE ?? "";
                    ws.Cell(row, 12).Style.Alignment.WrapText = true;

                    ws.Cell(row, 13).Value = item.Q22_VAN_DE ?? "";
                    ws.Cell(row, 13).Style.Alignment.WrapText = true;

                    ws.Cell(row, 14).Value = item.Q31A_CAM_XUC ?? "";
                    ws.Cell(row, 14).Style.Alignment.WrapText = true;

                    ws.Cell(row, 15).Value = item.Q31B_CANG_THANG_NGAN ?? "";
                    ws.Cell(row, 15).Style.Alignment.WrapText = true;

                    ws.Cell(row, 16).Value = item.Q31C_CANG_THANG_DAI ?? "";
                    ws.Cell(row, 16).Style.Alignment.WrapText = true;

                    ws.Cell(row, 17).Value = item.Q31D_THOI_QUEN ?? "";
                    ws.Cell(row, 17).Style.Alignment.WrapText = true;

                    ws.Cell(row, 18).Value = item.Q31E_NOI_DUNG_KHAC ?? "";
                    ws.Cell(row, 18).Style.Alignment.WrapText = true;

                    ws.Cell(row, 19).Value = item.Q31E1_KHAC ?? "";
                    ws.Cell(row, 19).Style.Alignment.WrapText = true;

                    ws.Cell(row, 20).Value = item.Q32_CAN_THIEP_CHINH ?? "";
                    ws.Cell(row, 20).Style.Alignment.WrapText = true;

                    ws.Cell(row, 21).Value = item.Q41_DONG_Y ?? "";
                    ws.Cell(row, 21).Style.Alignment.WrapText = true;

                    ws.Cell(row, 22).Value = item.Q421_VIEC_LAM ?? "";
                    ws.Cell(row, 22).Style.Alignment.WrapText = true;

                    ws.Cell(row, 23).Value = item.Q422_TAN_SUAT ?? "";
                    ws.Cell(row, 23).Style.Alignment.WrapText = true;

                    ws.Cell(row, 24).Value = item.NGAY_HEN_TIEP.HasValue ? item.NGAY_HEN_TIEP.Value.ToString("dd/MM/yyyy") : "";
                    ws.Cell(row, 24).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (int c = 1; c <= headers.Length; c++)
                    {
                        ws.Cell(row, c).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }

                    row++;
                }
            }

            int lastRow = row - 1;
            var tableRange = ws.Range(5, 1, lastRow, headers.Length);
            tableRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Column widths
            ws.Column(1).Width = 7;   // STT
            ws.Column(2).Width = 13;  // Record ID
            ws.Column(3).Width = 16;  // Ma KH
            ws.Column(4).Width = 13;  // Lan TV
            ws.Column(5).Width = 16;  // Tinh
            ws.Column(6).Width = 18;  // Nhom
            ws.Column(7).Width = 20;  // TCV
            ws.Column(8).Width = 14;  // Ngay TV
            ws.Column(9).Width = 22;  // Dia diem
            ws.Column(10).Width = 25; // 2.1.a
            ws.Column(11).Width = 20; // 2.1.a.1
            ws.Column(12).Width = 30; // 2.1.b
            ws.Column(13).Width = 30; // 2.2
            ws.Column(14).Width = 25; // 3.1.a
            ws.Column(15).Width = 25; // 3.1.b
            ws.Column(16).Width = 25; // 3.1.c
            ws.Column(17).Width = 25; // 3.1.d
            ws.Column(18).Width = 25; // 3.1.e
            ws.Column(19).Width = 20; // 3.1.e.1
            ws.Column(20).Width = 30; // 3.2
            ws.Column(21).Width = 28; // 4.1
            ws.Column(22).Width = 25; // 4.2.1
            ws.Column(23).Width = 20; // 4.2.2
            ws.Column(24).Width = 14; // Ngay hen
        }
    }
}
