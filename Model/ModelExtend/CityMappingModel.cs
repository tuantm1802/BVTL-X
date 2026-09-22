using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    /// <summary>
    /// Model danh mục 34 Tỉnh mới theo Nghị quyết số 202/2025/QH15
    /// </summary>
    public class CityNewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Code_Map { get; set; }
        public bool IsKeyProvince { get; set; }
        public int OldCount { get; set; }
        public string OldNamesSummary { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    /// <summary>
    /// Model ánh xạ 63 Tỉnh cũ sang 34 Tỉnh mới
    /// </summary>
    public class CityMappingModel
    {
        public string OldCityCode { get; set; }
        public string NewCityCode { get; set; }
        public string OldCityName { get; set; }
        public string NewCityName { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
