using Common;
using Model.Model;
using Model.ModelExtend;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Data.Admin;
using log4net;
using WebApp.Common;
using Model.ModelExtend.Base;
using Common.Common;
using Data.InterfaceDA.Admin;

namespace WebApp.Controllers
{
    public class TestGroupController : BaseController
    {
        IBVTL_NHOM_TBHDA _testGroupDA = new BVTL_NHOM_TBHDA();
        ICityDA _CityDA = new CityDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: TestGroup
        [HasCredential(ControllerName = "TestGroup")]
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
        public ActionResult GetAllByPage(ModelSearch modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                int totalItems = 0;
                var data = _testGroupDA.GetAllByPage(modelSearch);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu theo trang bảng Nhóm thu thập DL( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng Nhóm thu thập DL( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);
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
                AddLog("Lấy danh sách các botom được thực hiện trên from Nhóm thu thập DL thành công.");
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from Nhóm thu thập DL lỗi: " + ex.Message);
                return Json(obj);
            }
        }


        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "Role",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }

        [HttpPost]
        public ActionResult GetDanhMuc()
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var citys = _CityDA.GetAll();
                AddLog("Lấy dữ liệu danh mục thành công.");
                return Json(new { Citys = citys, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu danh mục lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public object GetItemByID(string maNhom)
        {
            try
            {
                var data = _testGroupDA.GetItemByMaNhom(maNhom); 

                AddLog("Lấy dữ liệu theo ID bảng Nhóm thu thập DL(ID: " + maNhom + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng Nhóm thu thập DL(ID: " + maNhom + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }
        [HttpPost]
        public object Add(BVTL_NHOM_TBH model)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var session = (UserLogin)Session["USER_SESSION"];
                obj = _testGroupDA.Add(model);
                if (obj.Error)
                    AddLog("Thêm mới dữ liệu bảng Nhóm thu thập DL(Name: " + model.tennhom_tbh + ", Descripttion: " + model.manhom_tbh + ") lỗi: " + obj.Title);
                else
                    AddLog("Thêm mới dữ liệu bảng Nhóm thu thập DL(Name: " + model.tennhom_tbh + ", Descripttion: " + model.manhom_tbh + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thêm mới dữ liệu bảng Nhóm thu thập DL(Name: " + model.tennhom_tbh + ", Descripttion: " + model.manhom_tbh + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public object Edit(BVTL_NHOM_TBH model)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var session = (UserLogin)Session["USER_SESSION"];
               
                obj = _testGroupDA.Edit(model);
                if (obj.Error)
                    AddLog("Cập nhật dữ liệu bảng Nhóm thu thập DL(Name: " + model.tennhom_tbh + ", Descripttion: " + model.manhom_tbh + ") lỗi: " + obj.Title);
                else
                    AddLog("Cập nhật dữ liệu bảng Nhóm thu thập DL(Name: " + model.tennhom_tbh + ", Descripttion: " + model.manhom_tbh + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Cập nhật dữ liệu bảng Nhóm thu thập DL(Name: " + model.tennhom_tbh + ", Descripttion: " + model.manhom_tbh + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public object Delete(string maNhom)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var session = (UserLogin)Session["USER_SESSION"];
                obj = _testGroupDA.Delete(maNhom, (int)session.UserID);
                if (obj.Error)
                    AddLog("Xóa dữ liệu bảng Nhóm thu thập DL(ID: " + maNhom + ") lỗi: " + obj.Title);
                else
                    AddLog("Xóa dữ liệu bảng Nhóm thu thập DL(ID: " + maNhom + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Xóa dữ liệu bảng Nhóm thu thập DL(ID: " + maNhom + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }
    }

}