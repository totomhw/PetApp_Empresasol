using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetApp_Empresa.Models;

namespace PetApp_Empresa.Controllers
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
            var refugios = await _context.Refugios.ToListAsync();

            if (refugios.Count == 0)
            {
                ModelState.AddModelError("", "No hay refugios disponibles para donar.");
                return View();
            }

            var montoPorRefugio = monto / refugios.Count;

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
            ViewData["RefugioId"] = new SelectList(_context.Refugios, "RefugioId", "Nombre");
            return View();
        }

        // POST: Donaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DonacionId,RefugioId,Monto,FechaDonacion")] Donacione donacione)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            donacione.UsuarioId = int.Parse(userIdClaim.Value); // Asigna el usuario autenticado
            donacione.FechaDonacion = DateTime.Now; // Fecha de donación actual

            if (true)
            {
                _context.Add(donacione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["RefugioId"] = new SelectList(_context.Refugios, "RefugioId", "Nombre", donacione.RefugioId);
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

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || donacione.UsuarioId != int.Parse(userIdClaim.Value))
            {
                return Unauthorized("No puedes editar donaciones que no te pertenecen.");
            }

            ViewData["RefugioId"] = new SelectList(_context.Refugios, "RefugioId", "Nombre", donacione.RefugioId);
            return View(donacione);
        }

        // POST: Donaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DonacionId,RefugioId,Monto,FechaDonacion")] Donacione donacione)
        {
            if (id != donacione.DonacionId)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var originalDonacion = await _context.Donaciones.FindAsync(id);
            if (originalDonacion == null || originalDonacion.UsuarioId != int.Parse(userIdClaim.Value))
            {
                return Unauthorized("No puedes editar donaciones que no te pertenecen.");
            }

            donacione.UsuarioId = originalDonacion.UsuarioId; // Mantén el usuario original
            donacione.FechaDonacion = originalDonacion.FechaDonacion; // Mantén la fecha original si es necesario

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

            ViewData["RefugioId"] = new SelectList(_context.Refugios, "RefugioId", "Nombre", donacione.RefugioId);
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


        // Acción para ver las donaciones realizadas al refugio del usuario autenticado
        public async Task<IActionResult> DonacionesPorRefugio()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            // Obtener los refugios administrados por el usuario
            var refugiosDelUsuario = await _context.Refugios
                .Where(r => r.UsuarioId == userId)
                .Select(r => r.RefugioId)
                .ToListAsync();

            // Obtener las donaciones realizadas a los refugios del usuario
            var donaciones = await _context.Donaciones
                .Include(d => d.Refugio)
                .Include(d => d.Usuario)
                .Where(d => refugiosDelUsuario.Contains(d.RefugioId))
                .ToListAsync();

            return View(donaciones);
        }

    }
}
