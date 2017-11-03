using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DattatecPanel.Models
{
    [Table("GL_Moneda")]
    public class Moneda
    {
        [Key]
        public int MonedaID { get; set; }
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        public string Abreviatura { get; set; }

    }
}