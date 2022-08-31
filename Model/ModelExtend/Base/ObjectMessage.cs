using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.Base
{
    public class ObjectMessage
    {
        public bool Error { get; set; }
        public string Title { get; set; }
        public object ObjectData { get; set; }

        public long Id { get; set; }
    }
}
