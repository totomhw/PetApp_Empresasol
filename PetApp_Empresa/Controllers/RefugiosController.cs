using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetApp_Empresa.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace PetApp_Empresa.Controllers
{
    public class RefugiosController : Controller
    {
        private readonly PettappPruebaContext _context;

        public RefugiosController(PettappPruebaContext context)
        {
            _context = context;
        }

        // GET: Refugios
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out int usuarioId))
            {
                return Unauthorized();
            }

            var refugios = await _context.Refugios
                .Where(r => r.UsuarioId == usuarioId)
                .Include(r => r.Usuario)
                .ToListAsync();

            return View(refugios);
        }

        // GET: Refugios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var refugio = await _context.Refugios
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.RefugioId == id);

            if (refugio == null || refugio.UsuarioId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
            {
                return Unauthorized();
            }

            return View(refugio);
        }

        // GET: Refugios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Refugios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Direccion,Telefono,Email")] Refugio refugio)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out int usuarioId))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                refugio.UsuarioId = usuarioId;
                _context.Add(refugio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(refugio);
        }

        // GET: Refugios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out int usuarioId))
            {
                return Unauthorized();
            }

            var refugio = await _context.Refugios
                .Where(r => r.RefugioId == id && r.UsuarioId == usuarioId)
                .FirstOrDefaultAsync();

            if (refugio == null)
            {
                return NotFound();
            }

            return View(refugio);
        }

        // POST: Refugios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RefugioId,Nombre,Direccion,Telefono,Email")] Refugio refugio)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out int usuarioId))
            {
                return Unauthorized();
            }

            var refugioExistente = await _context.Refugios
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RefugioId == id && r.UsuarioId == usuarioId);

            if (refugioExistente == null)
            {
                return NotFound();
            }

            refugio.UsuarioId = usuarioId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(refugio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RefugioExists(refugio.RefugioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(refugio);
        }

        // GET: Refugios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var refugio = await _context.Refugios
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.RefugioId == id);

            if (refugio == null || refugio.UsuarioId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
            {
                return Unauthorized();
            }

            return View(refugio);
        }

        // POST: Refugios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var refugio = await _context.Refugios.FindAsync(id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (refugio != null && int.TryParse(userId, out int usuarioId) && refugio.UsuarioId == usuarioId)
            {
                _context.Refugios.Remove(refugio);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RefugioExists(int id)
        {
            return _context.Refugios.Any(e => e.RefugioId == id);
        }

        // REFUGIO - Verificar Solicitudes de Adopción en Proceso
        public async Task<IActionResult> VerificarSolicitudes()
        {
            var solicitudesEnProceso = await _context.Adopciones
                .Include(a => a.Mascota)
                .Include(a => a.Usuario)
                .Where(a => a.Estado == "En Proceso")
                .ToListAsync();

            return View("VerificarSolicitudes", solicitudesEnProceso);
        }

        // POST: Refugios/AprobarAdopcion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprobarAdopcion(int adopcionId)
        {
            var adopcion = await _context.Adopciones
                .Include(a => a.Mascota)
                .FirstOrDefaultAsync(a => a.AdopcionId == adopcionId && a.Estado == "En Proceso");

            if (adopcion == null)
            {
                return NotFound("No se encontró la adopción en proceso.");
            }

            // Cambiar el estado de la adopción a "Adoptado"
            adopcion.Estado = "Adoptado";
            adopcion.Mascota.EstadoAdopcion = "Adoptado";
            adopcion.FechaAprobacion = DateTime.Now; // Registrar la fecha de aprobación
            _context.Adopciones.Update(adopcion);
            _context.Mascotas.Update(adopcion.Mascota);
            await _context.SaveChangesAsync();

            return RedirectToAction("VerificarSolicitudes");
        }

        // POST: Refugios/RechazarAdopcion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarAdopcion(int adopcionId)
        {
            var adopcion = await _context.Adopciones
                .Include(a => a.Mascota)
                .FirstOrDefaultAsync(a => a.AdopcionId == adopcionId && a.Estado == "En Proceso");

            if (adopcion == null)
            {
                return NotFound("No se encontró la adopción en proceso.");
            }

            // Cambiar el estado de la adopción a "Rechazado" y la mascota a "Disponible"
            adopcion.Estado = "Rechazado";
            adopcion.Mascota.EstadoAdopcion = "Disponible";
            _context.Adopciones.Update(adopcion);
            _context.Mascotas.Update(adopcion.Mascota);
            await _context.SaveChangesAsync();

            return RedirectToAction("VerificarSolicitudes");
        }
    }
}
