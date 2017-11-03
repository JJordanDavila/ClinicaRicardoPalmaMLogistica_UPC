using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;


namespace DattatecPanel.Models
{
    [Table("GL_RequerimientoCompra")]
    public class RequerimientoCompra
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int RequerimientoCompraID { get; set; }
        [ForeignKey("RequerimientoCompraID")]
        public virtual TransaccionCompra TransaccionCompra { get; set; }
        public int SolicitanteID { get; set; }
        [ForeignKey("SolicitanteID")]
        public virtual EmpleadoL Solicitante { get; set; }

        public int EncargadoID { get; set; }
        [ForeignKey("EncargadoID")]
        public virtual EmpleadoL Encargado { get; set; }

        public DateTime FechaEstimada { get; set; }
    }
}