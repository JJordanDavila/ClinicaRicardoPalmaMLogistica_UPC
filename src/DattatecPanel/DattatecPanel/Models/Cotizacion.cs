using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_Cotizacion")]
    public class Cotizacion
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int CotizacionID { get; set; }
        [ForeignKey("CotizacionID")]
        public virtual TransaccionCompra TransaccionCompra { get; set; }
        public int RequerimientoCompraID { get; set; }
        [ForeignKey("RequerimientoCompraID")]
        public virtual RequerimientoCompra RequerimientoCompra { get; set; }
        public int ProveedorID { get; set; }
        [ForeignKey("ProveedorID")]
        public virtual Proveedor Proveedor { get; set; }
        public int FormaPagoID { get; set; }
        [ForeignKey("FormaPagoID")]
        public virtual FormaPago FormaPago { get; set; }
        public int PlazoEntrega { get; set; }
        public int TiempoValidez { get; set; }
        public string NroReferencia { get; set; }

    }
}