using System;
using System.Collections.Generic;

namespace Model.ModelExtend
{
    public class DoctorItemModel
    {
        public string MA_BAC_SI { get; set; }
        public string TEN_BAC_SI { get; set; }
        public string TINH_THANH { get; set; }
        public string CITY_CODE { get; set; }
    }

    public class BaoCaoBacSiSummaryModel
    {
        public string MA_BAC_SI { get; set; }
        public string TEN_BAC_SI { get; set; }
        public string TINH_THANH { get; set; }
        public string CITY_CODE { get; set; }
        public int TongSoCa { get; set; }
        public int KhamLan1 { get; set; }
        public int TaiKham { get; set; }
        public int KhamKhac { get; set; }
        public int NgoaiTru { get; set; }
        public int NoiTru { get; set; }
    }

    public class BaoCaoBacSiDetailModel
    {
        public long ID { get; set; }
        public string RECORD_ID { get; set; }
        public string CITY_CODE { get; set; }
        public string TEN_TINH { get; set; }
        public string MA_NHOM { get; set; }
        public string TEN_NHOM { get; set; }
        public string MA_TCV { get; set; }
        public string TEN_TCV { get; set; }
        public DateTime? NGAY_KHAM { get; set; }
        public string NGAY_KHAM_STR => NGAY_KHAM.HasValue ? NGAY_KHAM.Value.ToString("dd/MM/yyyy") : "-";
        public string BAC_SI { get; set; }
        public string TEN_BAC_SI { get; set; }
        public string CO_SO_Y_TE { get; set; }
        public string TEN_CO_SO_Y_TE { get; set; }
        public byte? LAN_KHAM { get; set; }
        public string TEN_LAN_KHAM => LAN_KHAM == 1 ? "Khám lần 1" : (LAN_KHAM == 2 ? "Tái khám" : (LAN_KHAM > 2 ? $"Lần {LAN_KHAM}" : "-"));
        public string CHAN_DOAN_CHINH { get; set; }
        public string TEN_CHAN_DOAN { get; set; }
        public string HINH_THUC_DIEU_TRI { get; set; }
        public string TEN_HINH_THUC_DIEU_TRI => FormatHinhThucDieuTri(HINH_THUC_DIEU_TRI);
        public string COMPLETE_STATUS { get; set; }

        public static string FormatHinhThucDieuTri(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "-";

            var parts = raw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "-";

            var names = new List<string>();
            foreach (var part in parts)
            {
                var code = part.Trim();
                switch (code)
                {
                    case "1":
                        names.Add("Ngoại trú");
                        break;
                    case "2":
                        names.Add("Nội trú");
                        break;
                    case "3":
                        names.Add("Tư vấn tâm lý");
                        break;
                    case "4":
                        names.Add("Khác");
                        break;
                    default:
                        if (!string.IsNullOrEmpty(code))
                            names.Add(code);
                        break;
                }
            }

            return names.Count > 0 ? string.Join(", ", names) : "-";
        }
    }
}
