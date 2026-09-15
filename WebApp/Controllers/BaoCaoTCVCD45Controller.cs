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
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class BaoCaoTCVCD45Controller : BaseController
    {
        readonly ICityDA _CityDA;
        readonly IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA;
        readonly IBaoCaoCD45DA _BaoCaoCD45DA;

        public BaoCaoTCVCD45Controller(
            ICityDA cityDA,
            IBVTL_NHOM_TBHDA bvtlNhomTbhDA,
            IBaoCaoCD45DA baoCaoCD45DA)
        {
            _CityDA = cityDA;
            _BVTL_NHOM_TBHDA = bvtlNhomTbhDA;
            _BaoCaoCD45DA = baoCaoCD45DA;
        }

        [HasCredential(ControllerName = "BaoCaoTCVCD45")]
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
                var cities = _CityDA.GetAll().Where(x => new[] { "HNO", "HPG", "HYE", "NAN", "NBI", "HCM" }.Contains(x.Code)).Select(x => new { CityCode = x.Code, CityName = x.Name }).ToList();
                var nhoms = _BVTL_NHOM_TBHDA.GetAll().Where(x => x.maduan == "CD45")
                    .Select(x => new {
                        manhom_tbh = (x.manhom_tbh ?? "").Trim(),
                        tennhom_tbh = (x.tennhom_tbh ?? "").Trim(),
                        city_code = (x.city_code ?? "").Trim(),
                        manhom_tbh_map = (x.manhom_tbh_map ?? "").Trim()
                    }).ToList();
                var tcvs = _BaoCaoCD45DA.GetListTCV(null, null);
                return Json(new { Success = true, Cities = cities, Nhoms = nhoms, TCVs = tcvs });
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
                var tcvs = _BaoCaoCD45DA.GetListTCV(cityCode, maNhom);
                return Json(new { Success = true, TCVs = tcvs });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SearchBaoCao(string FromDate, string ToDate, string MaNhom, string MaTCV)
        {
            try
            {
                if (!ValidateDateRange(FromDate, ToDate, out var dateError))
                {
                    return Json(new { Success = false, Message = dateError });
                }

                var data = _BaoCaoCD45DA.GetBaoCao(FromDate, ToDate, null, MaNhom, MaTCV);
                return Json(new { Success = true, Data = data });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json(new { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExportSingleExcel(string FromDate, string ToDate, string MaNhom, string MaTCV, string TenTCV, string TenNhom)
        {
            if (!ValidateDateRange(FromDate, ToDate, out var dateError))
            {
                return Content("<script>alert('" + dateError.Replace("'", "\\'") + "'); window.history.back();</script>", "text/html; charset=utf-8");
            }

            var data = _BaoCaoCD45DA.GetBaoCao(FromDate, ToDate, null, MaNhom, MaTCV);
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("BaoCao");
                BuildTCVWorksheet(ws, data, FromDate, ToDate, TenNhom ?? MaNhom, TenTCV ?? MaTCV);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var fileName = "BaoCao_TCV_" + (TenTCV ?? MaTCV) + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExportExcel(string FromDate, string ToDate, string MaNhom, string MaTCV, string TenTCV, string TenNhom)
        {
            return ExportSingleExcel(FromDate, ToDate, MaNhom, MaTCV, TenTCV, TenNhom);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExportExcelZip(string FromDate, string ToDate, string DanhSachTCVJson)
        {
            try
            {
                if (!ValidateDateRange(FromDate, ToDate, out var dateError))
                {
                    return Content("<script>alert('" + dateError.Replace("'", "\\'") + "'); window.history.back();</script>", "text/html; charset=utf-8");
                }

                if (string.IsNullOrEmpty(DanhSachTCVJson)) return Content("Vui lòng chọn ít nhất 1 TCV.");

                var listTCV = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CD45_TCV_ItemModel>>(DanhSachTCVJson);
                if (listTCV == null || listTCV.Count == 0) return Content("Không có TCV nào được chọn.");

                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var tcv in listTCV)
                        {
                            var data = _BaoCaoCD45DA.GetBaoCao(FromDate, ToDate, null, tcv.MA_NHOM, tcv.MA_TCV);

                            using (var wb = new XLWorkbook())
                            {
                                var ws = wb.Worksheets.Add("BaoCao");
                                BuildTCVWorksheet(ws, data, FromDate, ToDate, tcv.TEN_NHOM ?? tcv.MA_NHOM, tcv.TEN_TCV ?? tcv.MA_TCV);

                                var cleanName = (tcv.TEN_TCV ?? ("TCV_" + tcv.MA_TCV)).Replace("/", "_").Replace("\\", "_");
                                var zipEntry = archive.CreateEntry("BaoCao_" + (tcv.MA_NHOM ?? "CD45") + "_" + cleanName + ".xlsx", CompressionLevel.Fastest);
                                using (var zipStream = zipEntry.Open())
                                {
                                    wb.SaveAs(zipStream);
                                }
                            }
                        }
                    }
                    return File(memoryStream.ToArray(), "application/zip", "BaoCao_TCV_CD45_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".zip");
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi xuất file ZIP: " + ex.Message);
            }
        }

        private void BuildTCVWorksheet(IXLWorksheet ws, List<BaoCaoCD45Model> data, string fromDate, string toDate, string tenNhom, string tenTCV)
        {
            // Title Header
            ws.Cell("A1").Value = "BÁO CÁO HOẠT ĐỘNG - DỰ ÁN CD45";
            ws.Range("A1:H1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = $"Kỳ báo cáo: Từ {fromDate} đến {toDate}";
            ws.Range("A2:H2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Italic = true;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A3").Value = $"Nhóm: {tenNhom} | Tiếp cận viên: {tenTCV}";
            ws.Range("A3:H3").Row(1).Merge();
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Header Table
            ws.Cell("A5").Value = "#";
            ws.Cell("B5").Value = "Thông tin báo cáo";
            ws.Cell("C5").Value = "Tổng";
            ws.Cell("D5").Value = "PUD";
            ws.Cell("E5").Value = "PLHIV";
            ws.Cell("F5").Value = "TG";
            ws.Cell("G5").Value = "SW";
            ws.Cell("H5").Value = "MSM";

            var headerRange = ws.Range("A5:H5");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8ECEF");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            Action<IXLCell, int?> setVal = (c, val) =>
            {
                if (val.HasValue && val.Value > 0)
                {
                    c.Value = val.Value;
                    c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                else
                {
                    c.Value = "-";
                    c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            };

            int row = 6;
            foreach (var item in data)
            {
                ws.Cell(row, 1).Value = item.STT;
                ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var prefix = item.IndentLevel == 1 ? "      " : "";
                ws.Cell(row, 2).Value = prefix + item.ChiTieu;

                if (item.IsBold)
                {
                    for (int c = 3; c <= 8; c++) ws.Cell(row, c).Value = "";
                    ws.Range(row, 1, row, 8).Style.Font.Bold = true;
                    ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF3CD");
                }
                else
                {
                    setVal(ws.Cell(row, 3), item.Tong);
                    setVal(ws.Cell(row, 4), item.PUD);
                    setVal(ws.Cell(row, 5), item.PLHIV);
                    setVal(ws.Cell(row, 6), item.TG);
                    setVal(ws.Cell(row, 7), item.SW);
                    setVal(ws.Cell(row, 8), item.MSM);
                }
                row++;
            }

            // Borders
            var dataTableRange = ws.Range(5, 1, row - 1, 8);
            dataTableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataTableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Signature Footer
            row += 2;
            ws.Cell(row, 2).Value = "Tiếp cận viên";
            ws.Cell(row, 2).Style.Font.Bold = true;
            ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(row, 5).Value = "Cán bộ dự án";
            ws.Cell(row, 5).Style.Font.Bold = true;
            ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(row, 8).Value = "MnE";
            ws.Cell(row, 8).Style.Font.Bold = true;
            ws.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Columns().AdjustToContents();
        }
    }
}
