using Common;
using Model.ModelExtend.API;
using Newtonsoft.Json;
using SyncBVTL.Push.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SyncBVTL.Web.Services
{
    public class GetDataAPIServices : BaseServices
    {
        public static async Task<List<ResultApi1344Model>> GetDataFromApi(string url, string token, string reportId)
        {
            var result = new List<ResultApi1344Model>();
            try
            {
                log.Info("****************************Bắt đầu lấy dữ liệu  api "+ url + ", report id: "+ reportId + " ********************************");
                HttpResponseMessage response = await ApiBase.PostJsonAsyncRaw(url,  token,  reportId);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new List<ResultApi1344Model>();
                }
                else
                {
                    string responseString = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        result = JsonConvert.DeserializeObject<List<ResultApi1344Model>>(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                result = new List<ResultApi1344Model>();
            }

            log.Info("Số bản ghi api trả ra: "+result.Count());

            log.Info("****************************Kết thúc lấy dữ liệu  api " + url + ", report id: " + reportId + " ********************************");
            return result;
        }
    }
}