using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Collections.Generic;
using System.Linq;

namespace DattatecPanel.Models
{
    [Table("GL_RegistroProveedorParticipante")]
    public class RegistroProveedorParticipante
    {
        //  ProveedorID, LicitacionID, envioExpediente, fechaColExpediente, expediente, adjudicado, monto


        [Key, Column(Order = 0)]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int ProveedorID { get; set; }
        
        [ForeignKey("ProveedorID")]
        public virtual Proveedor Proveedor { get; set; }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int LicitacionID { get; set; }

        [ForeignKey("LicitacionID")]
        public virtual Licitacion Licitacion { get; set; }

        public bool envioExpediente { get; set; }


        [DisplayName("Fecha Expediente")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> fechaColExpediente { get; set; }

        [DisplayName("Expediente")]
        public byte[] expediente { get; set; }

        public bool adjudicado { get; set; }

        public decimal monto { get; set; }
    }
}