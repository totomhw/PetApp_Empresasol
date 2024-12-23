﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

                // Validar si se usa una tarjeta guardada
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

                // Validar datos de nueva tarjeta
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
                return RedirectToAction("ConfirmacionPago", "CarritoDeCompras");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la compra. Intenta nuevamente.";
                return RedirectToAction("ResumenCarrito");
            }
        }

        // GET: CarritoDeCompras/HistorialCompras
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

        // GET: CarritoDeCompras/ResumenCarrito
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

        // GET: CarritoDeCompras/ObtenerCantidadElementosCarrito
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

        // GET: CarritoDeCompras/ConfirmacionPago
        public IActionResult ConfirmacionPago()
        {
            return View();
        }

        // GET: CarritoDeCompras/ContarProductosEnCarrito
        [HttpGet]
        public async Task<int> ContarProductosEnCarrito()
        {
            var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);
            return carrito.CarritoAccesorios.Sum(ca => ca.Cantidad);
        }
    }
}
