using Common;
using Common.Common;
using Common.ICommon;
using Data.InterfaceDA;
using Data.InterfaceDA.API;
using log4net;
using Model.ModelExtend.API;
using Model.ModelExtend.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Data.API
{
    public class GetDataFromAPI: IGetDataFromAPI
    {
        protected readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IApiBase _apiBase = new ApiBase();

        /// <summary>
        /// Lấy dữ liệu từ đầu Api trả lại list T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="reportId"></param>
        /// <param name="rawOrLabel"></param>
        /// <returns></returns>
        public async Task<List<T>> PostDataFromApiReturnList<T>(string url, string token, string reportId, string rawOrLabel)      {
            var result = new List<T>();
            try
            {
                log.Info("****************************Bắt đầu lấy dữ liệu  api " + url + ", report id: " + reportId + " ********************************");
                HttpResponseMessage response = await _apiBase.PostJsonAsyncRaw(url, token, reportId, rawOrLabel);
                log.Info("****************************###GetDataFromAPI:::PostDataFromApiReturnList:::response.StatusCode= " + response.StatusCode + "###***************************");
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
        public async Task<string> PostDataFromApiReturnString(string url, string token, string reportId, string rawOrLabel)
        {
            var result ="";
            try
            {
                log.Info("****************************Bắt đầu lấy dữ liệu  api " + url + ", report id: " + reportId + " ********************************");
                HttpResponseMessage response = await _apiBase.PostJsonAsyncRaw(url, token, reportId, rawOrLabel);

                log.Info("****************************###GetDataFromAPI:::PostDataFromApiReturnString:::response.StatusCode= " + response.StatusCode + "|report id= " + reportId + "###***************************");
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
                        //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 ;

                        //await SendTelegramMessage("589101034", "🎉 API:"+ reportId+ " đồng bộ OK! [GetDataFromAPI::PostDataFromApiReturnString]").ConfigureAwait(false);

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

        public static async Task SendTelegramMessage(string chatId, string message)
        {
            string botToken = "7553997923:AAFBabzEfRLdluri42vy3VixZwrLfv2BHPs";
            string url = $"https://api.telegram.org/bot{botToken}/sendMessage";

            using (var client = new HttpClient())
            {
                var parameters = new Dictionary<string, string>
        {
            { "chat_id", chatId },
            { "text", message }
        };

                var content = new FormUrlEncodedContent(parameters);
                HttpResponseMessage response = await client.PostAsync(url, content).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error sending Telegram message: {error}");
                }
            }
        }

        #region Call api tạo các file báo cáo

        /// <summary>
        /// Gọi api tạo danh sách file báo cáo
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<List<UserSendReportModel>> GetListFileReport(string url)
        {
            var result = new List<UserSendReportModel>();
            try
            {
                log.Info("****************************Bắt đầu gọi api tạo file báo cáo api " + url + "********************************");
                HttpResponseMessage response = await _apiBase.GetJsonAsyncResponseReport(url);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new List<UserSendReportModel>();
                }
                else
                {
                    string responseString = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        result = JsonConvert.DeserializeObject<List<UserSendReportModel>>(responseString); 
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"gọi api tạo file báo cáo  api {url} lỗi: {ex.Message}\n {ex.StackTrace}");
                result = new List<UserSendReportModel>();
            }
            log.Info("****************************Kết thúc gọi api tạo file báo cáo api " + url + " ********************************");
            return result;
        }
        #endregion

    }
}
