using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend.Base;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.Data;
using System.IO;

namespace WebApp.Controllers
{
    public class TrainingDataCollVIIVController : Controller
    {
        // GET: TrainingDataColl
        ITrainingDataCollVIIVDA _TrainingDataCollVIIVDA = new TrainingDataCollVIIVDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        IDuAnDA _DuAnDA = new DuAnDA();
        BaseController _helperController = new BaseController();
        ICityDA _CityDA = new CityDA();
        public ActionResult Index()
        {
            try
            {
                // Kiểm tra quyền 
                var modelSearch = new ModelSearch
                {
                    KeyWord = string.Empty,
                    currentPage = 1,
                    pageSize = int.MaxValue,
                    SortColumn = "record_id"
                };
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);
                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);

                if (user.IsAdmin || (duAn != null && user.MaDuAn.Contains(duAn.maduan)))
                {
                    modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";

                    var citys = _CityDA.GetCityReport((int)user.UserID);
                    if (citys != null && citys.Count > 0)
                        modelSearch.CityCodes = string.Join(",", citys.Select(x => x.Code));
                    var data = _TrainingDataCollVIIVDA.GetAllByPage(modelSearch);
                    return View(data);
                }
                else
                    return Redirect("/ErrorPage/Error404");
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
                return Redirect("/ErrorPage/Error404");
            }
        }

        [HttpPost]
        public object GetItemByID(int Id)
        {
            try
            {
                var data = _TrainingDataCollVIIVDA.GetItemById(Id);
                AddLog("Lấy dữ liệu theo ID bảng kết quả TrainingDataCollVIIV ( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng kết quả TrainingDataCollVIIV( ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetBottomAction()
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var menu = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var bottoms = _helperController.GetBottomRoleByController(controllerName, menu);
                // Lấy danh sách du an
                var duAns = _DuAnDA.GetAll().Select(x => new { Code = x.maduan, Name = x.tenduan }).ToList();

                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả TrainingDataCollVIIV thành công.");
                return Json(new { Buttoms = bottoms, Error = false, DuAns = duAns, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả TrainingDataCollVIIV lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        #region Xuất dữ liệu ra excel
        [HttpGet]
        public ActionResult ExportData(string keyword)
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                var modelSearch = new ModelSearch
                {
                    KeyWord = keyword == "undefined" ? string.Empty : keyword,
                    currentPage = 1,
                    pageSize = int.MaxValue,
                    SortColumn = "record_id"
                };
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";

                var citys = _CityDA.GetCityReport((int)user.UserID);
                if (citys != null && citys.Count > 0)
                    modelSearch.CityCodes = string.Join(",", citys.Select(x => x.Code));
                var data = _TrainingDataCollVIIVDA.GetAllByPage(modelSearch);

                string file_name = "TrainingDataCollVIIV_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".xlsx";

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[26] {
                         new DataColumn("Tỉnh"),
                         new DataColumn("Thời gian tổ chức"),
                         new DataColumn("Đối tượng tập huấn"),
                         new DataColumn("Loại tập huấn"),
                         new DataColumn("Khác"),
                         new DataColumn("Nội dung tập huấn"),
                         new DataColumn("Nhà tài trợ"),
                         new DataColumn("Nhân viên NGOs/ Tổ chức làm về giảm hại"),
                         new DataColumn("Nhân viên tiếp cận cộng đồng"),
                         new DataColumn("Cán bộ Cơ quan quản lý nhà nước"),
                         new DataColumn("Nhân viên tư vấn"),
                         new DataColumn("Cán bộ y tế"),
                         new DataColumn("Cán bộ kỹ thuật SCDI - Trainers"),
                         new DataColumn("Khác (Nêu rõ)"),
                         new DataColumn("Số lượng tổ chức tham gia tập huấn"),
                         new DataColumn("Nhà tài trợ"),
                         new DataColumn("NGOs/ Tổ chức làm về giảm hại"),
                         new DataColumn("Tổ chức/ Mạng lưới Cộng đồng"),
                         new DataColumn("Cơ quan quản lý nhà nước"),
                         new DataColumn("Cơ sở cung cấp dịch vụ: (Phòng methadone, HIV...)"),
                         new DataColumn("Trung tâm cai nghiện"),
                         new DataColumn("Bệnh viện"),
                         new DataColumn("SCDI"),
                         new DataColumn("Khác (nêu rõ tên và số lượng)"),
                         new DataColumn("Các tỉnh tham gia tập huấn"),
                         new DataColumn("Các nước tham gia tập huấn")

                });
                foreach (var item in data)
                {
                    dt.Rows.Add(
                        item.CityName,
                        item.ngaythtext,
                        item.doituong,
                        item.taphuan,
                        item.khac,
                        item.noidung,
                        item.nhataitro,
                        item.nvngo,
                        item.cbcqnn,
                        item.nvtv,
                        item.cbyt,
                        item.scdi,
                        item.khac1,
                        item.tochuc1,
                        item.nhataitro_2,
                        item.ngo,
                        item.tochuc1,
                        item.qlnn,
                        item.ccdv,
                        item.ttcn,
                        item.bc,
                        item.scdi1,
                        item.scdi2,
                        item.tinh,
                        item.tinh_2
                        );
                }

                //Tên nhóm:,Mã KH,Ngày xét nghiệm:,Kết quả xét nghiệm ma túy đá,Kết quả xét nghiệm heroin

                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Viiv - Training Data Collection");
                    ws.Cell("A2").Value = "Viiv - TRAINING DATA COLLECTION";
                    ws.Range("A2:AA2").Row(1).Merge();
                    ws.Cell("A2").Style.Font.Bold = true;
                    ws.Cell("A2").Style.Font.FontSize = 20;
                    ws.Column("A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("K").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("K").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("L").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("L").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("M").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("M").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("N").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("N").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("O").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("O").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("P").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("P").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("Q").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("Q").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("R").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("R").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("T").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("T").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("U").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("U").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("V").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("V").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("W").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("W").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("X").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("X").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Row(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Row(3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell(3, 1).InsertTable(dt);
                    AddLog("Viiv - TRAINING DATA COLLECTION");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        ws.Columns(1, 10).AdjustToContents();
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog("Export Viiv - Training Data Collection lỗi: " + ex.Message);
                return Json(new { message = "Lỗi xử lý dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "TrainingDataCollVIIV",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }
    }
}