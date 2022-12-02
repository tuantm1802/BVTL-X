using Model.ModelExtend.API;
using Quartz;
using SyncBVTL.Push.Services;
using SyncBVTL.Push.Utils;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace SyncBVTL.Push.Jobs
{
    public class CheckConnectAPIJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            string trangThaiKetNoi = "";
            ApiResult result = new ApiResult();
            //ApiResult result = await BaseServices.CheckConnectAPIAsync();
            if (result != null && result.code == "200")
            {
                trangThaiKetNoi = "Kết nối thành công";
            }
            else
            {
                trangThaiKetNoi = "Kết nối không thành công";
            }

            //string logPath = "Log\\TrangThaiKetNoi" + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            //if (!Directory.Exists("Log"))
            //{
            //    Directory.CreateDirectory("Log");
            //}

            //if (!File.Exists(logPath))
            //{
            //    File.Create(logPath);
            //}

            //using (var file = File.Open(logPath, FileMode.Open, FileAccess.ReadWrite))
            //{
            //    file.Prepend($"{DateTime.Now:yy.MM.dd HH:mm:ss} - {trangThaiKetNoi}" + "\n");
            //}
        }
    }
}