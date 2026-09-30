namespace Model.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class BVTL_QT_NGUOI_DUNG_CITY
    {
        public int Id { get; set; }
        public long NguoiDungId { get; set; }
        public string CityCode { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
