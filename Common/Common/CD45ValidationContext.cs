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

        public CD45ValidationContext()
        {
            KhachHangLookup = new Dictionary<string, CD45ClientValidationInfo>(StringComparer.OrdinalIgnoreCase);
            F7CompletedClients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            F6VisitedClients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            F9LostClients = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
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
    }
}
