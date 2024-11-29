using System;
using System.Collections.Generic;

namespace PetApp_Empresa.Models
{
    public class Compra
    {
        public int CompraId { get; set; } // ID único de la compra
        public int UsuarioId { get; set; } // ID del usuario asociado a la compra
        public int? TarjetaId { get; set; } // ID de la tarjeta usada, opcional

        public DateTime FechaCompra { get; set; } // Fecha en la que se realizó la compra
        public decimal Total { get; set; } // Total del monto de la compra

        public string? BancoDestino { get; set; } // Nuevo campo para registrar el banco de destino
        public string ?NumeroTransaccion { get; set; } // Nuevo campo para el número de transacción proporcionado

        public virtual Usuario Usuario { get; set; } // Relación con la tabla Usuario
        public virtual Tarjeta Tarjeta { get; set; } // Relación con la tabla Tarjeta
        public bool Validado { get; set; } // Campo para indicar si el pago fue validado por el administrador
        public virtual ICollection<DetalleCompra> DetallesCompra { get; set; } = new List<DetalleCompra>(); // Detalles de los productos comprados
    }
}
