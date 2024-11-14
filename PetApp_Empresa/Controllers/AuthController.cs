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
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Usuario loginModel)
        {
            // Verificar si el modelo es válido antes de continuar
            if (ModelState.IsValid)
            {
                return View(loginModel);
            }

            var user = await _context.Usuarios
                .Include(u => u.UsuarioRols)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.Nombre == loginModel.Nombre && u.Activo == true);

            if (user != null && VerifyPassword(loginModel.Password, user.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Nombre),
                    new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                foreach (var userRole in user.UsuarioRols)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Rol.Nombre)); // Agregar rol como claim
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                    IsPersistent = false,
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
            return View(loginModel);
        }

        // GET: Auth/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.Roles = _context.Roles.ToList(); // Cargar lista de roles para el dropdown en la vista
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Nombre,Password,Email")] Usuario usuario, int rolId)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el usuario o el email ya están en uso
                if (await _context.Usuarios.AnyAsync(u => u.Nombre == usuario.Nombre || u.Email == usuario.Email))
                {
                    ModelState.AddModelError("", "El usuario o email ya está en uso.");
                    ViewBag.Roles = _context.Roles.ToList(); // Recargar roles en caso de error
                    return View(usuario);
                }

                // Encriptar la contraseña antes de almacenarla
                usuario.Password = HashPassword(usuario.Password);
                usuario.Activo = true;

                // Guardar el usuario en la base de datos
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // Asignar el rol seleccionado al usuario
                var usuarioRol = new UsuarioRol
                {
                    UsuarioId = usuario.UsuarioId,
                    RolId = rolId,
                    FechaAsignacion = DateTime.Now
                };
                _context.Add(usuarioRol);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Login));
            }

            ViewBag.Roles = _context.Roles.ToList();
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

        // Verificación de la contraseña ingresada contra la almacenada
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
