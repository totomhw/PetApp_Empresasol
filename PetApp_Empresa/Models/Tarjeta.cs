
namespace PetApp_Empresa.Models;
using System;
using System.Collections.Generic;

public class Tarjeta
{
    public int TarjetaId { get; set; } // ID único de la tarjeta
    public int UsuarioId { get; set; } // Relación con el usuario propietario
    public string Numero { get; set; } // Número de tarjeta
    public string FechaVencimiento { get; set; } // Fecha de vencimiento (formato MM/YY)
    public string CVV { get; set; } // Código de seguridad (3 dígitos)
    public bool EsVisible { get; set; } = true;

    public DateTime FechaRegistro { get; set; } = DateTime.Now; // Fecha de registro de la tarjeta

    // Relación con el Usuario
    public virtual Usuario Usuario { get; set; } // Usuario propietario de la tarjeta

    // Relación con las Compras realizadas con esta tarjeta
    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}