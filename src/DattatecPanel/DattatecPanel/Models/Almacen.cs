using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_Almacen")]
    public class Almacen
    {
        [Key]
        public int AlmacenID { get; set; }

        [Display(Name = "Dirección")]
        public string Direccion { get; set; }
    }
}