using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    public class Hosting
    {
        public int ID { get; set; }
        public string dominio{ get; set; }
        public string servicio { get; set; }
        public string usuario { get; set; }
        public string  clave{ get; set; }

        [DisplayName("Fecha de Creacion"), Required(ErrorMessage = "Debe ingresar fecha de Creacion.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fecCrea { get; set; }

        [DisplayName("Fecha de Actualizacion"), Required(ErrorMessage = "Debe ingresar fecha de Actualizacion.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fecActualiza { get; set; }

        [DisplayName("Fecha de Vencimiento"), Required(ErrorMessage = "Debe ingresar fecha de Vencimiento.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fecVence { get; set; }
        
        public int espacio { get; set; }
        public string contacto { get; set; }
        public string emailContacto { get; set; }
        public decimal precio { get; set; }
        public string estado { get; set; }
        public string observaciones { get; set; }
        public string certificadoSSL { get; set; }
        public DateTime fecCreaSSL { get; set; }
        public DateTime fecActualizaSSL { get; set; }
        public DateTime fecVenceSSL { get; set; }
    }

    public class CLinicaDBContext : DbContext
    {
        public DbSet<Hosting> Hosting { get; set; }
    }

}