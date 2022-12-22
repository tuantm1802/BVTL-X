using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class BangHoiDanhGiaController : Controller
    {
        IKetQuaXNNTDA _KetQuaXNNTDA = new KetQuaXNNTDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        IDuAnDA _DuAnDA = new DuAnDA();
        BaseController _helperController = new BaseController();

        [HasCredential(ControllerName = "BangHoiDanhGia")]
        // GET: BangHoiDanhGia
        public ActionResult Index()
        {
            return View();
        }
    }
}