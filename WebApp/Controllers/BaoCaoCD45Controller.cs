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
        public JsonResult SearchBaoCao(string FromDate, string ToDate, string MaTinh, string MaNhom)
        {
            try
            {
                var data = _BaoCaoCD45DA.GetBaoCao(FromDate, ToDate, MaTinh, MaNhom, null);
                var jsonResult = Json(new { Success = true, Data = data });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                // TODO: Log error
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json(new { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetDrillDownData(string ChiTieuCode, string FromDate, string ToDate, string MaTinh, string MaNhom, int? DoiTuong)
        {
            try
            {
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

        [HttpPost]
        public ActionResult ExportExcel(string FromDate, string ToDate, string MaTinh, string MaNhom)
        {
            var data = _BaoCaoCD45DA.GetBaoCao(FromDate, ToDate, MaTinh, MaNhom, null);
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("BaoCao");
                ws.Cell(1, 1).Value = "BÁO CÁO KẾT QUẢ HOẠT ĐỘNG (DỰ ÁN CD45)";
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 14;

                ws.Cell(3, 1).Value = "STT";
                ws.Cell(3, 2).Value = "Thông tin báo cáo";
                ws.Cell(3, 3).Value = "Tổng";
                ws.Cell(3, 4).Value = "PUD";
                ws.Cell(3, 5).Value = "PLHIV";
                ws.Cell(3, 6).Value = "TG";
                ws.Cell(3, 7).Value = "SW";
                ws.Cell(3, 8).Value = "MSM";

                var headerRange = ws.Range("A3:H3");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 4;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.STT;
                    ws.Cell(row, 2).Value = item.ChiTieu;
                    ws.Cell(row, 3).Value = item.Tong ?? 0;
                    ws.Cell(row, 4).Value = item.PUD ?? 0;
                    ws.Cell(row, 5).Value = item.PLHIV ?? 0;
                    ws.Cell(row, 6).Value = item.TG ?? 0;
                    ws.Cell(row, 7).Value = item.SW ?? 0;
                    ws.Cell(row, 8).Value = item.MSM ?? 0;

                    if (item.IsBold)
                    {
                        ws.Range(row, 1, row, 8).Style.Font.Bold = true;
                        ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.Yellow;
                    }
                    row++;
                }
                
                ws.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCao_CD45_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
                }
            }
        }
    }
}

