﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
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

        public async Task<IActionResult> Index()
        {
            var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

            carrito = await _context.CarritoDeCompras
                .Include(c => c.CarritoAccesorios)
                .ThenInclude(ca => ca.Accesorio)
                .FirstOrDefaultAsync(c => c.CarritoId == carrito.CarritoId);

            return View(carrito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarAlCarrito([FromBody] CarritoRequestModel request)
        {
            try
            {
                if (request == null || request.AccesorioId <= 0 || request.Cantidad <= 0)
                {
                    return BadRequest(new { message = "Datos inválidos." });
                }

                var accesorio = await _context.Accesorios.FindAsync(request.AccesorioId);
                if (accesorio == null)
                {
                    return NotFound(new { message = "El accesorio no existe." });
                }

                if (accesorio.CantidadDisponible < request.Cantidad)
                {
                    return BadRequest(new { message = "Stock insuficiente." });
                }

                var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

                var carritoAccesorio = carrito.CarritoAccesorios.FirstOrDefault(ca => ca.AccesorioId == request.AccesorioId);
                if (carritoAccesorio == null)
                {
                    carritoAccesorio = new CarritoAccesorio
                    {
                        CarritoId = carrito.CarritoId,
                        AccesorioId = request.AccesorioId,
                        Cantidad = request.Cantidad
                    };
                    _context.CarritoAccesorios.Add(carritoAccesorio);
                }
                else
                {
                    carritoAccesorio.Cantidad += request.Cantidad;

                    if (carritoAccesorio.Cantidad > accesorio.CantidadDisponible)
                    {
                        return BadRequest(new { message = "Cantidad solicitada supera el stock disponible." });
                    }
                }

                await _context.SaveChangesAsync();

                return Json(new { message = "Producto añadido al carrito correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al agregar el producto al carrito.", error = ex.Message });
            }
        }

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
                    }).ToList(),
                    Validado = true // Marcar como validada automáticamente
                };

                // Reducir el stock de los accesorios
                foreach (var detalle in compra.DetallesCompra)
                {
                    var accesorio = await _context.Accesorios.FindAsync(detalle.AccesorioId);
                    if (accesorio != null)
                    {
                        accesorio.CantidadDisponible -= detalle.Cantidad;

                        if (accesorio.CantidadDisponible < 0)
                        {
                            return BadRequest(new { message = "Stock insuficiente para uno o más productos." });
                        }

                        _context.Accesorios.Update(accesorio);
                    }
                }

                _context.Compras.Add(compra);

                // Vaciar el carrito
                _context.CarritoAccesorios.RemoveRange(carrito.CarritoAccesorios);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "¡Compra realizada con éxito!";
                return RedirectToAction("ConfirmacionPago", "CarritoDeCompras");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la compra. Intenta nuevamente.";
                return RedirectToAction("ResumenCarrito");
            }
        }



        public async Task<IActionResult> HistorialCompras()
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

        public async Task<IActionResult> ResumenCarrito()
        {
            var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

            carrito = await _context.CarritoDeCompras
                .Include(c => c.CarritoAccesorios)
                .ThenInclude(ca => ca.Accesorio)
                .FirstOrDefaultAsync(c => c.CarritoId == carrito.CarritoId);

            var tarjetasGuardadas = await _context.Tarjetas
                .Where(t => t.UsuarioId == carrito.UsuarioId)
                .ToListAsync();

            ViewBag.Tarjetas = tarjetasGuardadas;

            return View(carrito);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCantidadElementosCarrito()
        {
            try
            {
                var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);
                int cantidadElementos = carrito.CarritoAccesorios.Count;

                return Json(new { cantidad = cantidadElementos });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al obtener la cantidad de elementos: " + ex.Message });
            }
        }

        public IActionResult ConfirmacionPago()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Usuario no válido.";
                return RedirectToAction("Index");
            }

            var compraReciente = _context.Compras
                .Include(c => c.Tarjeta)
                .Include(c => c.DetallesCompra)
                .ThenInclude(d => d.Accesorio)
                .Where(c => c.UsuarioId == userId)
                .OrderByDescending(c => c.FechaCompra)
                .FirstOrDefault();

            if (compraReciente != null)
            {
                TempData["CompraReciente"] = compraReciente.CompraId;
            }

            return View();
        }

        public IActionResult PagarConQR()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DescargarQR()
        {
            var qrPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "qr_code.jpeg");

            if (!System.IO.File.Exists(qrPath))
            {
                return NotFound("El código QR no está disponible.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(qrPath);
            var fileName = "QR_Pago.jpeg";

            return File(fileBytes, "image/jpeg", fileName);
        }

        public IActionResult FormularioPago()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarFormularioPago(string nombre, string apellido, string bancoDestino, string numeroTransaccion, IFormFile comprobante)
        {
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || string.IsNullOrEmpty(bancoDestino) || string.IsNullOrEmpty(numeroTransaccion) || comprobante == null)
            {
                TempData["ErrorMessage"] = "Todos los campos son obligatorios.";
                return RedirectToAction("FormularioPago");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "comprobantes");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, comprobante.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                comprobante.CopyTo(stream);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Usuario no válido.";
                return RedirectToAction("FormularioPago");
            }

            var carrito = _context.CarritoDeCompras
                .Include(c => c.CarritoAccesorios)
                .ThenInclude(ca => ca.Accesorio)
                .FirstOrDefault(c => c.UsuarioId == userId);

            if (carrito == null || carrito.CarritoAccesorios == null || !carrito.CarritoAccesorios.Any())
            {
                TempData["ErrorMessage"] = "No hay productos en el carrito.";
                return RedirectToAction("FormularioPago");
            }

            var compra = new Compra
            {
                UsuarioId = userId,
                FechaCompra = DateTime.Now,
                Total = carrito.CarritoAccesorios.Sum(ca => (ca.Accesorio?.Precio ?? 0) * ca.Cantidad),
                DetallesCompra = carrito.CarritoAccesorios.Select(ca => new DetalleCompra
                {
                    AccesorioId = ca.AccesorioId,
                    Cantidad = ca.Cantidad,
                    PrecioUnitario = ca.Accesorio?.Precio ?? 0
                }).ToList(),
                BancoDestino = bancoDestino,
                NumeroTransaccion = numeroTransaccion
            };

            _context.Compras.Add(compra);

            _context.CarritoAccesorios.RemoveRange(carrito.CarritoAccesorios);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Formulario enviado exitosamente.";
            return RedirectToAction("ConfirmacionPago");
        }
    }

    public class CarritoRequestModel
    {
        public int AccesorioId { get; set; }
        public int Cantidad { get; set; }
    }
}
