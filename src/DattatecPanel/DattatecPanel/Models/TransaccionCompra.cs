using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DattatecPanel.Models
{
    [Table("GL_TransaccionCompra")]
    public class TransaccionCompra
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int TransaccionCompraID { get; set; }

        public int MonedaID { get; set; }

        [ForeignKey("MonedaID")]
        public virtual Moneda Moneda { get; set; }
        [Display(Name = "Número de Requerimiento de Compra")]
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }

        [Display(Name = "Motivo de Anulación")]
        public string ObservacionAnulacion { get; set; }
    }
}