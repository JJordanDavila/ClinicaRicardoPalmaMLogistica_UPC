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
    
    public partial class GS_ORDEN_MEDICA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GS_ORDEN_MEDICA()
        {
            this.GS_ORDEN_PROCEDIMIENTO = new HashSet<GS_ORDEN_PROCEDIMIENTO>();
            this.GS_SRS = new HashSet<GS_SRS>();
        }
    
        public int IDOrdenMedica { get; set; }
        public int IDAtencion { get; set; }
        public string Estado { get; set; }
        public Nullable<System.DateTime> Fecha_registro { get; set; }
    
        public virtual GS_ATENCION GS_ATENCION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GS_ORDEN_PROCEDIMIENTO> GS_ORDEN_PROCEDIMIENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GS_SRS> GS_SRS { get; set; }
    }
}
