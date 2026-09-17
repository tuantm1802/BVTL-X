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

namespace WebApp.Controllers
{
    public class BaoCaoCD45Controller : BaseController
    {
        readonly ICityDA _CityDA;
        readonly IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA;
        readonly IBaoCaoCD45DA _BaoCaoCD45DA;
        readonly IDuAnDA _DuAnDA;

        public BaoCaoCD45Controller(
            ICityDA cityDA,
            IBVTL_NHOM_TBHDA bvtlNhomTbhDA,
            IBaoCaoCD45DA baoCaoCD45DA,
            IDuAnDA duAnDA)
        {
            _CityDA = cityDA;
            _BVTL_NHOM_TBHDA = bvtlNhomTbhDA;
            _BaoCaoCD45DA = baoCaoCD45DA;
            _DuAnDA = duAnDA;
        }

        [HasCredential(ControllerName = "BaoCaoCD45")]
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
                var nhoms = _BVTL_NHOM_TBHDA.GetAll().Where(x => x.maduan == "CD45").ToList();
                return Json(new { Success = true, Cities = cities, Nhoms = nhoms });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SearchBaoCao(string FromDate, string ToDate, string MaTinh, string MaNhom, string LoaiBaoCao)
        {
            try
            {
                if (!ValidateDateRange(FromDate, ToDate, out var dateError))
                {
                    return Json(new { Success = false, Message = dateError });
                }

                // Khi LoaiBaoCao = "TuyChon" hoặc rỗng → truyền null để hiển thị tất cả chỉ tiêu
                string loaiFilter = (LoaiBaoCao == "TuyChon" || string.IsNullOrEmpty(LoaiBaoCao)) ? null : LoaiBaoCao;
                var data = _BaoCaoCD45DA.GetBaoCao(FromDate, ToDate, MaTinh, MaNhom, null, loaiFilter);

                // VR-01: Kiểm toán cấu trúc số học (Tổng = PUD + PLHIV + TG + SW + MSM)
                ReportValidatorHelper.ValidateReportArithmetic(data, out string arithmeticWarning);

                var jsonResult = Json(new { Success = true, Data = data, Warning = arithmeticWarning });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json(new { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetDrillDownData(string ChiTieuCode, string FromDate, string ToDate, string MaTinh, string MaNhom, int? DoiTuong)
        {
            try
            {
                if (!ValidateDateRange(FromDate, ToDate, out var dateError))
                {
                    return Json(new { Success = false, Message = dateError });
                }

                var list = _BaoCaoCD45DA.GetDrillDown(ChiTieuCode, FromDate, ToDate, MaTinh, MaNhom, null, DoiTuong);
                var jsonResult = Json(new { Success = true, Data = list, Total = list.Count });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json(new { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExportExcel(string FromDate, string ToDate, string MaTinh, string MaNhom, string LoaiBaoCao)
        {
            if (!ValidateDateRange(FromDate, ToDate, out var dateError))
            {
                return Content("<script>alert('" + dateError.Replace("'", "\\'") + "'); window.history.back();</script>", "text/html; charset=utf-8");
            }

            // Khi LoaiBaoCao = "TuyChon" hoặc rỗng → hiển thị tất cả chỉ tiêu
            string loaiFilter = (LoaiBaoCao == "TuyChon" || string.IsNullOrEmpty(LoaiBaoCao)) ? null : LoaiBaoCao;
            var data = _BaoCaoCD45DA.GetBaoCao(FromDate, ToDate, MaTinh, MaNhom, null, loaiFilter);

            // VR-01 [BLOCKING]: Chặn xuất báo cáo nếu có bất kỳ dòng nào vi phạm Tổng = 5 nhóm đích
            if (!ReportValidatorHelper.ValidateReportArithmetic(data, out var arithmeticError))
            {
                return Content("<script>alert('" + arithmeticError.Replace("'", "\\'") + "'); window.history.back();</script>", "text/html; charset=utf-8");
            }

            // Nhãn và tiêu đề kỳ báo cáo
            string kyLabel = loaiFilter == null ? "TuyChon" :
                             loaiFilter == "Thang" ? "Thang" :
                             loaiFilter == "Quy" ? "Quy" :
                             loaiFilter == "6T" ? "6Thang" : "Nam12T";
            string kyTitle = loaiFilter == null ? "Tùy chọn ngày" :
                             loaiFilter == "Thang" ? "Báo cáo Tháng" :
                             loaiFilter == "Quy" ? "Báo cáo Quý" :
                             loaiFilter == "6T" ? "Báo cáo 6 Tháng" : "Báo cáo Năm (12T)";

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("BaoCao");
                ws.Cell(1, 1).Value = "BÁO CÁO KẾT QUẢ HOẠT ĐỘNG (DỰ ÁN CD45 - DREAMH)";
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 14;
                ws.Range("A1:H1").Row(1).Merge();

                ws.Cell(2, 1).Value = "Kỳ báo cáo: " + kyTitle + "   |   Từ ngày: " + FromDate + " đến ngày: " + ToDate;
                ws.Cell(2, 1).Style.Font.Italic = true;
                ws.Range("A2:H2").Row(1).Merge();

                ws.Cell(4, 1).Value = "STT";
                ws.Cell(4, 2).Value = "Thông tin báo cáo";
                ws.Cell(4, 3).Value = "Tổng";
                ws.Cell(4, 4).Value = "PUD";
                ws.Cell(4, 5).Value = "PLHIV";
                ws.Cell(4, 6).Value = "TG";
                ws.Cell(4, 7).Value = "SW";
                ws.Cell(4, 8).Value = "MSM";

                var headerRange = ws.Range("A4:H4");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

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

                int row = 5;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.STT;
                    ws.Cell(row, 2).Value = item.ChiTieu;

                    if (item.IsBold)
                    {
                        for (int c = 3; c <= 8; c++) ws.Cell(row, c).Value = "";
                        ws.Range(row, 1, row, 8).Style.Font.Bold = true;
                        ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.Yellow;
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

                ws.Columns().AdjustToContents();

                string fileName = "BaoCao_CD45_" + kyLabel + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // ========== QUẢN TRỊ CẤU HÌNH CHỈ TIÊU ==========

        [HasCredential(ControllerName = "BaoCaoCD45")]
        public ActionResult CauHinhChiTieu()
        {
            var user = Session["USER_SESSION"] as UserLogin;
            if (user == null) return Redirect("/Login/Index");
            return View();
        }

        [HttpPost]
        public JsonResult GetCauHinhChiTieu()
        {
            try
            {
                var data = _BaoCaoCD45DA.GetCauHinhChiTieu();
                return Json(new { Success = true, Data = data });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi tải cấu hình: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveCauHinhChiTieu(List<CD45_BcTieuCauHinhModel> items)
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                string updatedBy = user?.UserName ?? "System";
                bool ok = _BaoCaoCD45DA.SaveCauHinhChiTieu(items, updatedBy);
                return Json(new { Success = ok, Message = ok ? "Lưu cấu hình thành công!" : "Không có dữ liệu để lưu." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi lưu cấu hình: " + ex.Message });
            }
        }
    }
}
