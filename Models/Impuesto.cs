using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models
{
    public class Impuesto
    {
        public string vkorg { get; set; }
        public string vtweg { get; set; }
        public string spart { get; set; }
        public string kunnr { get; set; }
        public string mwskz { get; set; }
        public bool activo { get; set; }
        public IEnumerable<CLIENTEI> listaBD { get; set; }
        public IEnumerable<IMPUESTO> listaBD2 { get; set; }    
    }
}