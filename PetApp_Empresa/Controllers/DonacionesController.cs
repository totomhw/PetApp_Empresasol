using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetApp_Empresa.Models;


namespace Pettapp2.Controllers
{
    public class DonacionesController : Controller
    {
        private readonly PettappPruebaContext _context;

        public DonacionesController(PettappPruebaContext context)
        {
            _context = context;
        }

        

        // Método para mostrar la vista de donación global
        public IActionResult DonacionGlobal()
        {
            return View();
        }

        // POST: Donaciones/DonacionGlobal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DonacionGlobal(decimal monto)
        {
            // Obtener todos los refugios existentes
            var refugios = await _context.Refugios.ToListAsync();

            if (refugios.Count == 0)
            {
                ModelState.AddModelError("", "No hay refugios disponibles para donar.");
                return View();
            }

            // Calcular el monto a donar a cada refugio
            var montoPorRefugio = monto / refugios.Count;

            // Crear la donación para cada refugio
            foreach (var refugio in refugios)
            {
                var donacion = new Donacione
                {
                    UsuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value), // Usuario autenticado
                    RefugioId = refugio.RefugioId,
                    Monto = montoPorRefugio,
                    FechaDonacion = DateTime.Now
                };

                _context.Donaciones.Add(donacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Donaciones
        public async Task<IActionResult> Index()
        {
            var petappContext = _context.Donaciones.Include(d => d.Refugio).Include(d => d.Usuario);
            return View(await petappContext.ToListAsync());
        }

        // GET: Donaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donacione = await _context.Donaciones
                .Include(d => d.Refugio)
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(m => m.DonacionId == id);
            if (donacione == null)
            {
                return NotFound();
            }

            return View(donacione);
        }

        // GET: Donaciones/Create
        public IActionResult Create()
        {
            ViewData["RefugioId"] = new SelectList(_context.Refugios, "RefugioId", "RefugioId");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId");
            return View();
        }

        // POST: Donaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DonacionId,UsuarioId,RefugioId,Monto,FechaDonacion")] Donacione donacione)
        {
            if (true)
            {
                _context.Add(donacione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RefugioId"] = new SelectList(_context.Refugios, "RefugioId", "RefugioId", donacione.RefugioId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", donacione.UsuarioId);
            return View(donacione);
        }

        // GET: Donaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donacione = await _context.Donaciones.FindAsync(id);
            if (donacione == null)
            {
                return NotFound();
            }
            ViewData["RefugioId"] = new SelectList(_context.Refugios, "RefugioId", "RefugioId", donacione.RefugioId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", donacione.UsuarioId);
            return View(donacione);
        }

        // POST: Donaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DonacionId,UsuarioId,RefugioId,Monto,FechaDonacion")] Donacione donacione)
        {
            if (id != donacione.DonacionId)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    _context.Update(donacione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonacioneExists(donacione.DonacionId))
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
            ViewData["RefugioId"] = new SelectList(_context.Refugios, "RefugioId", "RefugioId", donacione.RefugioId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", donacione.UsuarioId);
            return View(donacione);
        }

        // GET: Donaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donacione = await _context.Donaciones
                .Include(d => d.Refugio)
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(m => m.DonacionId == id);
            if (donacione == null)
            {
                return NotFound();
            }

            return View(donacione);
        }

        // POST: Donaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donacione = await _context.Donaciones.FindAsync(id);
            if (donacione != null)
            {
                _context.Donaciones.Remove(donacione);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DonacioneExists(int id)
        {
            return _context.Donaciones.Any(e => e.DonacionId == id);
        }
    }
}
