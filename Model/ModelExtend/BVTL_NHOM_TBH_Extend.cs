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
}
