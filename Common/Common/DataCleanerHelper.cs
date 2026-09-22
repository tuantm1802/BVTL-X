using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Model.ModelExtend.API.CD45;

namespace Common.Common
{
    public class DagSummaryCollector
    {
        private readonly Dictionary<string, (string RawDag, string MaNhomStd, string MaNhomMap, int Count)> _counts 
            = new Dictionary<string, (string, string, string, int)>(StringComparer.OrdinalIgnoreCase);

        public void Record(string rawDag, string maNhomStd, string maNhomMap)
        {
            if (string.IsNullOrWhiteSpace(rawDag)) return;
            string key = rawDag.Trim();
            if (_counts.TryGetValue(key, out var val))
            {
                _counts[key] = (val.RawDag, val.MaNhomStd, val.MaNhomMap, val.Count + 1);
            }
            else
            {
                _counts[key] = (key, maNhomStd, maNhomMap, 1);
            }
        }

        public void FlushToLogs(string maDuAn, string apiCode, string tableName, string reportId, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (logs == null) return;
            foreach (var item in _counts.Values)
            {
                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = $"BATCH (N={item.Count})",
                    FIELD_NAME = "redcap_data_access_group",
                    OLD_VALUE = item.RawDag,
                    NEW_VALUE = $"{item.MaNhomStd} ({item.MaNhomMap})",
                    RULE_CODE = "R3_DAG_NORMALIZED",
                    SEVERITY = "INFO",
                    ACTION_TAKEN = "AUTO_NORMALIZED",
                    MESSAGE = $"Tự động chuẩn hóa {item.Count:N0} bản ghi từ nhóm '{item.RawDag}' -> Mã nhóm '{item.MaNhomStd}' (Map: '{item.MaNhomMap}').",
                    CREATED_DATE = DateTime.Now,
                    MA_NHOM = item.MaNhomMap,
                    CITY_CODE = CD45Helper.NormalizeDagToGroupCode(item.RawDag, out _, out _, out string cDag) ? cDag : null
                });
            }
            _counts.Clear();
        }
    }

    public static class DataCleanerHelper
    {
        // Mốc thời gian khởi động chính thức của dự án (2026-01-01)
        public static readonly DateTime ProjectStartDate = new DateTime(2026, 1, 1);

        // Regex kiểm tra mã khách hàng chuẩn: D + 2 ký tự chữ cái tỉnh/địa bàn + 2 ký tự số nhóm + 4 số thứ tự (đúng 9 ký tự)
        private static readonly Regex RecordIdRegex = new Regex(@"^D[A-Z]{2}\d{2}\d{4}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// R1 & VR-02 [BLOCKING]. Chuẩn hóa Record ID (Trim, UPPER), kiểm tra định dạng 9 ký tự và đối chiếu mã tỉnh với DAG.
        /// Chặn cứng (trả về null và gắn log ERROR) khi vi phạm.
        /// </summary>
        public static string CleanRecordId(string rawRecordId, string apiCode, string tableName, string reportId, string maDuAn, 
            ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, string rawDag = null)
        {
            string inferredNhom = null;
            string inferredCity = null;
            CD45Helper.InferGroupAndCity(rawRecordId, rawDag, out inferredNhom, out inferredCity);

            if (string.IsNullOrWhiteSpace(rawRecordId))
            {
                if (logs != null)
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
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = "Mã khách hàng record_id bị trống. Bắt buộc cách ly không nạp vào hệ thống.",
                        CREATED_DATE = DateTime.Now,
                        MA_NHOM = inferredNhom,
                        CITY_CODE = inferredCity
                    });
                }
                return null;
            }

            string clean = rawRecordId.Trim().ToUpper();
            if (string.IsNullOrEmpty(inferredCity) || string.IsNullOrEmpty(inferredNhom))
            {
                CD45Helper.InferGroupAndCity(clean, rawDag, out string n2, out string c2);
                if (string.IsNullOrEmpty(inferredNhom)) inferredNhom = n2;
                if (string.IsNullOrEmpty(inferredCity)) inferredCity = c2;
            }

            // Nếu có sự thay đổi (chữ thường -> chữ hoa hoặc có khoảng trắng thừa)
            if (clean != rawRecordId && logs != null)
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
                    CREATED_DATE = DateTime.Now,
                    MA_NHOM = inferredNhom,
                    CITY_CODE = inferredCity
                });
            }

            // VR-02(1) [BLOCKING]: Kiểm tra format chuẩn 9 ký tự: D + Mã Tỉnh (2 ký tự) + 6 số
            if (!RecordIdRegex.IsMatch(clean))
            {
                if (logs != null)
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
                        NEW_VALUE = null,
                        RULE_CODE = "ERR_RECORD_ID_FORMAT",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = $"Mã khách hàng '{clean}' không đúng quy chuẩn 9 ký tự (ví dụ: DHN010001, DHP100124). Bắt buộc chặn và cách ly.",
                        CREATED_DATE = DateTime.Now,
                        MA_NHOM = inferredNhom,
                        CITY_CODE = inferredCity
                    });
                }
                return null; // Chặn cứng không cho nạp vào bảng hoạt động
            }

            // VR-02(1) [BLOCKING]: Đối chiếu mã tỉnh trong Record ID với Data Access Group (DAG)
            if (!string.IsNullOrWhiteSpace(rawDag))
            {
                string idCity = CD45Helper.ExtractCityCode(clean);
                if (CD45Helper.NormalizeDagToGroupCode(rawDag.Trim(), out string maNhomStd, out string maNhomMap, out string dagCity))
                {
                    // Ngoại lệ hợp lệ: Nhóm Quỳnh Hương Xanh (qunh_hng_xanh / qhx) được phép dùng cả mã NA (Nghệ An) và NT (Nha Trang)
                    bool isAllowedCrossProvince = (idCity == "NT" || idCity == "NAN") && (maNhomMap == "qhx" || rawDag.IndexOf("qunh_hng_xanh", StringComparison.OrdinalIgnoreCase) >= 0);

                    if (!isAllowedCrossProvince && !string.IsNullOrEmpty(dagCity) && !string.Equals(idCity, dagCity, StringComparison.OrdinalIgnoreCase))
                    {
                        if (logs != null)
                        {
                            logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                            {
                                MADUAN = maDuAn,
                                REPORT_ID = reportId,
                                API_CODE = apiCode,
                                TABLE_NAME = tableName,
                                RECORD_ID = clean,
                                FIELD_NAME = "record_id / redcap_data_access_group",
                                OLD_VALUE = $"{clean} (DAG: {rawDag})",
                                NEW_VALUE = null,
                                RULE_CODE = "ERR_RECORD_ID_DAG_MISMATCH",
                                SEVERITY = "ERROR",
                                ACTION_TAKEN = "QUARANTINED",
                                MESSAGE = $"Mã khách hàng '{clean}' (Mã tỉnh: {idCity}) không khớp với tỉnh của nhóm truy cập DAG '{rawDag}' (Tỉnh: {dagCity}). Bắt buộc cách ly.",
                                CREATED_DATE = DateTime.Now,
                                MA_NHOM = inferredNhom ?? maNhomMap,
                                CITY_CODE = inferredCity ?? dagCity
                            });
                        }
                        return null; // Chặn cứng không cho nạp
                    }
                }
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
            return clean;
        }

        /// <summary>
        /// R3. Chuẩn hóa DAG (Nhóm truy cập dữ liệu). Hỗ trợ gom nhóm đếm tổng hợp qua DagSummaryCollector.
        /// </summary>
        public static void ProcessDag(string rawDag, string recordId, string apiCode, string tableName, string reportId, string maDuAn, 
            ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, out string maNhomStd, out string maNhomMap, out string cityCode, DagSummaryCollector collector = null)
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
                if (collector != null)
                {
                    collector.Record(cleanDag, maNhomStd, maNhomMap);
                }
            }
            else
            {
                // Cảnh báo DAG không nhận diện được (WARNING - cần quản trị viên kiểm tra)
                if (logs != null)
                {
                    CD45Helper.InferGroupAndCity(recordId, rawDag, out string infNhom, out string infCity);
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
                        CREATED_DATE = DateTime.Now,
                        MA_NHOM = infNhom,
                        CITY_CODE = infCity
                    });
                }
            }
        }

        /// <summary>
        /// R4 & VR-03 [BLOCKING]. Chuẩn hóa ngày tháng, chặn ngày tương lai và ngày trước khi khởi động dự án (2026-01-01).
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
                    if (logs != null)
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
                            SEVERITY = "ERROR",
                            ACTION_TAKEN = "QUARANTINED",
                            MESSAGE = $"Định dạng ngày tháng '{rawDateStr}' của trường '{fieldName}' không hợp lệ. Bắt buộc cách ly.",
                            CREATED_DATE = DateTime.Now
                        });
                    }
                    return null;
                }
            }

            // Danh sách các trường ngày hẹn/tái khám trong tương lai hợp lệ (không kiểm tra lỗi ngày tương lai)
            bool isFutureAllowed = allowFutureDate ||
                                  "f6_followup_visit".Equals(fieldName, StringComparison.OrdinalIgnoreCase) ||
                                  "f7_q61".Equals(fieldName, StringComparison.OrdinalIgnoreCase) ||
                                  "f8_q61".Equals(fieldName, StringComparison.OrdinalIgnoreCase);

            // VR-03(a) [BLOCKING]: Kiểm tra ngày tương lai
            if (!isFutureAllowed && parsed.Date > DateTime.Today)
            {
                if (logs != null)
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
                        RULE_CODE = "ERR_DATE_FUTURE",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = $"Ngày phát sinh sự kiện '{parsed:dd/MM/yyyy}' của trường '{fieldName}' nằm ở tương lai so với ngày hiện tại ({DateTime.Today:dd/MM/yyyy}). Bắt buộc cách ly.",
                        CREATED_DATE = DateTime.Now
                    });
                }
                return null;
            }

            // VR-03(a) [BLOCKING]: Kiểm tra ngày trước khởi động dự án (2026-01-01)
            if (parsed.Date < ProjectStartDate)
            {
                if (logs != null)
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
                        RULE_CODE = "ERR_DATE_BEFORE_PROJECT",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = $"Ngày phát sinh sự kiện '{parsed:dd/MM/yyyy}' của trường '{fieldName}' xảy ra trước ngày khởi động dự án ({ProjectStartDate:dd/MM/yyyy}). Bắt buộc cách ly.",
                        CREATED_DATE = DateTime.Now
                    });
                }
                return null;
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
                if (logs != null)
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
            }

            return (short)yob.Value;
        }

        #region P1 & P2: Quy Tắc Xác Thực Nghiệp Vụ Liên Kết Form & Dịch Vụ

        /// <summary>
        /// VR-04(a) [BLOCKING]. Kiểm tra ràng buộc hồ sơ gốc F1 phải tồn tại, Complete ('2') và có nhóm đích hợp lệ (1..5).
        /// </summary>
        public static bool ValidateClientF1(string recordId, CD45ValidationContext context, string apiCode, string tableName, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (context == null || context.KhachHangLookup == null || context.KhachHangLookup.Count == 0) return true;
            if (string.IsNullOrEmpty(recordId)) return false;

            if (!context.KhachHangLookup.TryGetValue(recordId, out var f1))
            {
                if (logs != null)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = tableName,
                        RECORD_ID = recordId,
                        FIELD_NAME = "record_id",
                        OLD_VALUE = recordId,
                        NEW_VALUE = null,
                        RULE_CODE = "ERR_F1_MISSING",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = $"Khách hàng {recordId} chưa có hồ sơ gốc F1. Bắt buộc cách ly không nạp dịch vụ vào bảng {tableName}.",
                        CREATED_DATE = DateTime.Now
                    });
                }
                return false;
            }

            if (!f1.IsComplete)
            {
                if (logs != null)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = tableName,
                        RECORD_ID = recordId,
                        FIELD_NAME = "f1_thng_tin_khch_hng_complete",
                        OLD_VALUE = "Incomplete",
                        NEW_VALUE = null,
                        RULE_CODE = "ERR_F1_NOT_COMPLETE",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = $"Hồ sơ gốc F1 của khách hàng {recordId} chưa hoàn thành (chưa Complete). Bắt buộc cách ly không nạp dịch vụ vào bảng {tableName}.",
                        CREATED_DATE = DateTime.Now
                    });
                }
                return false;
            }

            if (!f1.DoiTuong.HasValue || f1.DoiTuong.Value < 1 || f1.DoiTuong.Value > 5)
            {
                if (logs != null)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = tableName,
                        RECORD_ID = recordId,
                        FIELD_NAME = "f1_q_a4",
                        OLD_VALUE = f1.DoiTuong?.ToString(),
                        NEW_VALUE = null,
                        RULE_CODE = "ERR_F1_INVALID_TARGET_GROUP",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = $"Hồ sơ gốc F1 của khách hàng {recordId} có nhóm đích không hợp lệ ({f1.DoiTuong}). Bắt buộc cách ly.",
                        CREATED_DATE = DateTime.Now
                    });
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// VR-03(b) [BLOCKING]. Kiểm tra ngày dịch vụ F2-F10 phải >= ngày tham gia F1.
        /// </summary>
        public static bool ValidateServiceDateAgainstF1(string recordId, DateTime? serviceDate, string dateFieldName, CD45ValidationContext context, string apiCode, string tableName, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (context == null || !serviceDate.HasValue || string.IsNullOrEmpty(recordId)) return true;

            if (context.KhachHangLookup.TryGetValue(recordId, out var f1) && f1.NgayThamGia.HasValue)
            {
                if (serviceDate.Value.Date < f1.NgayThamGia.Value.Date)
                {
                    if (logs != null)
                    {
                        logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                        {
                            MADUAN = maDuAn,
                            REPORT_ID = reportId,
                            API_CODE = apiCode,
                            TABLE_NAME = tableName,
                            RECORD_ID = recordId,
                            FIELD_NAME = dateFieldName,
                            OLD_VALUE = serviceDate.Value.ToString("yyyy-MM-dd"),
                            NEW_VALUE = null,
                            RULE_CODE = "ERR_SERVICE_DATE_BEFORE_F1",
                            SEVERITY = "ERROR",
                            ACTION_TAKEN = "QUARANTINED",
                            MESSAGE = $"Ngày dịch vụ {serviceDate.Value:dd/MM/yyyy} ({dateFieldName}) xảy ra trước ngày khách hàng tham gia tại F1 ({f1.NgayThamGia.Value:dd/MM/yyyy}). Bắt buộc cách ly.",
                            CREATED_DATE = DateTime.Now
                        });
                    }
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// VR-04(a) [BLOCKING]. Kiểm tra F8 (Tư vấn L2) bắt buộc phải có F7 (Tư vấn L1) ở trạng thái Complete.
        /// </summary>
        public static bool ValidateF8Dependency(string recordId, CD45ValidationContext context, string apiCode, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (context == null || (context.F7CompletedClients.Count == 0 && context.KhachHangLookup.Count == 0)) return true;
            if (string.IsNullOrEmpty(recordId)) return false;

            if (!context.F7CompletedClients.Contains(recordId))
            {
                if (logs != null)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = "CD45_TU_VAN_L2",
                        RECORD_ID = recordId,
                        FIELD_NAME = "f8_date",
                        OLD_VALUE = recordId,
                        NEW_VALUE = null,
                        RULE_CODE = "ERR_F8_WITHOUT_F7_COMPLETE",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = $"Khách hàng {recordId} chưa hoàn thành phiếu tư vấn lần 1 (F7 Complete). Chặn nạp phiếu tư vấn lần 2 (F8).",
                        CREATED_DATE = DateTime.Now
                    });
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// VR-04(a) [BLOCKING]. Kiểm tra F5 (Tuân thủ điều trị) bắt buộc phải có bản ghi khám SKTT (F6).
        /// </summary>
        public static bool ValidateF5Dependency(string recordId, CD45ValidationContext context, string apiCode, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (context == null || (context.F6VisitedClients.Count == 0 && context.KhachHangLookup.Count == 0)) return true;
            if (string.IsNullOrEmpty(recordId)) return false;

            if (!context.F6VisitedClients.Contains(recordId))
            {
                if (logs != null)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = "CD45_TUAN_THU",
                        RECORD_ID = recordId,
                        FIELD_NAME = "f5_date",
                        OLD_VALUE = recordId,
                        NEW_VALUE = null,
                        RULE_CODE = "ERR_F5_WITHOUT_F6",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = $"Khách hàng {recordId} chưa từng có bản ghi khám & chẩn đoán SKTT (F6). Chặn nạp phiếu tuân thủ điều trị (F5).",
                        CREATED_DATE = DateTime.Now
                    });
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// VR-06(d) [WARNING]. Cảnh báo nếu phát sinh dịch vụ sau ngày khách hàng đã xác nhận mất dấu tại F9.
        /// </summary>
        public static void ValidateServiceAfterLostToFollowUp(string recordId, DateTime? serviceDate, CD45ValidationContext context, string apiCode, string tableName, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (context == null || !serviceDate.HasValue || string.IsNullOrEmpty(recordId)) return;

            if (context.F9LostClients.TryGetValue(recordId, out var lostDate))
            {
                if (serviceDate.Value.Date > lostDate.Date)
                {
                    if (logs != null)
                    {
                        logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                        {
                            MADUAN = maDuAn,
                            REPORT_ID = reportId,
                            API_CODE = apiCode,
                            TABLE_NAME = tableName,
                            RECORD_ID = recordId,
                            FIELD_NAME = "service_date",
                            OLD_VALUE = serviceDate.Value.ToString("yyyy-MM-dd"),
                            NEW_VALUE = serviceDate.Value.ToString("yyyy-MM-dd"),
                            RULE_CODE = "WARN_SERVICE_AFTER_LOST_TO_FOLLOWUP",
                            SEVERITY = "WARNING",
                            ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                            MESSAGE = $"Phát hiện dịch vụ {tableName} phát sinh ngày {serviceDate.Value:dd/MM/yyyy} sau ngày khách hàng {recordId} đã xác nhận mất dấu ({lostDate:dd/MM/yyyy}). Cần rà soát lại thông tin.",
                            CREATED_DATE = DateTime.Now
                        });
                    }
                }
            }
        }

        /// <summary>
        /// VR-04(b) [WARNING]. Gắn cờ cảnh báo tất cả các form ở trạng thái "Incomplete" (0) hoặc "Unverified" (1).
        /// </summary>
        public static void CheckFormCompletionStatus(string recordId, string completeStatus, string formName, string apiCode, string tableName, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (logs == null || string.IsNullOrEmpty(recordId)) return;

            if (completeStatus == "0" || completeStatus == "1")
            {
                string statusText = completeStatus == "0" ? "Incomplete" : "Unverified";
                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = recordId,
                    FIELD_NAME = "complete_status",
                    OLD_VALUE = completeStatus,
                    NEW_VALUE = completeStatus,
                    RULE_CODE = completeStatus == "1" ? "WARN_FORM_UNVERIFIED" : "WARN_FORM_INCOMPLETE",
                    SEVERITY = "WARNING",
                    ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                    MESSAGE = $"Form {formName} của khách hàng {recordId} đang ở trạng thái chưa hoàn thành ({statusText}). Cần CBO rà soát bổ sung.",
                    CREATED_DATE = DateTime.Now
                });
            }
        }

        /// <summary>
        /// VR-07(a) [WARNING]. Gắn cờ cảnh báo các Repeat Instance rỗng để CBO xác nhận xóa trên REDCap.
        /// </summary>
        public static void LogEmptyInstance(string recordId, int? repeatInstance, string formName, string apiCode, string tableName, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (logs == null || string.IsNullOrEmpty(recordId)) return;

            logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
            {
                MADUAN = maDuAn,
                REPORT_ID = reportId,
                API_CODE = apiCode,
                TABLE_NAME = tableName,
                RECORD_ID = recordId,
                FIELD_NAME = "redcap_repeat_instance",
                OLD_VALUE = repeatInstance?.ToString(),
                NEW_VALUE = null,
                RULE_CODE = "WARN_EMPTY_INSTANCE",
                SEVERITY = "WARNING",
                ACTION_TAKEN = "SKIPPED",
                MESSAGE = $"Phát hiện Repeat Instance #{repeatInstance ?? 1} rỗng trên REDCap tại form {formName} (Record: {recordId}). Đề nghị CBO xác nhận xóa bản ghi rác này.",
                CREATED_DATE = DateTime.Now
            });
        }

        /// <summary>
        /// VR-07(c) [WARNING]. Kiểm tra các cặp thông tin mâu thuẫn nghiệp vụ.
        /// </summary>
        public static void CheckContradictoryRules(string recordId, bool? thamGiaNc, string maKhNc, byte? doiTuong, byte? ketQuaHivF4, string apiCode, string tableName, string reportId, string maDuAn, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (logs == null || string.IsNullOrEmpty(recordId)) return;

            // F1: Không tham gia nghiên cứu nhưng có mã nghiên cứu
            if (thamGiaNc.HasValue && thamGiaNc.Value == false && !string.IsNullOrWhiteSpace(maKhNc))
            {
                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = recordId,
                    FIELD_NAME = "f1_cus_id",
                    OLD_VALUE = maKhNc,
                    NEW_VALUE = maKhNc,
                    RULE_CODE = "WARN_CONTRADICTORY_RESEARCH_INFO",
                    SEVERITY = "WARNING",
                    ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                    MESSAGE = $"Khách hàng {recordId} khai báo 'Không tham gia nghiên cứu' nhưng vẫn có Mã KH nghiên cứu ('{maKhNc}').",
                    CREATED_DATE = DateTime.Now
                });
            }

            // F4: Nhóm đích PLHIV nhưng làm test sàng lọc mới
            if (doiTuong.HasValue && doiTuong.Value == 2 && ketQuaHivF4.HasValue)
            {
                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = recordId,
                    FIELD_NAME = "f4_hiv_result",
                    OLD_VALUE = ketQuaHivF4.Value.ToString(),
                    NEW_VALUE = ketQuaHivF4.Value.ToString(),
                    RULE_CODE = "WARN_CONTRADICTORY_HIV_STATUS",
                    SEVERITY = "WARNING",
                    ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                    MESSAGE = $"Khách hàng {recordId} là người sống chung với HIV (PLHIV) nhưng vẫn ghi nhận làm xét nghiệm sàng lọc HIV mới tại Form F4.",
                    CREATED_DATE = DateTime.Now
                });
            }
        }

        /// <summary>
        /// VR-07(b) [WARNING]. Cảnh báo cụm khi phát hiện chuỗi liên tiếp >= 3 khách hàng của cùng 1 TCV bị Incomplete trong cùng 1 ngày.
        /// </summary>
        public static void CheckClusterIncomplete<T>(
            List<T> entities,
            Func<T, string> getTcv,
            Func<T, DateTime?> getDate,
            Func<T, string> getStatus,
            string tableName,
            string apiCode,
            string reportId,
            string maDuAn,
            ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs,
            Func<T, string> getRecordId = null,
            Func<T, string> getMaNhom = null,
            Func<string, string, string> resolveTcvName = null)
        {
            if (entities == null || logs == null) return;

            var incompleteClusters = entities
                .Where(x => getStatus(x) == "0" && getDate(x).HasValue && !string.IsNullOrEmpty(getTcv(x)))
                .GroupBy(x => new { Tcv = getTcv(x), Date = getDate(x).Value.Date })
                .Where(g => g.Count() >= 3);

            foreach (var grp in incompleteClusters)
            {
                string clientListStr = "";
                if (getRecordId != null)
                {
                    var clientIds = grp.Select(x => getRecordId(x))
                        .Where(r => !string.IsNullOrWhiteSpace(r))
                        .Distinct()
                        .ToList();
                    if (clientIds.Count > 0)
                    {
                        clientListStr = $" (gồm các KH: {string.Join(", ", clientIds)})";
                    }
                }

                string tcvName = null;
                string maNhomFound = getMaNhom != null ? grp.Select(x => getMaNhom(x)).FirstOrDefault(m => !string.IsNullOrEmpty(m)) : null;
                if (resolveTcvName != null)
                {
                    tcvName = resolveTcvName(maNhomFound, grp.Key.Tcv);
                }

                string tcvDisplay = !string.IsNullOrEmpty(tcvName) ? $"TCV '{tcvName}' (Mã: {grp.Key.Tcv})" : $"TCV '{grp.Key.Tcv}'";
                string clusterCity = null;
                string firstClientId = grp.Select(x => getRecordId != null ? getRecordId(x) : null).FirstOrDefault(r => !string.IsNullOrEmpty(r));
                if (!string.IsNullOrEmpty(firstClientId))
                {
                    CD45Helper.InferGroupAndCity(firstClientId, null, out string infN, out string infC);
                    if (string.IsNullOrEmpty(maNhomFound)) maNhomFound = infN;
                    clusterCity = infC;
                }

                logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                {
                    MADUAN = maDuAn,
                    REPORT_ID = reportId,
                    API_CODE = apiCode,
                    TABLE_NAME = tableName,
                    RECORD_ID = $"CLUSTER (TCV: {grp.Key.Tcv})",
                    FIELD_NAME = "complete_status",
                    OLD_VALUE = "0 (Incomplete)",
                    NEW_VALUE = null,
                    RULE_CODE = "WARN_CLUSTER_INCOMPLETE",
                    SEVERITY = "WARNING",
                    ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                    MESSAGE = $"Cảnh báo cụm bất thường: Có {grp.Count()} khách hàng{clientListStr} của {tcvDisplay} cùng bị Incomplete tại dịch vụ {tableName} trong ngày {grp.Key.Date:dd/MM/yyyy}. Cần kiểm tra sự kiện bị hủy hoặc khách bỏ về.",
                    CREATED_DATE = DateTime.Now,
                    MA_NHOM = maNhomFound,
                    CITY_CODE = clusterCity
                });
            }
        }

        #endregion
    }
}
