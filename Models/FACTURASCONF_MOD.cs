using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAT001.Models
{
    public class FACTURASCONF_MOD
    {
        public decimal? NUM_DOC { get; set; }
        public bool? POS { get; set; }
        public string SOCIEDAD_ID { get; set; }
        public string PAIS_ID { get; set; }
        public string TSOL { get; set; }
        public bool FACTURA { get; set; }
        public bool FECHA { get; set; }
        public bool PROVEEDOR { get; set; }
        public bool? PROVEEDOR_TXT { get; set; }
        public bool CONTROL { get; set; }
        public bool AUTORIZACION { get; set; }
        public bool VENCIMIENTO { get; set; }
        public bool FACTURAK { get; set; }
        public bool EJERCICIOK { get; set; }
        public bool BILL_DOC { get; set; }
        public bool BELNR { get; set; }
        //jemo 09-07-2018 inicio
        public bool IMPORTE_FAC { get; set; }
        public bool PAYER { get; set; }
        public bool DESCRIPCION { get; set; }
        public bool SOCIEDAD { get; internal set; }
        //jemo 09-07-2018 fin
    }
}