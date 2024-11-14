using System;
using System.Collections.Generic;

namespace PetApp_Empresa.Models;

public partial class Adopcione
{
    public int AdopcionId { get; set; }
    public int MascotaId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime? FechaSolicitud { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string Estado { get; set; } = "Pendiente"; // Estado inicial

    // Información del formulario de adopción
    public string NombreCompleto { get; set; }
    public string CorreoElectronico { get; set; }
    public string Telefono { get; set; }
    public string Direccion { get; set; }

    public virtual Mascota Mascota { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
}
