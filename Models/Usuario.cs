namespace TAT001.Models
{
    using System;
    using System.Collections.Generic;
    using TAT001.Entities;

    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
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
        public string PASS { get; set; }
        public string NOMBRE { get; set; }
        public string APELLIDO_P { get; set; }
        public string APELLIDO_M { get; set; }
        public string EMAIL { get; set; }
        public string SPRAS_ID { get; set; }
        public Nullable<bool> ACTIVO { get; set; }
        public Nullable<int> PUESTO_ID { get; set; }
        public string MANAGER { get; set; }
        public string BACKUP_ID { get; set; }
        public string BUNIT { get; set; }
        public int ROL { get; set; }

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
        public virtual PUESTO PUESTO { get; set; }
        public virtual SPRA SPRA { get; set; }
    }
}
