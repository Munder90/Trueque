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
    
    public partial class SOCIEDAD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SOCIEDAD()
        {
            this.CALENDARIO_AC = new HashSet<CALENDARIO_AC>();
            this.CONPOSAPHs = new HashSet<CONPOSAPH>();
            this.CUENTAs = new HashSet<CUENTA>();
            this.DET_AGENTEH = new HashSet<DET_AGENTEH>();
            this.DET_APROB = new HashSet<DET_APROB>();
            this.DET_APROBH = new HashSet<DET_APROBH>();
            this.DOCUMENTOes = new HashSet<DOCUMENTO>();
            this.FACTURASCONFs = new HashSet<FACTURASCONF>();
            this.GAUTORIZACIONs = new HashSet<GAUTORIZACION>();
            this.LAYOUT_CARGA = new HashSet<LAYOUT_CARGA>();
            this.PAIS = new HashSet<PAI>();
            this.TAXEOHs = new HashSet<TAXEOH>();
            this.TS_FORM = new HashSet<TS_FORM>();
            this.USUARIOs = new HashSet<USUARIO>();
            this.USUARIOs1 = new HashSet<USUARIO>();
        }
    
        public string BUKRS { get; set; }
        public string BUTXT { get; set; }
        public string ORT01 { get; set; }
        public string LAND { get; set; }
        public string SUBREGIO { get; set; }
        public string WAERS { get; set; }
        public string SPRAS { get; set; }
        public string NAME1 { get; set; }
        public string KTOPL { get; set; }
        public bool ACTIVO { get; set; }
        public string REGION { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CALENDARIO_AC> CALENDARIO_AC { get; set; }
        public virtual CONFDIST_CAT CONFDIST_CAT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONPOSAPH> CONPOSAPHs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CUENTA> CUENTAs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_AGENTEH> DET_AGENTEH { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_APROB> DET_APROB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_APROBH> DET_APROBH { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTO> DOCUMENTOes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FACTURASCONF> FACTURASCONFs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GAUTORIZACION> GAUTORIZACIONs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LAYOUT_CARGA> LAYOUT_CARGA { get; set; }
        public virtual MONEDA MONEDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAI> PAIS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAXEOH> TAXEOHs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TS_FORM> TS_FORM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIOs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIOs1 { get; set; }
    }
}