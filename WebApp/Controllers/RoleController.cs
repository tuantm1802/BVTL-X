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
    public class RoleController : BaseController
    {
        readonly IRoleDA _roleDA;
        readonly IRolePageDA _rolePageDA;
        readonly IPageMenuDA _pageMenuDA;
        readonly IPageActionDA _pageActionDA;
        readonly ISysLogDA _sysLogDA;
        BaseController _helperController = new BaseController();

        public RoleController(IRoleDA roleDA, IRolePageDA rolePageDA, IPageMenuDA pageMenuDA, IPageActionDA pageActionDA, ISysLogDA sysLogDA)
        {
            _roleDA = roleDA;
            _rolePageDA = rolePageDA;
            _pageMenuDA = pageMenuDA;
            _pageActionDA = pageActionDA;
            _sysLogDA = sysLogDA;
        }

        // GET: User
        [HasCredential(ControllerName = "Role")]
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
                int pageSize = 0;
                var data = _roleDA.GetAllByPage(modelSearch, ref pageSize);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu theo trang bảng Quyền( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, pageSize = pageSize, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng Quyền( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);
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
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng thành công.");
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng lỗi: " + ex.Message);
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
                var pageMenus = _pageMenuDA.GetAll();
                var pageActions = _pageActionDA.GetAll();
                List<TreeModel> lstTreeModel = new List<TreeModel>();
                ConvertTreePageMenu(lstTreeModel, pageMenus, pageActions, null, new List<BVTL_QT_QUYEN_PAGE>());
                lstTreeModel = lstTreeModel.OrderBy(e => e.name).ToList();
                AddLog("Lấy dữ liệu danh mục thành công.");
                return Json(new { TreeDatas = lstTreeModel, Error = false, Title = "Lấy dữ liệu thành công." }); ;
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
        public object GetItemByID(string Id)
        {
            try
            {
                var data = _roleDA.GetItemById(Id);

                var rolePages = new List<BVTL_QT_QUYEN_PAGE>();
                if (!string.IsNullOrEmpty(Id))
                {
                    rolePages = _rolePageDA.GetAllByRole(Id);
                }
                var pageMenus = _pageMenuDA.GetAll();
                var pageActions = _pageActionDA.GetAll();
                List<TreeModel> lstTreeModel = new List<TreeModel>();
                ConvertTreePageMenu(lstTreeModel, pageMenus, pageActions, null, rolePages);
                lstTreeModel = lstTreeModel.OrderBy(e => e.name).ToList();
                AddLog("Lấy dữ liệu theo ID bảng Quyền(ID: " + Id + ") thành công.");
                return Json(new { TreeDatas = lstTreeModel, Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng Quyền(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }
        [HttpPost]
        public object Add(BVTL_QT_QUYEN role, List<TreeModel> pageMenus)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                obj = _roleDA.Add(role, pageMenus);
                if (obj.Error)
                    AddLog("Thêm mới dữ liệu bảng Quyền(Name: " + role.Name + ", Descripttion: " + role.Descripttion + ") lỗi: " + obj.Title);
                else
                    AddLog("Thêm mới dữ liệu bảng Quyền(Name: " + role.Name + ", Descripttion: " + role.Descripttion + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thêm mới dữ liệu bảng Quyền(Name: " + role.Name + ", Descripttion: " + role.Descripttion + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        /// <summary>
        /// Tạo tree chức năng
        /// </summary>
        /// <param name="lstTreeModel"></param>
        /// <param name="pageMenu"></param>
        /// <param name="pageFunction"></param>
        /// <param name="parentId"></param>
        public void ConvertTreePageMenu(List<TreeModel> lstTreeModel, List<BVTL_QT_PAGE_MENU> pageMenu, List<BVTL_QT_PAGE_ACTION> pageFunction, int? parentId, List<BVTL_QT_QUYEN_PAGE> rolePages)
        {
            if (pageMenu != null && pageMenu.Count > 0)
            {
                var lstPageMenu = new List<BVTL_QT_PAGE_MENU>();
                if (parentId > 0)
                    lstPageMenu = pageMenu.Where(x => x.PARENT_PAGE_ID == parentId).ToList();
                else
                    lstPageMenu = pageMenu.Where(x => x.PARENT_PAGE_ID == null || x.PARENT_PAGE_ID == 0).ToList();
                if (lstPageMenu.Count > 0)
                {
                    foreach (var item in lstPageMenu)
                    {
                        var rolePage = rolePages.FirstOrDefault(x => x.PageID == item.ID && x.IS_ACTIVE == true);

                        var tree = new TreeModel
                        {
                            id = item.ID.ToString() + "_",
                            name = item.NAME,
                            pace_id = item.ID.ToString(),
                            pId = item.PARENT_PAGE_ID == null ? "0" : item.PARENT_PAGE_ID.ToString() + "_",
                            @checked = (rolePage != null && rolePage.ID > 0) ? true : false,
                            open = (rolePage != null && rolePage.ID > 0) ? true : false
                        };
                        lstTreeModel.Add(tree);
                        var pf = pageFunction.Where(x => x.PAGE_ID == item.ID).ToList();
                        if (pf.Count > 0)
                        {
                            var listAction = new List<string>();
                            if (rolePage != null && rolePage.ID > 0 && !string.IsNullOrEmpty(rolePage.CONTROL_STRING))
                            {
                                listAction = rolePage.CONTROL_STRING.Split('|').ToList();
                            }
                            foreach (var itemF in pf)
                            {
                                tree = new TreeModel
                                {
                                    @checked = listAction.Contains(itemF.CONTROL_NAME),
                                    id = "_" + itemF.PAGE_ID.ToString() + "/" + itemF.ID,
                                    name = itemF.CONTROL_DESC ?? itemF.CONTROL_NAME,
                                    name_control = itemF.CONTROL_NAME,
                                    pace_id = itemF.PAGE_ID.ToString(),
                                    pId = item.ID.ToString() + "_"
                                };
                                lstTreeModel.Add(tree);
                            }
                        }
                        ConvertTreePageMenu(lstTreeModel, pageMenu, pageFunction, item.ID, rolePages);
                    }
                }
            }
        }

        [HttpPost]
        public object Edit(BVTL_QT_QUYEN role, List<TreeModel> pageMenus)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                obj = _roleDA.Edit(role, pageMenus);
                if (obj.Error)
                    AddLog("Cập nhật dữ liệu bảng Quyền(Name: " + role.Name + ", Descripttion: " + role.Descripttion + ") lỗi: " + obj.Title);
                else
                    AddLog("Cập nhật dữ liệu bảng Quyền(Name: " + role.Name + ", Descripttion: " + role.Descripttion + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Cập nhật dữ liệu bảng Quyền(Name: " + role.Name + ", Descripttion: " + role.Descripttion + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public object Delete(string Id)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                obj = _roleDA.Delete(Id);
                if (obj.Error)
                    AddLog("Xóa dữ liệu bảng Quyền(ID: " + Id + ") lỗi: " + obj.Title);
                else
                    AddLog("Xóa dữ liệu bảng Quyền(ID: " + Id + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Xóa dữ liệu bảng Quyền(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }
    }

}