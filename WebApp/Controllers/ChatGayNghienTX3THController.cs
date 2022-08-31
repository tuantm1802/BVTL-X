using Common;
using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class ChatGayNghienTX3THController : BaseController
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        ICustomerDA _CustomerDA = new CustomerDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: ChatGayNghienTX3TH
        [HasCredential(ControllerName = "ChatGayNghienTX3TH")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Add()
        {
            return PartialView("_add");
        }
        public ActionResult _Edit()
        {
            return PartialView("_edit");
        }
        [HttpPost]
        public ActionResult GetAll(ModelSearch modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                int totalItems = 0;
                int pageSize = 0;
                var data = _CustomerDA.GetAllByPage(modelSearch, ref pageSize);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu theo trang bảng khách hàng( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công.", pageSize = pageSize }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng khách hàng( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);

                return Json(obj);
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
                AddLog("Lấy danh sách các botom được thực hiện trên from khách hàng thành công.");
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }); ;
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


        [HttpPost]
        public object GetItemByID(int? Id)
        {
            try
            {
                var data = db.BVTL_KHACH_HANG.Select(x => new
                {
                    x.khachhang_id,
                    x.makh,
                    x.hoten,
                    x.gioitinh,
                    x.namsinh,
                    x.loai_doi_tuong_id,
                    x.ngaytiepcan,
                    x.ngayngungchamsoc,
                    x.sodienthoai,
                    x.diachi,
                    x.city_code
                }).FirstOrDefault(x => x.khachhang_id == Id);
                AddLog("Lấy dữ liệu theo ID bảng khách hàng( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng khách hàng( ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }
    }
}