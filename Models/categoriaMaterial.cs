using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAT001.Models
{
    public class CategoriaMaterial
    {
        public string ID { get; set; }
        public string DESCRIPCION { get; set; }
        public List<DOCUMENTOM_MOD> MATERIALES { get; set; }
        public bool EXCLUIR { get; set; }//RSG 09.07.2018 ID167
        public bool UNICA { get; set; }//LEJ 18.07.2018 ID167
        public decimal TOTALCAT { get; set; }//RSG 22.11.2018
    }
}