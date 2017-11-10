using DattatecPanel.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DattatecPanel.Context
{
    public class ClinicaDBContext : DbContext
    {
        public ClinicaDBContext(): base("ClinicaDBContext")
        { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ClinicaDBContext>());
            Database.SetInitializer<ClinicaDBContext>(null);
            base.OnModelCreating(modelBuilder);
        }

        //GL_LOGISTICA
        public DbSet<OrdenCompra> DB_OrdenCompra { get; set; }
        public DbSet<Cotizacion> DB_Cotizacion { get; set; }
        public DbSet<TransaccionCompra> DB_TransaccionCompra { get; set; }
        public DbSet<DetalleTransaccionCompra> DB_DetalleTransaccionCompra { get; set; }
        public DbSet<FormaPago> DB_FormaPago { get; set; }
        public DbSet<Proveedor> DB_Proveedor { get; set; }
        public DbSet<RequerimientoCompra> DB_RequerimientoCompra { get; set; }
        public DbSet<EmpleadoL> DB_Empleado { get; set; }
        public DbSet<Almacen> DB_Almacen { get; set; }
        public DbSet<Licitacion> Licitaciones { get; set; }
        public DbSet<Area> DB_Areas { get; set; }
        public DbSet<RegistroProveedorParticipante> DB_RegistroProveedorParticipante { get; set; }
        public System.Data.Entity.DbSet<DattatecPanel.Models.Rubro> Rubroes { get; set; }
        public DbSet<Convocatoria> DB_Convocatoria { get; set; }

        public DbSet<Postulante> DB_Postulante { get; set; }
        public DbSet<DetallePostulante> DB_DetallePostulante { get; set; }

    }
}