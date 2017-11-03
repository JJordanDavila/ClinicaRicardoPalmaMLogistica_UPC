using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Collections.Generic;

namespace DattatecPanel.Models
{
    [Table("GL_Licitacion")]
    public class Licitacion
    {
        [Key]
        public int LicitacionID { get; set; }

        [DisplayName("Requerimiento de Compra")]
        public int RequerimientoCompraID { get; set; }
        [ForeignKey("RequerimientoCompraID")]
        public virtual RequerimientoCompra RequerimientoCompra { get; set; }

        //public virtual TransaccionCompra TransaccionCompra { get; set; }

        [DisplayName("Número de Licitación")]
        public string Numero { get; set; }

        [DisplayName("Fecha de Registro")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> Fecha { get; set; }

        [DisplayName("Término de Referencia Inicio")]
        public byte[] TerminoRefInicial { get; set; }

        [DisplayName("Término de Referencia Final")]
        public byte[] TerminoRefFinal { get; set; }


        [DisplayName("Fecha de Convocatoria")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FechaConvocatoria { get; set; }

        [DisplayName("Fecha de Recepción Consultas")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FecRecepcionConsultas { get; set; }

        [DisplayName("Fecha de Absolución Consultas")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FecAbsolucionConsultas { get; set; }

        [DisplayName("Fecha de Recepción Expediente")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FecRecepcionExpediente { get; set; }

        [DisplayName("Fecha de Evaluación Inicial")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FecEvaluacionIni { get; set; }

        [DisplayName("Fecha de Evaluación Final")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FecEvaluacionFin { get; set; }


        [DisplayName("Fecha de Adjudicación")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FecAdjudicacion { get; set; }

  
        [DisplayName("Motivo de Anulación")]
        public string ObservacionAnulacion { get; set; }

        public string Estado { get; set; }

        [NotMapped]
        [DisplayName("Término de Referencia de Inicio")]
        public HttpPostedFileBase FileTDR1 { get; set; }
        public string FileNameTDR1 { get; set; }
        public string ContentTypeTDR1 { get; set; }

        [NotMapped]
        [DisplayName("Término de Referencia Final")]
        public HttpPostedFileBase FileTDR2 { get; set; }
        public string FileNameTDR2 { get; set; }
        public string ContentTypeTDR2 { get; set; }


        [NotMapped]
        public List<TransaccionCompra> mob_transaccionCompra;

        [NotMapped]
        public List<RegistroProveedorParticipante> registroProveedorParticipante_detalle;

    }
}