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
    
    public partial class NEGOCIACION2
    {
        public int ID { get; set; }
        public string TITULO { get; set; }
        public System.DateTime FINICIO { get; set; }
        public string FRECUENCIA { get; set; }
        public int FRECUENCIA_N { get; set; }
        public string DIA_SEMANA { get; set; }
        public Nullable<int> DIA_MES { get; set; }
        public Nullable<int> ORDINAL_MES { get; set; }
        public string ORDINAL_DSEMANA { get; set; }
    }
}
