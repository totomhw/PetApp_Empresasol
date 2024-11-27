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
                var accesorio = await _context.Accesorios.FindAsync(accesorioId);
                if (accesorio == null)
                {
                    return BadRequest(new { message = "El accesorio no existe." });
                }

                var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

                var carritoAccesorio = carrito.CarritoAccesorios.FirstOrDefault(ca => ca.AccesorioId == accesorioId);
                if (carritoAccesorio == null)
                {
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
                    carritoAccesorio.Cantidad += cantidad;
                }

                await _context.SaveChangesAsync();

                return Json(new { message = "Producto añadido al carrito" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al agregar el producto al carrito.", error = ex.Message });
            }
        }

        // GET: CarritoDeCompras/ResumenCarrito
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
                    return RedirectToAction("Index");
                }

                var tarjetasGuardadas = await _context.Tarjetas
                    .Where(t => t.UsuarioId == carrito.UsuarioId)
                    .ToListAsync();

                ViewBag.Tarjetas = tarjetasGuardadas;

                return View(carrito);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al cargar el resumen del carrito: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: CarritoDeCompras/ConfirmarCompra
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarCompra(
            int? tarjetaId,
            string? numeroTarjeta,
            int? mesVencimiento,
            int? anioVencimiento,
            string? cvv,
            bool guardarTarjeta)
        {
            try
            {
                var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

                if (carrito.CarritoAccesorios == null || !carrito.CarritoAccesorios.Any())
                {
                    TempData["ErrorMessage"] = "El carrito está vacío.";
                    return RedirectToAction("ResumenCarrito");
                }

                if (tarjetaId.HasValue)
                {
                    var tarjeta = await _context.Tarjetas.FindAsync(tarjetaId.Value);
                    if (tarjeta == null)
                    {
                        TempData["ErrorMessage"] = "La tarjeta seleccionada no es válida.";
                        return RedirectToAction("ResumenCarrito");
                    }

                    return await ProcesarCompra(carrito, tarjetaId.Value);
                }

                if (!string.IsNullOrEmpty(numeroTarjeta) && mesVencimiento.HasValue && anioVencimiento.HasValue && !string.IsNullOrEmpty(cvv))
                {
                    var nuevaTarjeta = new Tarjeta
                    {
                        UsuarioId = carrito.UsuarioId,
                        Numero = numeroTarjeta,
                        FechaVencimiento = $"{mesVencimiento.Value:D2}/{anioVencimiento.Value % 100:D2}",
                        CVV = cvv,
                        FechaRegistro = DateTime.Now
                    };

                    _context.Tarjetas.Add(nuevaTarjeta);
                    await _context.SaveChangesAsync();

                    return await ProcesarCompra(carrito, nuevaTarjeta.TarjetaId);
                }

                TempData["ErrorMessage"] = "Debe seleccionar una tarjeta válida o ingresar una nueva.";
                return RedirectToAction("ResumenCarrito");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un error al confirmar la compra. Intenta nuevamente.";
                return RedirectToAction("ResumenCarrito");
            }
        }

        private async Task<IActionResult> ProcesarCompra(CarritoDeCompra carrito, int tarjetaId)
        {
            try
            {
                decimal totalCompra = carrito.CarritoAccesorios.Sum(item => (item.Accesorio?.Precio ?? 0) * item.Cantidad);

                var compra = new Compra
                {
                    UsuarioId = carrito.UsuarioId,
                    TarjetaId = tarjetaId,
                    FechaCompra = DateTime.Now,
                    Total = totalCompra,
                    DetallesCompra = carrito.CarritoAccesorios.Select(item => new DetalleCompra
                    {
                        AccesorioId = item.AccesorioId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.Accesorio?.Precio ?? 0
                    }).ToList()
                };

                _context.Compras.Add(compra);

                _context.CarritoAccesorios.RemoveRange(carrito.CarritoAccesorios);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "¡Compra realizada con éxito!";
                return RedirectToAction("HistorialCompras");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la compra. Intenta nuevamente.";
                return RedirectToAction("ResumenCarrito");
            }
        }

        // NUEVA VISTA: Pagar con QR
        public IActionResult PagarConQR()
        {
            ViewBag.QRImagePath = "/images/QR.jpeg";
            return View();
        }

        // NUEVA VISTA: Formulario de transacción bancaria
        public IActionResult FormularioTransaccionBancaria()
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

        // POST: Guardar transacción bancaria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarTransaccion(string nombre, string apellido, string numeroTransaccion, string bancoOrigen, IFormFile comprobante)
        {
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || string.IsNullOrEmpty(numeroTransaccion) || string.IsNullOrEmpty(bancoOrigen))
            {
                TempData["ErrorMessage"] = "Todos los campos son obligatorios.";
                return RedirectToAction("FormularioTransaccionBancaria");
            }

            if (comprobante != null && comprobante.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/comprobantes", comprobante.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    comprobante.CopyTo(stream);
                }
            }

            TempData["SuccessMessage"] = "Transacción realizada con éxito.";
            return RedirectToAction("ConfirmacionCompra");
        }

        // NUEVA VISTA: Confirmación de compra
        public IActionResult ConfirmacionCompra()
        {
            ViewBag.Mensaje = "Gracias por comprar en nuestra tienda, vuelva pronto.";
            return View();
        }

        // GET: CarritoDeCompras/HistorialCompras
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
                    .Include(c => c.DetallesCompra)
                    .ThenInclude(d => d.Accesorio)
                    .Where(c => c.UsuarioId == userId)
                    .ToListAsync();

                return View(compras);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al cargar el historial de compras: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
