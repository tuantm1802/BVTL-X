using Common;
using log4net;
using Model.ModelExtend.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Data.API
{
    public class GetDataFromAPI
    {
        protected readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lấy dữ liệu từ đầu Api trả lại list T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public async Task<List<T>> PostDataFromApiReturnList<T>(string url, string token, string reportId)      {
            var result = new List<T>();
            try
            {
                log.Info("****************************Bắt đầu lấy dữ liệu  api " + url + ", report id: " + reportId + " ********************************");
                HttpResponseMessage response = await ApiBase.PostJsonAsyncRaw(url, token, reportId);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new List<T>();
                }
                else
                {
                    string responseString = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        result = JsonConvert.DeserializeObject<List<T>>(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Llấy dữ liệu  api {url}, report id:{reportId} lỗi: {ex.Message}\n {ex.StackTrace}");
                result = new List<T>();
            }

            log.Info("Số bản ghi api trả ra: " + result.Count());

            log.Info("****************************Kết thúc lấy dữ liệu  api " + url + ", report id: " + reportId + " ********************************");
            return result;
        }

        /// <summary>
        /// Lấy dữ liệu từ đầu Api trả lại string json đầu api trả ra
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public async Task<string> PostDataFromApiReturnString(string url, string token, string reportId)
        {
            var result ="";
            try
            {
                log.Info("****************************Bắt đầu lấy dữ liệu  api " + url + ", report id: " + reportId + " ********************************");
                HttpResponseMessage response = await ApiBase.PostJsonAsyncRaw(url, token, reportId);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return "";
                }
                else
                {
                    string responseString = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        result = responseString;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Llấy dữ liệu  api {url}, report id:{reportId} lỗi: {ex.Message}\n {ex.StackTrace}");
                result = "";
            }

            log.Info("Số bản ghi api trả ra: " + result.Count());

            log.Info("****************************Kết thúc lấy dữ liệu  api " + url + ", report id: " + reportId + " ********************************");
            return result;
        }
    }
}
