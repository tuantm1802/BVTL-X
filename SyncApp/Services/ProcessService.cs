using Data.API;
using Model.Model;
using Model.ModelExtend.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Data.InterfaceDA.API;

namespace SyncBVTL.Push.Services
{
    public class ProcessService
    {
        readonly IInsertDataDA da = new InsertDataDA();

        public List<ProcessModel> GetListProcess()
        {
            var result = new List<ProcessModel>();
            var apis = da.GetAllApi();
            var tableAlls = da.GetAllApi_Table();

            if (apis != null && apis.Count > 0)
            {
                var tableByApi = new List<BVTL_MASTER_TABLE>();
                var process = new ProcessModel();
                foreach (var api in apis)
                {
                    process = new ProcessModel
                    {
                        Code = api.TokenApi,
                        Name = api.NameSyncdata,
                        TimeLoop = api.TimeReCall > 0 ? (int)api.TimeReCall : 60,
                        Active = true,
                        Url = api.HrefApi,
                        Token = api.TokenApi,
                        ReportId = api.ReportId,
                        MaDuAn = api.maduan
                    };

                    if (!string.IsNullOrEmpty(api.TableNameSaveData))
                        process.TableNames = new List<string> { api.TableNameSaveData};
                    else
                    {
                        tableByApi = tableAlls.Where(x=>x.Api_Id == api.Api_Id).ToList();
                        if (tableByApi != null && tableByApi.Count > 0)
                            process.TableNames = tableByApi.Select(x=>x.table_name).ToList();
                    }

                    result.Add(process);
                }
            }

            //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Start/ProcessConfig.json");
            //var listProcess = JsonHelper.GetDataFromJsonFile<List<ProcessModel>>(path);
            //return listProcess.ToList();

            return result;
        }
    }
}