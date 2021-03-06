//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DattatecPanel.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class GS_SRS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GS_SRS()
        {
            this.GS_SRS_PROCEDIMIENTO = new HashSet<GS_SRS_PROCEDIMIENTO>();
        }
    
        public int IDSRS { get; set; }
        public int IDAseguradora { get; set; }
        public int IDOrdenMedica { get; set; }
        public Nullable<System.DateTime> Fecha_registro { get; set; }
        public string Observacion { get; set; }
        public string Estado { get; set; }
        public Nullable<System.DateTime> Fecha_aprobacion { get; set; }
    
        public virtual GS_ASEGURADORA GS_ASEGURADORA { get; set; }
        public virtual GS_ORDEN_MEDICA GS_ORDEN_MEDICA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GS_SRS_PROCEDIMIENTO> GS_SRS_PROCEDIMIENTO { get; set; }
    }
}
