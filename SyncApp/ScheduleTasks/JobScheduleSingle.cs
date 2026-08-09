using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend.API;
using Quartz;
using Quartz.Impl;
using SyncBVTL.Push.Jobs.PAJobs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SyncBVTL.Push.ScheduleTasks
{
    public class JobScheduleSingle
    {
        static ISysLogDA _sysLogDA = new SysLogDA();

        public static async Task StartSingle(ProcessModel item)
        {
            await JobScheduleChangeTimeloop.ChangeTimeloop(item);
        }
    }
}