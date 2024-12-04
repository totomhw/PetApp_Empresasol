using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetApp_Empresa.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PetApp_Empresa.Controllers
{
    public class ValidacionesController : Controller
    {
        private readonly PettappPruebaContext _context;

        public ValidacionesController(PettappPruebaContext context)
        {
            _context = context;
        }

        // GET: Validaciones/ValidacionesPago
        public async Task<IActionResult> ValidacionesPago()
        {
            var compras = await _context.Compras
                .Include(c => c.DetallesCompra)
                .ThenInclude(d => d.Accesorio)
                .ToListAsync();

            return View(compras);
        }

        [HttpPost]
        public async Task<IActionResult> ValidarPago(int compraId)
        {
            var compra = await _context.Compras
                .Include(c => c.DetallesCompra)
                .ThenInclude(d => d.Accesorio)
                .FirstOrDefaultAsync(c => c.CompraId == compraId);

            if (compra == null)
            {
                TempData["ErrorMessage"] = "La compra no existe.";
                return RedirectToAction("ValidacionesPago");
            }

            if (compra.Validado)
            {
                TempData["ErrorMessage"] = "El pago ya ha sido validado anteriormente.";
                return RedirectToAction("ValidacionesPago");
            }

            // Reducir el stock de los accesorios
            foreach (var detalle in compra.DetallesCompra)
            {
                var accesorio = await _context.Accesorios.FindAsync(detalle.AccesorioId);
                if (accesorio != null)
                {
                    accesorio.CantidadDisponible -= detalle.Cantidad;

                    if (accesorio.CantidadDisponible < 0)
                    {
                        TempData["ErrorMessage"] = "Stock insuficiente para uno o más productos.";
                        return RedirectToAction("ValidacionesPago");
                    }

                    _context.Accesorios.Update(accesorio);
                }
            }

            // Marcar la compra como validada
            compra.Validado = true;
            _context.Compras.Update(compra);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pago validado y stock actualizado correctamente.";
            return RedirectToAction("ValidacionesPago");
        }


    }
}
