using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DattatecPanel.Models.Entidades
{
    [Table("GL_Evidencia")]
    public class Evidencia
    {
        [Key]
        public int EvidenciaId { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
    }
}