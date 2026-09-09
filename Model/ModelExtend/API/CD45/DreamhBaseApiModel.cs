using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Model.ModelExtend.API.CD45
{
    /// <summary>
    /// Model Base hung toan bo du lieu JSON tu REDCap linh hoat
    /// </summary>
    public class DreamhBaseApiModel
    {
        public string record_id { get; set; }
        public string redcap_repeat_instance { get; set; }
        public string redcap_data_access_group { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }

        public int? RepeatInstance => int.TryParse(redcap_repeat_instance, out int r) ? (int?)r : null;

        public string GetString(string key) {
            if (string.Equals(key, "record_id", StringComparison.OrdinalIgnoreCase)) return record_id;
            if (string.Equals(key, "redcap_repeat_instance", StringComparison.OrdinalIgnoreCase)) return redcap_repeat_instance;
            if (string.Equals(key, "redcap_data_access_group", StringComparison.OrdinalIgnoreCase)) return redcap_data_access_group;
            if (AdditionalData != null && AdditionalData.ContainsKey(key)) {
                var val = AdditionalData[key];
                return val != null ? val.ToString() : null;
            }
            return null;
        }

        public byte? GetByte(string key) {
            var s = GetString(key);
            if (byte.TryParse(s, out byte r)) return r;
            return null;
        }

        public int? GetInt(string key) {
            var s = GetString(key);
            if (int.TryParse(s, out int r)) return r;
            return null;
        }
        
        public bool? GetBool(string key) {
            var s = GetString(key);
            if (string.IsNullOrEmpty(s)) return null;
            return s == "1";
        }

        public DateTime? GetDate(string key) {
            var s = GetString(key);
            if (DateTime.TryParse(s, out DateTime d)) return d;
            return null;
        }
    }
}
