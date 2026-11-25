using ClosedXML.Excel;
using Common;
using Common.Common;
using Data.InterfaceDA.Admin;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class BaoCaoChiSoCD45Controller : BaseController
    {
        private readonly ICityDA _CityDA;
        private readonly IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA;
        private readonly IBaoCaoChiSoCD45DA _BaoCaoChiSoCD45DA;

        public BaoCaoChiSoCD45Controller(
            ICityDA cityDA,
            IBVTL_NHOM_TBHDA bvtlNhomTbhDA,
            IBaoCaoChiSoCD45DA baoCaoChiSoCD45DA)
        {
            _CityDA = cityDA;
            _BVTL_NHOM_TBHDA = bvtlNhomTbhDA;
            _BaoCaoChiSoCD45DA = baoCaoChiSoCD45DA;
        }

        [HasCredential(ControllerName = "BaoCaoChiSoCD45")]
        public ActionResult Index()
        {
            var user = Session["USER_SESSION"] as UserLogin;
            if (user == null) return Redirect("/Login/Index");
            ViewBag.IsAdmin = IsCurrentUserAdmin();
            return View();
        }

        [HttpPost]
        public JsonResult GetFilterData(string cityMode = "NEW34")
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                if (user == null) return Json(new { Success = false, Message = "Chưa đăng nhập" });

                var allowedCodes = GetUserAllowedCityCodes();
                bool isAdmin = IsCurrentUserAdmin();

                object cities;
                if (string.Equals(cityMode, "OLD63", StringComparison.OrdinalIgnoreCase))
                {
                    var allOld = _CityDA.GetAll();
                    if (!isAdmin)
                    {
                        allOld = allOld.Where(x => allowedCodes.Contains(x.Code)).ToList();
                    }
                    cities = allOld.OrderByDescending(x => !string.IsNullOrEmpty(x.Code_Map))
                                   .ThenBy(x => x.Name)
                                   .Select(x => new
                                   {
                                       CityCode = x.Code,
                                       CityName = (!string.IsNullOrEmpty(x.Code_Map) ? "⭐ " : "") + x.Name + (!string.IsNullOrEmpty(x.Code_Map) ? " (" + x.Code_Map + ")" : ""),
                                       IsKey = !string.IsNullOrEmpty(x.Code_Map),
                                       OldCodes = new[] { x.Code }
                                   })
                                   .ToList();
                }
                else
                {
                    var newCities = _CityDA.GetAllNewCities();
                    var mappings = _CityDA.GetCityMappings();
                    var mapGroup = mappings.GroupBy(m => m.NewCityCode).ToDictionary(g => g.Key, g => g.Select(m => m.OldCityCode).ToArray(), StringComparer.OrdinalIgnoreCase);

                    if (!isAdmin)
                    {
                        newCities = newCities.Where(x => allowedCodes.Contains(x.Code) ||
                            (mapGroup.ContainsKey(x.Code) && mapGroup[x.Code].Any(old => allowedCodes.Contains(old)))).ToList();
                    }

                    cities = newCities.OrderByDescending(x => x.IsKeyProvince)
                                      .ThenBy(x => x.DisplayOrder)
                                      .ThenBy(x => x.Name)
                                      .Select(x => new
                                      {
                                          CityCode = x.Code,
                                          CityName = (x.IsKeyProvince ? "⭐ " : "") + x.Name + (x.OldCount > 1 ? string.Format(" ({0} tỉnh gộp)", x.OldCount) : ""),
                                          IsKey = x.IsKeyProvince,
                                          OldCodes = mapGroup.ContainsKey(x.Code) ? mapGroup[x.Code] : new[] { x.Code }
                                      })
                                      .ToList();
                }

                var nhomsQuery = _BVTL_NHOM_TBHDA.GetAll().Where(x => x.maduan == "CD45");
                if (!isAdmin)
                {
                    nhomsQuery = nhomsQuery.Where(x => !string.IsNullOrEmpty(x.city_code) && allowedCodes.Contains(x.city_code.Trim()));
                }
                var nhoms = nhomsQuery.OrderBy(x => x.city_code).ThenBy(x => x.tennhom_tbh).ToList();

                return Json(new { Success = true, Cities = cities, Nhoms = nhoms, IsAdmin = isAdmin });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SearchBaoCao(string FromDate, string ToDate, string MaTinh, string MaNhom, bool IsCumulative = true, int? Nam = null, string MaKy = "12T")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(FromDate) || !string.IsNullOrWhiteSpace(ToDate))
                {
                    if (!ValidateDateRange(FromDate, ToDate, out var dateError))
                    {
                        return Json(new { Success = false, Message = dateError });
                    }
                }

                string scopedCity = ScopeCityCodeFilter(MaTinh);
                if (scopedCity == "__NO_ACCESS__")
                {
                    return Json(new { Success = true, Data = new List<BaoCaoChiSoCD45Model>(), Warning = (string)null });
                }

                var data = _BaoCaoChiSoCD45DA.GetBaoCaoChiSo(FromDate, ToDate, scopedCity, MaNhom, IsCumulative, Nam, MaKy);

                ReportValidatorHelper.ValidateChiSoReportArithmetic(data, out string arithmeticWarning);

                var jsonResult = Json(new { Success = true, Data = data, Warning = arithmeticWarning });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetDrillDownData(string ChiTieuCode, string FromDate, string ToDate, string MaTinh, string MaNhom, bool IsCumulative = true, int? DoiTuong = null, int? GioiTinh = null, int? NhomTuoi = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(FromDate) || !string.IsNullOrWhiteSpace(ToDate))
                {
                    if (!ValidateDateRange(FromDate, ToDate, out var dateError))
                    {
                        return Json(new { Success = false, Message = dateError });
                    }
                }

                string scopedCity = ScopeCityCodeFilter(MaTinh);
                if (scopedCity == "__NO_ACCESS__")
                {
                    return Json(new { Success = true, Data = new List<object>(), Total = 0 });
                }

                var list = _BaoCaoChiSoCD45DA.GetDrillDown(ChiTieuCode, FromDate, ToDate, scopedCity, MaNhom, IsCumulative, DoiTuong, GioiTinh, NhomTuoi);
                var jsonResult = Json(new { Success = true, Data = list, Total = list.Count });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExportDrillDownExcel(string ChiTieuCode, string FromDate, string ToDate, string MaTinh, string MaNhom, bool IsCumulative = true, int? DoiTuong = null, int? GioiTinh = null, int? NhomTuoi = null)
        {
            try
            {
                string scopedCity = ScopeCityCodeFilter(MaTinh);
                if (scopedCity == "__NO_ACCESS__")
                {
                    return Content("<script>alert('Bạn không có quyền truy cập dữ liệu địa bàn này!'); window.history.back();</script>", "text/html; charset=utf-8");
                }

                var list = _BaoCaoChiSoCD45DA.GetDrillDown(ChiTieuCode, FromDate, ToDate, scopedCity, MaNhom, IsCumulative, DoiTuong, GioiTinh, NhomTuoi);

                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("ChiTiet");
                    ws.Cell(1, 1).Value = $"DANH SÁCH CHI TIẾT CHỈ SỐ [{ChiTieuCode}] - DỰ ÁN CD45 (DREAMH)";
                    ws.Cell(1, 1).Style.Font.Bold = true;
                    ws.Cell(1, 1).Style.Font.FontSize = 13;

                    var headers = new[] { "STT", "Mã KH / Đối tượng", "Tỉnh / Thành", "Nhóm CBO", "Tiếp cận viên", "Nhóm KP / Phân loại", "Ngày thực hiện", "Chi tiết" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = ws.Cell(3, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
                        cell.Style.Font.FontColor = XLColor.White;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    int row = 4;
                    int stt = 1;
                    foreach (var d in list)
                    {
                        ws.Cell(row, 1).Value = stt++;
                        ws.Cell(row, 2).Value = d.RECORD_ID;
                        ws.Cell(row, 3).Value = d.TEN_TINH ?? d.CITY_CODE;
                        ws.Cell(row, 4).Value = d.TEN_NHOM ?? d.MA_NHOM;
                        ws.Cell(row, 5).Value = d.TEN_TCV ?? d.MA_TCV ?? "-";
                        ws.Cell(row, 6).Value = d.DOI_TUONG_TEXT;
                        ws.Cell(row, 7).Value = d.NGAY_THUC_HIEN;
                        ws.Cell(row, 8).Value = d.CHI_TIET;
                        row++;
                    }

                    var rng = ws.Range(3, 1, Math.Max(row - 1, 3), headers.Length);
                    rng.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    rng.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    ws.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"ChiTiet_{ChiTieuCode}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi xuất Excel chi tiết: " + ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetConfig(int Nam, string MaKy)
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                if (user == null) return Json(new { Success = false, Message = "Chưa đăng nhập" });

                var data = _BaoCaoChiSoCD45DA.GetConfig(Nam, MaKy);
                return Json(new { Success = true, Data = data, IsAdmin = IsCurrentUserAdmin() });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi tải cấu hình: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveConfig(int Nam, string MaKy, List<CD45_ChiSoDuAnConfigModel> Items)
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                if (user == null) return Json(new { Success = false, Message = "Phiên làm việc đã hết hạn!" });

                if (!IsCurrentUserAdmin())
                {
                    return Json(new { Success = false, Message = "Chỉ Quản trị viên (Admin / M&E) mới có quyền cập nhật cấu hình chỉ số dự án." });
                }

                string msg;
                bool ok = _BaoCaoChiSoCD45DA.SaveConfig(Nam, MaKy, Items, user.UserName, out msg);
                return Json(new { Success = ok, Message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi khi lưu cấu hình: " + ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExportExcel(string FromDate, string ToDate, string MaTinh, string MaNhom, bool IsCumulative = true, int? Nam = null, string MaKy = "12T", bool IncludeSubRows = true, bool IncludeMetaCols = true)
        {
            if (!string.IsNullOrWhiteSpace(FromDate) || !string.IsNullOrWhiteSpace(ToDate))
            {
                if (!ValidateDateRange(FromDate, ToDate, out var dateError))
                {
                    return Content("<script>alert('" + dateError.Replace("'", "\\'") + "'); window.history.back();</script>", "text/html; charset=utf-8");
                }
            }

            string scopedCity = ScopeCityCodeFilter(MaTinh);
            if (scopedCity == "__NO_ACCESS__")
            {
                return Content("<script>alert('Bạn không có quyền truy cập dữ liệu địa bàn này!'); window.history.back();</script>", "text/html; charset=utf-8");
            }

            var data = _BaoCaoChiSoCD45DA.GetBaoCaoChiSo(FromDate, ToDate, scopedCity, MaNhom, IsCumulative, Nam, MaKy);
            if (!IncludeSubRows)
            {
                data = data.Where(x => x.RowType != "SUB").ToList();
            }

            string tenTinh = "Toàn dự án (6 Tỉnh/TP)";
            if (!string.IsNullOrEmpty(MaTinh))
            {
                var newCity = _CityDA.GetAllNewCities()?.FirstOrDefault(x => string.Equals(x.Code, MaTinh, StringComparison.OrdinalIgnoreCase));
                var oldCity = _CityDA.GetAll()?.FirstOrDefault(x => string.Equals(x.Code, MaTinh, StringComparison.OrdinalIgnoreCase));
                tenTinh = newCity?.Name ?? oldCity?.Name ?? MaTinh;
            }

            string tenNhom = "Tất cả CBO / DNXH";
            string xungDanh = "Nhóm";
            string chucDanh = "Trưởng nhóm";
            if (!string.IsNullOrEmpty(MaNhom))
            {
                var nhom = _BVTL_NHOM_TBHDA.GetAll()?.FirstOrDefault(x =>
                    string.Equals(x.manhom_tbh, MaNhom, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.manhom_tbh_map, MaNhom, StringComparison.OrdinalIgnoreCase));
                tenNhom = nhom != null ? nhom.tennhom_tbh : MaNhom;
                xungDanh = nhom != null ? nhom.GetXungDanh() : "Nhóm";
                chucDanh = nhom != null ? nhom.GetChucDanh() : "Trưởng nhóm";
            }

            string kyTitle = MaKy == "6T" ? "Báo cáo 6 Tháng" : (MaKy == "ALL" ? "Lũy kế toàn dự án" : (MaKy == "TuyChon" ? "Tùy chọn ngày" : "Báo cáo Năm (12T)"));
            string modeCalc = IsCumulative ? "Lũy kế đến cuối kỳ" : "Phát sinh trong kỳ";
            string dateDesc = (!string.IsNullOrWhiteSpace(FromDate) && !string.IsNullOrWhiteSpace(ToDate))
                ? $"Từ ngày: {FromDate} đến ngày: {ToDate}"
                : (!string.IsNullOrWhiteSpace(ToDate) ? $"Tính đến ngày: {ToDate}" : "Toàn bộ thời gian dự án");

            int totalCols = IncludeMetaCols ? 18 : 16;

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("BaoCaoChiSo");

                // Title
                ws.Cell(1, 1).Value = "DỰ ÁN DREAMH (CD45) — BÁO CÁO KẾT QUẢ THỰC HIỆN CÁC CHỈ SỐ DỰ ÁN";
                ws.Range(1, 1, 1, totalCols).Merge();
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 14;
                ws.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#1F4E78");
                ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(1).Height = 26;

                ws.Cell(2, 1).Value = $"{kyTitle} ({modeCalc})   |   {dateDesc}   |   Địa bàn: {tenTinh}   |   {xungDanh}: {tenNhom}";
                ws.Range(2, 1, 2, totalCols).Merge();
                ws.Cell(2, 1).Style.Font.Italic = true;
                ws.Cell(2, 1).Style.Font.FontSize = 10.5;
                ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(2).Height = 20;

                // Headers (Rows 4 & 5)
                ws.Range(4, 1, 5, 1).Merge().Value = "STT";
                ws.Range(4, 2, 5, 2).Merge().Value = "Mã chỉ số";
                ws.Range(4, 3, 5, 3).Merge().Value = "Tên chỉ số / Chỉ tiêu thành phần";
                ws.Range(4, 4, 5, 4).Merge().Value = "Đơn vị tính";
                ws.Range(4, 5, 5, 5).Merge().Value = "Tổng số";

                ws.Range(4, 6, 4, 10).Merge().Value = "Phân tách theo Nhóm đối tượng đích (KP)";
                string[] kpHeaders = { "PUD", "PLHIV", "TG", "SW", "MSM" };
                for (int i = 0; i < kpHeaders.Length; i++) ws.Cell(5, 6 + i).Value = kpHeaders[i];

                ws.Range(4, 11, 4, 13).Merge().Value = "Phân tách theo Giới tính";
                string[] gtHeaders = { "Nam", "Nữ", "Khác" };
                for (int i = 0; i < gtHeaders.Length; i++) ws.Cell(5, 11 + i).Value = gtHeaders[i];

                ws.Range(4, 14, 4, 16).Merge().Value = "Phân tách theo Nhóm tuổi";
                string[] ageHeaders = { "18 - 25", "26 - 35", "Trên 35" };
                for (int i = 0; i < ageHeaders.Length; i++) ws.Cell(5, 14 + i).Value = ageHeaders[i];

                if (IncludeMetaCols)
                {
                    ws.Range(4, 17, 5, 17).Merge().Value = "Nguồn dữ liệu / Công thức";
                    ws.Range(4, 18, 5, 18).Merge().Value = "Ghi chú";
                }

                ws.Row(4).Height = 22;
                ws.Row(5).Height = 22;
                for (int r = 4; r <= 5; r++)
                {
                    for (int c = 1; c <= totalCols; c++)
                    {
                        var hc = ws.Cell(r, c);
                        hc.Style.Font.Bold = true;
                        hc.Style.Font.FontColor = XLColor.White;
                        hc.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        hc.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        hc.Style.Alignment.WrapText = true;
                        hc.Style.Fill.BackgroundColor = c <= 5 ? XLColor.FromHtml("#1F4E78")
                            : (c <= 10 ? XLColor.FromHtml("#2F5597")
                            : (c <= 13 ? XLColor.FromHtml("#385723")
                            : (c <= 16 ? XLColor.FromHtml("#833C0C") : XLColor.FromHtml("#595959"))));
                    }
                }

                // Map code -> row index to wire Excel dynamic formulas for PCT rows when IncludeSubRows == true
                var rowStart = 6;
                var rowMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int idx = 0; idx < data.Count; idx++)
                {
                    if (!string.IsNullOrEmpty(data[idx].Code))
                        rowMap[data[idx].Code] = rowStart + idx;
                }

                int row = rowStart;
                foreach (var item in data)
                {
                    if (item.RowType == "SEC")
                    {
                        ws.Cell(row, 1).Value = item.STT;
                        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(row, 2, row, totalCols).Merge().Value = item.ChiTieu;
                        var secRange = ws.Range(row, 1, row, totalCols);
                        secRange.Style.Font.Bold = true;
                        secRange.Style.Font.FontColor = XLColor.FromHtml("#1F4E78");
                        secRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#D9E1F2");
                        ws.Row(row).Height = 24;
                        row++;
                        continue;
                    }

                    ws.Cell(row, 1).Value = item.STT;
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 2).Value = item.Code;
                    ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 3).Value = item.ChiTieu;
                    ws.Cell(row, 3).Style.Alignment.WrapText = true;
                    ws.Cell(row, 4).Value = item.DonVi;
                    ws.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    bool isPct = item.RowType == "PCT";
                    bool isSub = item.RowType == "SUB";
                    var rowBg = isPct ? XLColor.FromHtml("#FFF2CC") : (isSub ? XLColor.White : XLColor.FromHtml("#F2F4F8"));

                    for (int c = 1; c <= totalCols; c++)
                    {
                        ws.Cell(row, c).Style.Fill.BackgroundColor = rowBg;
                        ws.Cell(row, c).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        if (!isSub) ws.Cell(row, c).Style.Font.Bold = true;
                    }
                    if (isSub)
                    {
                        ws.Cell(row, 3).Style.Alignment.Indent = 1;
                        ws.Cell(row, 3).Style.Font.Italic = true;
                    }

                    int?[] nums = { item.Tong, item.PUD, item.PLHIV, item.TG, item.SW, item.MSM, item.Nam, item.Nu, item.Khac, item.Tuoi_18_25, item.Tuoi_26_35, item.Tuoi_Tren35 };
                    int?[] dens = { item.Denom_Tong, item.Denom_PUD, item.Denom_PLHIV, item.Denom_TG, item.Denom_SW, item.Denom_MSM, item.Denom_Nam, item.Denom_Nu, item.Denom_Khac, item.Denom_Tuoi_18_25, item.Denom_Tuoi_26_35, item.Denom_Tuoi_Tren35 };

                    bool canFormula = isPct && IncludeSubRows && rowMap.ContainsKey(item.Code + "_NUM") && rowMap.ContainsKey(item.Code + "_DEN");
                    int numRowIdx = canFormula ? rowMap[item.Code + "_NUM"] : 0;
                    int denRowIdx = canFormula ? rowMap[item.Code + "_DEN"] : 0;

                    for (int i = 0; i < 12; i++)
                    {
                        int col = 5 + i;
                        var cell = ws.Cell(row, col);
                        if (isPct)
                        {
                            cell.Style.Font.FontColor = XLColor.FromHtml("#C00000");
                            cell.Style.Font.Bold = true;
                            if (dens[i].HasValue)
                            {
                                if (canFormula)
                                {
                                    string colLetter = XLHelper.GetColumnLetterFromNumber(col);
                                    cell.FormulaA1 = $"=IF({colLetter}{denRowIdx}>0,{colLetter}{numRowIdx}/{colLetter}{denRowIdx},0)";
                                }
                                else
                                {
                                    cell.Value = dens[i].Value > 0 ? (double)(nums[i] ?? 0) / dens[i].Value : 0.0;
                                }
                                cell.Style.NumberFormat.Format = "0.0%";
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            }
                            else
                            {
                                cell.Value = "-";
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }
                        }
                        else if (nums[i].HasValue)
                        {
                            cell.Value = nums[i].Value;
                            cell.Style.NumberFormat.Format = "#,##0";
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        else
                        {
                            cell.Value = "-";
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }
                    }

                    if (IncludeMetaCols)
                    {
                        ws.Cell(row, 17).Value = item.NguonDuLieu ?? "";
                        ws.Cell(row, 17).Style.Alignment.WrapText = true;
                        ws.Cell(row, 17).Style.Font.FontSize = 9.5;
                        ws.Cell(row, 18).Value = item.GhiChu ?? "";
                        ws.Cell(row, 18).Style.Alignment.WrapText = true;
                        ws.Cell(row, 18).Style.Font.FontSize = 9.5;
                    }

                    ws.Row(row).Height = isSub ? 20 : 28;
                    row++;
                }

                var tableRange = ws.Range(4, 1, Math.Max(row - 1, 5), totalCols);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Signature Footer
                row += 2;
                string signer1Title = (!string.IsNullOrEmpty(MaNhom) && tenNhom != "Tất cả CBO / DNXH")
                    ? $"{chucDanh} {tenNhom.Replace("\"", "").Trim()}"
                    : (!string.IsNullOrEmpty(MaTinh) ? "Đại diện đơn vị / Tỉnh" : "Đại diện dự án");

                ws.Range(row, 1, row, 4).Merge().Value = signer1Title;
                ws.Range(row, 1, row, 4).Style.Font.Bold = true;
                ws.Range(row, 1, row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range(row + 1, 1, row + 1, 4).Merge().Value = "(Ký, ghi rõ họ tên)";
                ws.Range(row + 1, 1, row + 1, 4).Style.Font.Italic = true;
                ws.Range(row + 1, 1, row + 1, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Range(row, 6, row, 10).Merge().Value = "Cán bộ dự án";
                ws.Range(row, 6, row, 10).Style.Font.Bold = true;
                ws.Range(row, 6, row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range(row + 1, 6, row + 1, 10).Merge().Value = "(Ký, ghi rõ họ tên)";
                ws.Range(row + 1, 6, row + 1, 10).Style.Font.Italic = true;
                ws.Range(row + 1, 6, row + 1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Range(row, 11, row, 16).Merge().Value = "MnE";
                ws.Range(row, 11, row, 16).Style.Font.Bold = true;
                ws.Range(row, 11, row, 16).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range(row + 1, 11, row + 1, 16).Merge().Value = "(Ký, ghi rõ họ tên)";
                ws.Range(row + 1, 11, row + 1, 16).Style.Font.Italic = true;
                ws.Range(row + 1, 11, row + 1, 16).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Column widths
                ws.Column(1).Width = 5.5;
                ws.Column(2).Width = 14.5;
                ws.Column(3).Width = 46.0;
                ws.Column(4).Width = 12.5;
                ws.Column(5).Width = 11.0;
                for (int c = 6; c <= 16; c++) ws.Column(c).Width = 9.0;
                if (IncludeMetaCols)
                {
                    ws.Column(17).Width = 34.0;
                    ws.Column(18).Width = 34.0;
                }

                ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
                ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                ws.PageSetup.FitToPages(1, 0);
                ws.SheetView.FreezeRows(5);
                ws.SheetView.FreezeColumns(5);

                string scopeTag = !string.IsNullOrEmpty(MaNhom) ? $"_{MaTinh}_{MaNhom}" : (!string.IsNullOrEmpty(MaTinh) ? $"_{MaTinh}" : "_TOANQUOC");
                string fileName = $"BaoCao_ChiSo_DREAMH_{MaKy}{scopeTag}_{DateTime.Now:yyyyMMdd}.xlsx";

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }
}
