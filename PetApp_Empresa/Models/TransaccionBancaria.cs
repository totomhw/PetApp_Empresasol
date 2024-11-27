using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetApp_Empresa.Models
{
    public class TransaccionBancaria
    {
        [Key]
        public int TransaccionBancariaId { get; set; } // Propiedad agregada como clave primaria

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El número de transacción es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número de transacción no puede superar los 50 caracteres.")]
        public string NumeroTransaccion { get; set; } = null!;

        [Required(ErrorMessage = "El banco de origen es obligatorio.")]
        [StringLength(100, ErrorMessage = "El banco no puede superar los 100 caracteres.")]
        public string BancoOrigen { get; set; } = null!;

        [StringLength(255)]
        public string? ComprobantePath { get; set; }

        [Required]
        public DateTime FechaTransaccion { get; set; } = DateTime.Now;

        // Relación con Usuario
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; } = null!;
    }
}
