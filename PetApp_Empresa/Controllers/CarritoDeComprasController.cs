using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetApp_Empresa.Models;

namespace PetApp_Empresa.Controllers
{
    public class CarritoDeComprasController : Controller
    {
        private readonly PettappPruebaContext _context;

        public CarritoDeComprasController(PettappPruebaContext context)
        {
            _context = context;
        }

        // GET: CarritoDeCompras
        public async Task<IActionResult> Index()
        {
            var pettappPruebaContext = _context.CarritoDeCompras.Include(c => c.Usuario);
            return View(await pettappPruebaContext.ToListAsync());
        }

        // GET: CarritoDeCompras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoDeCompra = await _context.CarritoDeCompras
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CarritoId == id);
            if (carritoDeCompra == null)
            {
                return NotFound();
            }

            return View(carritoDeCompra);
        }

        // GET: CarritoDeCompras/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId");
            return View();
        }

        // POST: CarritoDeCompras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarritoId,UsuarioId,Total")] CarritoDeCompra carritoDeCompra)
        {
            if (true)
            {
                _context.Add(carritoDeCompra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", carritoDeCompra.UsuarioId);
            return View(carritoDeCompra);
        }

        // GET: CarritoDeCompras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoDeCompra = await _context.CarritoDeCompras.FindAsync(id);
            if (carritoDeCompra == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", carritoDeCompra.UsuarioId);
            return View(carritoDeCompra);
        }

        // POST: CarritoDeCompras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarritoId,UsuarioId,Total")] CarritoDeCompra carritoDeCompra)
        {
            if (id != carritoDeCompra.CarritoId)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    _context.Update(carritoDeCompra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoDeCompraExists(carritoDeCompra.CarritoId))
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
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", carritoDeCompra.UsuarioId);
            return View(carritoDeCompra);
        }

        // GET: CarritoDeCompras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoDeCompra = await _context.CarritoDeCompras
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CarritoId == id);
            if (carritoDeCompra == null)
            {
                return NotFound();
            }

            return View(carritoDeCompra);
        }

        // POST: CarritoDeCompras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carritoDeCompra = await _context.CarritoDeCompras.FindAsync(id);
            if (carritoDeCompra != null)
            {
                _context.CarritoDeCompras.Remove(carritoDeCompra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoDeCompraExists(int id)
        {
            return _context.CarritoDeCompras.Any(e => e.CarritoId == id);
        }
    }
}
