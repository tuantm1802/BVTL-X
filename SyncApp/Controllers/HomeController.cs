using Model.ModelExtend.API;
using Newtonsoft.Json;
using SyncBVTL.Push.Models;
using SyncBVTL.Push.ScheduleTasks;
using SyncBVTL.Push.Services;
using SyncBVTL.Push.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SyncBVTL.Push.Controllers
{
    public class HomeController : Controller
    {
        readonly ProcessService processSrv = new ProcessService();
        readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Start/ProcessConfig.json");
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetListProcess()
        {
            var model = processSrv.GetListProcess();
            return Json(new ResponseList<ProcessModel> { code = "200", data = model }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateProcess(ProcessModel model)
        {
            try
            {
                var listProcess = processSrv.GetListProcess();
                var process = listProcess.FirstOrDefault(x => x.Code == model.Code);
                if (process == null)
                    throw new Exception("Lỗi file config!");

                if (model.TimeLoop != process.TimeLoop)
                {
                    _ = JobScheduleChangeTimeloop.ChangeTimeloopAsync(model);
                }

                if (model.Active != process.Active)
                {
                    JobScheduleSingle.StartSingle(model);
                }
                listProcess[listProcess.IndexOf(process)] = model;

                System.IO.File.WriteAllText(configPath, JsonConvert.SerializeObject(listProcess));
                return Json(new ResponseList<ProcessModel> { code = ((int)HttpStatusCode.OK).ToString(), data = listProcess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ApiResult { code = ((int)HttpStatusCode.InternalServerError).ToString(), message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShowConfig(string name)
        {
            var cErr = 0;
            while (true)
            {
                if (cErr == 5) { return Json(new ApiResult() { message = "Lỗi!", code = "500" }); }
                try
                {
                    string filePath = Directory.GetCurrentDirectory() + "\\Log\\" + name + $"_{DateTime.Now:yyyyMMdd}" + ".log";
                    return Json(new ApiResult() { message = StringUtils.ReadNLineOfFile(filePath, 200), code = "200" });
                }
                catch (Exception)
                {
                    cErr++;
                    throw;
                }
            }
        }


        
    }
}