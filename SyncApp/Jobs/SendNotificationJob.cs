using Model.ModelExtend.API;
using Quartz;
using SyncBVTL.Push.Controllers.PA;
using SyncBVTL.Push.Services;
using SyncBVTL.Push.Utils;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SyncBVTL.Push.Jobs
{
    public class SendNotificationJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var controller = DependencyResolver.Current.GetService<SendEmailController>();

            var currentDate = DateTime.Now;
            if (currentDate.Hour == 1)
            {
                await controller.SendNotification();
            }
        }
    }
}