using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetApp_Empresa.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace PetApp_Empresa.Controllers
{
    public class HomeController : Controller
    {
        private readonly PettappPruebaContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, PettappPruebaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Usuario()
        {
            // Obtener el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); // Verificar si el usuario está autenticado
            }
            int userId = int.Parse(userIdClaim.Value);

            // Consultar las adopciones del usuario autenticado
            var adopciones = await _context.Adopciones
                .Include(a => a.Mascota)
                .Where(a => a.UsuarioId == userId) // Filtrar por id de usuario autenticado
                .ToListAsync();

            // Pasar las adopciones a la vista
            ViewData["Mensaje"] = adopciones.Any() ? null : "No has adoptado ninguna mascota aún.";
            ViewData["Adopciones"] = adopciones;

            return View();
        }

        public IActionResult Adoptar()
        {
            return RedirectToAction("VistaMascota", "Mascotas");
        }

        public IActionResult ComprarAccesorios()
        {
            return RedirectToAction("Index", "Accesorios");
        }

        public IActionResult Donar()
        {
            return RedirectToAction("Index", "Donaciones");
        }

        // Dashboard para Administrador
        [Authorize(Roles = "Admin,Refugio,Vendedor,Cliente")]
        public IActionResult DashboardAdmin()
        {
            return View();
        }

        // Dashboard para Vendedor
        [Authorize(Roles = "Vendedor,Admin")]
        public IActionResult DashboardVendedor()
        {
            return View();
        }

        // Dashboard para Refugio
        [Authorize(Roles = "Refugio,Admin")]
        public IActionResult DashboardRefugio()
        {
            return View();
        }

        // Dashboard para Cliente
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<IActionResult> DashboardCliente()
        {
            // Obtener el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);

            // Obtener adopciones y donaciones del cliente autenticado
            var adopciones = await _context.Adopciones
                .Include(a => a.Mascota)
                .Where(a => a.UsuarioId == userId)
                .ToListAsync();

            var donaciones = await _context.Donaciones
                .Include(d => d.Refugio)
                .Where(d => d.UsuarioId == userId)
                .ToListAsync();

            // Pasar los datos a la vista
            ViewData["Adopciones"] = adopciones;
            ViewData["Donaciones"] = donaciones;

            return View("DashboardCliente", await _context.Usuarios.FindAsync(userId));
        }
    }
}
