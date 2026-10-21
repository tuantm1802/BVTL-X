using ClosedXML.Excel;
using Common;
using Common.Common;
using Data.InterfaceDA;
using Data.InterfaceDA.Admin;
using Model.Model;
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
    public class BaoCaoTuVanCD45Controller : BaseController
    {
        private readonly ICityDA _CityDA;
        private readonly IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA;
        private readonly IBaoCaoCD45DA _BaoCaoCD45DA;
        private readonly IBaoCaoTuVanCD45DA _BaoCaoTuVanCD45DA;

        public BaoCaoTuVanCD45Controller(
            ICityDA cityDA,
            IBVTL_NHOM_TBHDA bvtlNhomTbhDA,
            IBaoCaoCD45DA baoCaoCD45DA,
            IBaoCaoTuVanCD45DA baoCaoTuVanCD45DA)
        {
            _CityDA = cityDA;
            _BVTL_NHOM_TBHDA = bvtlNhomTbhDA;
            _BaoCaoCD45DA = baoCaoCD45DA;
            _BaoCaoTuVanCD45DA = baoCaoTuVanCD45DA;
        }

        [HasCredential(ControllerName = "BaoCaoTuVanCD45")]
        public ActionResult Index()
        {
            var user = Session["USER_SESSION"] as UserLogin;
            if (user == null) return Redirect("/Login/Index");
            return View();
        }

        [HttpPost]
        public JsonResult GetFilterData()
        {
            try
            {
                var allowedCodes = GetUserAllowedCityCodes();
                bool isAdmin = IsCurrentUserAdmin();

                var all6Provinces = new[] { "HNO", "HPG", "HYE", "NAN", "NBI", "HCM" };
                var allowedTarget = isAdmin ? all6Provinces : all6Provinces.Where(c => allowedCodes.Contains(c)).ToArray();

                var cities = _CityDA.GetAll().Where(x => allowedTarget.Contains(x.Code)).Select(x => new { CityCode = x.Code, CityName = x.Name }).ToList();
                var nhomsQuery = _BVTL_NHOM_TBHDA.GetAll().Where(x => x.maduan == "CD45");
                if (!isAdmin)
                {
                    nhomsQuery = nhomsQuery.Where(x => !string.IsNullOrEmpty(x.city_code) && allowedCodes.Contains(x.city_code.Trim()));
                }
                var nhoms = nhomsQuery.Select(x => new {
                    manhom_tbh = (x.manhom_tbh ?? "").Trim(),
                    tennhom_tbh = (x.tennhom_tbh ?? "").Trim(),
                    city_code = (x.city_code ?? "").Trim(),
                    manhom_tbh_map = (x.manhom_tbh_map ?? "").Trim(),
                    Prefix = x.GetXungDanh(),
                    ShortPrefix = x.GetShortXungDanh(),
                    DisplayName = $"[{x.GetShortXungDanh()}] {x.tennhom_tbh}"
                }).ToList();

                string defaultScopedCity = ScopeCityCodeFilter(null);
                var tcvs = defaultScopedCity == "__NO_ACCESS__" ? new List<CD45_TCV_ItemModel>() : _BaoCaoCD45DA.GetListTCV(defaultScopedCity, null);
                return Json(new { Success = true, Cities = cities, Nhoms = nhoms, TCVs = tcvs, IsAdmin = isAdmin });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetTCVsByFilter(string cityCode, string maNhom)
        {
            try
            {
                string scopedCity = ScopeCityCodeFilter(cityCode);
                if (scopedCity == "__NO_ACCESS__")
                {
                    return Json(new { Success = true, TCVs = new List<CD45_TCV_ItemModel>() });
                }
                var tcvs = _BaoCaoCD45DA.GetListTCV(scopedCity, maNhom);
                return Json(new { Success = true, TCVs = tcvs });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SearchTuVan(string fromDate, string toDate, string cityCode, string maNhom, string maTCV, string loaiTuVan = "ALL", int? tcvId = null)
        {
            try
            {
                if (!ValidateDateRange(fromDate, toDate, out var dateError))
                {
                    return Json(new { Success = false, Message = dateError });
                }

                if (tcvId.HasValue && tcvId.Value > 0)
                {
                    var allTcvs = _BaoCaoCD45DA.GetListTCV(null, null);
                    var tcv = allTcvs.FirstOrDefault(x => x.ID == tcvId.Value);
                    if (tcv != null)
                    {
                        maTCV = (tcv.MA_TCV ?? "").Trim();
                        if (string.IsNullOrEmpty(maNhom)) maNhom = (tcv.MA_NHOM ?? "").Trim();
                        if (string.IsNullOrEmpty(cityCode)) cityCode = (tcv.CITY_CODE ?? "").Trim();
                    }
                }

                string scopedCity = ScopeCityCodeFilter(cityCode);
                if (scopedCity == "__NO_ACCESS__")
                {
                    return Json(new { Success = true, L1 = new List<CD45BaoCaoTuVanL1Model>(), L2 = new List<CD45BaoCaoTuVanL2Model>() });
                }

                List<CD45BaoCaoTuVanL1Model> listL1 = null;
                List<CD45BaoCaoTuVanL2Model> listL2 = null;

                if (string.IsNullOrEmpty(loaiTuVan) || loaiTuVan == "ALL" || loaiTuVan == "L1")
                {
                    listL1 = _BaoCaoTuVanCD45DA.GetBaoCaoTuVanL1(fromDate, toDate, scopedCity, maNhom, maTCV);
                }
                if (string.IsNullOrEmpty(loaiTuVan) || loaiTuVan == "ALL" || loaiTuVan == "L2")
                {
                    listL2 = _BaoCaoTuVanCD45DA.GetBaoCaoTuVanL2(fromDate, toDate, scopedCity, maNhom, maTCV);
                }

                var jsonResult = Json(new
                {
                    Success = true,
                    L1 = listL1 ?? new List<CD45BaoCaoTuVanL1Model>(),
                    L2 = listL2 ?? new List<CD45BaoCaoTuVanL2Model>(),
                    CountL1 = listL1 != null ? listL1.Count : 0,
                    CountL2 = listL2 != null ? listL2.Count : 0
                });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi truy vấn: " + ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExportExcel(string FromDate, string ToDate, string CityCode, string MaNhom, string MaTCV, string LoaiTuVan = "ALL", string TenNhom = null, string TenTCV = null, int? tcvId = null)
        {
            try
            {
                if (!ValidateDateRange(FromDate, ToDate, out var dateError))
                {
                    return Content("<script>alert('" + dateError.Replace("'", "\\'") + "'); window.history.back();</script>", "text/html; charset=utf-8");
                }

                if (tcvId.HasValue && tcvId.Value > 0)
                {
                    var allTcvs = _BaoCaoCD45DA.GetListTCV(null, null);
                    var tcv = allTcvs.FirstOrDefault(x => x.ID == tcvId.Value);
                    if (tcv != null)
                    {
                        MaTCV = (tcv.MA_TCV ?? "").Trim();
                        if (string.IsNullOrEmpty(MaNhom)) MaNhom = (tcv.MA_NHOM ?? "").Trim();
                        if (string.IsNullOrEmpty(CityCode)) CityCode = (tcv.CITY_CODE ?? "").Trim();
                        if (string.IsNullOrEmpty(TenTCV)) TenTCV = tcv.TEN_TCV;
                        if (string.IsNullOrEmpty(TenNhom)) TenNhom = tcv.TEN_NHOM;
                    }
                }

                string scopedCity = ScopeCityCodeFilter(CityCode);
                if (scopedCity == "__NO_ACCESS__")
                {
                    return Content("<script>alert('Bạn không có quyền truy cập dữ liệu địa bàn này!'); window.history.back();</script>", "text/html; charset=utf-8");
                }

                using (var wb = new XLWorkbook())
                {
                    if (string.IsNullOrEmpty(LoaiTuVan) || LoaiTuVan == "ALL" || LoaiTuVan == "L1")
                    {
                        var listL1 = _BaoCaoTuVanCD45DA.GetBaoCaoTuVanL1(FromDate, ToDate, scopedCity, MaNhom, MaTCV);
                        var ws1 = wb.Worksheets.Add("TuVan_L1");
                        ExcelTuVanHelper.BuildTuVanL1Worksheet(ws1, listL1, FromDate, ToDate, TenNhom, TenTCV);
                    }

                    if (string.IsNullOrEmpty(LoaiTuVan) || LoaiTuVan == "ALL" || LoaiTuVan == "L2")
                    {
                        var listL2 = _BaoCaoTuVanCD45DA.GetBaoCaoTuVanL2(FromDate, ToDate, scopedCity, MaNhom, MaTCV);
                        var ws2 = wb.Worksheets.Add("TuVan_L2");
                        ExcelTuVanHelper.BuildTuVanL2Worksheet(ws2, listL2, FromDate, ToDate, TenNhom, TenTCV);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        wb.SaveAs(memoryStream);
                        var fileName = $"BaoCao_TuVan_CD45_{LoaiTuVan ?? "ALL"}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                        return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("<script>alert('Lỗi xuất Excel: " + ex.Message.Replace("'", "\\'") + "'); window.history.back();</script>", "text/html; charset=utf-8");
            }
        }
    }
}
