using PagedList;
using System.Collections.Generic;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models
{
    public class MaterialViewModel
    {
        public IPagedList<MVKEMaterial> materiales { get; set; }
        public int numRegistros { get; set; }
        public List<SelectListItem> pageSizes { get; set; }
        public string ordenActual { get; set; }
        public string buscar { get; set; }
    }
    public class MVKEMaterial
    {
        public string MATERIAL_ID { get; set; }
        public string VKORG { get; set; }
        public string VTWEG { get; set; }
        public string MATKL_ID { get; set; }
        public string MAKTX { get; set; }
        public string MATERIAL_GROUP { get; set; }
        public bool ACTIVO { get; set; }
    }
}