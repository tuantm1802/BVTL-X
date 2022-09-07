using Common;
using Data.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Common.Common;
using Data.InterfaceDA.Admin;

namespace WebApp.Controllers
{
    public class SysParameterController : BaseController
    {
        ISysParameterDA _sysParameterDA = new SysParameterDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: SysParameter
        [HasCredential(ControllerName = "SysParameter")]
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
                var data = _sysParameterDA.GetAllByPage(modelSearch, ref pageSize);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu theo trang bảng Tham số( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công.", pageSize = pageSize }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng Tham số( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);

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
                        ControllerName = "SysParameter",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }


        [HttpPost]
        public object GetItemByID(int Id)
        {
            try
            {
                var data = _sysParameterDA.GetItemById(Id); 
                AddLog("Lấy dữ liệu theo Id bảng Tham số( Id: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo Id bảng Tham số( Id: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }
        [HttpPost]
        public object Add(BVTL_QT_THAM_SO sysParameter)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                obj = _sysParameterDA.Add(sysParameter);
                if (obj.Error)
                    AddLog("Thêm mới dữ liệu Tham số(ParamCode: " + sysParameter.ParamCode + ", ParamValue: " + sysParameter.ParamValue + ", Desctiption: " + sysParameter.Desctiption + ") lỗi: " + obj.Title);
                else
                    AddLog("Thêm mới dữ liệu Tham số( ParamCode: " + sysParameter.ParamCode + ", ParamValue: " + sysParameter.ParamValue + ", Desctiption: " + sysParameter.Desctiption + " ) thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thêm mới dữ liệu Tham số(ParamCode: " + sysParameter.ParamCode + ", ParamValue: " + sysParameter.ParamValue + ", Desctiption: " + sysParameter.Desctiption + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public object Edit(BVTL_QT_THAM_SO sysParameter)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                obj = _sysParameterDA.Edit(sysParameter);
                if (obj.Error)
                    AddLog("Cập nhật dữ liệu Tham số(ParamCode: " + sysParameter.ParamCode + ", ParamValue: " + sysParameter.ParamValue + ", Desctiption: " + sysParameter.Desctiption + ") lỗi: " + obj.Title);
                else
                    AddLog("Cập nhật dữ liệu Tham số( ParamCode: " + sysParameter.ParamCode + ", ParamValue: " + sysParameter.ParamValue + ", Desctiption: " + sysParameter.Desctiption + " ) thành công.");

                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Cập nhật dữ liệu Tham số(ParamCode: " + sysParameter.ParamCode + ", ParamValue: " + sysParameter.ParamValue + ", Desctiption: " + sysParameter.Desctiption + ") lỗi: " + ex.Message);
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
                obj = _sysParameterDA.Delete(Id);
                if (obj.Error)
                    AddLog("Xóa dữ liệu Tham số( ID: " + Id + " ) lỗi: " + obj.Title);
                else
                    AddLog("Xóa dữ liệu Tham số( ID: " + Id + " ) thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Xóa dữ liệu Tham số( ID: " + Id + " ) lỗi: " + ex.Message + ".");
                return Json(obj);
            }
        }
    }
}