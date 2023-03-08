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

namespace Common.ICommon
{
    public interface IApiBase
    {

        Task<HttpResponseMessage> UPPostJsonAsync(string uri, string json);

        Task<HttpResponseMessage> PostJsonAsync(string uri, string json);
        //TEST
        Task<HttpResponseMessage> ReCallPostJsonAsync(string uri, string json);
        //TEST
        Task<string> GetJsonAsync(string url);

        Task<HttpResponseMessage> GetJsonAsyncResponse(string url);

        Task<HttpResponseMessage> GetJsonAsyncResponseReport(string url);

        Task<string> GetBase64Async(string url);

        // CuongHM add
        Task<string> PutJsonAsync(string url, string json);

        Task<HttpResponseMessage> PutJsonAsyncResponse(string url, string json);

        Task<HttpResponseMessage> PostJsonAsyncRaw(string url, string token, string reportId);
    }
}
