using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Models
{
    public class ArchivoC
    {
        public decimal num_doc { get; set; }
        public int pos { get; set; }
        public string estatus { get; set; }
        public string dirFile{ get; set; }
        public CONPOSAPH tab { get; set; }
        public DOCUMENTO doc { get; set; }
        public List<DetalleContab> det { get; set; }
    }
}