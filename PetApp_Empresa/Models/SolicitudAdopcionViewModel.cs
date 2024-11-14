using System.ComponentModel.DataAnnotations;

namespace PetApp_Empresa.Models
{
    public class SolicitudAdopcionViewModel
    {
        public int MascotaId { get; set; }

        [Required]
        public string NombreCompleto { get; set; }

        [Required]
        [EmailAddress]
        public string CorreoElectronico { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public string Direccion { get; set; }
    }

}
