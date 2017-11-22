using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DattatecPanel.Models.Entidades
{
    [Table("GL_DetalleConvocatoria")]
    public class DetalleConvocatoria
    {
        [Key]
        public int ConvocatoriaId { get; set; }

        [Display(Name = "Postulante")]
        public int PostulanteId { get; set; }

        [ForeignKey("PostulanteId")]
        public virtual Postulante Postulante { get; set; }

        [Display(Name = "Fecha Registro")]
        public DateTime? Fecha_Registro { get; set; }

       [ForeignKey("ConvocatoriaId")]
        public virtual Convocatoria Convocatoria { get; set; }

        [NotMapped]
        public string RUC { get; set; }

        [NotMapped]
        public string RazonSocial { get; set; }

        [NotMapped]
        public string Correo { get; set; }

        [NotMapped]
        public string Rubro { get; set; }
        
     }
}