using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        // GET: Accesorios/Create
        public IActionResult Create()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                ModelState.AddModelError("", "No se pudo identificar al usuario autenticado.");
                return RedirectToAction(nameof(Index));
            }

            ViewData["VendedorId"] = userIdClaim;
            return View();
        }

        // POST: Accesorios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccesorioId,Nombre,Descripcion,Precio,CantidadDisponible")] Accesorio accesorio, IFormFile ImagenArchivo)
        {
            if (true)
            {
                try
                {
                    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userIdClaim))
                    {
                        ModelState.AddModelError("", "No se pudo identificar al usuario autenticado.");
                        return View(accesorio);
                    }
                    accesorio.VendedorId = int.Parse(userIdClaim);

                    if (ImagenArchivo != null && ImagenArchivo.Length > 0)
                    {
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extensionArchivo = Path.GetExtension(ImagenArchivo.FileName).ToLower();

                        if (!extensionesPermitidas.Contains(extensionArchivo))
                        {
                            ModelState.AddModelError("", "El archivo debe ser una imagen con extensión .jpg, .jpeg, .png o .gif.");
                            return View(accesorio);
                        }

                        var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenesAccesorios");
                        if (!Directory.Exists(rutaCarpeta))
                        {
                            Directory.CreateDirectory(rutaCarpeta);
                        }

                        var nombreArchivo = Guid.NewGuid().ToString() + extensionArchivo;
                        var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await ImagenArchivo.CopyToAsync(stream);
                        }

                        accesorio.ImagenUrl = "/imagenesAccesorios/" + nombreArchivo;
                    }

                    _context.Add(accesorio);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ocurrió un error al guardar el accesorio: {ex.Message}");
                }
            }

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
            ViewData["VendedorId"] = accesorio.VendedorId;
            return View(accesorio);
        }

        // POST: Accesorios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccesorioId,Nombre,Descripcion,Precio,VendedorId,CantidadDisponible")] Accesorio accesorio, IFormFile ImagenArchivo)
        {
            if (id != accesorio.AccesorioId)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    if (ImagenArchivo != null && ImagenArchivo.Length > 0)
                    {
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extensionArchivo = Path.GetExtension(ImagenArchivo.FileName).ToLower();

                        if (!extensionesPermitidas.Contains(extensionArchivo))
                        {
                            ModelState.AddModelError("", "El archivo debe ser una imagen con extensión .jpg, .jpeg, .png o .gif.");
                            return View(accesorio);
                        }

                        var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenesAccesorios");
                        if (!Directory.Exists(rutaCarpeta))
                        {
                            Directory.CreateDirectory(rutaCarpeta);
                        }

                        var nombreArchivo = Guid.NewGuid().ToString() + extensionArchivo;
                        var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await ImagenArchivo.CopyToAsync(stream);
                        }

                        accesorio.ImagenUrl = "/imagenesAccesorios/" + nombreArchivo;
                    }

                    _context.Update(accesorio);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ocurrió un error al guardar el accesorio: {ex.Message}");
                }
            }

            return View(accesorio);
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

        // POST: Accesorios/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accesorio = await _context.Accesorios.FindAsync(id);
            if (accesorio != null)
            {
                _context.Accesorios.Remove(accesorio);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Nueva acción: ListadoAccesorios
        public async Task<IActionResult> ListadoAccesorios()
        {
            var accesorios = await _context.Accesorios.ToListAsync();
            var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);
            var carritoCount = carrito.CarritoAccesorios.Sum(ca => ca.Cantidad);

            ViewData["CarritoCount"] = carritoCount;
            return View(accesorios);
        }

        // POST: Accesorios/AgregarAlCarrito
        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int accesorioId, int cantidad)
        {
            if (cantidad <= 0)
            {
                return BadRequest(new { message = "La cantidad debe ser mayor a 0." });
            }

            var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);
            var accesorio = await _context.Accesorios.FindAsync(accesorioId);

            if (accesorio == null || accesorio.CantidadDisponible < cantidad)
            {
                return BadRequest(new { message = "No hay suficiente stock para agregar al carrito." });
            }

            var itemCarrito = carrito.CarritoAccesorios.FirstOrDefault(ca => ca.AccesorioId == accesorioId);
            if (itemCarrito != null)
            {
                itemCarrito.Cantidad += cantidad;
            }
            else
            {
                carrito.CarritoAccesorios.Add(new CarritoAccesorio
                {
                    AccesorioId = accesorioId,
                    Cantidad = cantidad
                });
            }

            accesorio.CantidadDisponible -= cantidad;
            _context.Update(accesorio);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Producto añadido al carrito." });
        }

        private bool AccesorioExists(int id)
        {
            return _context.Accesorios.Any(e => e.AccesorioId == id);
        }
    }
}
