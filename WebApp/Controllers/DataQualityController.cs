using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using Data.Admin;
using Data.InterfaceDA;
using Model.ModelExtend.API.CD45;

namespace WebApp.Controllers
{
    public class DataQualityController : BaseController
    {
        private readonly IDataQualityDA _dataQualityDA = new DataQualityDA();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetLogs(string maDuAn, string apiCode, string severity, string keyword, bool? isResolved, int pageIndex = 1, int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrEmpty(maDuAn)) maDuAn = "CD45";
                int totalRows = 0;
                var logs = _dataQualityDA.GetLogs(maDuAn, apiCode, severity, keyword, isResolved, pageIndex, pageSize, out totalRows);
                return Json(new { Success = true, Data = logs, TotalRows = totalRows, PageIndex = pageIndex, PageSize = pageSize });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetStats(string maDuAn)
        {
            try
            {
                if (string.IsNullOrEmpty(maDuAn)) maDuAn = "CD45";
                var stats = _dataQualityDA.GetStats(maDuAn);
                return Json(new { Success = true, Stats = stats });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ResolveLog(long id, string note)
        {
            try
            {
                bool ok = _dataQualityDA.MarkResolved(id, note);
                return Json(new { Success = ok });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetGroupedLogs(string maDuAn, string severity)
        {
            try
            {
                if (string.IsNullOrEmpty(maDuAn)) maDuAn = "CD45";
                var list = _dataQualityDA.GetGroupedLogs(maDuAn, severity);
                return Json(new { Success = true, Data = list });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetStatsByNhom(string maDuAn)
        {
            try
            {
                if (string.IsNullOrEmpty(maDuAn)) maDuAn = "CD45";
                var list = _dataQualityDA.GetStatsByNhom(maDuAn);
                return Json(new { Success = true, Data = list });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetLogsByUnit(string maDuAn, string cityCode, string maNhom, string metricType)
        {
            try
            {
                if (string.IsNullOrEmpty(maDuAn)) maDuAn = "CD45";
                var logs = _dataQualityDA.GetLogsByUnit(maDuAn, cityCode, maNhom, metricType);
                return Json(new { Success = true, Data = logs, TotalCount = logs.Count });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ScanDuplicates(string maDuAn)
        {
            try
            {
                if (string.IsNullOrEmpty(maDuAn)) maDuAn = "CD45";
                int count = _dataQualityDA.ScanDuplicateClients(maDuAn);
                return Json(new { Success = true, Count = count, Message = $"Đã hoàn tất quét trùng lặp F1. Ghi nhận {count} bản ghi nghi vấn trùng hồ sơ." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ExportExcel(string tabType, string maDuAn, string apiCode, string severity, string keyword, string isResolved)
        {
            try
            {
                if (string.IsNullOrEmpty(maDuAn)) maDuAn = "CD45";
                tabType = (tabType ?? "details").ToLower();

                bool? resolvedBool = null;
                if (!string.IsNullOrEmpty(isResolved))
                {
                    if (string.Equals(isResolved, "true", StringComparison.OrdinalIgnoreCase)) resolvedBool = true;
                    else if (string.Equals(isResolved, "false", StringComparison.OrdinalIgnoreCase)) resolvedBool = false;
                }

                using (var workbook = new XLWorkbook())
                {
                    string fileName = "";

                    if (tabType == "grouped")
                    {
                        var list = _dataQualityDA.GetGroupedLogs(maDuAn, severity);
                        var ws = workbook.Worksheets.Add("GomNhom_QuyTac");
                        string subtitle = $"Thời gian xuất: {DateTime.Now:dd/MM/yyyy HH:mm} | Bộ lọc mức độ: {(string.IsNullOrEmpty(severity) ? "Tất cả" : severity)} | Tổng số quy tắc vi phạm: {list.Count}";
                        BuildSheetGrouped(ws, list, subtitle);
                        fileName = $"GomNhom_QuyTac_CD45_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    }
                    else if (tabType == "byunit")
                    {
                        var list = _dataQualityDA.GetStatsByNhom(maDuAn);
                        var ws = workbook.Worksheets.Add("ThongKe_DonVi");
                        string subtitle = $"Thời gian xuất: {DateTime.Now:dd/MM/yyyy HH:mm} | Dự án: {maDuAn} | Tổng số đơn vị (Tỉnh/CBO): {list.Count}";
                        BuildSheetByUnit(ws, list, subtitle);
                        fileName = $"ThongKe_DonVi_CD45_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    }
                    else if (tabType == "multi")
                    {
                        // 1. Sheet Thống kê theo Đơn vị (Cấp quản trị đơn vị)
                        var unitList = _dataQualityDA.GetStatsByNhom(maDuAn);
                        var wsUnit = workbook.Worksheets.Add("ThongKe_DonVi");
                        BuildSheetByUnit(wsUnit, unitList, $"Thời gian xuất: {DateTime.Now:dd/MM/yyyy HH:mm} | Dự án: {maDuAn} | Tổng số đơn vị (Tỉnh/CBO): {unitList.Count}");

                        // 2. Sheet Gom nhóm theo Quy tắc (Cấp phân loại quy tắc)
                        var grpList = _dataQualityDA.GetGroupedLogs(maDuAn, null);
                        var wsGrp = workbook.Worksheets.Add("GomNhom_QuyTac");
                        BuildSheetGrouped(wsGrp, grpList, $"Thời gian xuất: {DateTime.Now:dd/MM/yyyy HH:mm} | Dự án: {maDuAn} | Tổng số quy tắc vi phạm: {grpList.Count}");

                        // 3. Sheet Chi tiết Sự kiện & Cảnh báo (Cấp chi tiết bản ghi)
                        int totalRows = 0;
                        var logs = _dataQualityDA.GetLogs(maDuAn, apiCode, severity, keyword, resolvedBool, 1, 10000, out totalRows);
                        var wsDetails = workbook.Worksheets.Add("ChiTiet_CanhBao");
                        string subtitle = $"Thời gian xuất: {DateTime.Now:dd/MM/yyyy HH:mm} | Tổng số bản ghi chi tiết: {logs.Count}";
                        BuildSheetDetails(wsDetails, logs, subtitle);

                        fileName = $"BaoCao_ChatLuongDuLieu_CD45_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    }
                    else if (tabType == "warnings")
                    {
                        int totalRows = 0;
                        var logs = _dataQualityDA.GetLogs(maDuAn, apiCode, "WARNING", null, false, 1, 10000, out totalRows);
                        var ws = workbook.Worksheets.Add("CanhBaoDuLieu");
                        string subtitle = $"Thời gian xuất: {DateTime.Now:dd/MM/yyyy HH:mm} | Danh sách cảnh báo chưa xử lý (cho M&E) | Tổng số: {logs.Count}";
                        BuildSheetDetails(ws, logs, subtitle);
                        fileName = $"CanhBao_REDCap_CD45_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    }
                    else // "details"
                    {
                        int totalRows = 0;
                        var logs = _dataQualityDA.GetLogs(maDuAn, apiCode, severity, keyword, resolvedBool, 1, 10000, out totalRows);
                        var ws = workbook.Worksheets.Add("ChiTiet_SuKien");
                        string filterDesc = $"Biểu mẫu: {(string.IsNullOrEmpty(apiCode) ? "Tất cả" : apiCode)}, Mức độ: {(string.IsNullOrEmpty(severity) ? "Tất cả" : severity)}, Trạng thái: {(resolvedBool.HasValue ? (resolvedBool.Value ? "Đã sửa" : "Chưa sửa") : "Tất cả")}";
                        if (!string.IsNullOrEmpty(keyword)) filterDesc += $", Từ khóa: '{keyword}'";
                        string subtitle = $"Thời gian xuất: {DateTime.Now:dd/MM/yyyy HH:mm} | Bộ lọc: [{filterDesc}] | Tổng số bản ghi: {logs.Count}";
                        BuildSheetDetails(ws, logs, subtitle);
                        fileName = $"NhatKy_ChiTiet_CD45_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi xuất file Excel: " + ex.Message);
            }
        }

        [HttpGet]
        public ActionResult ExportExcelWarnings(string maDuAn, string apiCode)
        {
            return ExportExcel("warnings", maDuAn, apiCode, null, null, "false");
        }

        private void BuildSheetDetails(IXLWorksheet ws, List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, string subtitle)
        {
            ws.Cell("A1").Value = "NHẬT KÝ CHI TIẾT SỰ KIỆN CHUẨN HÓA & CẢNH BÁO DỮ LIỆU REDCAP (DỰ ÁN CD45)";
            ws.Range("A1:L1").Merge().Style.Font.Bold = true;
            ws.Range("A1:L1").Style.Font.FontSize = 14;
            ws.Range("A1:L1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = subtitle;
            ws.Range("A2:L2").Merge().Style.Font.Italic = true;

            string[] headers = {
                "#", "Thời gian", "API / Form", "Mã KH (record_id)", "Tỉnh", "Nhóm",
                "Trường dữ liệu", "Giá trị REDCap", "Sau chuẩn hóa", "Mức độ",
                "Mã quy tắc & Nội dung cảnh báo", "Trạng thái xử lý"
            };

            for (int c = 0; c < headers.Length; c++)
            {
                var cell = ws.Cell(4, c + 1);
                cell.Value = headers[c];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4E73DF");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int row = 5;
            for (int i = 0; i < logs.Count; i++)
            {
                var item = logs[i];
                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 2).Value = item.CREATED_DATE.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 3).Value = item.API_CODE;
                ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 4).Value = item.RECORD_ID;
                ws.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 4).Style.Font.Bold = true;
                ws.Cell(row, 5).Value = item.CITY_CODE ?? "-";
                ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 6).Value = item.MA_NHOM ?? "-";
                ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 7).Value = item.FIELD_NAME;
                ws.Cell(row, 8).Value = item.OLD_VALUE;
                ws.Cell(row, 9).Value = item.NEW_VALUE;
                ws.Cell(row, 10).Value = item.SEVERITY;
                ws.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                string ruleAndMsg = string.IsNullOrEmpty(item.RULE_CODE) ? item.MESSAGE : $"[{item.RULE_CODE}] {item.MESSAGE}";
                ws.Cell(row, 11).Value = ruleAndMsg;

                string status = item.IS_RESOLVED ? (string.IsNullOrEmpty(item.RESOLVED_NOTE) ? "Đã xử lý" : $"Đã xử lý: {item.RESOLVED_NOTE}") : "Chưa xử lý (Pending)";
                ws.Cell(row, 12).Value = status;
                if (item.IS_RESOLVED)
                {
                    ws.Cell(row, 12).Style.Font.FontColor = XLColor.FromHtml("#1CC88A");
                }
                else
                {
                    ws.Cell(row, 12).Style.Font.FontColor = XLColor.FromHtml("#E74A3B");
                    ws.Cell(row, 12).Style.Font.Bold = true;
                }

                if (item.SEVERITY == "ERROR")
                {
                    ws.Range(row, 1, row, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#FDF2F2");
                }
                else if (item.SEVERITY == "WARNING")
                {
                    ws.Range(row, 1, row, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#FEF9E7");
                }

                row++;
            }

            ws.Columns().AdjustToContents();
        }

        private void BuildSheetGrouped(IXLWorksheet ws, List<GroupedDataQualityLogModel> list, string subtitle)
        {
            ws.Cell("A1").Value = "BẢNG TỔNG HỢP VI PHẠM THEO MÃ QUY TẮC CHUẨN HÓA DỮ LIỆU REDCAP (DỰ ÁN CD45)";
            ws.Range("A1:K1").Merge().Style.Font.Bold = true;
            ws.Range("A1:K1").Style.Font.FontSize = 14;
            ws.Range("A1:K1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = subtitle;
            ws.Range("A2:K2").Merge().Style.Font.Italic = true;

            string[] headers = {
                "#", "Bảng dữ liệu", "Mã Quy tắc", "Mức độ",
                "Tổng phát hiện", "Chưa xử lý (Pending)", "Đã xử lý (Resolved)", "Tỷ lệ hoàn thành",
                "Phát hiện đầu", "Phát hiện cuối", "Thông điệp cảnh báo mẫu"
            };

            for (int c = 0; c < headers.Length; c++)
            {
                var cell = ws.Cell(4, c + 1);
                cell.Value = headers[c];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4E73DF");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int row = 5;
            int sumTotal = 0;
            int sumPending = 0;
            int sumResolved = 0;

            for (int i = 0; i < list.Count; i++)
            {
                var grp = list[i];
                int unresolved = grp.SEVERITY == "INFO" ? 0 : grp.UnresolvedCount;
                int resolved = grp.SEVERITY == "INFO" ? grp.TotalCount : Math.Max(0, grp.TotalCount - grp.UnresolvedCount);
                double rate = grp.TotalCount > 0 ? (double)resolved / grp.TotalCount * 100 : 100;

                sumTotal += grp.TotalCount;
                sumPending += unresolved;
                sumResolved += resolved;

                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 2).Value = string.IsNullOrEmpty(grp.TABLE_NAME) ? "TOÀN CỤC" : grp.TABLE_NAME;
                ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 3).Value = grp.RULE_CODE;
                ws.Cell(row, 3).Style.Font.Bold = true;
                ws.Cell(row, 4).Value = grp.SEVERITY;
                ws.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(row, 5).Value = grp.TotalCount;
                ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0";

                ws.Cell(row, 6).Value = unresolved;
                ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
                if (unresolved > 0)
                {
                    ws.Cell(row, 6).Style.Font.FontColor = XLColor.FromHtml("#E74A3B");
                    ws.Cell(row, 6).Style.Font.Bold = true;
                }

                ws.Cell(row, 7).Value = resolved;
                ws.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0";
                ws.Cell(row, 7).Style.Font.FontColor = XLColor.FromHtml("#1CC88A");

                ws.Cell(row, 8).Value = $"{rate:F1}%";
                ws.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(row, 9).Value = grp.FirstSeen.HasValue ? grp.FirstSeen.Value.ToString("dd/MM/yyyy HH:mm") : "-";
                ws.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(row, 10).Value = grp.LastSeen.HasValue ? grp.LastSeen.Value.ToString("dd/MM/yyyy HH:mm") : "-";
                ws.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(row, 11).Value = grp.SampleMessage;

                row++;
            }

            // Dòng tổng cộng
            ws.Cell(row, 1).Value = "TỔNG CỘNG";
            ws.Range(row, 1, row, 4).Merge();
            ws.Range(row, 1, row, 4).Style.Font.Bold = true;
            ws.Range(row, 1, row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(row, 5).Value = sumTotal;
            ws.Cell(row, 5).Style.Font.Bold = true;
            ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0";

            ws.Cell(row, 6).Value = sumPending;
            ws.Cell(row, 6).Style.Font.Bold = true;
            ws.Cell(row, 6).Style.Font.FontColor = XLColor.FromHtml("#E74A3B");
            ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0";

            ws.Cell(row, 7).Value = sumResolved;
            ws.Cell(row, 7).Style.Font.Bold = true;
            ws.Cell(row, 7).Style.Font.FontColor = XLColor.FromHtml("#1CC88A");
            ws.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0";

            double totalRate = sumTotal > 0 ? (double)sumResolved / sumTotal * 100 : 100;
            ws.Cell(row, 8).Value = $"{totalRate:F1}%";
            ws.Cell(row, 8).Style.Font.Bold = true;
            ws.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Range(row, 1, row, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#EAECF4");
            ws.Range(row, 1, row, 11).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range(row, 1, row, 11).Style.Border.BottomBorder = XLBorderStyleValues.Double;

            ws.Columns().AdjustToContents();
        }

        private void BuildSheetByUnit(IXLWorksheet ws, List<DataQualityStatsByNhomModel> list, string subtitle)
        {
            ws.Cell("A1").Value = "BẢNG THỐNG KÊ CHẤT LƯỢNG DỮ LIỆU REDCAP THEO ĐƠN VỊ TỈNH / NHÓM CBO (DỰ ÁN CD45)";
            ws.Range("A1:K1").Merge().Style.Font.Bold = true;
            ws.Range("A1:K1").Style.Font.FontSize = 14;
            ws.Range("A1:K1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = subtitle;
            ws.Range("A2:K2").Merge().Style.Font.Italic = true;

            string[] headers = {
                "#", "Tỉnh / Thành phố", "Mã Tỉnh", "Nhóm CBO", "Mã Nhóm",
                "Cần xử lý (Pending)", "Tổng Cảnh báo (Warning)", "Tổng Lỗi chặn (Error)",
                "Đã khắc phục (Resolved)", "Tổng sự kiện", "Đánh giá rủi ro chất lượng"
            };

            for (int c = 0; c < headers.Length; c++)
            {
                var cell = ws.Cell(4, c + 1);
                cell.Value = headers[c];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4E73DF");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int row = 5;
            int sumPending = 0;
            int sumWarnings = 0;
            int sumErrors = 0;
            int sumResolved = 0;
            int sumTotal = 0;

            for (int i = 0; i < list.Count; i++)
            {
                var u = list[i];
                int unitTotal = u.TotalPending + u.TotalResolved;
                sumPending += u.TotalPending;
                sumWarnings += u.TotalWarnings;
                sumErrors += u.TotalErrors;
                sumResolved += u.TotalResolved;
                sumTotal += unitTotal;

                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(row, 2).Value = string.IsNullOrEmpty(u.TEN_TINH) ? (u.CITY_CODE ?? "-") : u.TEN_TINH;
                ws.Cell(row, 2).Style.Font.Bold = true;

                ws.Cell(row, 3).Value = u.CITY_CODE ?? "-";
                ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(row, 4).Value = string.IsNullOrEmpty(u.TEN_NHOM) ? (u.MA_NHOM ?? "CHƯA PHÂN NHÓM") : u.TEN_NHOM;
                ws.Cell(row, 4).Style.Font.Bold = true;
                ws.Cell(row, 4).Style.Font.FontColor = XLColor.FromHtml("#4E73DF");

                ws.Cell(row, 5).Value = u.MA_NHOM ?? "-";
                ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(row, 6).Value = u.TotalPending;
                ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
                if (u.TotalPending > 0)
                {
                    ws.Cell(row, 6).Style.Font.FontColor = XLColor.FromHtml("#E74A3B");
                    ws.Cell(row, 6).Style.Font.Bold = true;
                }

                ws.Cell(row, 7).Value = u.TotalWarnings;
                ws.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0";

                ws.Cell(row, 8).Value = u.TotalErrors;
                ws.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(row, 8).Style.NumberFormat.Format = "#,##0";
                if (u.TotalErrors > 0)
                {
                    ws.Cell(row, 8).Style.Font.FontColor = XLColor.FromHtml("#E74A3B");
                }

                ws.Cell(row, 9).Value = u.TotalResolved;
                ws.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(row, 9).Style.NumberFormat.Format = "#,##0";
                ws.Cell(row, 9).Style.Font.FontColor = XLColor.FromHtml("#1CC88A");

                ws.Cell(row, 10).Value = unitTotal;
                ws.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(row, 10).Style.NumberFormat.Format = "#,##0";

                string risk = u.TotalPending > 20 ? "Rủi ro cao: Dữ liệu nguồn sai sót nhiều" :
                              u.TotalPending > 0 ? "Rủi ro trung bình: Cần rà soát bổ sung" :
                              "Dữ liệu sạch / Đã xử lý hoàn tất";
                ws.Cell(row, 11).Value = risk;

                row++;
            }

            // Dòng tổng cộng
            ws.Cell(row, 1).Value = "TỔNG CỘNG";
            ws.Range(row, 1, row, 5).Merge();
            ws.Range(row, 1, row, 5).Style.Font.Bold = true;
            ws.Range(row, 1, row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(row, 6).Value = sumPending;
            ws.Cell(row, 6).Style.Font.Bold = true;
            ws.Cell(row, 6).Style.Font.FontColor = XLColor.FromHtml("#E74A3B");
            ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0";

            ws.Cell(row, 7).Value = sumWarnings;
            ws.Cell(row, 7).Style.Font.Bold = true;
            ws.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0";

            ws.Cell(row, 8).Value = sumErrors;
            ws.Cell(row, 8).Style.Font.Bold = true;
            ws.Cell(row, 8).Style.Font.FontColor = XLColor.FromHtml("#E74A3B");
            ws.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(row, 8).Style.NumberFormat.Format = "#,##0";

            ws.Cell(row, 9).Value = sumResolved;
            ws.Cell(row, 9).Style.Font.Bold = true;
            ws.Cell(row, 9).Style.Font.FontColor = XLColor.FromHtml("#1CC88A");
            ws.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(row, 9).Style.NumberFormat.Format = "#,##0";

            ws.Cell(row, 10).Value = sumTotal;
            ws.Cell(row, 10).Style.Font.Bold = true;
            ws.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(row, 10).Style.NumberFormat.Format = "#,##0";

            ws.Range(row, 1, row, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#EAECF4");
            ws.Range(row, 1, row, 11).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range(row, 1, row, 11).Style.Border.BottomBorder = XLBorderStyleValues.Double;

            ws.Columns().AdjustToContents();
        }

    }
}
