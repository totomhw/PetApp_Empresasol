using System;
using System.Collections.Generic;

namespace PetApp_Empresa.Models
{
    public partial class Refugio
    {
        public int RefugioId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        public int? UsuarioId { get; set; } // Usuario encargado del refugio

        public virtual Usuario? Usuario { get; set; } // Relación con Usuario
        public virtual ICollection<Donacione> Donaciones { get; set; } = new List<Donacione>();
        public virtual ICollection<Mascota> Mascota { get; set; } = new List<Mascota>();
    }
}
