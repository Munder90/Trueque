using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAT001.Models
{
    public class MaterialVal
    {
        public string ID { get; set; }
        public string MATKL_ID { get; set; }
        public string MAKTX { get; set; }
        public string MAKTG { get; set; }
        public string MEINS { get; set; }
        public Nullable<decimal> PUNIT { get; set; }
        public Nullable<bool> ACTIVO { get; set; }
    }
    public class MaterialGPId
    {
        public string CATEGORY_ID { get; set; }
        public Nullable<bool> ACTIVO { get; set; }
        public Nullable<bool> UNICA { get; set; }
        public Nullable<bool> EXCLUIR { get; set; }
    }
}