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
    
    public partial class PERIODOT
    {
        public string SPRAS_ID { get; set; }
        public int PERIODO_ID { get; set; }
        public string TXT50 { get; set; }
        public string TXT03 { get; set; }
        public string TXT01 { get; set; }
    
        public virtual PERIODO PERIODO { get; set; }
        public virtual SPRA SPRA { get; set; }
    }
}
