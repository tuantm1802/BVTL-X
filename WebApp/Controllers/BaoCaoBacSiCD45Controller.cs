using ClosedXML.Excel;
using Common;
using Common.Common;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class BaoCaoBacSiCD45Controller : BaseController
    {
        private readonly ICityDA _CityDA;
        private readonly IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA;
        private readonly IBaoCaoBacSiCD45DA _BaoCaoBacSiDA;

        public BaoCaoBacSiCD45Controller(
            ICityDA cityDA,
            IBVTL_NHOM_TBHDA bvtlNhomTbhDA,
            IBaoCaoBacSiCD45DA baoCaoBacSiDA)
        {
            _CityDA = cityDA;
            _BVTL_NHOM_TBHDA = bvtlNhomTbhDA;
            _BaoCaoBacSiDA = baoCaoBacSiDA;
        }

        [HasCredential(ControllerName = "BaoCaoBacSiCD45")]
        public ActionResult Index()
        {
            var user = Session["USER_SESSION"] as UserLogin;
            if (user == null) return Redirect("/Login/Index");
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
                var nhoms = nhomsQuery.OrderBy(x => x.city_code)
                                      .ThenBy(x => x.tennhom_tbh)
                                      .Select(x => new
                                      {
                                          manhom_tbh = x.manhom_tbh,
                                          tennhom_tbh = x.tennhom_tbh,
                                          city_code = x.city_code
                                      })
                                      .ToList();

                var doctors = _BaoCaoBacSiDA.GetDoctors();

                return Json(new { Success = true, Cities = cities, Nhoms = nhoms, Doctors = doctors, IsAdmin = isAdmin });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SearchBaoCao(string FromDate, string ToDate, string MaTinh, string DoctorId, string MaNhom)
        {
            try
            {
                if (!ValidateDateRange(FromDate, ToDate, out var dateError))
                {
                    return Json(new { Success = false, Message = dateError });
                }

                string scopedCity = ScopeCityCodeFilter(MaTinh);
                if (scopedCity == "__NO_ACCESS__")
                {
                    return Json(new { Success = true, Summary = new List<BaoCaoBacSiSummaryModel>(), Details = new List<BaoCaoBacSiDetailModel>() });
                }

                var summary = _BaoCaoBacSiDA.GetBaoCaoTongHop(FromDate, ToDate, scopedCity, DoctorId, MaNhom);
                var details = _BaoCaoBacSiDA.GetBaoCaoChiTiet(FromDate, ToDate, scopedCity, DoctorId, MaNhom);

                var jsonResult = Json(new { Success = true, Summary = summary, Details = details });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExportExcel(string FromDate, string ToDate, string MaTinh, string DoctorId, string MaNhom)
        {
            if (!ValidateDateRange(FromDate, ToDate, out var dateError))
            {
                return Content("<script>alert('" + dateError.Replace("'", "\\'") + "'); window.history.back();</script>", "text/html; charset=utf-8");
            }

            string scopedCity = ScopeCityCodeFilter(MaTinh);
            if (scopedCity == "__NO_ACCESS__")
            {
                return Content("<script>alert('Bạn không có quyền truy cập dữ liệu địa bàn này!'); window.history.back();</script>", "text/html; charset=utf-8");
            }

            var summary = _BaoCaoBacSiDA.GetBaoCaoTongHop(FromDate, ToDate, scopedCity, DoctorId, MaNhom);
            var details = _BaoCaoBacSiDA.GetBaoCaoChiTiet(FromDate, ToDate, scopedCity, DoctorId, MaNhom);

            string tenTinh = "Toàn quốc";
            if (!string.IsNullOrEmpty(MaTinh))
            {
                var city = _CityDA.GetAll()?.FirstOrDefault(x => string.Equals(x.Code, MaTinh, StringComparison.OrdinalIgnoreCase));
                tenTinh = city != null ? city.Name : MaTinh;
            }

            using (var workbook = new XLWorkbook())
            {
                // =========================================================================
                // SHEET 1: TỔNG HỢP THANH TOÁN BÁC SĨ
                // =========================================================================
                var wsSum = workbook.Worksheets.Add("TongHop_ThanhToan");
                wsSum.Cell(1, 1).Value = "BÁO CÁO TỔNG HỢP THANH TOÁN BÁC SĨ (DỰ ÁN CD45 - DREAMH)";
                wsSum.Cell(1, 1).Style.Font.Bold = true;
                wsSum.Cell(1, 1).Style.Font.FontSize = 14;
                wsSum.Range("A1:J1").Row(1).Merge();

                wsSum.Cell(2, 1).Value = $"Thời gian khám: Từ ngày {FromDate} đến ngày {ToDate}   |   Địa bàn: {tenTinh}";
                wsSum.Cell(2, 1).Style.Font.Italic = true;
                wsSum.Range("A2:J2").Row(1).Merge();

                string[] sumHeaders = { "STT", "Mã BS", "Bác sĩ phụ trách", "Tỉnh / Thành", "Tổng số ca", "Khám lần 1", "Tái khám", "Khám khác", "Ngoại trú", "Nội trú" };
                for (int i = 0; i < sumHeaders.Length; i++)
                {
                    wsSum.Cell(4, i + 1).Value = sumHeaders[i];
                }
                var sumHeaderRange = wsSum.Range("A4:J4");
                sumHeaderRange.Style.Font.Bold = true;
                sumHeaderRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                sumHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                int rowSum = 5;
                int sttSum = 1;
                foreach (var s in summary)
                {
                    wsSum.Cell(rowSum, 1).Value = sttSum++;
                    wsSum.Cell(rowSum, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsSum.Cell(rowSum, 2).Value = s.MA_BAC_SI;
                    wsSum.Cell(rowSum, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsSum.Cell(rowSum, 3).Value = s.TEN_BAC_SI;
                    wsSum.Cell(rowSum, 4).Value = s.TINH_THANH;
                    wsSum.Cell(rowSum, 5).Value = s.TongSoCa;
                    wsSum.Cell(rowSum, 5).Style.Font.Bold = true;
                    wsSum.Cell(rowSum, 6).Value = s.KhamLan1;
                    wsSum.Cell(rowSum, 7).Value = s.TaiKham;
                    wsSum.Cell(rowSum, 8).Value = s.KhamKhac;
                    wsSum.Cell(rowSum, 9).Value = s.NgoaiTru;
                    wsSum.Cell(rowSum, 10).Value = s.NoiTru;

                    for (int c = 5; c <= 10; c++)
                    {
                        wsSum.Cell(rowSum, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                    rowSum++;
                }

                // Dòng Tổng cộng
                if (summary.Count > 0)
                {
                    wsSum.Cell(rowSum, 1).Value = "";
                    wsSum.Cell(rowSum, 2).Value = "";
                    wsSum.Cell(rowSum, 3).Value = "TỔNG CỘNG";
                    wsSum.Cell(rowSum, 4).Value = "";
                    wsSum.Cell(rowSum, 5).Value = summary.Sum(x => x.TongSoCa);
                    wsSum.Cell(rowSum, 6).Value = summary.Sum(x => x.KhamLan1);
                    wsSum.Cell(rowSum, 7).Value = summary.Sum(x => x.TaiKham);
                    wsSum.Cell(rowSum, 8).Value = summary.Sum(x => x.KhamKhac);
                    wsSum.Cell(rowSum, 9).Value = summary.Sum(x => x.NgoaiTru);
                    wsSum.Cell(rowSum, 10).Value = summary.Sum(x => x.NoiTru);

                    var totalRange = wsSum.Range(rowSum, 1, rowSum, 10);
                    totalRange.Style.Font.Bold = true;
                    totalRange.Style.Fill.BackgroundColor = XLColor.FromArgb(240, 243, 246);
                    for (int c = 5; c <= 10; c++)
                    {
                        wsSum.Cell(rowSum, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                    rowSum++;
                }

                var sumTableRange = wsSum.Range(4, 1, Math.Max(rowSum - 1, 5), 10);
                sumTableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sumTableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Signatures
                rowSum += 2;
                wsSum.Range(rowSum, 1, rowSum, 3).Merge();
                wsSum.Cell(rowSum, 1).Value = "Người lập biểu";
                wsSum.Cell(rowSum, 1).Style.Font.Bold = true;
                wsSum.Cell(rowSum, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wsSum.Range(rowSum + 1, 1, rowSum + 1, 3).Merge();
                wsSum.Cell(rowSum + 1, 1).Value = "(Ký, ghi rõ họ tên)";
                wsSum.Cell(rowSum + 1, 1).Style.Font.Italic = true;
                wsSum.Cell(rowSum + 1, 1).Style.Font.FontSize = 9;
                wsSum.Cell(rowSum + 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                wsSum.Range(rowSum, 4, rowSum, 7).Merge();
                wsSum.Cell(rowSum, 4).Value = "Cán bộ dự án CD45";
                wsSum.Cell(rowSum, 4).Style.Font.Bold = true;
                wsSum.Cell(rowSum, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wsSum.Range(rowSum + 1, 4, rowSum + 1, 7).Merge();
                wsSum.Cell(rowSum + 1, 4).Value = "(Ký, ghi rõ họ tên)";
                wsSum.Cell(rowSum + 1, 4).Style.Font.Italic = true;
                wsSum.Cell(rowSum + 1, 4).Style.Font.FontSize = 9;
                wsSum.Cell(rowSum + 1, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                wsSum.Range(rowSum, 8, rowSum, 10).Merge();
                wsSum.Cell(rowSum, 8).Value = "Kế toán / Phụ trách thanh toán";
                wsSum.Cell(rowSum, 8).Style.Font.Bold = true;
                wsSum.Cell(rowSum, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wsSum.Range(rowSum + 1, 8, rowSum + 1, 10).Merge();
                wsSum.Cell(rowSum + 1, 8).Value = "(Ký, ghi rõ họ tên)";
                wsSum.Cell(rowSum + 1, 8).Style.Font.Italic = true;
                wsSum.Cell(rowSum + 1, 8).Style.Font.FontSize = 9;
                wsSum.Cell(rowSum + 1, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                wsSum.Columns().AdjustToContents();

                // =========================================================================
                // SHEET 2: DANH SÁCH CHI TIẾT CÁC CA KHÁM
                // =========================================================================
                var wsDet = workbook.Worksheets.Add("ChiTiet_CaKham");
                wsDet.Cell(1, 1).Value = "DANH SÁCH CHI TIẾT CA KHÁM BÁC SĨ (PHỤ LỤC THANH TOÁN)";
                wsDet.Cell(1, 1).Style.Font.Bold = true;
                wsDet.Cell(1, 1).Style.Font.FontSize = 14;
                wsDet.Range("A1:K1").Row(1).Merge();

                wsDet.Cell(2, 1).Value = $"Thời gian: Từ ngày {FromDate} đến ngày {ToDate}   |   Địa bàn: {tenTinh}";
                wsDet.Cell(2, 1).Style.Font.Italic = true;
                wsDet.Range("A2:K2").Row(1).Merge();

                string[] detHeaders = { "STT", "Mã Khách hàng", "Ngày khám", "Bác sĩ phụ trách", "Cơ sở khám / Bệnh viện", "Tỉnh / Thành", "Nhóm CBO", "TCV", "Lần khám", "Chẩn đoán chính", "Hình thức điều trị" };
                for (int i = 0; i < detHeaders.Length; i++)
                {
                    wsDet.Cell(4, i + 1).Value = detHeaders[i];
                }
                var detHeaderRange = wsDet.Range("A4:K4");
                detHeaderRange.Style.Font.Bold = true;
                detHeaderRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                detHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                int rowDet = 5;
                int sttDet = 1;
                foreach (var d in details)
                {
                    wsDet.Cell(rowDet, 1).Value = sttDet++;
                    wsDet.Cell(rowDet, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsDet.Cell(rowDet, 2).Value = d.RECORD_ID;
                    wsDet.Cell(rowDet, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsDet.Cell(rowDet, 3).Value = d.NGAY_KHAM_STR;
                    wsDet.Cell(rowDet, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsDet.Cell(rowDet, 4).Value = d.TEN_BAC_SI;
                    wsDet.Cell(rowDet, 5).Value = d.TEN_CO_SO_Y_TE;
                    wsDet.Cell(rowDet, 6).Value = d.TEN_TINH;
                    wsDet.Cell(rowDet, 7).Value = d.TEN_NHOM;
                    wsDet.Cell(rowDet, 8).Value = d.TEN_TCV;
                    wsDet.Cell(rowDet, 9).Value = d.TEN_LAN_KHAM;
                    wsDet.Cell(rowDet, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsDet.Cell(rowDet, 10).Value = d.TEN_CHAN_DOAN;
                    wsDet.Cell(rowDet, 11).Value = d.TEN_HINH_THUC_DIEU_TRI;
                    wsDet.Cell(rowDet, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rowDet++;
                }

                var detTableRange = wsDet.Range(4, 1, Math.Max(rowDet - 1, 5), 11);
                detTableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                detTableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                wsDet.Columns().AdjustToContents();

                string fileName = $"BaoCao_BacSi_CD45_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        private bool ValidateDateRange(string fromDate, string toDate, out string error)
        {
            error = null;
            if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
            {
                error = "Vui lòng chọn đầy đủ Từ ngày và Đến ngày!";
                return false;
            }
            if (!DateTime.TryParseExact(fromDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var d1) ||
                !DateTime.TryParseExact(toDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var d2))
            {
                error = "Định dạng ngày không hợp lệ (dd/MM/yyyy)!";
                return false;
            }
            if (d1 > d2)
            {
                error = "Từ ngày không được lớn hơn Đến ngày!";
                return false;
            }
            return true;
        }

        private string ScopeCityCodeFilter(string inputCityCode)
        {
            var allowedCodes = GetUserAllowedCityCodes();
            if (IsCurrentUserAdmin() || allowedCodes == null || allowedCodes.Count == 0)
            {
                return inputCityCode;
            }
            if (!string.IsNullOrEmpty(inputCityCode))
            {
                return allowedCodes.Contains(inputCityCode) ? inputCityCode : "__NO_ACCESS__";
            }
            return allowedCodes.FirstOrDefault();
        }
    }
}
