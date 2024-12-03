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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Usuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.UsuarioRols)
                .ThenInclude(ur => ur.Rol)
                .ToListAsync();

            return View(usuarios);
        }

        // Acción raíz: Redirige al DashboardCliente
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }
            return RedirectToAction("DashboardCliente"); // Redirección por defecto si no está autenticado
        }

        // Página de privacidad (Ejemplo de una página pública)
        public IActionResult Privacy()
        {
            return View();
        }

        // Manejo de errores
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Información del usuario autenticado (Ejemplo)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Usuario()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); // Si no está autenticado
            }
            int userId = int.Parse(userIdClaim.Value);

            var adopciones = await _context.Adopciones
                .Include(a => a.Mascota)
                .Where(a => a.UsuarioId == userId)
                .ToListAsync();

            ViewData["Mensaje"] = adopciones.Any() ? null : "No has adoptado ninguna mascota aún.";
            ViewData["Adopciones"] = adopciones;

            return View();
        }

        // Redirección a la página de adopción
        public IActionResult Adoptar()
        {
            return RedirectToAction("VistaMascota", "Mascotas");
        }

        // Redirección a la página de accesorios
        public IActionResult ComprarAccesorios()
        {
            return RedirectToAction("Index", "Accesorios");
        }

        // Redirección a la página de donaciones
        public IActionResult Donar()
        {
            return RedirectToAction("Index", "Donaciones");
        }

        // Dashboard para Administrador
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DashboardAdmin()
        {
            // Consulta la cantidad de usuarios por rol desde la tabla intermedia UsuarioRol
            var usuariosPorRol = await _context.UsuarioRols
                .Include(ur => ur.Rol) // Incluye información del Rol
                .GroupBy(ur => ur.Rol.Nombre) // Agrupa por el nombre del rol
                .Select(g => new { Rol = g.Key, Cantidad = g.Count() }) // Obtén el rol y la cantidad
                .ToDictionaryAsync(x => x.Rol, x => x.Cantidad); // Convierte a diccionario

            // Pasa los datos a la vista
            ViewBag.UsuariosPorRol = usuariosPorRol;

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

        [AllowAnonymous]
        public async Task<IActionResult> DashboardCliente()
        {
            // Permitir acceso al Dashboard sin autenticación
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                // Vista básica sin datos personalizados
                return View();
            }

            // Si el usuario está autenticado, mostrar datos personalizados
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdClaim.Value);

            var adopciones = await _context.Adopciones
                .Include(a => a.Mascota)
                .Where(a => a.UsuarioId == userId)
                .ToListAsync();

            var donaciones = await _context.Donaciones
                .Include(d => d.Refugio)
                .Where(d => d.UsuarioId == userId)
                .ToListAsync();

            ViewData["Adopciones"] = adopciones;
            ViewData["Donaciones"] = donaciones;

            return View(await _context.Usuarios.FindAsync(userId));
        }

        // Redirigir al Dashboard según el rol
        private IActionResult RedirectToDashboard()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            return userRole switch
            {
                "Admin" => RedirectToAction("DashboardAdmin"),
                "Vendedor" => RedirectToAction("DashboardVendedor"),
                "Refugio" => RedirectToAction("DashboardRefugio"),
                "Cliente" => RedirectToAction("DashboardCliente"),
                _ => RedirectToAction("DashboardCliente")
            };
        }
    }
}
