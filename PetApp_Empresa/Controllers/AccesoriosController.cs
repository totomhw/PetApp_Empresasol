using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetApp_Empresa.Models; 

namespace PetApp_Empresa.Controllers
{
    [Authorize]
    public class AccesoriosController : Controller
    {
        private readonly PettappPruebaContext _context;

        public AccesoriosController(PettappPruebaContext context)
        {
            _context = context;
        }

        // GET: Accesorios
        public async Task<IActionResult> Index()
        {
            var pettappPruebaContext = _context.Accesorios.Include(a => a.Vendedor);
            return View(await pettappPruebaContext.ToListAsync());
        }

        // GET: Accesorios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accesorio = await _context.Accesorios
                .Include(a => a.Vendedor)
                .FirstOrDefaultAsync(m => m.AccesorioId == id);
            if (accesorio == null)
            {
                return NotFound();
            }

            return View(accesorio);
        }

        // GET: Accesorios/Create
        public IActionResult Create()
        {
            ViewData["VendedorId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId");
            return View();
        }

        // POST: Accesorios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccesorioId,Nombre,Descripcion,Precio,VendedorId,CantidadDisponible")] Accesorio accesorio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accesorio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VendedorId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", accesorio.VendedorId);
            return View(accesorio);
        }

        // GET: Accesorios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accesorio = await _context.Accesorios.FindAsync(id);
            if (accesorio == null)
            {
                return NotFound();
            }
            ViewData["VendedorId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", accesorio.VendedorId);
            return View(accesorio);
        }

        // POST: Accesorios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccesorioId,Nombre,Descripcion,Precio,VendedorId,CantidadDisponible")] Accesorio accesorio)
        {
            if (id != accesorio.AccesorioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accesorio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccesorioExists(accesorio.AccesorioId))
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
            ViewData["VendedorId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", accesorio.VendedorId);
            return View(accesorio);
        }

        // GET: Accesorios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accesorio = await _context.Accesorios
                .Include(a => a.Vendedor)
                .FirstOrDefaultAsync(m => m.AccesorioId == id);
            if (accesorio == null)
            {
                return NotFound();
            }

            return View(accesorio);
        }

        // POST: Accesorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accesorio = await _context.Accesorios.FindAsync(id);
            if (accesorio != null)
            {
                _context.Accesorios.Remove(accesorio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccesorioExists(int id)
        {
            return _context.Accesorios.Any(e => e.AccesorioId == id);
        }

        // GET: Accesorios/ListadoAccesorios
        public async Task<IActionResult> ListadoAccesorios()
        {
            var accesorios = await _context.Accesorios.ToListAsync();

            // Usar el helper para obtener el carrito del usuario autenticado
            var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

            // Calcular la cantidad total de artículos en el carrito
            var carritoCount = carrito.CarritoAccesorios.Sum(ca => ca.Cantidad);

            ViewData["CarritoCount"] = carritoCount; // Pasar el valor a la vista
            return View(accesorios);
        }
    }
}
