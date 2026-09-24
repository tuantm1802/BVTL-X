using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Model
{
    public partial class BVTL_NHOM_TBH
    {
        [NotMapped]
        public string PREFIX { get; set; }

        [NotMapped]
        public string SHORT_PREFIX { get; set; }

        [NotMapped]
        public string CHUC_DANH { get; set; }

        public string GetXungDanh()
        {
            return !string.IsNullOrWhiteSpace(PREFIX) ? PREFIX.Trim() : "Nhóm";
        }

        public string GetShortXungDanh()
        {
            if (!string.IsNullOrWhiteSpace(SHORT_PREFIX)) return SHORT_PREFIX.Trim();
            if (!string.IsNullOrWhiteSpace(PREFIX)) return PREFIX.Trim();
            return "Nhóm";
        }

        public string GetChucDanh()
        {
            if (!string.IsNullOrWhiteSpace(CHUC_DANH)) return CHUC_DANH.Trim();
            if (city_code == "HCM" || manhom_tbh == "HN_TT" || manhom_tbh_map == "tt" || (tennhom_tbh != null && tennhom_tbh.IndexOf("Time", StringComparison.OrdinalIgnoreCase) >= 0))
                return "Giám đốc";
            return "Trưởng nhóm";
        }

        public string GetDaiDienNhomSigner()
        {
            return $"{GetChucDanh()} \"{tennhom_tbh}\"";
        }

        public string GetFullDisplayName()
        {
            return $"{GetXungDanh()}: {tennhom_tbh}";
        }

        public string GetTitleName()
        {
            return $"{GetXungDanh()} {tennhom_tbh}";
        }

        public string GetShortTitleName()
        {
            return $"{GetShortXungDanh()} {tennhom_tbh}";
        }
    }

    public class BVTL_NHOM_TBH_DTO
    {
        public string manhom_tbh { get; set; }
        public string tennhom_tbh { get; set; }
        public string city_code { get; set; }
        public string manhom_tbh_map { get; set; }
        public string maduan { get; set; }
        public string PREFIX { get; set; }
        public string SHORT_PREFIX { get; set; }
        public string CHUC_DANH { get; set; }
    }
}

