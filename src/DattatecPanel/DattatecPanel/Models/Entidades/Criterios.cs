using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DattatecPanel.Models.Entidades
{
    [Table("GL_Criterios")]
    public class Criterios
    {
        [Key]
        [Column("PK_IdCriterio")]
        public int id { get; set; }
        public string Descripcion { get; set; }
    }
}