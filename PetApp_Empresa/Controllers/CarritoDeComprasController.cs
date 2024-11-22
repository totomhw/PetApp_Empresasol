using System;
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

            // Incluir los detalles de los accesorios en el carrito
            carrito = await _context.CarritoDeCompras
                .Include(c => c.CarritoAccesorios)
                .ThenInclude(ca => ca.Accesorio)
                .FirstOrDefaultAsync(c => c.CarritoId == carrito.CarritoId);

            return View(carrito);
        }

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

                if (accesorio.CantidadDisponible < cantidad)
                {
                    return BadRequest(new { message = "Stock insuficiente para la cantidad solicitada." });
                }

                var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

                // Verificar si el accesorio ya está en el carrito
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

                // Reducir el stock del accesorio
                accesorio.CantidadDisponible -= cantidad;

                // Guardar los cambios
                await _context.SaveChangesAsync();

                var cantidadCarrito = carrito.CarritoAccesorios.Sum(ca => ca.Cantidad);
                return Json(new { message = "Producto añadido al carrito", cantidadCarrito });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al agregar el producto al carrito.", error = ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcesarPago()
        {
            try
            {
                var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

                if (carrito.CarritoAccesorios == null || !carrito.CarritoAccesorios.Any())
                {
                    TempData["ErrorMessage"] = "El carrito está vacío.";
                    return RedirectToAction("ResumenCarrito");
                }

                carrito.Total = carrito.CarritoAccesorios
                    .Where(item => item.Accesorio != null)
                    .Sum(item => item.Accesorio.Precio * item.Cantidad);

                _context.CarritoAccesorios.RemoveRange(carrito.CarritoAccesorios);
                await _context.SaveChangesAsync();

                return RedirectToAction("ConfirmacionPago");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un error al procesar el pago. Intenta nuevamente.";
                return RedirectToAction("ResumenCarrito");
            }
        }



        // GET: CarritoDeCompras/ResumenCarrito
        public async Task<IActionResult> ResumenCarrito()
        {
            var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

            carrito = await _context.CarritoDeCompras
                .Include(c => c.CarritoAccesorios)
                .ThenInclude(ca => ca.Accesorio)
                .FirstOrDefaultAsync(c => c.CarritoId == carrito.CarritoId);

            return View(carrito);
        }

        // GET: CarritoDeCompras/ObtenerCantidadCarrito
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObtenerCantidadElementosCarrito()
        {
            try
            {
                // Obtener el carrito del usuario autenticado
                var carrito = await CarritoHelper.ObtenerOCrearCarritoUsuario(_context, User);

                // Usar el helper para contar elementos únicos
                int cantidadElementos = CarritoHelper.ContarElementosEnCarrito(carrito);

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
