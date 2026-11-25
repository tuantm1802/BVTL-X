using System;

namespace Model.ModelExtend
{
    public class BaoCaoChiSoCD45Model
    {
        public string STT { get; set; }
        public string Code { get; set; }
        public string ChiTieu { get; set; }
        public string DonVi { get; set; }
        public string RowType { get; set; }
        public string ParentCode { get; set; }

        public int? Tong { get; set; }
        public int? PUD { get; set; }
        public int? PLHIV { get; set; }
        public int? TG { get; set; }
        public int? SW { get; set; }
        public int? MSM { get; set; }

        public int? Nam { get; set; }
        public int? Nu { get; set; }
        public int? Khac { get; set; }

        public int? Tuoi_18_25 { get; set; }
        public int? Tuoi_26_35 { get; set; }
        public int? Tuoi_Tren35 { get; set; }

        public int? Denom_Tong { get; set; }
        public int? Denom_PUD { get; set; }
        public int? Denom_PLHIV { get; set; }
        public int? Denom_TG { get; set; }
        public int? Denom_SW { get; set; }
        public int? Denom_MSM { get; set; }

        public int? Denom_Nam { get; set; }
        public int? Denom_Nu { get; set; }
        public int? Denom_Khac { get; set; }

        public int? Denom_Tuoi_18_25 { get; set; }
        public int? Denom_Tuoi_26_35 { get; set; }
        public int? Denom_Tuoi_Tren35 { get; set; }

        public string NguonDuLieu { get; set; }
        public string GhiChu { get; set; }
        public bool IsBold { get; set; }
        public int? IndentLevel { get; set; }
        public bool CanDrillDown { get; set; }
    }

    public class CD45_ChiSoDuAnConfigModel
    {
        public int ID { get; set; }
        public int NAM { get; set; }
        public string MA_KY { get; set; }
        public string MA_THAM_SO { get; set; }
        public string CHI_SO_CODE { get; set; }
        public string TEN_THAM_SO { get; set; }
        public int? GIA_TRI { get; set; }
        public string GHI_CHU { get; set; }
        public int SORT_ORDER { get; set; }
        public DateTime? UPDATED_AT { get; set; }
        public string UPDATED_BY { get; set; }
    }
}
