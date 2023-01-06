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
using System.Data;
using System.IO;
using ClosedXML.Excel;
using log4net;

namespace WebApp.Controllers
{
    public class DGHLVIIVController : Controller
    {
        // GET: DGHLVIIV
        IDGHLVIIVDA _DGHLVIIVDA = new DGHLVIIVDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        IDuAnDA _DuAnDA = new DuAnDA();
        BaseController _helperController = new BaseController();
        
        public ActionResult Index()
        {
            var modelSearch = new ModelSearch
            {
                KeyWord = string.Empty,
                currentPage = 1,
                pageSize = int.MaxValue,
                SortColumn = "record_id"
            };
            var data = _DGHLVIIVDA.GetAllByPage(modelSearch);
            return View(data);
        }

        [HttpPost]
        public object GetItemByID(int Id)
        {
            try
            {
                var data = _DGHLVIIVDA.GetItemById(Id);
                AddLog("Lấy dữ liệu theo ID bảng kết quả DGHLVIIV ( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng kết quả DGHLVIIV( ID: " + Id + ") lỗi: " + ex.Message);
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

                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả DGHLVIIV thành công.");
                return Json(new { Buttoms = bottoms, Error = false, DuAns = duAns, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả DGHLVIIV lỗi: " + ex.Message);
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
                var data = _DGHLVIIVDA.GetAllByPage(modelSearch);

                string file_name = "DGHLVIIVDA_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".xlsx";

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[13] {
                        new DataColumn("Tỉnh"),
                        new DataColumn("Mã nhóm TBH"),
                        new DataColumn("Tên nhóm TBH"),
                        new DataColumn("Mã KH"),
                        new DataColumn("Họ tên KH"),
                        new DataColumn("Ngày thực hiện bảng hỏi"),
                        new DataColumn("Lần tư vấn thứ"),
                        new DataColumn("Câu 1: Mức độ hài lòng với trải nghiệm gặp tiếp cận viên. Theo thang từ 1 - 5 điểm, 1 là tệ nhất, 5 là tốt nhất, bạn hài lòng với trải nghiệm hôm nay ở mức nào?"),
                        new DataColumn("Câu 2: Mức độ được đối xử tôn trọng, thân thiện, không phán xét. Theo thang từ 1 - 5 điểm, 1 là tệ nhất, 5 là tốt nhất, Bạn cảm thấy mình được đối xử tôn trọng, thân thiện, không phán xét ở mức nào?"),
                        new DataColumn("Câu 3: Mức độ hài lòng với dịch vụ nhận được. Theo thang từ 1 - 5 điểm, 1 là tệ nhất, 5 là tốt nhất, dịch vụ bạn nhận được ngày hôm nay (vật phẩm, tư vấn, chuyển gửi đến các cơ sở dịch vụ...) đáp ứng được nhu cầu của bạn ở mức nào?"),
                        new DataColumn("Câu 4: Khả năng giới thiệu bạn bè đến nhóm. Theo thang từ 1 - 5 điểm, 1 là chắc chắn không giới thiệu, 5 là chắc chắn sẽ giới thiệu, Khả năng bạn sẽ giới thiệu một người bạn đến nhận dịch vụ tại nhóm Niềm tin xanh ở mức nào?"),
                        new DataColumn("Câu 5: địa điểm gặp an toàn. Theo thang từ 1 - 5 điểm, 1 là rất không an toàn, 5 là rất an toàn, bạn đánh giá địa điểm bạn gặp Tiếp cận viên như thế nào?"),
                        new DataColumn("Câu 6: Gợi ý để cải thiện chất lượng dịch vụ. Nếu có một điểm bạn muốn cải thiện ở dịch vụ ngày hôm nay bạn nhận được của nhóm Niềm tin xanh thì đó là gì?")
                });
                foreach (var item in data)
                {
                    dt.Rows.Add(
                        item.CityName,
                        item.manhom_tbh,
                        item.tennhom_tbh,
                        item.makh,
                        item.hoten,
                        item.ngay,
                        item.lantuvan,
                        item.cau1,
                        item.cau2,
                        item.cau3,
                        item.cau4,
                        item.cau5,
                        item.cau6
                        );
                }

                //Tên nhóm:,Mã KH,Ngày xét nghiệm:,Kết quả xét nghiệm ma túy đá,Kết quả xét nghiệm heroin

                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Viiv - Đánh giá hài lòng Khách hàng");
                    ws.Cell("A2").Value = "Viiv - ĐÁNH GIÁ HÀI LÒNG KHÁCH HÀNG";
                    ws.Range("A2:L2").Row(1).Merge();
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
                    ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Row(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Row(3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell(3, 1).InsertTable(dt);
                    AddLog("Viiv - ĐÁNH GIÁ HÀI LÒNG KHÁCH HÀNG");
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
                AddLog("Export Viiv - ĐÁNH GIÁ HÀI LÒNG KHÁCH HÀNG lỗi: " + ex.Message);
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
                        ControllerName = "DGHLVIIV",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }
    }
}