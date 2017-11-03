using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_Convocatoria")]
    public class Convocatoria
    {
        [Key]
        public int Convocatoriaid { get; set; }

        [Display(Name = "Número Convocatoria")]
        public string Numero { get; set; }

        [Display(Name = "Fecha Inicio")]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha Fin")]
        public DateTime FechaFin { get; set; }
        public byte? Requisito { get; set; }
        public string Estado { get; set; }

        [Display(Name = "Rubro")]
        public int RubroID { get; set; }

        [ForeignKey("RubroID")]
        public virtual Rubro Rubro { get; set; }

        [Display(Name = "Empleado")]
        public int EmpleadoID { get; set; }

        [ForeignKey("EmpleadoID")]
        public virtual EmpleadoL Empleado { get; set; }

        [Display(Name = "Fecha Suspensión")]
        public DateTime?FechaSuspension { get; set; }

        [Display(Name = "Observación")]
        public string ObservacionSuspension { get; set; }
    }
}