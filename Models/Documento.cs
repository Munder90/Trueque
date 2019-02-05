using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAT001.Models
{
    public class Documento
    {
        public string BUTTON { get; set; }
        public decimal NUM_DOC { get; set; }
        public string NUM_DOC_TEXT { get; set; }
        public string SOCIEDAD_ID { get; set; }
        public string PAIS_ID { get; set; }
        public string FECHADD { get; set; }
        public string FECHAD { get; set; }
        public string HORAC { get; set; }
        public string PERIODO { get; set; }
        public string ESTATUS { get; set; }
        public string ESTATUS_CLASS { get; set; }
        public string ESTATUS_WF { get; set; }
        public string PAYER_ID { get; set; }
        public string CLIENTE { get; set; }
        public string CANAL { get; set; }
        public string TSOL { get; set; }
        public string TALL { get; set; }
        public string CUENTAS { get; set; }
        public string CONCEPTO { get; set; }
        public string MONTO_DOC_ML { get; set; }
        public string FACTURA { get; set; }
        public string FACTURAK { get; set; }
        public string USUARIOC_ID { get; set; }
        public string USUARIOM_ID { get; set; }
        //public string NC { get; set; }
        public string NUM_PRO { get; set; }
        public string NUM_NC { get; set; }
        public string NUM_AP { get; set; }
        public string NUM_REV { get; set; }
        public string BLART { get; set; }
        public string NUM_PAYER { get; set; }
        public string NUM_CLIENTE { get; set; }
        public string NUM_IMPORTE { get; set; }
        public string NUM_CUENTA { get; set; }
        public decimal? CUENTAP { get; set; }
        public decimal? CUENTAPL { get; set; }
        public decimal? CUENTACL { get; set; }
        public string TIPO_RECURRENTE { get; set; }
        public decimal? DOCUMENTO_REF { get; set; }
    }
}