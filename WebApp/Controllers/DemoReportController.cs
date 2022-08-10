using Common;
using Data.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Report;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class DemoReportController : BaseController
    {
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();
        SysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: DemoReport
        [HasCredential(ControllerName = "DemoReport")]
        public ActionResult Index()
        {
            return View();
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
                var data = new List<BaoCaoModel>() {
                    new BaoCaoModel
                    {
                        ThongTinBC = "THÔNG TIN CHUNG",
                        BoldText = "Y",
                        Rowpan = 1,
                        Colpan = 1

                    },
                    new BaoCaoModel
                    {
                        STT = "1",
                        ThongTinBC = "Tổng số KH được hỗ trợ",
                        ThongTinBC_Them = "Tổng số KH được hỗ trợ",
                        BoldText = "N",
                        Rowpan = 1,
                        Colpan = 2,
                        MSM = 30,
                        PUD = 112,
                        SW = 2,
                        Nam = 101,
                        Nu = 43,
                        ChuyenGioi=2,
                        Tong =146
                    }
                    ,
                    new BaoCaoModel
                    {
                        STT = "2",
                        ThongTinBC = "Số KH mới",
                        ThongTinBC_Them= "Số KH mới",
                        BoldText = "N",
                        Rowpan = 1,
                        Colpan = 2,
                        MSM = 30,
                        PUD = 112,
                        SW = 2,
                        Nam = 101,
                        Nu = 43,
                        ChuyenGioi=2,
                        Tong =146
                    }
                    ,
                    new BaoCaoModel
                    {
                        STT = "3",
                        ThongTinBC = "Số KH mất dấu ",
                        BoldText = "N",
                        Rowpan = 1,
                        Colpan = 1,
                        MSM = 0,
                        PUD = 0,
                        SW = 0,
                        Nam = 0,
                        Nu = 0,
                        ChuyenGioi=0,
                        Tong =0
                    }
                    ,
                    new BaoCaoModel
                    {
                        STT = "4",
                        ThongTinBC = "Độ tuổi của KH",
                         ThongTinBC_Them = "16- 18 tuổi",
                        BoldText = "N",
                        Rowpan = 3,
                        Colpan = 1,
                        MSM = 19,
                        PUD = 34,
                        SW = 0,
                        Nam = 38,
                        Nu = 15,
                        ChuyenGioi=1,
                        Tong =54
                    }
                     ,
                    new BaoCaoModel
                    {
                        STT = "",
                         ThongTinBC = "Độ tuổi của KH",
                       ThongTinBC_Them = "19- 22 tuổi",
                        BoldText = "N",
                        Rowpan = 0,
                        Colpan = 1,
                        MSM = 48,
                        PUD = 80,
                        SW = 2,
                        Nam = 86,
                        Nu = 44,
                        ChuyenGioi=2,
                        Tong =132
                    }
                     ,
                    new BaoCaoModel
                    {
                        STT = "",
                         ThongTinBC = "Độ tuổi của KH",
                        ThongTinBC_Them = "23- 24 tuổi",
                        BoldText = "N",
                        Rowpan = 0,
                        Colpan = 1,
                        MSM = 36,
                        PUD = 52,
                        SW = 0,
                        Nam = 75,
                        Nu = 13,
                        ChuyenGioi=6,
                        Tong =94
                    }
                };

                //var data = _DemoReportDA.GetAllByPage(modelSearch, ref pageSize);
                //if (data != null && data.Count > 0)
                //    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu báo cáo( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công.", pageSize = pageSize }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu báo cáo( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);

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
                    new SysLog
                    {
                        ControllerName = "DemoReport",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }

    }
}