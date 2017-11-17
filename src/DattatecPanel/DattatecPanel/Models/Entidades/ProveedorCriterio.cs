using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DattatecPanel.Models.Entidades
{
    [Table("GL_ProveedorCriterio")]
    public class ProveedorCriterio
    {
        [Key]
        public int PK_IdProveedorCriterio { get; set; }
        public int FK_IdProveedor { get; set; }
        [ForeignKey("FK_IdProveedor")]
        public virtual Proveedor Proveedor { get; set; }
        public int FK_IdCriterio { get; set; }
        [ForeignKey("FK_IdCriterio")]
        public virtual Criterios Criterios { get; set; }
        public decimal Puntuacion { get; set; }
    }
}