using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAT001.Models.Masiva
{
    public class Encabezado
    {
        public string NUM_DOC { get; set; }
        public string TSOL_ID { get; set; }
        public string TALL_ID { get; set; } //03/12/2018
        public string TALL_NAME { get; set; }//03/12/2018
        public string SOCIEDAD_ID { get; set; }
        public string PAIS_ID { get; set; }
        public string PAIS_NAME { get; set; }//RSG 01.11.2018
        public string ESTADO { get; set; }
        public string CIUDAD { get; set; }
        public string CONCEPTO { get; set; }
        public string NOTAS { get; set; }
        public string PAYER_ID { get; set; }
        public string PAYER_NOMBRE { get; set; }
        public string VKORG { get; set; }
        public string VTWEG { get; set; }
        public string CONTACTO_NOMBRE { get; set; }
        public string CONTACTO_EMAIL { get; set; }
        public string FECHAI_VIG { get; set; }
        public string FECHAF_VIG { get; set; }
        public string MONEDA_ID { get; set; }

        public string SPART { get; set; }//04-12-2018
        public string Decimales { get; set; }
        public string Miles { get; set; }

    }
}