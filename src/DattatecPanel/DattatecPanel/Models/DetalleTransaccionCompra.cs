using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_DetalleTransaccionCompra")]
    public class DetalleTransaccionCompra
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int TransaccionCompraID { get; set; }

        [ForeignKey("TransaccionCompraID")]
        public virtual TransaccionCompra TransaccionCompra { get; set; }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int DetalleTransaccionCompraID { get; set; }
        public int ArticuloID { get; set; }
        [ForeignKey("ArticuloID")]
        public virtual Articulo Articulo { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get { return (Cantidad * Precio); } }
    }
}