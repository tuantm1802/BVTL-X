using Model.ModelExtend.API;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
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
        //readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Start/ProcessConfig.json");
        public async Task<ActionResult> Index()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;

            var currentlyExecuting = scheduler.GetCurrentlyExecutingJobs().Result;
            if(currentlyExecuting.Count > 0)
            {
                foreach (var job in currentlyExecuting)
                {
                    await scheduler.UnscheduleJob(job.Trigger.Key);
                    await scheduler.DeleteJob(job.JobDetail.Key);
                }
            }

            return View();
        }

        public ActionResult GetListProcess()
        {
            var model = processSrv.GetListProcess();
            return Json(new { code = "200", data = model }, JsonRequestBehavior.AllowGet);
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
                    _ = JobScheduleChangeTimeloop.ChangeTimeloop(model);
                }

                if (model.Active != process.Active)
                {
                    JobScheduleSingle.StartSingle(model);
                }
                listProcess[listProcess.IndexOf(process)] = model;

                processSrv.EditJobSync(new Model.Model.BVTL_API { Api_Code = model.Code, IsActive = model.Active, TimeReCall = model.TimeLoop});

                var listProcessNew = processSrv.GetListProcess();

                //System.IO.File.WriteAllText(configPath, JsonConvert.SerializeObject(listProcess));
                return Json(new ResponseList<ProcessModel> { code = ((int)HttpStatusCode.OK).ToString(), data = listProcessNew }, JsonRequestBehavior.AllowGet);
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
                    if (!System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Create(filePath).Dispose();
                    }
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