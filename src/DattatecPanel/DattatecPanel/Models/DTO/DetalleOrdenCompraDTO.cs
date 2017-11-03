namespace DattatecPanel.Models.DTO
{
    public class DetalleOrdenCompraDTO
    {
        public string Item { get; set; }
        public string Producto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Total { get; set; }
    }
}