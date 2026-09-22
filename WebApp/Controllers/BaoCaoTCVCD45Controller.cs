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
        public JsonResult SearchBaoCao(string FromDate, string ToDate, string MaNhom, string MaTCV, string LoaiBaoCao = null)
        {
            try
            {
                if (!ValidateDateRange(FromDate, ToDate, out var dateError))
                {
                    return Json(new { Success = false, Message = dateError });
                }

                string loaiFilter = (LoaiBaoCao == "TuyChon" || string.IsNullOrEmpty(LoaiBaoCao)) ? null : LoaiBaoCao;
                var data = _BaoCaoCD45DA.GetBaoCao(FromDate, ToDate, null, MaNhom, MaTCV, loaiFilter);
                return Json(new { Success = true, Data = data });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json(new { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExportSingleExcel(string FromDate, string ToDate, string MaNhom, string MaTCV, string TenTCV, string TenNhom, string LoaiBaoCao = null)
        {
            if (!ValidateDateRange(FromDate, ToDate, out var dateError))
            {
                return Content("<script>alert('" + dateError.Replace("'", "\\'") + "'); window.history.back();</script>", "text/html; charset=utf-8");
            }

            string loaiFilter = (LoaiBaoCao == "TuyChon" || string.IsNullOrEmpty(LoaiBaoCao)) ? null : LoaiBaoCao;
            var data = _BaoCaoCD45DA.GetBaoCao(FromDate, ToDate, null, MaNhom, MaTCV, loaiFilter);

            string kyLabel = loaiFilter == null ? "TuyChon" :
                             loaiFilter == "Thang" ? "Thang" :
                             loaiFilter == "Quy" ? "Quy" :
                             loaiFilter == "6T" ? "6Thang" : "Nam12T";
            string kyTitle = loaiFilter == null ? "Tùy chọn ngày" :
                             loaiFilter == "Thang" ? "Báo cáo Tháng" :
                             loaiFilter == "Quy" ? "Báo cáo Quý" :
                             loaiFilter == "6T" ? "Báo cáo 6 Tháng" : "Báo cáo Năm (12T)";

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("BaoCao");
                BuildTCVWorksheet(ws, data, FromDate, ToDate, TenNhom ?? MaNhom, TenTCV ?? MaTCV, kyTitle);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var cleanTcv = (TenTCV ?? MaTCV ?? "TCV").Replace("/", "_").Replace("\\", "_");
                    var fileName = "BaoCao_TCV_" + cleanTcv + "_" + kyLabel + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExportExcel(string FromDate, string ToDate, string MaNhom, string MaTCV, string TenTCV, string TenNhom, string LoaiBaoCao = null)
        {
            return ExportSingleExcel(FromDate, ToDate, MaNhom, MaTCV, TenTCV, TenNhom, LoaiBaoCao);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExportExcelZip(string FromDate, string ToDate, string DanhSachTCVJson, string LoaiBaoCao = null)
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

                string loaiFilter = (LoaiBaoCao == "TuyChon" || string.IsNullOrEmpty(LoaiBaoCao)) ? null : LoaiBaoCao;
                string kyLabel = loaiFilter == null ? "TuyChon" :
                                 loaiFilter == "Thang" ? "Thang" :
                                 loaiFilter == "Quy" ? "Quy" :
                                 loaiFilter == "6T" ? "6Thang" : "Nam12T";
                string kyTitle = loaiFilter == null ? "Tùy chọn ngày" :
                                 loaiFilter == "Thang" ? "Báo cáo Tháng" :
                                 loaiFilter == "Quy" ? "Báo cáo Quý" :
                                 loaiFilter == "6T" ? "Báo cáo 6 Tháng" : "Báo cáo Năm (12T)";

                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var tcv in listTCV)
                        {
                            var data = _BaoCaoCD45DA.GetBaoCao(FromDate, ToDate, null, tcv.MA_NHOM, tcv.MA_TCV, loaiFilter);

                            using (var wb = new XLWorkbook())
                            {
                                var ws = wb.Worksheets.Add("BaoCao");
                                BuildTCVWorksheet(ws, data, FromDate, ToDate, tcv.TEN_NHOM ?? tcv.MA_NHOM, tcv.TEN_TCV ?? tcv.MA_TCV, kyTitle);

                                var cleanName = (tcv.TEN_TCV ?? ("TCV_" + tcv.MA_TCV)).Replace("/", "_").Replace("\\", "_");
                                var zipEntry = archive.CreateEntry("BaoCao_" + (tcv.MA_NHOM ?? "CD45") + "_" + cleanName + "_" + kyLabel + ".xlsx", CompressionLevel.Fastest);
                                using (var zipStream = zipEntry.Open())
                                {
                                    wb.SaveAs(zipStream);
                                }
                            }
                        }
                    }
                    return File(memoryStream.ToArray(), "application/zip", "BaoCao_TCV_CD45_" + kyLabel + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".zip");
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi xuất file ZIP: " + ex.Message);
            }
        }

        private void BuildTCVWorksheet(IXLWorksheet ws, List<BaoCaoCD45Model> data, string fromDate, string toDate, string tenNhom, string tenTCV, string kyTitle = null)
        {
            // Title Header
            ws.Cell("A1").Value = "BÁO CÁO HOẠT ĐỘNG - DỰ ÁN CD45";
            ws.Range("A1:H1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            string kyLine = string.IsNullOrEmpty(kyTitle)
                ? $"Kỳ báo cáo: Từ {fromDate} đến {toDate}"
                : $"Kỳ báo cáo: {kyTitle}   |   Từ ngày: {fromDate} đến ngày: {toDate}";
            ws.Cell("A2").Value = kyLine;
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
            int signTitleRow = row;
            int signNoteRow = row + 1;
            int signNameRow = row + 5;

            // 1. Khối chữ ký "Tiếp cận viên" (Merge cột A:B)
            ws.Range(signTitleRow, 1, signTitleRow, 2).Merge();
            ws.Cell(signTitleRow, 1).Value = "Tiếp cận viên";
            ws.Cell(signTitleRow, 1).Style.Font.Bold = true;
            ws.Cell(signTitleRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Range(signNoteRow, 1, signNoteRow, 2).Merge();
            ws.Cell(signNoteRow, 1).Value = "(Ký, ghi rõ họ tên)";
            ws.Cell(signNoteRow, 1).Style.Font.Italic = true;
            ws.Cell(signNoteRow, 1).Style.Font.FontSize = 9;
            ws.Cell(signNoteRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            if (!string.IsNullOrEmpty(tenTCV))
            {
                ws.Range(signNameRow, 1, signNameRow, 2).Merge();
                ws.Cell(signNameRow, 1).Value = tenTCV;
                ws.Cell(signNameRow, 1).Style.Font.Bold = true;
                ws.Cell(signNameRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // 2. Khối chữ ký "Cán bộ dự án" (Merge cột C:E)
            ws.Range(signTitleRow, 3, signTitleRow, 5).Merge();
            ws.Cell(signTitleRow, 3).Value = "Cán bộ dự án";
            ws.Cell(signTitleRow, 3).Style.Font.Bold = true;
            ws.Cell(signTitleRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Range(signNoteRow, 3, signNoteRow, 5).Merge();
            ws.Cell(signNoteRow, 3).Value = "(Ký, ghi rõ họ tên)";
            ws.Cell(signNoteRow, 3).Style.Font.Italic = true;
            ws.Cell(signNoteRow, 3).Style.Font.FontSize = 9;
            ws.Cell(signNoteRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // 3. Khối chữ ký "MnE" (Merge cột F:H)
            ws.Range(signTitleRow, 6, signTitleRow, 8).Merge();
            ws.Cell(signTitleRow, 6).Value = "MnE";
            ws.Cell(signTitleRow, 6).Style.Font.Bold = true;
            ws.Cell(signTitleRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Range(signNoteRow, 6, signNoteRow, 8).Merge();
            ws.Cell(signNoteRow, 6).Value = "(Ký, ghi rõ họ tên)";
            ws.Cell(signNoteRow, 6).Style.Font.Italic = true;
            ws.Cell(signNoteRow, 6).Style.Font.FontSize = 9;
            ws.Cell(signNoteRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Thiết lập độ rộng cột (Column Widths)
            ws.Column(1).Width = 5.5;  // Cột # (STT)
            ws.Column(2).Width = 44.0; // Cột Thông tin báo cáo
            ws.Column(2).Style.Alignment.WrapText = true;

            // Các cột số liệu từ C đến H (Tổng, PUD, PLHIV, TG, SW, MSM) có khoảng cách bằng nhau tuyệt đối
            for (int c = 3; c <= 8; c++)
            {
                ws.Column(c).Width = 10.0;
            }

            // Cấu hình trang in chuẩn A4 dọc vừa vặn trong 1 trang ngang
            ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
            ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;
            ws.PageSetup.FitToPages(1, 0);
            ws.PageSetup.Margins.Left = 0.4;
            ws.PageSetup.Margins.Right = 0.4;
            ws.PageSetup.Margins.Top = 0.6;
            ws.PageSetup.Margins.Bottom = 0.6;
        }
    }
}
