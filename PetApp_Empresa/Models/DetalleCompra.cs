namespace PetApp_Empresa.Models
{
    public class DetalleCompra
    {
        public int DetalleCompraId { get; set; }
        public int CompraId { get; set; }
        public int AccesorioId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Relación con Compra
        public Compra Compra { get; set; }

        // Relación con Accesorio
        public Accesorio Accesorio { get; set; }
    }

}
