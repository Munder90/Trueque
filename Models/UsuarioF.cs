namespace TAT001.Models
{
    using System;
    using System.Collections.Generic;
    using TAT001.Entities;

    public partial class UsuarioF
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UsuarioF()
        {
            this.CARTAs = new HashSet<CARTA>();
            this.DOCUMENTOes = new HashSet<DOCUMENTO>();
            this.FLUJOes = new HashSet<FLUJO>();
            this.FLUJOes1 = new HashSet<FLUJO>();
            this.MIEMBROS = new HashSet<MIEMBRO>();
            this.PRESUPSAPHs = new HashSet<PRESUPSAPH>();
            this.PRESUPUESTOHs = new HashSet<PRESUPUESTOH>();
        }


        public string ID { get; set; }
        public string KUNNR { get; set; }
        public bool KUNNRX { get; set; }
        public string VKORG { get; set; }
        public string VTWEG { get; set; }
        public string SPART { get; set; }
        public Nullable<bool> ACTIVO { get; set; }
        public string USUARIOC_ID { get; set; }
        public string FECHAC { get; set; }
        public string USUARIOM_ID { get; set; }
        public string FECHAM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CARTA> CARTAs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTO> DOCUMENTOes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FLUJO> FLUJOes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FLUJO> FLUJOes1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MIEMBRO> MIEMBROS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRESUPSAPH> PRESUPSAPHs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRESUPUESTOH> PRESUPUESTOHs { get; set; }
        
    }
}
