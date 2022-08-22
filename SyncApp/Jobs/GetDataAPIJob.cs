using Quartz;
using SyncBVTL.Push.Controllers.PA;
using SyncBVTL.Push.Models;
using SyncBVTL.Push.Utils;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SyncBVTL.Push.Jobs.PAJobs
{
    public class GetDataAPIJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var controller = DependencyResolver.Current.GetService<SyncDataController>();

            //var Token = (string)dataMap["Token"] ;
            // var Url = (string)dataMap["Url"] = ;
            // var TableName = (string)dataMap["TableName";
            // var ReportId = (string)dataMap["ReportId"];
            var data = (ProcessModel)dataMap["Data"];
            var result = await controller.GetDataFromAPI(data);

            
            string logPath = "Log\\" + data.TableNames + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            if (!Directory.Exists("Log"))
            {
                Directory.CreateDirectory("Log");
            }

            if (!File.Exists(logPath))
            {
                File.Create(logPath);
            }

            using (var file = File.Open(logPath, FileMode.Open, FileAccess.ReadWrite))
            {
                file.Prepend(result.Message + "\n");
            }
        }
    }
}