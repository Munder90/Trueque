using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAT001.Models
{
    public class DOCUMENTOP_MOD
    {
        public decimal NUM_DOC { get; set; }
        public decimal POS { get; set; }
        public string MATNR { get; set; }
        public string MATKL { get; set; }
        public string MATKL_ID { get; set; } //Agregado
        public string DESC { get; set; }
        public decimal CANTIDAD { get; set; }
        public decimal MONTO { get; set; }
        public decimal PORC_APOYO { get; set; }
        public decimal MONTO_APOYO { get; set; }
        public decimal MONTOC_APOYO { get; set; } //Agregado
        public decimal PORC_APOYOEST { get; set; } //Agregado
        public decimal PRECIO_SUG { get; set; }
        public decimal VOLUMEN_EST { get; set; }
        public Nullable<decimal> VOLUMEN_REAL { get; set; }
        public Nullable<decimal> APOYO_REAL { get; set; }
        public Nullable<System.DateTime> VIGENCIA_DE { get; set; }
        public Nullable<System.DateTime> VIGENCIA_AL { get; set; }
        public Nullable<decimal> APOYO_EST { get; set; }
        public bool ACTIVO { get; set; }
        public string ORIGINAL { get; set; }

        public virtual Entities.DOCUMENTO DOCUMENTO { get; set; }
    }
}