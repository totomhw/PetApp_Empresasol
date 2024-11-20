using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PetApp_Empresa.Models;

namespace Pettapp2.Controllers
{
    public class AuthController : Controller
    {
        private readonly PettappPruebaContext _context;

        public AuthController(PettappPruebaContext context)
        {
            _context = context;
        }

        // GET: Auth/Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["HideLayout"] = true; // Ocultar el layout en la vista de Login
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string nombre, string password)
        {
            ViewData["HideLayout"] = true;

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "El nombre de usuario y la contraseña son obligatorios.");
                return View();
            }

            var user = await _context.Usuarios
                .Include(u => u.UsuarioRols)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.Nombre == nombre && u.Activo == true);

            if (user != null && VerifyPassword(password, user.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Nombre),
                    new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email ?? "")
                };

                foreach (var userRole in user.UsuarioRols)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Rol.Nombre)); // Agregar roles como claims
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirigir según el rol del usuario
                string userRoleName = user.UsuarioRols.FirstOrDefault()?.Rol.Nombre;
                return userRoleName switch
                {
                    "Admin" => RedirectToAction("DashboardAdmin", "Home"),
                    "Vendedor" => RedirectToAction("DashboardVendedor", "Home"),
                    "Refugio" => RedirectToAction("DashboardRefugio", "Home"),
                    "Cliente" => RedirectToAction("DashboardCliente", "Home"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View();
        }

        // GET: Auth/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewData["HideLayout"] = true; // Ocultar el layout en la vista de Registro
            ViewBag.Roles = _context.Roles.ToList(); // Cargar lista de roles para el dropdown
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Nombre,Password,Email")] Usuario usuario, int rolId)
        {
            ViewData["HideLayout"] = true;

            if (ModelState.IsValid)
            {
                // Verificar si el usuario o el email ya están en uso
                if (await _context.Usuarios.AnyAsync(u => u.Nombre == usuario.Nombre || u.Email == usuario.Email))
                {
                    ModelState.AddModelError("", "El nombre de usuario o el email ya están en uso.");
                    ViewBag.Roles = _context.Roles.ToList(); // Recargar roles en caso de error
                    return View(usuario);
                }

                // Encriptar la contraseña antes de almacenarla
                usuario.Password = HashPassword(usuario.Password);
                usuario.Activo = true;

                // Guardar el usuario en la base de datos
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Asignar el rol seleccionado al usuario
                var usuarioRol = new UsuarioRol
                {
                    UsuarioId = usuario.UsuarioId,
                    RolId = rolId,
                    FechaAsignacion = DateTime.Now
                };
                _context.UsuarioRols.Add(usuarioRol);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Login));
            }

            ViewBag.Roles = _context.Roles.ToList(); // Recargar roles en caso de error
            return View(usuario);
        }

        // POST: Auth/Logout
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // Verificar la contraseña ingresada contra la almacenada
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var hashedEnteredPassword = HashPassword(enteredPassword);
            return storedHash == hashedEnteredPassword;
        }

        // Función para encriptar la contraseña
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
