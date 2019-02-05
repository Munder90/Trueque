using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Models
{
    public class Contactoc
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public string vkorg { get; set; }
        public string vtweg { get; set; }
        public string spart { get; set; }
        public string kunnr { get; set; }
        public bool activo { get; set; }
        public bool defecto { get; set; }
        public bool carta { get; set; }
        public string spras { get; set; }
        public IEnumerable<CONTACTOC> tabContacto { get; set; }
    }
}