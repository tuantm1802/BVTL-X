using log4net;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Common
{
    public static class HttpRetryHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HttpRetryHelper));

        /// <summary>
        /// Thực thi một hành động gọi HTTP bất đồng bộ với chính sách tự động thử lại khi gặp lỗi tạm thời (Exponential Backoff).
        /// </summary>
        /// <param name="action">Hàm bất đồng bộ thực hiện cuộc gọi HTTP</param>
        /// <param name="maxRetries">Số lần tự động thử lại tối đa (Mặc định: 3 lần)</param>
        /// <param name="initialDelayMs">Thời gian chờ ban đầu tính bằng mili giây (Mặc định: 1000ms)</param>
        /// <returns>HttpResponseMessage của cuộc gọi thành công hoặc kết quả của lần thử cuối</returns>
        public static async Task<HttpResponseMessage> ExecuteWithRetryAsync(
            Func<Task<HttpResponseMessage>> action,
            int maxRetries = 3,
            int initialDelayMs = 1000
        )
        {
            int currentRetry = 0;
            int delayMs = initialDelayMs;
            HttpResponseMessage response = null;

            while (true)
            {
                try
                {
                    response = await action();

                    // Nếu thành công hoặc không phải lỗi có thể rủi ro ngắt mạng, trả về kết quả
                    if (response != null && IsSuccessOrNonTransientStatus(response.StatusCode))
                    {
                        return response;
                    }

                    // Nếu phản hồi lỗi Server (5xx), xem xét thử lại
                    if (currentRetry >= maxRetries)
                    {
                        log.Warn($"[HttpRetryHelper] Đã thử lại tối đa {maxRetries} lần nhưng API vẫn trả về mã lỗi HTTP {response?.StatusCode}.");
                        return response;
                    }

                    log.Warn($"[HttpRetryHelper] Nhận phản hồi HTTP {response?.StatusCode}. Thử lại lần {currentRetry + 1}/{maxRetries} sau {delayMs}ms...");
                }
                catch (Exception ex) when (IsTransientException(ex) && currentRetry < maxRetries)
                {
                    log.Warn($"[HttpRetryHelper] Gặp ngoại lệ ngắt kết nối mạng: {ex.Message}. Thử lại lần {currentRetry + 1}/{maxRetries} sau {delayMs}ms...");
                }
                catch (Exception ex)
                {
                    log.Error($"[HttpRetryHelper] Gặp lỗi nghiêm trọng không thể khôi phục khi gọi HTTP: {ex.Message}", ex);
                    throw;
                }

                currentRetry++;
                await Task.Delay(delayMs);
                delayMs *= 2; // Exponential Backoff: 1s -> 2s -> 4s
            }
        }

        private static bool IsSuccessOrNonTransientStatus(System.Net.HttpStatusCode statusCode)
        {
            int code = (int)statusCode;
            // Trả về thành công nếu 2xx, 3xx hoặc các lỗi Client 4xx (trừ 408 Request Timeout hay 429 Too Many Requests)
            if (code >= 200 && code < 500 && code != 408 && code != 429)
            {
                return true;
            }
            return false;
        }

        private static bool IsTransientException(Exception ex)
        {
            return ex is HttpRequestException || 
                   ex is TaskCanceledException || 
                   ex is TimeoutException;
        }
    }
}
