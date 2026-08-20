using Data.API;
using Data.InterfaceDA.API;
using Model.Model;
using Model.ModelExtend.API;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Services
{
    public class ProcessService
    {
        readonly IInsertDataDA _da = new InsertDataDA();

        public List<ProcessModel> GetListProcess()
        {
            var result = new List<ProcessModel>();
            var apis = _da.GetAllApi();
            var tableAlls = _da.GetAllApi_Table();

            if (apis != null && apis.Count > 0)
            {
                foreach (var api in apis)
                {
                    var process = new ProcessModel
                    {
                        Code = api.Api_Code,
                        Name = api.NameSyncdata,
                        TimeLoop = api.TimeReCall > 0 ? (int)api.TimeReCall : 60,
                        Active = api.IsActive,
                        Url = api.HrefApi,
                        Token = api.TokenApi,
                        ReportId = api.ReportId,
                        MaDuAn = api.maduan,
                        Message = api.Message,
                        RawOrLabel = api.RawOrLabel,
                    };

                    if (api.Start_Time_Sync != null)
                    {
                        process.Start_Time_Sync = api.Start_Time_Sync?.ToString("dd/MM/yyyy HH:mm");
                    }

                    if (api.End_Time_Sync.HasValue)
                    {
                        process.End_Time_Syc = api.End_Time_Sync.Value.ToString("dd/MM/yyyy HH:mm");
                    }

                    if (!string.IsNullOrEmpty(api.TableNameSaveData))
                    {
                        process.TableNames = new List<string> { api.TableNameSaveData };
                    }
                    else
                    {
                        var tableByApi = tableAlls.Where(x => x.Api_Id == api.Api_Id).ToList();
                        if (tableByApi != null && tableByApi.Count > 0)
                        {
                            process.TableNames = tableByApi.Select(x => x.table_name).ToList();
                        }
                    }

                    if (process.TableNames != null && process.TableNames.Count > 0)
                    {
                        result.Add(process);
                    }
                }
            }

            return result;
        }

        public ObjectMessage EditJobSync(BVTL_API model)
        {
            return _da.EditJobSync(model);
        }
    }
}
