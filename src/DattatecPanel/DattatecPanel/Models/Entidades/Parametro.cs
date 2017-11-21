using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DattatecPanel.Models.Entidades
{
    [Table("GL_Parametro")]
    public class Parametro
    {
        [Key]
        public int ParametroId { get; set; }

        [Display(Name = "Fecha Inicio")]
        public DateTime FecIni { get; set; }

        [Display(Name = "Fecha Final")]
        public DateTime FecFin { get; set; }

        [Range(0, 30, ErrorMessage = "Debe tener un rango entre 0 y 30 ")]
        [MaxLength(3, ErrorMessage = "La propiedad {0} no puede tener más de {1} elementos")]
        [Display(Name = "Intervalo")]
         public int Intervalo { get; set; }

        [Display(Name = "Unidad Medida Intervalo")]
        public string UnidadMedidaIntervalo { get; set; }

        //// [Display(Name = "Unidad Medida Intervalo")]
        ////public virtual UnidadMedida UnidadMedidaIntervalo { get; set; }


        [Display(Name = "Fecha Ultimo Proceso")]
        public DateTime FecUltPro { get; set; }

        [Display(Name = "URL Sunat")]
        public string UrlServicio01 { get; set; }

        [Display(Name = "URL OSCE")]
        public string UrlServicio02 { get; set; }

        [NotMapped]
        public string cbxMedidasIntervalos { get; set; }
    }

    }
