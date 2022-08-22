using Common;
using log4net;
using Model.ModelExtend.API;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using SyncBVTL.Push.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SyncBVTL.Push.Services
{
    public class BaseServices
    {
        protected static string ConnectionStr = ConfigurationManager.AppSettings.Get("ConnectionString");
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static async Task<ApiResult> SyncInfoAsync(string json,  string url)
        {
            try
            {
                var resultSync = new ApiResult();

                log.Info("JSON: " + json);
                HttpResponseMessage response = await ApiBase.PostJsonAsync( url, json);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new ApiResult
                    {
                        code = "401",
                        message = $"Bạn không có quyền sử dụng API: {url}"
                    };
                }
                else
                {
                    string responseString = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        resultSync = JsonConvert.DeserializeObject<ApiResult>(responseString);
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.NotImplemented
                            || response.StatusCode == HttpStatusCode.BadGateway || response.StatusCode == HttpStatusCode.ServiceUnavailable
                            || response.StatusCode == HttpStatusCode.GatewayTimeout || response.StatusCode == HttpStatusCode.HttpVersionNotSupported)
                        {
                        }
                        return new ApiResult() { code = response.StatusCode.ToString(), message = responseString };
                    }

                    log.Info("****************************END UPLOAD DOCUMENT LIST TO TTĐH********************************");
                    return resultSync;
                }

            }
            catch (Exception ex)
            {
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                return new ApiResult { code = "501", message = ex.Message };
            }
        }
        
       
    }
}