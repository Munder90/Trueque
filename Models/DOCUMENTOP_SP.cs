using System;

namespace TAT001.Models
{
    public class DOCUMENTOP_SP
    {
        public decimal NUM_DOC { get; set; }
        public string MATNR { get; set; }
        public string DESCRIPCION { get; set; }
        public string MAKTX { get; set; }
        public decimal MONTO { get; set; }
        public decimal? PUNIT { get; set; }
        public decimal PORC_APOYO { get; set; }
        public decimal MONTO_APOYO { get; set; }
        public decimal RESTA { get; set; }
        public decimal PRECIO_SUG { get; set; }
        public decimal? APOYO_EST { get; set; }
        public decimal VOLUMEN_EST { get; set; }
        public decimal? VOLUMEN_REAL { get; set; }
        public DateTime? VIGENCIA_DE { get; set; }
        public DateTime? VIGENCIA_AL { get; set; }
        public decimal? APOYO_REAL { get; set; }

    }
}