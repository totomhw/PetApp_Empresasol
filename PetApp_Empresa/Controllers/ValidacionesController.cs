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

        // POST: Validaciones/ValidarPago
        [HttpPost]
        public async Task<IActionResult> ValidarPago(int compraId)
        {
            var compra = await _context.Compras.FindAsync(compraId);

            if (compra == null)
            {
                return NotFound();
            }

            // Marcar la compra como validada (simulamos esto con un campo adicional)
            compra.Validado = true;

            _context.Compras.Update(compra);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
