using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAT001.Models
{
    public class DOCUMENTOM_MOD
    {
        public string ID_CAT { get; set; }
        public string MATNR { get; set; }
        public string DESC { get; set; }
        public decimal VAL { get; set; }
        public decimal POR { get; set; }
        public bool EXCLUIR { get; set; }//RSG 09.07.2018 ID167

    }
}