namespace TAT001.Models
{
    using System;
    using System.Collections.Generic;
    using TAT001.Entities;

    public partial class Clientes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Clientes()
        {
            this.CARTAs = new HashSet<CARTA>();
            this.DOCUMENTOes = new HashSet<DOCUMENTO>();
            this.MIEMBROS = new HashSet<MIEMBRO>();
            this.PRESUPSAPHs = new HashSet<PRESUPSAPH>();
            this.PRESUPUESTOHs = new HashSet<PRESUPUESTOH>();
        }

        public string BUKRS { get; set; }
        public bool BUKRSX { get; set; }
        public string LAND { get; set; }
        public bool LANDX { get; set; }
        public string KUNNR { get; set; }
        public bool KUNNRX { get; set; }
        public string VKORG { get; set; }
        public string VTWEG { get; set; }
        public string SPART { get; set; }
        public string CLIENTE_N { get; set; }
        public string ID_US0 { get; set; }
        public bool ID_US0X { get; set; }
        public string ID_US1 { get; set; }
        public bool ID_US1X { get; set; }
        public string ID_US2 { get; set; }
        public bool ID_US2X { get; set; }
        public string ID_US3 { get; set; }
        public bool ID_US3X { get; set; }
        public string ID_US4 { get; set; }
        public bool ID_US4X { get; set; }
        public string ID_US5 { get; set; }
        public bool ID_US5X { get; set; }
        public string ID_US6 { get; set; }
        public bool ID_US6X { get; set; }
        public string ID_US7 { get; set; }
        public bool ID_US7X { get; set; }
        public string ID_PROVEEDOR { get; set; }
        public bool ID_PROVEEDORX { get; set; }
        public string BANNER { get; set; }
        public bool BANNERX { get; set; }
        public string BANNERG { get; set; }
        public bool BANNERGX { get; set; }
        public string CANAL { get; set; }
        public bool CANALX { get; set; }
        public string EXPORTACION { get; set; }
        public string CONTACTO { get; set; }
        public string CONTACTOE { get; set; }
        public bool CONTACTOEX { get; set; }
        public string MESS { get; set; }


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
