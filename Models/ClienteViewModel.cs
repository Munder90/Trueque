using PagedList;
using System.Collections.Generic;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models
{
    public class ClienteViewModel 
    {
        public IPagedList<CLIENTE> clientes { get; set; }
        public int numRegistros { get; set; }
        public List<SelectListItem> pageSizes{ get; set; }
        public string ordenActual { get; set; }
        public string buscar { get; set; }


    }
}