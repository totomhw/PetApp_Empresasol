using System;
using System.Collections.Generic;

namespace PetApp_Empresa.Models
{
    public class Compra
    {
        public int CompraId { get; set; }
        public int UsuarioId { get; set; }
        public int? TarjetaId { get; set; } // Ahora es opcional

        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual Tarjeta Tarjeta { get; set; } // Relación con Tarjeta
        public virtual ICollection<DetalleCompra> DetallesCompra { get; set; } = new List<DetalleCompra>();
    }

}
