using Data.API;
using Model.Model;
using Model.ModelExtend.API;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Data.InterfaceDA.API;
using Model.ModelExtend.Base;

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
                        Code = api.Api_Code,
                        Name = api.NameSyncdata,
                        TimeLoop = api.TimeReCall > 0 ? (int)api.TimeReCall : 60,
                        Active = api.IsActive,
                        Url = api.HrefApi,
                        Token = api.TokenApi,
                        ReportId = api.ReportId,
                        MaDuAn = api.maduan,
                        Message = api.Message
                    };

                    if (api.Start_Time_Sync != null)
                        process.Start_Time_Sync = Convert.ToDateTime(api.Start_Time_Sync).ToString("dd/MM/yyyy HH:mm");

                    if (api.End_Time_Sync != null)
                        process.End_Time_Syc = Convert.ToDateTime(api.End_Time_Sync).ToString("dd/MM/yyyy HH:mm");

                    if (!string.IsNullOrEmpty(api.TableNameSaveData))
                        process.TableNames = new List<string> { api.TableNameSaveData };
                    else
                    {
                        tableByApi = tableAlls.Where(x => x.Api_Id == api.Api_Id).ToList();
                        if (tableByApi != null && tableByApi.Count > 0)
                            process.TableNames = tableByApi.Select(x => x.table_name).ToList();
                    }

                    if (process.TableNames != null && process.TableNames.Count > 0)
                        result.Add(process);
                }
            }

            //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Start/ProcessConfig.json");
            //var listProcess = JsonHelper.GetDataFromJsonFile<List<ProcessModel>>(path);
            //return listProcess.ToList();

            return result;
        }

        /// <summary>
        /// Cập nhật job đồng bộ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ObjectMessage EditJobSync(BVTL_API model)
        {
            return da.EditJobSync(model);
        }
    }
}