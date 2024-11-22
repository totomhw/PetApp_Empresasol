using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PetApp_Empresa.Models;

namespace PetApp_Empresa.Controllers
{
    public class AuthController : Controller
    {
        private readonly PettappPruebaContext _context;

        public AuthController(PettappPruebaContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }

            ViewData["ReturnUrl"] = returnUrl ?? "/Home/DashboardCliente";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string nombre, string password, string? returnUrl = null)
        {
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
                    new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString())
                };

                foreach (var role in user.UsuarioRols)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Rol.Nombre));
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

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToDashboard();
            }

            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("DashboardCliente", "Home");
        }

        private IActionResult RedirectToDashboard()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            return userRole switch
            {
                "Admin" => RedirectToAction("DashboardAdmin", "Home"),
                "Vendedor" => RedirectToAction("DashboardVendedor", "Home"),
                "Refugio" => RedirectToAction("DashboardRefugio", "Home"),
                "Cliente" => RedirectToAction("DashboardCliente", "Home"),
                _ => RedirectToAction("DashboardCliente", "Home")
            };
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var hashedEnteredPassword = HashPassword(enteredPassword);
            return storedHash == hashedEnteredPassword;
        }

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
