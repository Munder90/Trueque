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
    
    public partial class DET_APROBH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DET_APROBH()
        {
            this.DET_APROBP = new HashSet<DET_APROBP>();
        }
    
        public string SOCIEDAD_ID { get; set; }
        public int PUESTOC_ID { get; set; }
        public int VERSION { get; set; }
        public bool ACTIVO { get; set; }
    
        public virtual PUESTO PUESTO { get; set; }
        public virtual SOCIEDAD SOCIEDAD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_APROBP> DET_APROBP { get; set; }
    }
}
