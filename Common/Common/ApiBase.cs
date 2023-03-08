using Common.ICommon;
using log4net;
using Model.ModelExtend.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Common.Common
{
    public class ApiBase : IApiBase
    {
        private  readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private  readonly string insideUrl = ConfigurationManager.AppSettings["insideUrl"].ToString();
        private  readonly string insideUrl = "";


        public  async Task<HttpResponseMessage> UPPostJsonAsync( string uri, string json)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(insideUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("token", Token);
                    //client.Timeout = TimeSpan.FromSeconds(20);s
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await Task.FromResult(client.PostAsync(uri, content).Result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                log.Info("---ex End API");
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }

        public  async Task<HttpResponseMessage> PostJsonAsync( string uri, string json)
        {
            HttpResponseMessage response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(insideUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(20);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    //client.DefaultRequestHeaders.Add("token", Token);
                    response = await client.PostAsync(uri, content);
                    return response;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }
        //TEST
        public  async Task<HttpResponseMessage> ReCallPostJsonAsync( string uri, string json)
        {
            try
            {
                string url = uri;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("token", Token);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content);
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Gọi lại khi token hết hạn
                        // Lưu lại token
                        Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        configuration.Save(ConfigurationSaveMode.Full, true);
                        ConfigurationManager.RefreshSection("appSettings");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //client.DefaultRequestHeaders.Add("token", Token);
                        response = await client.PostAsync(client.BaseAddress, content);

                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }
        //TEST
        public  async Task<string> GetJsonAsync( string url)
        {
            HttpResponseMessage response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(insideUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("token", Token);
                    response = await client.GetAsync(url);
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Gọi lại khi token hết hạn
                        // Lưu lại token
                        Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        configuration.Save(ConfigurationSaveMode.Full, true);
                        ConfigurationManager.RefreshSection("appSettings");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //client.DefaultRequestHeaders.Add("token", Token);
                        response = await client.GetAsync(url);

                    }
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;
                        return responseString;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                return ex.Message;
            }
            return null;
        }

        public  async Task<HttpResponseMessage> GetJsonAsyncResponse( string url)
        {
            HttpResponseMessage response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(insideUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("token", Token);
                    //client.Timeout = new TimeSpan(100);
                    response = await client.GetAsync(url);
                    log.Debug("Trạng thái của API " + client.BaseAddress + ": " + response.StatusCode);
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Gọi lại khi token hết hạn
                        // Lưu lại token
                        Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        configuration.Save(ConfigurationSaveMode.Full, true);
                        ConfigurationManager.RefreshSection("appSettings");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //client.DefaultRequestHeaders.Add("token", Token);
                        response = await (client.GetAsync(url));

                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }

        public async Task<HttpResponseMessage> GetJsonAsyncResponseReport(string url)
        {
            HttpResponseMessage response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("token", Token);
                    //client.Timeout = new TimeSpan(100);
                    response = await client.GetAsync(url);
                    log.Debug("Trạng thái của API " + client.BaseAddress + ": " + response.StatusCode);
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Gọi lại khi token hết hạn
                        // Lưu lại token
                        Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        configuration.Save(ConfigurationSaveMode.Full, true);
                        ConfigurationManager.RefreshSection("appSettings");
                        client.DefaultRequestHeaders.Accept.Clear();
                        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //client.DefaultRequestHeaders.Add("token", Token);
                        response = await (client.GetAsync(url));

                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }


        public async Task<string> GetBase64Async( string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(insideUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("token", Token);
                    var bytes = await client.GetByteArrayAsync(url);
                    string base64 = Convert.ToBase64String(bytes);
                    return base64;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                return ex.Message;
            }
        }

        // CuongHM add
        public  async Task<string> PutJsonAsync( string url, string json)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(insideUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("token", Token);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Gọi lại khi token hết hạn
                        // Lưu lại token
                        Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        configuration.Save(ConfigurationSaveMode.Full, true);
                        ConfigurationManager.RefreshSection("appSettings");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //client.DefaultRequestHeaders.Add("token", Token);
                        content = new StringContent(json, Encoding.UTF8, "application/json");
                        response = await client.PutAsync(url, content);
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;
                        return responseString;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                return ex.Message;
            }
            return null;
        }

        public  async Task<HttpResponseMessage> PutJsonAsyncResponse( string url, string json)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(insideUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("token", Token);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Gọi lại khi token hết hạn
                        // Lưu lại token
                        Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        configuration.Save(ConfigurationSaveMode.Full, true);
                        ConfigurationManager.RefreshSection("appSettings");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //client.DefaultRequestHeaders.Add("token", Token);
                        response = await client.PutAsync(url, content);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
            return null;
        }

        public  async Task<HttpResponseMessage> PostJsonAsyncRaw(string url, string token, string reportId)
        {
            HttpResponseMessage response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    // //Passing service base url  
                    // client.BaseAddress = new Uri(insideUrl);

                    // client.DefaultRequestHeaders.Clear();
                    // //Define request data format  
                    // client.Timeout = TimeSpan.FromSeconds(100);

                    var data = new[]
                    {
                         new KeyValuePair<string, string>("token", token),
                         new KeyValuePair<string, string>("content", "report"),
                         new KeyValuePair<string, string>("format", "json"),
                         new KeyValuePair<string, string>("report_id", reportId),
                         new KeyValuePair<string, string>("csvDelimiter", ""),
                         new KeyValuePair<string, string>("rawOrLabel", "label"),
                         new KeyValuePair<string, string>("rawOrLabelHeaders", "raw"),
                         new KeyValuePair<string, string>("exportCheckboxLabel", "false"),
                         new KeyValuePair<string, string>("returnFormat", "json")
                     };
                    // client.DefaultRequestHeaders.Add("content-type", "application/x-www-form-urlencoded");

                    // var content = new FormUrlEncodedContent(data);
                    //// content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    // response = await client.PostAsync(url, content);

                    using (var content = new FormUrlEncodedContent(data))
                    {
                        content.Headers.Clear();
                        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                        response = await client.PostAsync(url, content);
                    }

                    return response;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }
    }
}
