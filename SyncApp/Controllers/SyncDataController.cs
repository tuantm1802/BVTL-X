using Common;
using Data.API;
using Data.InterfaceDA.API;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.API;
using SyncBVTL.Push.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SyncBVTL.Push.Controllers.PA
{
    public class SyncDataController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string accessToken = "";
        public ISyncDataFromApi_SaveToDB syncDataFromApi_SaveToDB = new SyncDataFromApi_SaveToDB();

        public ActionResult Index()
        {
            return View();
        }

       
        public async Task<BaseResult> GetDataFromAPI(ProcessModel model)
        {
            BaseResult apiResult = new BaseResult();
            if (model.Active)
            {
                apiResult = await syncDataFromApi_SaveToDB.GetDataFromApi_SaveToDB(model.Url, model.Token, model.ReportId, model.MaDuAn, model.TableNames, model.Code);
            }
            return apiResult;
        }

    }
}