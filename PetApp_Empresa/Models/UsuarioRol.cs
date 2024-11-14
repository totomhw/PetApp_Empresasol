using System;
using System.Collections.Generic;

namespace PetApp_Empresa.Models;

public partial class UsuarioRol
{
    public int UsuarioRolId { get; set; }

    public int UsuarioId { get; set; }

    public int RolId { get; set; }

    public DateTime? FechaAsignacion { get; set; }

    public virtual Role Rol { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
