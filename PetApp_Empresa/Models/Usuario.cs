using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetApp_Empresa.Models
{
    public partial class Usuario
    {
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de usuario no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? FechaRegistro { get; set; }

        public bool? Activo { get; set; }

        public virtual ICollection<Accesorio> Accesorios { get; set; } = new List<Accesorio>();

        public virtual ICollection<Adopcione> Adopciones { get; set; } = new List<Adopcione>();

        public virtual ICollection<CarritoDeCompra> CarritoDeCompras { get; set; } = new List<CarritoDeCompra>();

        public virtual ICollection<Donacione> Donaciones { get; set; } = new List<Donacione>();

        public virtual ICollection<UsuarioRol> UsuarioRols { get; set; } = new List<UsuarioRol>();

        public virtual ICollection<Refugio> Refugios { get; set; } = new List<Refugio>();

        public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>(); // Agregado
        public virtual ICollection<Tarjeta> Tarjetas { get; set; } = new List<Tarjeta>(); // Agregado
    }
}
