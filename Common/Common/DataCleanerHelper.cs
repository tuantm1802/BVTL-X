using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Model.ModelExtend.API.CD45;

namespace Common.Common
{
    public static class DataCleanerHelper
    {
        // Regex kiểm tra mã khách hàng chuẩn: D + 2 ký tự tỉnh + 2 ký tự nhóm + 4 số thứ tự
        private static readonly Regex RecordIdRegex = new Regex(@"^D(HN|HP|HY|NA|NB|HC)\d{2}\d{4}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// R1. Chuẩn hóa Record ID (Trim, UPPER) và kiểm tra bất thường (ERR_RECORD_ID_FORMAT)
        /// </summary>
        public static string CleanRecordId(string rawRecordId, string apiCode, string tableName, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (string.IsNullOrWhiteSpace(rawRecordId))
            {
                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = rawRecordId,
                    FIELD_NAME = "record_id",
                    OLD_VALUE = rawRecordId,
                    NEW_VALUE = null,
                    RULE_CODE = "ERR_RECORD_ID_EMPTY",
                    SEVERITY = "ERROR",
                    ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                    MESSAGE = "Mã khách hàng record_id bị trống.",
                    CREATED_DATE = DateTime.Now
                });
                return null;
            }

            string clean = rawRecordId.Trim().ToUpper();

            // Nếu có sự thay đổi (chữ thường -> chữ hoa hoặc có khoảng trắng thừa)
            if (clean != rawRecordId)
            {
                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = clean,
                    FIELD_NAME = "record_id",
                    OLD_VALUE = rawRecordId,
                    NEW_VALUE = clean,
                    RULE_CODE = "R1_RECORD_ID_AUTO_UPPER",
                    SEVERITY = "INFO",
                    ACTION_TAKEN = "AUTO_NORMALIZED",
                    MESSAGE = "Tự động xóa khoảng trắng và viết HOA mã khách hàng.",
                    CREATED_DATE = DateTime.Now
                });
            }

            // Kiểm tra format chuẩn
            if (!RecordIdRegex.IsMatch(clean))
            {
                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = clean,
                    FIELD_NAME = "record_id",
                    OLD_VALUE = rawRecordId,
                    NEW_VALUE = clean,
                    RULE_CODE = "ERR_RECORD_ID_FORMAT",
                    SEVERITY = "WARNING",
                    ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                    MESSAGE = $"Mã khách hàng '{clean}' không đúng quy chuẩn 9 ký tự (ví dụ: DHN010001, DNA220001). Cần kiểm tra trên REDCap.",
                    CREATED_DATE = DateTime.Now
                });
            }

            return clean;
        }

        /// <summary>
        /// R2. Chuẩn hóa chuỗi văn bản (Trim, loại bỏ đa khoảng trắng)
        /// </summary>
        public static string CleanString(string text, string fieldName, string recordId, string apiCode, string tableName, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (text == null) return null;
            if (string.IsNullOrWhiteSpace(text)) return null;

            string clean = Regex.Replace(text.Trim(), @"\s+", " ");
            if (clean != text && logs != null)
            {
                // Chỉ log khi cần thiết để tránh spam log
            }
            return clean;
        }

        /// <summary>
        /// R3. Chuẩn hóa DAG (Nhóm truy cập dữ liệu)
        /// </summary>
        public static void ProcessDag(string rawDag, string recordId, string apiCode, string tableName, string reportId, string maDuAn, 
            ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, out string maNhomStd, out string maNhomMap, out string cityCode)
        {
            maNhomStd = null;
            maNhomMap = null;
            cityCode = null;

            if (string.IsNullOrWhiteSpace(rawDag))
            {
                return;
            }

            string cleanDag = rawDag.Trim();
            if (CD45Helper.NormalizeDagToGroupCode(cleanDag, out maNhomStd, out maNhomMap, out cityCode))
            {
                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = recordId,
                    FIELD_NAME = "redcap_data_access_group",
                    OLD_VALUE = rawDag,
                    NEW_VALUE = $"{maNhomStd} ({maNhomMap})",
                    RULE_CODE = "R3_DAG_NORMALIZED",
                    SEVERITY = "INFO",
                    ACTION_TAKEN = "AUTO_NORMALIZED",
                    MESSAGE = $"Chuẩn hóa nhóm '{rawDag}' -> Mã nhóm '{maNhomStd}' (Map: '{maNhomMap}').",
                    CREATED_DATE = DateTime.Now
                });
            }
            else
            {
                // Cảnh báo DAG không nhận diện được
                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = recordId,
                    FIELD_NAME = "redcap_data_access_group",
                    OLD_VALUE = rawDag,
                    NEW_VALUE = null,
                    RULE_CODE = "ERR_DAG_UNMAPPED",
                    SEVERITY = "WARNING",
                    ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                    MESSAGE = $"Giá trị Nhóm truy cập (DAG) '{rawDag}' chưa được định nghĩa trong danh mục 22 nhóm CD45.",
                    CREATED_DATE = DateTime.Now
                });
            }
        }

        /// <summary>
        /// R4. Chuẩn hóa ngày tháng & kiểm tra ngày tương lai (bỏ qua kiểm tra tương lai với ngày hẹn tái khám/lịch hẹn)
        /// </summary>
        public static DateTime? CleanDate(string rawDateStr, string fieldName, string recordId, string apiCode, string tableName, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, bool allowFutureDate = false)
        {
            if (string.IsNullOrWhiteSpace(rawDateStr)) return null;

            DateTime parsed;
            string[] formats = new string[] { "yyyy-MM-dd", "dd/MM/yyyy", "yyyy/MM/dd", "yyyy-MM-dd HH:mm:ss", "dd-MM-yyyy" };

            if (!DateTime.TryParseExact(rawDateStr.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                if (!DateTime.TryParse(rawDateStr, out parsed))
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = tableName,
                        RECORD_ID = recordId,
                        FIELD_NAME = fieldName,
                        OLD_VALUE = rawDateStr,
                        NEW_VALUE = null,
                        RULE_CODE = "ERR_DATE_INVALID_FORMAT",
                        SEVERITY = "WARNING",
                        ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                        MESSAGE = $"Định dạng ngày tháng '{rawDateStr}' của trường '{fieldName}' không hợp lệ.",
                        CREATED_DATE = DateTime.Now
                    });
                    return null;
                }
            }

            // Danh sách các trường ngày hẹn/tái khám trong tương lai hợp lệ (không kiểm tra lỗi ngày tương lai)
            bool isFutureAllowed = allowFutureDate ||
                                  "f6_followup_visit".Equals(fieldName, StringComparison.OrdinalIgnoreCase) ||
                                  "f7_q61".Equals(fieldName, StringComparison.OrdinalIgnoreCase) ||
                                  "f8_q61".Equals(fieldName, StringComparison.OrdinalIgnoreCase);

            // Kiểm tra ngày trong tương lai (chỉ cảnh báo cho các trường ngày phát sinh sự kiện trong quá khứ)
            if (!isFutureAllowed && parsed.Date > DateTime.Today)
            {
                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = recordId,
                    FIELD_NAME = fieldName,
                    OLD_VALUE = rawDateStr,
                    NEW_VALUE = parsed.ToString("yyyy-MM-dd"),
                    RULE_CODE = "ERR_DATE_FUTURE",
                    SEVERITY = "WARNING",
                    ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                    MESSAGE = $"Ngày phát sinh sự kiện '{parsed:dd/MM/yyyy}' lớn hơn ngày hiện tại ({DateTime.Today:dd/MM/yyyy}).",
                    CREATED_DATE = DateTime.Now
                });
            }

            return parsed;
        }

        /// <summary>
        /// R5. Kiểm tra năm sinh hợp lệ (1920 <= NamSinh <= CurrentYear - 10)
        /// </summary>
        public static short? CleanYearOfBirth(int? yob, string recordId, string apiCode, string tableName, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (!yob.HasValue) return null;

            int currentYear = DateTime.Now.Year;
            if (yob.Value < 1920 || yob.Value > (currentYear - 10))
            {
                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = recordId,
                    FIELD_NAME = "f1_q_a3 (NAM_SINH)",
                    OLD_VALUE = yob.Value.ToString(),
                    NEW_VALUE = yob.Value.ToString(),
                    RULE_CODE = "ERR_YOB_INVALID",
                    SEVERITY = "WARNING",
                    ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                    MESSAGE = $"Năm sinh {yob.Value} bất hợp lý (ngoài khoảng 1920 - {currentYear - 10}). Cần kiểm tra lại thông tin khách hàng.",
                    CREATED_DATE = DateTime.Now
                });
            }

            return (short)yob.Value;
        }
    }
}
