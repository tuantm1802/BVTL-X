using ClosedXML.Excel;
using Common;
using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using DocumentFormat.OpenXml.Office2010.Excel;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class KhachHangController : BaseController
    {
        readonly ICustomerDA _CustomerDA;
        readonly ISysLogDA _sysLogDA;
        readonly IDuAnDA _DuAnDA;
        BaseController _helperController = new BaseController();
        private BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        public KhachHangController(ICustomerDA customerDA, ISysLogDA sysLogDA, IDuAnDA duAnDA)
        {
            _CustomerDA = customerDA;
            _sysLogDA = sysLogDA;
            _DuAnDA = duAnDA;
        }

        // GET: Customer
        [System.Web.Mvc.HttpPost]
        [HasCredential(ControllerName = "KhachHang")]
        public JsonResult GetCustomers([FromBody] DataTableRequest request)
        {
            
            DataTableResponse<Customer> customers = _CustomerDA.GetAllCustomers(request);
            
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetCustomerDetails(int id)
        {
            var customer = _CustomerDA.GetCustomerById(id);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetCustomerChuyenGuiById(int id)
        {
            var customer = _CustomerDA.GetCustomerChuyenGuiById(id);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetCustomerSuDungChatById(int id)
        {
            var customer = _CustomerDA.GetCustomerSuDungChatById(id);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetCustomerQHTDById(int id)
        {
            var customer = _CustomerDA.GetCustomerQHTDById(id);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetDichVuChuyenGuiByCustomerId(int id, string recordid)
        {
            var customer = _CustomerDA.GetDichVuChuyenGuiByCustomerId(id, recordid);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
        
        [System.Web.Mvc.HttpGet]
        public JsonResult GetSinhHoatNhomByCustomerId(int id, string recordid)
        {
            var customer = _CustomerDA.GetSinhHoatNhomByCustomerId(id, recordid);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
        
        [System.Web.Mvc.HttpGet]
        public JsonResult GetPhieuTuVanCustomerId(int id, string recordid)
        {
            var customer = _CustomerDA.GetPhieuTuVanCustomerId(id, recordid);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
        
        [System.Web.Mvc.HttpGet]
        public JsonResult GetListPhieuTuVanCustomerId(int id, string recordid)
        {
            var customer = _CustomerDA.GetListPhieuTuVanCustomerId(id, recordid);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
        
        [System.Web.Mvc.HttpGet]
        public JsonResult GetListKhamVaDieuTriSKTTCustomerId(int id, string recordid)
        {
            var customer = _CustomerDA.GetListKhamVaDieuTriSKTTCustomerId(id, recordid);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
        
        [System.Web.Mvc.HttpGet]
        public JsonResult GetKhamVaDieuTriCustomerId(int id, string recordid)
        {
            var customer = _CustomerDA.GetKhamVaDieuTriCustomerId(id, recordid);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id)
        {
            var customer = db.CD43_KHACH_HANG_THONG_TIN_CO_BAN
            .Where(c => c.id == id)
            .Select(c => new Customer
            {
                Id = c.id,
                RecordId = c.record_id,
                MaNhomTbh = c.manhom_tbh,
                NgayThangNamSinh = c.ngay_thang_nam_sinh,
                GioiTinh = c.gioi_tinh,
                CapBacHocVan = c.cap_bac_hoc_van,
                NgheNghiep = c.nghe_nghiep,
                NgheNghiepKhac = c.nghe_nghiep_khac,
                NgayHoi = c.ngayhoi,
                NgayTuVan = c.ngaytuvan,
                PhiuTVanComplete = c.phiu_t_vn_complete,
                NgayBangHoiNgay = c.ngaybanghoi_ngay_3db24b,
                NgayShn = c.ngay_shn,
                NgayTheoDauTime = c.ngaytheodau_time,
                CityCodeMap = c.city_code_map
            })
            .FirstOrDefault();

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult GetAll(ModelSearch modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                modelSearch.pageSize = int.MaxValue;
                int totalItems = 0;
                var data = _CustomerDA.GetAllByPage(modelSearch);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;

                //var returnData = new DatatablesModel();
                //returnData.data = (List<object>)data;

                AddLog("Lấy dữ liệu theo trang bảng khách hàng( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng khách hàng( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);

                return Json(obj);
            }
        }

        [System.Web.Mvc.HttpPost]
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

                AddLog("Lấy danh sách các botom được thực hiện trên from khách hàng thành công.");
                return Json(new { Buttoms = bottoms, Error = false, DuAns = duAns, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from khách hàng lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "Customer",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }


        [System.Web.Mvc.HttpPost]
        public object GetItemByCode(string code)
        {
            try
            {
                var data = _CustomerDA.GetItemByCode(code);
                AddLog("Lấy dữ liệu theo code bảng khách hàng( code: " + code + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo code bảng khách hàng( code: " + code + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }

        #region Xuất dữ liệu ra excel
        [System.Web.Mvc.HttpGet]
        public ActionResult ExportData(string keyword)
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                var modelSearch = new ModelSearch
                {
                    KeyWord = keyword == "undefined"? string.Empty : keyword,
                    currentPage = 1,
                    pageSize = int.MaxValue,
                    SortColumn = "hoten"
                };
                var data = _CustomerDA.GetAllByPage(modelSearch);

                string file_name = "KhachHang_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".xlsx";

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[10] { new DataColumn("STT"),
                        new DataColumn("Mã KH"),
                        new DataColumn("Họ tên KH"),
                        new DataColumn("Giới tính"),
                         new DataColumn("Năm sinh"),
                         new DataColumn("Đối tượng"),
                         new DataColumn("Ngày tiếp cận"),
                         new DataColumn("Số điện thoại"),
                         new DataColumn("Tỉnh"),
                         new DataColumn("Địa chỉ")
                });
                var stt = 1;
                foreach (var item in data)
                {
                    dt.Rows.Add(stt,
                        item.makh,
                        item.hoten,
                        item.GioiTinhText,
                        item.namsinh,
                        item.LoaiDoiTuong,
                        item.ngaytiepcantext,
                        item.sodienthoai,
                        item.CityName,
                        item.diachi
                        );
                    stt++;
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Khách hàng");
                    ws.Cell("A2").Value = "KHÁCH HÀNG";
                    ws.Range("A2:J2").Row(1).Merge();
                    ws.Cell("A2").Style.Font.Bold = true;
                    ws.Cell("A2").Style.Font.FontSize = 20;
                    ws.Column("A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Column("A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Row(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Row(3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell(3, 1).InsertTable(dt);
                    AddLog("Khách hàng");
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
                AddLog("Export Khách hàng lỗi: " + ex.Message);
                return Json(new { message = "Lỗi xử lý dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion 
    }
}