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
    
    public partial class DET_AGENTE
    {
        public int PUESTOC_ID { get; set; }
        public int POS { get; set; }
        public int PUESTOA_ID { get; set; }
        public long AGROUP_ID { get; set; }
        public Nullable<decimal> MONTO { get; set; }
        public Nullable<bool> PRESUPUESTO { get; set; }
        public Nullable<bool> ACTIVO { get; set; }
        public string USUARIOA { get; set; }
        public string USUARIOC { get; set; }
    
        public virtual GAUTORIZACION GAUTORIZACION { get; set; }
        public virtual PUESTO PUESTO { get; set; }
        public virtual PUESTO PUESTO1 { get; set; }
    }
}