using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models
{
    public class ProveedorViewModel
    {
        public ProveedorViewModel()
        {
            proveedor = new PROVEEDOR();
        }
        public IPagedList<PROVEEDOR> proveedores { get; set; }
        public PROVEEDOR proveedor { get; set; }

        public string ordenActual { get; set; }
        public string buscar { get; set; }
        public int numRegistros { get; set; }
        public List<SelectListItem> pageSizes { get; set; }


    }
}