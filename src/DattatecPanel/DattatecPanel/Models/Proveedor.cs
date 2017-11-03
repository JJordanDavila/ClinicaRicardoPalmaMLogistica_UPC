using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_Proveedor")]
    public class Proveedor
    {
        [Key]
        public int ProveedorID { get; set; }

        public int RubroID { get; set; }
        [ForeignKey("RubroID")]
        public virtual Rubro Rubro { get; set; }

        public string NombreComercial { get; set; }
        [Display(Name = "Razón Social")]
        public string RazonSocial { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public string RUC { get; set; }
        public bool Certificado { get; set; }
        public string Estado { get; set; }

        [NotMapped]
        public int licitacion_id { get; set; }

        [NotMapped]
        public bool chk_proveedor { get; set; }
    }
}