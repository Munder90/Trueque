using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Models
{
    public class Delegados
    {
        public string usuario { get; set; }
        public string nombre { get; set; }
        public List<PAI> LISTA { get; set; }
    }
}