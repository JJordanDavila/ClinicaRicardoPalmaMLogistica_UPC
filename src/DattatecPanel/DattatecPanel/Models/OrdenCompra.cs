using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_OrdenCompra")]
    public class OrdenCompra
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int OrdenCompraID { get; set; }

        [ForeignKey("OrdenCompraID")]
        public virtual TransaccionCompra TransaccionCompra { get; set; }
        public int CotizacionID { get; set; }
        [ForeignKey("CotizacionID")]
        public virtual Cotizacion Cotizacion { get; set; }

        [Display(Name = "Almacén")]
        public int AlmacenID { get; set; }
        [ForeignKey("AlmacenID")]
        public virtual Almacen Almacen { get; set; }
        [Required]
        [Display(Name = "Observación")]
        public string Observacion { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Fecha de Entrega")]
        public Nullable<DateTime> FechaEntrega { get; set; }

        //Referencial, solo para las vistas
        [NotMapped]
        public List<DetalleTransaccionCompra> lstDetalle;
        [NotMapped]
        [Display(Name = "Motivo")]
        public string ObservacionAnulacion { get; set; }
    }
}