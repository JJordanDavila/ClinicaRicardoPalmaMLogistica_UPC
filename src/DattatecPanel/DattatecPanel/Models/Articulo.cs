using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_Articulo")]
    public class Articulo
    {
        [Key]
        public int ArticuloID { get; set; }
        public int UnidadMedidaID { get; set; }
        [ForeignKey("UnidadMedidaID")]
        public virtual UnidadMedida UnidadMedida { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioReferencial { get; set; }
        public int StockMinimo { get; set; }
        public int StockMaximo { get; set; }
    }
}