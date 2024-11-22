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
    public class UsuarioRolsController : Controller
    {
        private readonly PettappPruebaContext _context;

        public UsuarioRolsController(PettappPruebaContext context)
        {
            _context = context;
        }

        // GET: UsuarioRols
        public async Task<IActionResult> Index()
        {
            var pettappPruebaContext = _context.UsuarioRols.Include(u => u.Rol).Include(u => u.Usuario);
            return View(await pettappPruebaContext.ToListAsync());
        }

        // GET: UsuarioRols/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioRol = await _context.UsuarioRols
                .Include(u => u.Rol)
                .Include(u => u.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioRolId == id);
            if (usuarioRol == null)
            {
                return NotFound();
            }

            return View(usuarioRol);
        }

        // GET: UsuarioRols/Create
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "RolId");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId");
            return View();
        }

        // POST: UsuarioRols/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioRolId,UsuarioId,RolId,FechaAsignacion")] UsuarioRol usuarioRol)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuarioRol);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "RolId", usuarioRol.RolId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", usuarioRol.UsuarioId);
            return View(usuarioRol);
        }

        // GET: UsuarioRols/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioRol = await _context.UsuarioRols.FindAsync(id);
            if (usuarioRol == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "RolId", usuarioRol.RolId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", usuarioRol.UsuarioId);
            return View(usuarioRol);
        }

        // POST: UsuarioRols/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioRolId,UsuarioId,RolId,FechaAsignacion")] UsuarioRol usuarioRol)
        {
            if (id != usuarioRol.UsuarioRolId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarioRol);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioRolExists(usuarioRol.UsuarioRolId))
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
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "RolId", usuarioRol.RolId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", usuarioRol.UsuarioId);
            return View(usuarioRol);
        }

        // GET: UsuarioRols/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioRol = await _context.UsuarioRols
                .Include(u => u.Rol)
                .Include(u => u.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioRolId == id);
            if (usuarioRol == null)
            {
                return NotFound();
            }

            return View(usuarioRol);
        }

        // POST: UsuarioRols/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioRol = await _context.UsuarioRols.FindAsync(id);
            if (usuarioRol != null)
            {
                _context.UsuarioRols.Remove(usuarioRol);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioRolExists(int id)
        {
            return _context.UsuarioRols.Any(e => e.UsuarioRolId == id);
        }
    }
}
