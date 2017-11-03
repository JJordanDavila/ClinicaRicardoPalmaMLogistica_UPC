using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_FormaPago")]
    public class FormaPago
    {
        [Key]
        public int FormaPagoID { get; set; }
        public string Nombre { get; set; }
        public int NroDiasPago { get; set; }
    }
}