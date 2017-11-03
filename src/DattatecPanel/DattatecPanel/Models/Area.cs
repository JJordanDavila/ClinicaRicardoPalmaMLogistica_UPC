using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_Area")]
    public class Area
    {
        [Key]
        public int AreaID { get; set; }

        [Display(Name = "Área")]
        public string Descripcion { get; set; }

    }
}