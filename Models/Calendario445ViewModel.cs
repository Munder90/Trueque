using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models
{
    public class Calendario445ViewModel
    {
        public Calendario445ViewModel()
        {
            calendario445 = new CALENDARIO_AC();
            calendarioEx445 = new CALENDARIO_EX();
            ejercicio = new List<SelectListItem>();
            sociedades = new List<SelectListItem>();
            periodos = new List<SelectListItem>();
            cmbTiposSolicitud = new List<SelectListItem>();
            usuarios = new List<SelectListItem>();

            treeTiposSolicitud = new List<SelectTreeItem>();

            pageSizes = new List<SelectListItem>();

    }
        public CALENDARIO_AC calendario445 { get; set; }
        public CALENDARIO_EX calendarioEx445 { get; set; }
        public IPagedList<CALENDARIO_AC> calendarios445 { get; set; }
        public IPagedList<CALENDARIO_EX> calendariosEx445 { get; set; }
        public List<CALENDARIO_EX> calendariosEx445List { get; set; }
        public List<SelectListItem> ejercicio { get; set; }
        public List<SelectListItem> sociedades { get; set; }
        public List<SelectListItem> periodos { get; set; }
        public List<SelectListItem> cmbTiposSolicitud { get; set; }
        public List<SelectListItem> usuarios { get; set; }

        public List<SelectTreeItem> treeTiposSolicitud { get; set; }


        public List<SelectListItem> pageSizes { get; set; }
        public int numRegistros { get; set; }
        public int numRegistrosEx { get; set; }
        public string ordenActual { get; set; }
        public string buscar { get; set; }



    }
}
