using System;
using System.Collections.Generic;
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
    public class CarritoAccesoriosController : Controller
    {
        private readonly PettappPruebaContext _context;

        public CarritoAccesoriosController(PettappPruebaContext context)
        {
            _context = context;
        }

        // GET: CarritoAccesorios
        public async Task<IActionResult> Index()
        {
            var pettappPruebaContext = _context.CarritoAccesorios.Include(c => c.Accesorio).Include(c => c.Carrito);
            return View(await pettappPruebaContext.ToListAsync());
        }

        // GET: CarritoAccesorios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoAccesorio = await _context.CarritoAccesorios
                .Include(c => c.Accesorio)
                .Include(c => c.Carrito)
                .FirstOrDefaultAsync(m => m.CarritoAccesorioId == id);
            if (carritoAccesorio == null)
            {
                return NotFound();
            }

            return View(carritoAccesorio);
        }

        // GET: CarritoAccesorios/Create
        public IActionResult Create()
        {
            ViewData["AccesorioId"] = new SelectList(_context.Accesorios, "AccesorioId", "AccesorioId");
            ViewData["CarritoId"] = new SelectList(_context.CarritoDeCompras, "CarritoId", "CarritoId");
            return View();
        }

        // POST: CarritoAccesorios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarritoAccesorioId,CarritoId,AccesorioId")] CarritoAccesorio carritoAccesorio)
        {
            if (true)
            {
                _context.Add(carritoAccesorio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccesorioId"] = new SelectList(_context.Accesorios, "AccesorioId", "AccesorioId", carritoAccesorio.AccesorioId);
            ViewData["CarritoId"] = new SelectList(_context.CarritoDeCompras, "CarritoId", "CarritoId", carritoAccesorio.CarritoId);
            return View(carritoAccesorio);
        }

        // GET: CarritoAccesorios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoAccesorio = await _context.CarritoAccesorios.FindAsync(id);
            if (carritoAccesorio == null)
            {
                return NotFound();
            }
            ViewData["AccesorioId"] = new SelectList(_context.Accesorios, "AccesorioId", "AccesorioId", carritoAccesorio.AccesorioId);
            ViewData["CarritoId"] = new SelectList(_context.CarritoDeCompras, "CarritoId", "CarritoId", carritoAccesorio.CarritoId);
            return View(carritoAccesorio);
        }

        // POST: CarritoAccesorios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarritoAccesorioId,CarritoId,AccesorioId")] CarritoAccesorio carritoAccesorio)
        {
            if (id != carritoAccesorio.CarritoAccesorioId)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    _context.Update(carritoAccesorio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoAccesorioExists(carritoAccesorio.CarritoAccesorioId))
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
            ViewData["AccesorioId"] = new SelectList(_context.Accesorios, "AccesorioId", "AccesorioId", carritoAccesorio.AccesorioId);
            ViewData["CarritoId"] = new SelectList(_context.CarritoDeCompras, "CarritoId", "CarritoId", carritoAccesorio.CarritoId);
            return View(carritoAccesorio);
        }

        // GET: CarritoAccesorios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoAccesorio = await _context.CarritoAccesorios
                .Include(c => c.Accesorio)
                .Include(c => c.Carrito)
                .FirstOrDefaultAsync(m => m.CarritoAccesorioId == id);
            if (carritoAccesorio == null)
            {
                return NotFound();
            }

            return View(carritoAccesorio);
        }

        // POST: CarritoAccesorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carritoAccesorio = await _context.CarritoAccesorios.FindAsync(id);
            if (carritoAccesorio != null)
            {
                _context.CarritoAccesorios.Remove(carritoAccesorio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoAccesorioExists(int id)
        {
            return _context.CarritoAccesorios.Any(e => e.CarritoAccesorioId == id);
        }
    }
}
