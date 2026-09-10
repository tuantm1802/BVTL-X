using ClosedXML.Excel;
using Common.Common;
using Data.InterfaceDA.Admin;
using Model.ModelExtend.Base;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class NhomTCVCD45Controller : BaseController
    {
        private readonly ICD45NhomTcvDA _nhomTcvDA;
        private readonly ICityDA _cityDA;
        private readonly IBVTL_NHOM_TBHDA _nhomDA;

        public NhomTCVCD45Controller(
            ICD45NhomTcvDA nhomTcvDA,
            ICityDA cityDA,
            IBVTL_NHOM_TBHDA nhomDA)
        {
            _nhomTcvDA = nhomTcvDA;
            _cityDA = cityDA;
            _nhomDA = nhomDA;
        }

        [HasCredential(ControllerName = "NhomTCVCD45")]
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
        public JsonResult GetList(string cityCode, string maNhom, string keyword)
        {
            try
            {
                var list = _nhomTcvDA.GetListNhomTcv(cityCode, maNhom, keyword);
                var kpi = _nhomTcvDA.GetKpiStats();
                return Json(new { Success = true, Data = list, Kpi = kpi });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi khi lấy danh sách: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ExportExcel(string cityCode, string maNhom)
        {
            try
            {
                var data = _nhomTcvDA.GetAllForExport(cityCode, maNhom);

                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Nhom_TCV_CD45");
                    ws.Cell(1, 1).Value = "DANH SÁCH TIẾP CẬN VIÊN & NHÓM - DỰ ÁN CD45 (DREAMH)";
                    ws.Cell(1, 1).Style.Font.Bold = true;
                    ws.Cell(1, 1).Style.Font.FontSize = 14;

                    var headers = new string[] { "STT", "Tỉnh/Thành", "Mã Nhóm", "Tên Nhóm CBO", "Mã TCV", "Tên Tiếp Cận Viên", "Trạng thái", "Ngày cập nhật" };

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
                        ws.Cell(row, 2).Value = item.CityName ?? item.CITY_CODE;
                        ws.Cell(row, 3).Value = item.MA_NHOM;
                        ws.Cell(row, 4).Value = item.TEN_NHOM;
                        ws.Cell(row, 5).Value = item.MA_TCV;
                        ws.Cell(row, 6).Value = item.TEN_TCV;
                        ws.Cell(row, 7).Value = item.IsActive ? "Đang hoạt động" : "Ngừng hoạt động";
                        ws.Cell(row, 8).Value = item.CreatedDate.ToString("dd/MM/yyyy HH:mm");
                        row++;
                    }

                    ws.Columns().AdjustToContents();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSach_TCV_CD45_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
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
