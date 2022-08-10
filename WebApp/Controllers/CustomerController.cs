using Common;
using Data.Admin;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class CustomerController : BaseController
    {
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();
        CustomerDA _CustomerDA = new CustomerDA();
        SysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: Customer
        [HasCredential(ControllerName = "Customer")]
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
                    new SysLog
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
                var data = db.Customers.Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.FullName,
                    x.Gender,
                    x.DateOfBirth,
                    x.CityId,
                    x.TypeObject,
                    x.Code_TCV
                }).FirstOrDefault(x => x.Id == Id);
                AddLog("Lấy dữ liệu theo ID bảng khách hàng( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng khách hàng( ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }
        //[HttpPost]
        //public object Add(Customer Customer)
        //{
        //    ObjectMessage obj = new ObjectMessage
        //    {
        //        Error = false
        //    };
        //    try
        //    {
        //        obj = _CustomerDA.Add(Customer);
        //        if (obj.Error)
        //            AddLog("Thêm mới dữ liệu khách hàng(Code: " + Customer.Code + ", FullName: " + Customer.FullName + ") lỗi: " + obj.Title);
        //        else
        //            AddLog("Thêm mới dữ liệu khách hàng(Code: " + Customer.Code + ", FullName: " + Customer.FullName + ") thành công.");
        //        return Json(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        obj.Error = true;
        //        obj.Title = ex.Message.ToString();
        //        AddLog("Thêm mới dữ liệu khách hàng(Code: " + Customer.Code + ", FullName: " + Customer.FullName + ") lỗi: " + ex.Message);
        //        return Json(obj);
        //    }
        //}

        //[HttpPost]
        //public object Edit(Customer Customer)
        //{
        //    ObjectMessage obj = new ObjectMessage
        //    {
        //        Error = false
        //    };
        //    try
        //    {
        //        obj = _CustomerDA.Edit(Customer);
        //        if (obj.Error)
        //            AddLog("Cập nhật dữ liệu khách hàng(Code: " + Customer.Code + ", FullName: " + Customer.FullName + ") lỗi: " + obj.Title);
        //        else
        //            AddLog("Cập nhật dữ liệu khách hàng( Code: " + Customer.Code + ", FullName: " + Customer.FullName + " ) thành công.");

        //        return Json(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        obj.Error = true;
        //        obj.Title = ex.Message.ToString();
        //        AddLog("Cập nhật dữ liệu khách hàng(Code: " + Customer.Code + ", FullName: " + Customer.FullName + ") lỗi: " + ex.Message);
        //        return Json(obj);
        //    }
        //}
        //[HttpPost]
        //public object Delete(int Id)
        //{
        //    ObjectMessage obj = new ObjectMessage
        //    {
        //        Error = false
        //    };
        //    try
        //    {
        //        obj = _CustomerDA.Delete(Id);
        //        if (obj.Error)
        //            AddLog("Xóa dữ liệu khách hàng( ID: " + Id + " ) lỗi: " + obj.Title);
        //        else
        //            AddLog("Xóa dữ liệu khách hàng( ID: " + Id + " ) thành công.");
        //        return Json(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        obj.Error = true;
        //        obj.Title = ex.Message.ToString();
        //        AddLog("Xóa dữ liệu khách hàng( ID: " + Id + " ) lỗi: " + ex.Message + ".");
        //        return Json(obj);
        //    }
        //}
    }
}