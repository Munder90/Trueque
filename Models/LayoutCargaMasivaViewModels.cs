using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models
{
    public class LayoutCargaMasivaViewModels
    {
        public LayoutCargaMasivaViewModels()
        {
            layouts = new List<LAYOUT_CARGA>();
            layoutMasiva = new LAYOUT_CARGA();
            paises = new List<SelectListItem>();
            sociedades = new List<SelectListItem>();
        }
        
        public List<LAYOUT_CARGA> layouts { get; set; }
        public LAYOUT_CARGA layoutMasiva { get; set; }
        public List<SelectListItem> paises { get; set; }
        public List<SelectListItem> sociedades { get; set; }
    }
}