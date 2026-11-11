using System;
using System.Configuration;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace Common.Common
{
    /// <summary>
    /// Bảo vệ cổng đăng nhập: kiểm tra định dạng tên đăng nhập, phát hiện payload tấn công
    /// (SQLi / scanner) và giới hạn số lần đăng nhập thất bại theo IP / theo tài khoản (brute-force).
    /// Bộ đếm lưu trong MemoryCache (theo tiến trình ứng dụng).
    /// </summary>
    public static class LoginGuard
    {
        public const int MaxUserNameLength = 50;

        // Tên đăng nhập hợp lệ: chữ, số, . _ - @ (3-50 ký tự)
        private static readonly Regex ValidUserName =
            new Regex(@"^[A-Za-z0-9._@\-]{3,50}$", RegexOptions.Compiled);

        // Dấu hiệu payload tấn công thường gặp
        private static readonly Regex AttackPattern = new Regex(
            @"(['""`;]|--|/\*|\*/|%27|%22|\b(select|union|insert|update|delete|drop|waitfor|sleep|pg_sleep|benchmark|sysdate|xor|exec|char|cast|dual)\b|\bor\b\s+[\w'""(]+\s*=|\d\s*\*\s*\d\s*=|@@)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly MemoryCache Cache = new MemoryCache("LoginGuard");

        public static int MaxFailPerIp { get { return GetInt("LoginGuard.MaxFailPerIp", 5); } }
        public static int MaxFailPerUser { get { return GetInt("LoginGuard.MaxFailPerUser", 5); } }
        public static int WindowMinutes { get { return GetInt("LoginGuard.WindowMinutes", 5); } }
        public static int LockMinutes { get { return GetInt("LoginGuard.LockMinutes", 15); } }

        private static int GetInt(string key, int def)
        {
            int v;
            return int.TryParse(ConfigurationManager.AppSettings[key], out v) && v > 0 ? v : def;
        }

        public static bool IsValidUserName(string userName)
        {
            return !string.IsNullOrEmpty(userName) && ValidUserName.IsMatch(userName);
        }

        /// <summary>Chuỗi có chứa dấu hiệu tấn công (SQLi, time-based, ký tự điều khiển...).</summary>
        public static bool IsSuspicious(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            if (input.Length > MaxUserNameLength) return true;
            foreach (char c in input)
                if (char.IsControl(c)) return true;
            return AttackPattern.IsMatch(input);
        }

        /// <summary>Làm sạch giá trị để ghi log (bỏ ký tự điều khiển, cắt độ dài).</summary>
        public static string SanitizeForLog(string input, int maxLength = 255)
        {
            if (input == null) return "";
            var s = Regex.Replace(input, @"[\r\n\t\p{Cc}]", " ");
            return s.Length > maxLength ? s.Substring(0, maxLength) : s;
        }

        public static bool IsLoopback(string ip)
        {
            return string.IsNullOrEmpty(ip) || ip == "::1" || ip == "127.0.0.1";
        }

        // ===== Theo IP =====
        public static bool IsIpBlocked(string ip, out int retryAfterMinutes)
        {
            retryAfterMinutes = 0;
            if (IsLoopback(ip)) return false;
            return IsLocked("lock:ip:" + ip, out retryAfterMinutes);
        }

        public static void RegisterIpFailure(string ip)
        {
            if (IsLoopback(ip)) return;
            Register("ip:" + ip, "lock:ip:" + ip, MaxFailPerIp);
        }

        // ===== Theo tài khoản (chỉ gọi cho tài khoản thật sự tồn tại) =====
        public static bool IsUserLocked(string userName, out int retryAfterMinutes)
        {
            retryAfterMinutes = 0;
            if (string.IsNullOrEmpty(userName)) return false;
            return IsLocked("lock:user:" + userName.ToLowerInvariant(), out retryAfterMinutes);
        }

        public static void RegisterUserFailure(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return;
            var k = userName.ToLowerInvariant();
            Register("user:" + k, "lock:user:" + k, MaxFailPerUser);
        }

        public static void ResetUser(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return;
            var k = userName.ToLowerInvariant();
            Cache.Remove("user:" + k);
            Cache.Remove("lock:user:" + k);
        }

        public static void ClearAll()
        {
            foreach (var item in Cache) Cache.Remove(item.Key);
        }

        private static bool IsLocked(string lockKey, out int retryAfterMinutes)
        {
            retryAfterMinutes = 0;
            var until = Cache.Get(lockKey) as DateTime?;
            if (until.HasValue && until.Value > DateTime.UtcNow)
            {
                retryAfterMinutes = Math.Max(1, (int)Math.Ceiling((until.Value - DateTime.UtcNow).TotalMinutes));
                return true;
            }
            return false;
        }

        private static readonly object _sync = new object();

        private static void Register(string counterKey, string lockKey, int max)
        {
            lock (_sync)
            {
                var count = (Cache.Get(counterKey) as int?) ?? 0;
                count++;
                if (count >= max)
                {
                    Cache.Set(lockKey, (DateTime?)DateTime.UtcNow.AddMinutes(LockMinutes),
                        new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(LockMinutes) });
                    Cache.Remove(counterKey);
                }
                else
                {
                    Cache.Set(counterKey, (int?)count,
                        new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(WindowMinutes) });
                }
            }
        }
    }
}
