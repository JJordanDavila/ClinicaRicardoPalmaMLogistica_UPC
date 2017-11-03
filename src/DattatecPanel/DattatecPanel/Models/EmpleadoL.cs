using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_Empleado")]
    public class EmpleadoL
    {
        [Key]
        public int EmpleadoID { get; set; }
        public int AreaID { get; set; }
        [ForeignKey("AreaID")]
        public virtual Area Area { get; set; }
        public string DNI { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombres { get; set; }
        public string Correo { get; set; }

        public string NombreCompleto { get { return ApellidoPaterno + " " + ApellidoMaterno + " " + Nombres; } }
    }
}