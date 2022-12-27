using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class BaoCaoChiSoController : Controller
    {
        ICityDA _CityDA = new CityDA();
        IDuAnDA _DuAnDA = new DuAnDA();
        IBaoCaoTongHopDA _BaoCaoTongHopDA = new BaoCaoTongHopDA();
        IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA = new BVTL_NHOM_TBHDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: BaoCaoChiSo
        [HasCredential(ControllerName = "BaoCaoChiSo")]
        public ActionResult Index()
        {
            try
            {
                // Kiểm tra quyền 
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                if (user.IsAdmin || (duAn != null && duAn.maduan == user.MaDuAn))
                    return View();
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
        public ActionResult SearchData(ReportSearchModel modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";
                //modelSearch.TypeReport = 1;
                var data = _BaoCaoTongHopDA.GetDataReportChiSo(modelSearch);
                AddLog("Lấy dữ liệu báo cáo tháng( tháng: " + modelSearch.Months + ", năm: " + modelSearch.Year + ", tỉnh: " + modelSearch.CityCodes + ") thành công.");
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu báo cáo tháng( tháng: " + modelSearch.Months + ", năm: " + modelSearch.Year + ", tỉnh: " + modelSearch.CityCodes + ") lỗi: " + ex.Message);

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

                var user = Session["USER_SESSION"] as UserLogin;
                var citys = _CityDA.GetCityReport((int)user.UserID);
                var duAns = _DuAnDA.GetDuAnReport((int)user.UserID);

                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng thành công.");
                return Json(new { Buttoms = bottoms, Citys = citys, DuAns = duAns, Error = false, Title = "Lấy dữ liệu thành công." });
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
                        ControllerName = "BaoCaoThang",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }
    }
}