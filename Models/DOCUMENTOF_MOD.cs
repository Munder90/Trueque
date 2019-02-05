using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAT001.Models
{
    public class DOCUMENTOF_MOD
    {
        public decimal NUM_DOC { get; set; }
        public int POS { get; set; }
        public string FACTURA { get; set; }
        public Nullable<System.DateTime> FECHA { get; set; }
        public string PROVEEDOR { get; set; }
        public string PROVEEDOR_TXT { get; set; }
        public bool PROVEEDOR_ACTIVO { get; set; }
        public string CONTROL { get; set; }
        public string AUTORIZACION { get; set; }
        public Nullable<System.DateTime> VENCIMIENTO { get; set; }
        public string FACTURAK { get; set; }
        public string EJERCICIOK { get; set; }
        public string BILL_DOC { get; set; }
        public string BELNR { get; set; }
        //jemo 10-17-2018 inicio
        public string IMPORTE_FACT { get; set; }
        public string PAYER { get; set; }
        public string DESCRIPCION { get; set; }
        public string SOCIEDAD { get; set; }
        //jemo 10-17-2018 fin
    }
}