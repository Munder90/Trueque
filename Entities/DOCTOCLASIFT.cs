//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TAT001.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class DOCTOCLASIFT
    {
        public int ID_CLASIFICACION { get; set; }
        public string SPRAS_ID { get; set; }
        public string TEXTO { get; set; }
    
        public virtual DOCTOCLASIF DOCTOCLASIF { get; set; }
    }
}
