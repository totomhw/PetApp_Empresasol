namespace PetApp_Empresa.Models
{
    public class QR
    {
        public int QRId { get; set; }
        public string Codigo { get; set; } // Puede ser el código en texto del QR
        public string ImagenPath { get; set; } // Ruta a la imagen del QR
        public DateTime FechaGeneracion { get; set; }
    }

}
