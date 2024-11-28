using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetApp_Empresa.Models;

namespace PetApp_Empresa.Controllers
{
    [Authorize]
    public class MascotasController : Controller
    {
        private readonly PettappPruebaContext _context;

        public MascotasController(PettappPruebaContext context)
        {
            _context = context;
        }

        // Método para que el usuario vea las mascotas de los refugios que administra
        public async Task<IActionResult> MisMascotas()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);

            var mascotas = await _context.Mascotas
                .Include(m => m.Refugio)
                .Where(m => m.Refugio.UsuarioId == userId)
                .ToListAsync();

            return View("MisMascotas", mascotas);
        }

        // Método para que el usuario vea las donaciones realizadas a los refugios que administra
        public async Task<IActionResult> MisDonaciones()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);

            var donaciones = await _context.Donaciones
                .Include(d => d.Refugio)
                .Include(d => d.Usuario)
                .Where(d => d.Refugio.UsuarioId == userId)
                .ToListAsync();

            return View("MisDonaciones", donaciones);
        }

        // GET: Mascotas
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);

            var mascotasUsuario = await _context.Mascotas
                .Include(m => m.Refugio)
                .Where(m => m.Refugio.UsuarioId == userId)
                .ToListAsync();

            return View("Index", mascotasUsuario);
        }

        // Nueva acción para ver todas las mascotas en general
        public async Task<IActionResult> VerTodas()
        {
            var todasLasMascotas = await _context.Mascotas.Include(m => m.Refugio).ToListAsync();
            return View("VerTodas", todasLasMascotas);
        }

        // GET: Mascotas/Create
        public async Task<IActionResult> Create()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);

            var refugiosUsuario = await _context.Refugios
                .Where(r => r.UsuarioId == userId)
                .ToListAsync();
            ViewData["RefugioId"] = new SelectList(refugiosUsuario, "RefugioId", "Nombre");

            return View();
        }

        // POST: Mascotas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MascotaId,Nombre,Raza,Sexo,Descripcion,EstadoAdopcion,RefugioId,Edad")] Mascota mascota, IFormFile ImagenArchivo)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);

            var refugio = await _context.Refugios
                .FirstOrDefaultAsync(r => r.RefugioId == mascota.RefugioId && r.UsuarioId == userId);
            if (refugio == null)
            {
                return Unauthorized("No puedes crear una mascota en un refugio que no administras.");
            }

            if (true)
            {
                try
                {
                    // Verificar si se subió un archivo de imagen
                    if (ImagenArchivo != null && ImagenArchivo.Length > 0)
                    {
                        // Extensiones permitidas
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extensionArchivo = Path.GetExtension(ImagenArchivo.FileName).ToLower();

                        if (!extensionesPermitidas.Contains(extensionArchivo))
                        {
                            ModelState.AddModelError("", "El archivo debe ser una imagen con extensión .jpg, .jpeg, .png o .gif.");
                            return View(mascota);
                        }

                        // Ruta para guardar la imagen
                        var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
                        if (!Directory.Exists(rutaCarpeta))
                        {
                            Directory.CreateDirectory(rutaCarpeta);
                        }

                        // Nombre único para la imagen
                        var nombreArchivo = Guid.NewGuid().ToString() + extensionArchivo;
                        var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                        // Guardar la imagen en el servidor
                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await ImagenArchivo.CopyToAsync(stream);
                        }

                        // Asignar la URL de la imagen al modelo
                        mascota.ImagenUrl = "/imagenes/" + nombreArchivo;
                    }

                    _context.Add(mascota);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ocurrió un error al guardar la mascota: {ex.Message}");
                }
            }

            var refugiosUsuario = await _context.Refugios
                .Where(r => r.UsuarioId == userId)
                .ToListAsync();
            ViewData["RefugioId"] = new SelectList(refugiosUsuario, "RefugioId", "Nombre", mascota.RefugioId);

            return View(mascota);
        }


        // POST: Mascotas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MascotaId,Nombre,Raza,Sexo,Descripcion,EstadoAdopcion,RefugioId,Edad")] Mascota mascota, IFormFile ImagenArchivo)
        {
            if (id != mascota.MascotaId)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);

            var refugio = await _context.Refugios
                .FirstOrDefaultAsync(r => r.RefugioId == mascota.RefugioId && r.UsuarioId == userId);
            if (refugio == null)
            {
                return Unauthorized("No puedes editar una mascota en un refugio que no administras.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si se subió un archivo de imagen
                    if (ImagenArchivo != null && ImagenArchivo.Length > 0)
                    {
                        // Extensiones permitidas
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extensionArchivo = Path.GetExtension(ImagenArchivo.FileName).ToLower();

                        if (!extensionesPermitidas.Contains(extensionArchivo))
                        {
                            ModelState.AddModelError("", "El archivo debe ser una imagen con extensión .jpg, .jpeg, .png o .gif.");
                            return View(mascota);
                        }

                        // Ruta para guardar la imagen
                        var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
                        if (!Directory.Exists(rutaCarpeta))
                        {
                            Directory.CreateDirectory(rutaCarpeta);
                        }

                        // Nombre único para la imagen
                        var nombreArchivo = Guid.NewGuid().ToString() + extensionArchivo;
                        var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                        // Guardar la imagen en el servidor
                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await ImagenArchivo.CopyToAsync(stream);
                        }

                        // Actualizar la URL de la imagen en el modelo
                        mascota.ImagenUrl = "/imagenes/" + nombreArchivo;
                    }

                    _context.Update(mascota);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ocurrió un error al guardar la mascota: {ex.Message}");
                }
            }

            var refugiosUsuario = await _context.Refugios
                .Where(r => r.UsuarioId == userId)
                .ToListAsync();
            ViewData["RefugioId"] = new SelectList(refugiosUsuario, "RefugioId", "Nombre", mascota.RefugioId);

            return View(mascota);
        }


        private bool MascotaExists(int id)
        {
            return _context.Mascotas.Any(e => e.MascotaId == id);
        }

        // GET: Mascotas/VistaMascota
        public async Task<IActionResult> VistaMascota(int? edadFiltro)
        {
            var mascotas = await _context.Mascotas.Include(m => m.Refugio).ToListAsync();

            if (edadFiltro.HasValue)
            {
                mascotas = mascotas.Where(m => m.Edad == edadFiltro.Value).ToList();
            }

            mascotas = mascotas.OrderBy(m => m.EstadoAdopcion == "Adoptado").ToList();

            return View("VistaMascota", mascotas);
        }

        // GET: Mascotas/DetailsMascota/5
        public async Task<IActionResult> DetailsMascota(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mascota = await _context.Mascotas.Include(m => m.Refugio).FirstOrDefaultAsync(m => m.MascotaId == id);
            if (mascota == null)
            {
                return NotFound();
            }

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Nombre");

            return View("DetailsMascota", mascota);
        }

        // GET: Mascotas/SolicitarAdopcion
        public async Task<IActionResult> SolicitarAdopcion(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null || mascota.EstadoAdopcion != "Disponible")
            {
                return NotFound("La mascota no está disponible para adopción.");
            }

            var adopcion = new Adopcione
            {
                MascotaId = id,
                FechaSolicitud = DateTime.Now
            };

            return View("FormularioAdopcion", adopcion);
        }

        // POST: Mascotas/SolicitarAdopcion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarAdopcion(Adopcione adopcion)
        {
            if (ModelState.IsValid)
            {
                return View("FormularioAdopcion", adopcion);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            adopcion.UsuarioId = userId;
            adopcion.Estado = "Pendiente";
            adopcion.FechaSolicitud = DateTime.Now;

            _context.Adopciones.Add(adopcion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ContratoAdopcion), new { mascotaId = adopcion.MascotaId });
        }

        // GET: Mascotas/ContratoAdopcion
        public async Task<IActionResult> ContratoAdopcion(int mascotaId)
        {
            var adopcion = await _context.Adopciones
                .Include(a => a.Mascota)
                .FirstOrDefaultAsync(a => a.MascotaId == mascotaId && a.Estado == "Pendiente");

            if (adopcion == null)
            {
                return NotFound("No se encontró la solicitud de adopción.");
            }

            return View("ContratoAdopcion", adopcion);
        }

        // POST: Mascotas/ConfirmarAdopcion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarAdopcion(int mascotaId)
        {
            var adopcion = await _context.Adopciones
                .FirstOrDefaultAsync(a => a.MascotaId == mascotaId && a.Estado == "Pendiente");

            if (adopcion == null)
            {
                return NotFound("No se encontró la solicitud de adopción.");
            }

            adopcion.Estado = "En Proceso";

            var mascota = await _context.Mascotas.FindAsync(mascotaId);
            if (mascota != null)
            {
                mascota.EstadoAdopcion = "En Proceso";
                _context.Mascotas.Update(mascota);
            }

            _context.Adopciones.Update(adopcion);
            await _context.SaveChangesAsync();

            return RedirectToAction("VistaMascota", "Mascotas");
        }

    }
}