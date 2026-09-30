using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Model
{
    public partial class BVTL_DU_AN
    {
        [NotMapped]
        public bool? IsActive { get; set; }
    }

    public class BVTL_DU_AN_DTO
    {
        public string maduan { get; set; }
        public string tenduan { get; set; }
        public bool? IsActive { get; set; }
    }
}
