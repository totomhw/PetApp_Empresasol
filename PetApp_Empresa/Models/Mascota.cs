namespace PetApp_Empresa.Models;

public partial class Mascota
{
    public int MascotaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Raza { get; set; }

    public string? Sexo { get; set; }

    public int? Edad { get; set; }

    public string? Descripcion { get; set; }

    public string? EstadoAdopcion { get; set; }

    public int? RefugioId { get; set; }

    public string? ImagenUrl { get; set; }

    public virtual ICollection<Adopcione> Adopciones { get; set; } = new List<Adopcione>();

    public virtual Refugio? Refugio { get; set; }
}
