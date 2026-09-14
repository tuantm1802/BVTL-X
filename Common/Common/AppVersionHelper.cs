using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Hosting;

namespace Common.Common
{
    public class AppVersionInfo
    {
        public string Version { get; set; }
        public string Commit { get; set; }
        public string Branch { get; set; }
        public string BuildDate { get; set; }
        public string Environment { get; set; }

        public string DisplayVersion
        {
            get
            {
                if (string.IsNullOrEmpty(Commit) || Commit == "unknown")
                    return Version ?? "v1.0.0";
                return string.Format("{0} ({1})", Version ?? "v1.0.0", Commit);
            }
        }

        public AppVersionInfo()
        {
            Version = "v1.0.0";
            Commit = "unknown";
            Branch = "main";
            BuildDate = "";
            Environment = "Production";
        }
    }

    public static class AppVersionHelper
    {
        private static AppVersionInfo _cachedInfo;
        private static readonly object _lock = new object();

        public static AppVersionInfo GetVersionInfo()
        {
            if (_cachedInfo != null) return _cachedInfo;

            lock (_lock)
            {
                if (_cachedInfo != null) return _cachedInfo;

                try
                {
                    string path = null;
                    if (HostingEnvironment.IsHosted)
                    {
                        path = HostingEnvironment.MapPath("~/version.json");
                    }
                    else
                    {
                        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version.json");
                    }

                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        var json = File.ReadAllText(path, Encoding.UTF8);
                        var parsed = JsonConvert.DeserializeObject<AppVersionInfo>(json);
                        if (parsed != null)
                        {
                            _cachedInfo = parsed;
                            return _cachedInfo;
                        }
                    }
                }
                catch
                {
                    // Fallback to assembly info
                }

                var asm = Assembly.GetExecutingAssembly();
                var asmName = asm.GetName();
                var fileInfo = new FileInfo(asm.Location);

                _cachedInfo = new AppVersionInfo
                {
                    Version = "v" + (asmName.Version != null ? asmName.Version.ToString(3) : "1.0.0"),
                    Commit = "unknown",
                    Branch = "main",
                    BuildDate = fileInfo.Exists ? fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Environment = "Production"
                };

                return _cachedInfo;
            }
        }

        public static void ResetCache()
        {
            lock (_lock)
            {
                _cachedInfo = null;
            }
        }
    }
}
