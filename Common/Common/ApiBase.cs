using Common.ICommon;
using log4net;
using Model.ModelExtend.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Common.Common
{
    public class ApiBase : IApiBase
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly HttpClient _httpClient;

        static ApiBase()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                MaxConnectionsPerServer = 100
            };
            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        public async Task<HttpResponseMessage> UPPostJsonAsync(string uri, string json)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                return await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                log.Error($"[UPPostJsonAsync] Lỗi: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }

        public async Task<HttpResponseMessage> PostJsonAsync(string uri, string json)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                return await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                log.Error($"[PostJsonAsync] Lỗi: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }

        public async Task<HttpResponseMessage> ReCallPostJsonAsync(string uri, string json)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                return await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                log.Error($"[ReCallPostJsonAsync] Lỗi: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }

        public async Task<string> GetJsonAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                log.Error($"[GetJsonAsync] Lỗi: {ex.Message}\n {ex.StackTrace}");
                return ex.Message;
            }
            return null;
        }

        public async Task<HttpResponseMessage> GetJsonAsyncResponse(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                log.Debug("[GetJsonAsyncResponse] Trạng thái API " + url + ": " + response.StatusCode);
                return response;
            }
            catch (Exception ex)
            {
                log.Error($"[GetJsonAsyncResponse] Lỗi: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }

        public async Task<HttpResponseMessage> GetJsonAsyncResponseReport(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                log.Debug("[GetJsonAsyncResponseReport] Trạng thái API " + url + ": " + response.StatusCode);
                return response;
            }
            catch (Exception ex)
            {
                log.Error($"[GetJsonAsyncResponseReport] Lỗi: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }

        public async Task<string> GetBase64Async(string url)
        {
            try
            {
                var bytes = await _httpClient.GetByteArrayAsync(url);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                log.Error($"[GetBase64Async] Lỗi: {ex.Message}\n {ex.StackTrace}");
                return ex.Message;
            }
        }

        public async Task<string> PutJsonAsync(string url, string json)
        {
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                log.Error($"[PutJsonAsync] Lỗi: {ex.Message}\n {ex.StackTrace}");
                return ex.Message;
            }
            return null;
        }

        public async Task<HttpResponseMessage> PutJsonAsyncResponse(string url, string json)
        {
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return await _httpClient.PutAsync(url, content);
            }
            catch (Exception ex)
            {
                log.Error($"[PutJsonAsyncResponse] Lỗi: {ex.Message}\n {ex.StackTrace}");
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResult { message = ex.Message }))
                };
            }
        }

        public async Task<HttpResponseMessage> PostJsonAsyncRaw(string url, string token, string reportId, string rawOrLabel = "label")
        {
            return await HttpRetryHelper.ExecuteWithRetryAsync(async () =>
            {
                var data = new[]
                {
                    new KeyValuePair<string, string>("token", token),
                    new KeyValuePair<string, string>("content", "report"),
                    new KeyValuePair<string, string>("format", "json"),
                    new KeyValuePair<string, string>("report_id", reportId),
                    new KeyValuePair<string, string>("csvDelimiter", ""),
                    new KeyValuePair<string, string>("rawOrLabel", rawOrLabel),
                    new KeyValuePair<string, string>("rawOrLabelHeaders", "raw"),
                    new KeyValuePair<string, string>("exportCheckboxLabel", "false"),
                    new KeyValuePair<string, string>("returnFormat", "json")
                };

                using (var content = new FormUrlEncodedContent(data))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    return await _httpClient.PostAsync(url, content);
                }
            });
        }
    }
}
