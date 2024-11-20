using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetApp_Empresa.Models;

public static class CarritoHelper
{
    public static async Task<CarritoDeCompra> ObtenerOCrearCarritoUsuario(PettappPruebaContext context, ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new Exception("Usuario no autenticado.");
        }

        // Buscar carrito existente
        var carrito = await context.CarritoDeCompras
            .Include(c => c.CarritoAccesorios)
            .ThenInclude(ca => ca.Accesorio)
            .FirstOrDefaultAsync(c => c.UsuarioId == userId);

        // Si no existe, crear uno nuevo
        if (carrito == null)
        {
            carrito = new CarritoDeCompra
            {
                UsuarioId = userId,
                Total = 0,
                CarritoAccesorios = new List<CarritoAccesorio>()
            };
            context.CarritoDeCompras.Add(carrito);
            await context.SaveChangesAsync();
        }

        return carrito;
    }

}
