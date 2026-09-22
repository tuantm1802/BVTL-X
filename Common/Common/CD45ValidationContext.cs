using System;
using System.Collections.Generic;

namespace Common.Common
{
    /// <summary>
    /// Thông tin tóm tắt hồ sơ khách hàng phục vụ kiểm thực chéo giữa các form
    /// </summary>
    public class CD45ClientValidationInfo
    {
        public string RecordId { get; set; }
        public bool IsComplete { get; set; }
        public byte? DoiTuong { get; set; }
        public DateTime? NgayThamGia { get; set; }
        public string MaNhom { get; set; }
        public string CityCode { get; set; }
    }

    /// <summary>
    /// Ngữ cảnh kiểm thực dữ liệu liên kết form CD45 (DREAMH)
    /// Hỗ trợ tra cứu nhanh trong bộ nhớ cho VR-03(b), VR-04(a,b), VR-05, VR-06(d)
    /// </summary>
    public class CD45ValidationContext
    {
        /// <summary>
        /// Danh mục hồ sơ gốc F1 (Key: RecordId)
        /// </summary>
        public Dictionary<string, CD45ClientValidationInfo> KhachHangLookup { get; set; }

        /// <summary>
        /// Danh sách RecordId đã có Form F7 (Tư vấn lần 1) ở trạng thái Complete (COMPLETE_STATUS = '2')
        /// </summary>
        public HashSet<string> F7CompletedClients { get; set; }

        /// <summary>
        /// Danh sách RecordId đã từng có phiếu khám chẩn đoán Form F6
        /// </summary>
        public HashSet<string> F6VisitedClients { get; set; }

        /// <summary>
        /// Danh sách RecordId đã xác nhận mất dấu Form F9 (Key: RecordId, Value: NgayMatDau)
        /// </summary>
        public Dictionary<string, DateTime> F9LostClients { get; set; }

        /// <summary>
        /// Danh bạ Tiếp cận viên (Key: "maNhom_maTcv" hoặc "maTcv", Value: Tên TCV)
        /// </summary>
        public Dictionary<string, string> TcvNameLookup { get; set; }

        public CD45ValidationContext()
        {
            KhachHangLookup = new Dictionary<string, CD45ClientValidationInfo>(StringComparer.OrdinalIgnoreCase);
            F7CompletedClients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            F6VisitedClients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            F9LostClients = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
            TcvNameLookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Thêm hoặc cập nhật thông tin hồ sơ F1
        /// </summary>
        public void RegisterClientF1(string recordId, string completeStatus, byte? doiTuong, DateTime? ngayThamGia, string maNhom = null, string cityCode = null)
        {
            if (string.IsNullOrEmpty(recordId)) return;

            bool isComplete = completeStatus == "2";
            KhachHangLookup[recordId] = new CD45ClientValidationInfo
            {
                RecordId = recordId,
                IsComplete = isComplete,
                DoiTuong = doiTuong,
                NgayThamGia = ngayThamGia,
                MaNhom = maNhom,
                CityCode = cityCode
            };
        }

        /// <summary>
        /// Đăng ký khách hàng đã hoàn thành Form F7
        /// </summary>
        public void RegisterF7Complete(string recordId)
        {
            if (!string.IsNullOrEmpty(recordId))
            {
                F7CompletedClients.Add(recordId);
            }
        }

        /// <summary>
        /// Đăng ký khách hàng đã có Form F6
        /// </summary>
        public void RegisterF6Visit(string recordId)
        {
            if (!string.IsNullOrEmpty(recordId))
            {
                F6VisitedClients.Add(recordId);
            }
        }

        /// <summary>
        /// Đăng ký khách hàng đã mất dấu Form F9
        /// </summary>
        public void RegisterF9Lost(string recordId, DateTime? ngayMatDau)
        {
            if (!string.IsNullOrEmpty(recordId) && ngayMatDau.HasValue)
            {
                F9LostClients[recordId] = ngayMatDau.Value.Date;
            }
        }

        /// <summary>
        /// Đăng ký thông tin Tiếp cận viên (TCV)
        /// </summary>
        public void RegisterTcv(string maNhom, string maTcv, string tenTcv)
        {
            if (string.IsNullOrWhiteSpace(maTcv) || string.IsNullOrWhiteSpace(tenTcv)) return;
            string cleanTcv = maTcv.Trim();
            string cleanTen = tenTcv.Trim();

            if (!string.IsNullOrWhiteSpace(maNhom))
            {
                string key = $"{maNhom.Trim().ToLower()}_{cleanTcv.ToLower()}";
                TcvNameLookup[key] = cleanTen;
            }
            if (!TcvNameLookup.ContainsKey(cleanTcv))
            {
                TcvNameLookup[cleanTcv] = cleanTen;
            }
        }

        /// <summary>
        /// Tra cứu tên Tiếp cận viên theo mã nhóm và mã TCV
        /// </summary>
        public string GetTcvName(string maNhom, string maTcv)
        {
            if (string.IsNullOrWhiteSpace(maTcv)) return null;
            string cleanTcv = maTcv.Trim();

            if (!string.IsNullOrWhiteSpace(maNhom))
            {
                string key = $"{maNhom.Trim().ToLower()}_{cleanTcv.ToLower()}";
                if (TcvNameLookup.TryGetValue(key, out var name))
                {
                    return name;
                }
            }
            if (TcvNameLookup.TryGetValue(cleanTcv, out var fallbackName))
            {
                return fallbackName;
            }
            return null;
        }

        public string GetMaNhom(string recordId)
        {
            if (!string.IsNullOrEmpty(recordId) && KhachHangLookup.TryGetValue(recordId, out var info))
            {
                return info.MaNhom;
            }
            return null;
        }

        public string GetCityCode(string recordId)
        {
            if (!string.IsNullOrEmpty(recordId) && KhachHangLookup.TryGetValue(recordId, out var info))
            {
                return info.CityCode;
            }
            return null;
        }
    }
}
