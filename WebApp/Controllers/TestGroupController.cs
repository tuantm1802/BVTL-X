using Common;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Data.Admin;
using log4net;
using WebApp.Common;

namespace WebApp.Controllers
{
    public class TestGroupController : BaseController
    {
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();
        TestGroupDA _testGroupDA = new TestGroupDA();
        SysLogDA _sysLogDA = new SysLogDA();
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
                var data = _testGroupDA.GetAllByPage(modelSearch, ref totalItems);
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
                    new SysLog
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
            //try
            //{
            //    List<TreeModel> lstTreeModel = new List<TreeModel>();
            //    ConvertTreePageMenu(lstTreeModel, pageMenus, pageActions, null, new List<RolePage>());
            //    lstTreeModel = lstTreeModel.OrderBy(e => e.name).ToList();
            //    AddLog("Lấy dữ liệu danh mục thành công.");
            //    return Json(new { TreeDatas = lstTreeModel, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            //}
            //catch (Exception ex)
            //{
            //    obj.Error = true;
            //    obj.Title = ex.Message.ToString();
            //    AddLog("Lấy dữ liệu danh mục lỗi: " + ex.Message);
            //    return Json(obj);
            //}

            return Json(obj);
        }

        [HttpPost]
        public object GetItemByID(int Id)
        {
            try
            {
                var data = db.TestGroups.FirstOrDefault(x => x.Id == Id);

                AddLog("Lấy dữ liệu theo ID bảng Nhóm thu thập DL(ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng Nhóm thu thập DL(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }
        [HttpPost]
        public object Add(TestGroup model)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var session = (UserLogin)Session["USER_SESSION"];
                model.CreatedBy = (int)session.UserID;
                obj = _testGroupDA.Add(model);
                if (obj.Error)
                    AddLog("Thêm mới dữ liệu bảng Nhóm thu thập DL(Name: " + model.Name + ", Descripttion: " + model.Code + ") lỗi: " + obj.Title);
                else
                    AddLog("Thêm mới dữ liệu bảng Nhóm thu thập DL(Name: " + model.Name + ", Descripttion: " + model.Code + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thêm mới dữ liệu bảng Nhóm thu thập DL(Name: " + model.Name + ", Descripttion: " + model.Code + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public object Edit(TestGroup model)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var session = (UserLogin)Session["USER_SESSION"];
                model.CreatedBy = (int)session.UserID;
                obj = _testGroupDA.Edit(model);
                if (obj.Error)
                    AddLog("Cập nhật dữ liệu bảng Nhóm thu thập DL(Name: " + model.Name + ", Descripttion: " + model.Code + ") lỗi: " + obj.Title);
                else
                    AddLog("Cập nhật dữ liệu bảng Nhóm thu thập DL(Name: " + model.Name + ", Descripttion: " + model.Code + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Cập nhật dữ liệu bảng Nhóm thu thập DL(Name: " + model.Name + ", Descripttion: " + model.Code + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public object Delete(int Id)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var session = (UserLogin)Session["USER_SESSION"];
                obj = _testGroupDA.Delete(Id, (int)session.UserID);
                if (obj.Error)
                    AddLog("Xóa dữ liệu bảng Nhóm thu thập DL(ID: " + Id + ") lỗi: " + obj.Title);
                else
                    AddLog("Xóa dữ liệu bảng Nhóm thu thập DL(ID: " + Id + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Xóa dữ liệu bảng Nhóm thu thập DL(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }
    }

}