using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetApp_Empresa.Models;

namespace PetApp_Empresa.Controllers
{
    [Authorize]
    public class CarritoDeComprasController : Controller
    {
        private readonly PettappPruebaContext _context;

        public CarritoDeComprasController(PettappPruebaContext context)
        {
            _context = context;
        }

        // GET: CarritoDeCompras/Index
        public async Task<IActionResult> Index()
        {
            var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

            carrito = await _context.CarritoDeCompras
                .Include(c => c.CarritoAccesorios)
                .ThenInclude(ca => ca.Accesorio)
                .FirstOrDefaultAsync(c => c.CarritoId == carrito.CarritoId);

            return View(carrito);
        }

        // POST: CarritoDeCompras/AgregarAlCarrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarAlCarrito(int accesorioId, int cantidad)
        {
            try
            {
                // Validar que el accesorio existe
                var accesorio = await _context.Accesorios.FindAsync(accesorioId);
                if (accesorio == null)
                {
                    return Json(new { success = false, message = "El accesorio no existe." });
                }

                // Validar que la cantidad sea válida
                if (cantidad <= 0)
                {
                    return Json(new { success = false, message = "La cantidad debe ser mayor a 0." });
                }

                // Obtener o crear el carrito del usuario
                var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

                // Comprobar si el accesorio ya está en el carrito
                var carritoAccesorio = await _context.CarritoAccesorios
                    .FirstOrDefaultAsync(ca => ca.CarritoId == carrito.CarritoId && ca.AccesorioId == accesorioId);

                if (carritoAccesorio == null)
                {
                    // Agregar un nuevo accesorio al carrito
                    carritoAccesorio = new CarritoAccesorio
                    {
                        CarritoId = carrito.CarritoId,
                        AccesorioId = accesorioId,
                        Cantidad = cantidad
                    };
                    _context.CarritoAccesorios.Add(carritoAccesorio);
                }
                else
                {
                    // Actualizar la cantidad del accesorio en el carrito
                    carritoAccesorio.Cantidad += cantidad;
                }

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Accesorio agregado al carrito correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al agregar el accesorio al carrito.", error = ex.Message });
            }
        }

        // GET: CarritoDeCompras/ResumenCarrito
        [HttpGet]
        public async Task<IActionResult> ResumenCarrito()
        {
            try
            {
                var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

                carrito = await _context.CarritoDeCompras
                    .Include(c => c.CarritoAccesorios)
                    .ThenInclude(ca => ca.Accesorio)
                    .FirstOrDefaultAsync(c => c.CarritoId == carrito.CarritoId);

                if (carrito == null)
                {
                    TempData["ErrorMessage"] = "No se pudo encontrar el carrito.";
                    return RedirectToAction("Index", "Home");
                }

                var tarjetasGuardadas = await _context.Tarjetas
                    .Where(t => t.UsuarioId == carrito.UsuarioId)
                    .ToListAsync();

                ViewBag.Tarjetas = tarjetasGuardadas;

                return View(carrito);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el carrito: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CarritoDeCompras/PagarConQR
        [HttpGet]
        public IActionResult PagarConQR()
        {
            try
            {
                ViewBag.QRImagePath = "/comprobantes/QR.jpeg";
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar la vista de QR: {ex.Message}";
                return RedirectToAction("ResumenCarrito");
            }
        }

        // GET: CarritoDeCompras/FormularioTransaccionBancaria
        [HttpGet]
        public IActionResult FormularioTransaccionBancaria()
        {
            try
            {
                ViewBag.Bancos = new string[]
                {
                    "Banco Nacional de Bolivia",
                    "Banco Mercantil Santa Cruz",
                    "Banco Unión",
                    "Banco Bisa",
                    "Banco Económico",
                    "Banco FIE",
                    "Banco Fortaleza",
                    "Banco Ganadero",
                    "BancoSol",
                    "Banco Prodem",
                    "Banco de Crédito de Bolivia"
                };

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar la vista del formulario: {ex.Message}";
                return RedirectToAction("ResumenCarrito");
            }
        }

        // POST: CarritoDeCompras/GuardarTransaccion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarTransaccion(
            string nombre,
            string apellido,
            string numeroTransaccion,
            string bancoOrigen,
            IFormFile comprobante)
        {
            try
            {
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) ||
                    string.IsNullOrEmpty(numeroTransaccion) || string.IsNullOrEmpty(bancoOrigen))
                {
                    TempData["ErrorMessage"] = "Todos los campos son obligatorios.";
                    return RedirectToAction("FormularioTransaccionBancaria");
                }

                string comprobantePath = null;

                if (comprobante != null && comprobante.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "comprobantes");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    comprobantePath = Path.Combine(uploadsFolder, comprobante.FileName);
                    using (var stream = new FileStream(comprobantePath, FileMode.Create))
                    {
                        await comprobante.CopyToAsync(stream);
                    }
                }

                var transaccion = new TransaccionBancaria
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    NumeroTransaccion = numeroTransaccion,
                    BancoOrigen = bancoOrigen,
                    ComprobantePath = comprobante != null ? $"/comprobantes/{comprobante.FileName}" : null,
                    FechaTransaccion = DateTime.Now
                };

                _context.TransaccionesBancarias.Add(transaccion);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Transacción guardada exitosamente.";
                return RedirectToAction("HistorialCompras");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al guardar la transacción: {ex.Message}";
                return RedirectToAction("FormularioTransaccionBancaria");
            }
        }

        // GET: CarritoDeCompras/HistorialCompras
        [HttpGet]
        public async Task<IActionResult> HistorialCompras()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var compras = await _context.Compras
                    .Include(c => c.Tarjeta)
                    .Include(c => c.QR)
                    .Include(c => c.DetallesCompra)
                    .ThenInclude(d => d.Accesorio)
                    .Where(c => c.UsuarioId == userId)
                    .ToListAsync();

                return View(compras);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el historial de compras: {ex.Message}";
                return RedirectToAction("ResumenCarrito");
            }
        }
    }
}
