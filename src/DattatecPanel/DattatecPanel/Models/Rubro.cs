using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_Rubro")]
    public class Rubro
    {
        [Key]
        public int RubroID { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}