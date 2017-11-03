using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_UnidadMedida")]
    public class UnidadMedida
    {
        [Key]
        public int UnidadMedidaID { get; set; }
        public string Abreviatura { get; set; }
        public string Descripcion { get; set; }
    }
}