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
    
    public partial class GS_PERIODO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GS_PERIODO()
        {
            this.GS_ASEGURADORA_PERIODO = new HashSet<GS_ASEGURADORA_PERIODO>();
        }
    
        public int IDPeriodo { get; set; }
        public Nullable<System.DateTime> Fecha_inicio { get; set; }
        public Nullable<System.DateTime> Fecha_final { get; set; }
        public string Descripcion { get; set; }
        public Nullable<System.DateTime> Fecha_registro { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GS_ASEGURADORA_PERIODO> GS_ASEGURADORA_PERIODO { get; set; }
    }
}
