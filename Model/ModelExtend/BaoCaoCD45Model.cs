using System;

namespace Model.ModelExtend
{
    public class BaoCaoCD45Model
    {
        public string STT { get; set; }
        public string ChiTieu { get; set; }
        public int Tong { get; set; }
        public int PUD { get; set; }
        public int PLHIV { get; set; }
        public int TG { get; set; }
        public int SW { get; set; }
        public int MSM { get; set; }
        public bool IsBold { get; set; }
        public int IndentLevel { get; set; }
    }

    public class CD45_TCV_ItemModel
    {
        public int ID { get; set; }
        public string MA_NHOM { get; set; }
        public string TEN_NHOM { get; set; }
        public string CITY_CODE { get; set; }
        public string MA_TCV { get; set; }
        public string TEN_TCV { get; set; }
    }
}
