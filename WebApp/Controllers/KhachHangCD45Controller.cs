using ClosedXML.Excel;
using Common.Common;
using Data.InterfaceDA.Admin;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class KhachHangCD45Controller : BaseController
    {
        private readonly ICD45KhachHangDA _khachHangDA;
        private readonly ICityDA _cityDA;
        private readonly IBVTL_NHOM_TBHDA _nhomDA;

        public KhachHangCD45Controller(
            ICD45KhachHangDA khachHangDA,
            ICityDA cityDA,
            IBVTL_NHOM_TBHDA nhomDA)
        {
            _khachHangDA = khachHangDA;
            _cityDA = cityDA;
            _nhomDA = nhomDA;
        }

        [HasCredential(ControllerName = "KhachHangCD45")]
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
                var cities = _cityDA.GetAll()
                                    .Where(x => new[] { "HNO", "HPG", "HYE", "NAN", "NBI", "HCM" }.Contains(x.Code))
                                    .Select(x => new { CityCode = x.Code, CityName = x.Name })
                                    .ToList();

                var nhoms = _nhomDA.GetAll()
                                   .Where(x => x.maduan == "CD45")
                                   .Select(x => new { 
                                       MaNhom = !string.IsNullOrEmpty(x.manhom_tbh_map) ? x.manhom_tbh_map : x.manhom_tbh, 
                                       TenNhom = x.tennhom_tbh, 
                                       CityCode = x.city_code 
                                   })
                                   .ToList();

                return Json(new { Success = true, Cities = cities, Nhoms = nhoms });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SearchCustomers(CD45KhachHangFilterModel filter)
        {
            try
            {
                var res = _khachHangDA.GetPagingCustomers(filter);
                return Json(new { Success = true, Data = res.data, Total = res.recordsTotal });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi khi lấy danh sách: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetCustomerDetail(string recordId)
        {
            try
            {
                var detail = _khachHangDA.GetCustomerDetail(recordId);
                if (detail == null)
                {
                    return Json(new { Success = false, Message = "Không tìm thấy hồ sơ khách hàng!" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Success = true, Data = detail }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi khi lấy chi tiết: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ExportExcel(string keyword, string cityCode, string maNhom, byte? doiTuong, bool? coBHYT, bool? coCCCD, string fromDate, string toDate)
        {
            try
            {
                var filter = new CD45KhachHangFilterModel
                {
                    Keyword = keyword,
                    CityCode = cityCode,
                    MaNhom = maNhom,
                    DoiTuong = doiTuong,
                    CoBHYT = coBHYT,
                    CoCCCD = coCCCD,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                var data = _khachHangDA.GetAllForExport(filter);

                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("KhachHangCD45");
                    ws.Cell(1, 1).Value = "DANH SÁCH KHÁCH HÀNG - DỰ ÁN CD45 (DREAMH)";
                    ws.Cell(1, 1).Style.Font.Bold = true;
                    ws.Cell(1, 1).Style.Font.FontSize = 14;

                    var headers = new string[]
                    {
                        "STT", "Mã Record ID", "Mã KH Nghiên cứu", "Tỉnh/Thành", "Mã Nhóm", "Tên Nhóm CBO",
                        "Ngày tham gia", "Đối tượng", "Giới tính", "Năm sinh", "Tuổi", "Có CCCD", "Có BHYT",
                        "Có thường trú", "Đang vô gia cư", "Bị tạm giữ 6T", "Trạng thái"
                    };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cell(3, i + 1).Value = headers[i];
                    }

                    var headerRange = ws.Range(3, 1, 3, headers.Length);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8ECEF");

                    int row = 4;
                    int stt = 1;
                    foreach (var item in data)
                    {
                        ws.Cell(row, 1).Value = stt++;
                        ws.Cell(row, 2).Value = item.RECORD_ID;
                        ws.Cell(row, 3).Value = item.MA_KH_NGHIEN_CUU ?? "";
                        ws.Cell(row, 4).Value = item.CityName ?? item.CITY_CODE;
                        ws.Cell(row, 5).Value = item.MA_NHOM ?? "";
                        ws.Cell(row, 6).Value = item.TenNhom ?? "";
                        ws.Cell(row, 7).Value = item.NGAY_THAM_GIA.HasValue ? item.NGAY_THAM_GIA.Value.ToString("dd/MM/yyyy") : "";
                        ws.Cell(row, 8).Value = item.TenDoiTuong ?? "";
                        ws.Cell(row, 9).Value = item.TenGioiTinh ?? "";
                        ws.Cell(row, 10).Value = item.NAM_SINH.HasValue ? item.NAM_SINH.Value.ToString() : "";
                        ws.Cell(row, 11).Value = item.Tuoi.HasValue ? item.Tuoi.Value.ToString() : "";
                        ws.Cell(row, 12).Value = item.CO_CCCD == true ? "Có" : "Không";
                        ws.Cell(row, 13).Value = item.CO_BHYT == true ? "Có" : "Không";
                        ws.Cell(row, 14).Value = item.CO_THUONG_TRU == true ? "Có" : "Không";
                        ws.Cell(row, 15).Value = item.DANG_VO_GIA_CU == true ? "Có" : "Không";
                        ws.Cell(row, 16).Value = item.BI_TAM_GIU_6T == true ? "Có" : "Không";
                        ws.Cell(row, 17).Value = item.COMPLETE_STATUS ?? "";

                        row++;
                    }

                    ws.Columns().AdjustToContents();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSach_KH_CD45_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi khi xuất Excel: " + ex.Message);
            }
        }
    }
}
