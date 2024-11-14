using PetApp_Empresa.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar la cadena de conexi�n desde el archivo appsettings.json
builder.Services.AddDbContext<PettappPruebaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar servicios para controladores y vistas
builder.Services.AddControllersWithViews();



// Configurar la autenticaci�n mediante cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Ruta al login
        options.LogoutPath = "/Auth/Logout"; // Ruta al logout
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // La sesi�n expira despu�s de 20 minutos
        options.SlidingExpiration = true; // Renovaci�n autom�tica de la cookie si hay actividad
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// Configurar las pol�ticas de autorizaci�n seg�n roles
builder.Services.AddAuthorization(options =>
{
    // Pol�tica para los usuarios con rol Admin
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));

    // Pol�tica para los usuarios con rol Vendedor
    options.AddPolicy("RequireVendedorRole", policy => policy.RequireRole("Vendedor"));

    // Pol�tica para los usuarios con rol Refugio
    options.AddPolicy("RequireRefugioRole", policy => policy.RequireRole("Refugio"));

    // Pol�tica para los usuarios con rol Cliente
    options.AddPolicy("RequireClienteRole", policy => policy.RequireRole("Cliente"));
});

var app = builder.Build();

// Configurar el pipeline de solicitud HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Aplicar HSTS en producci�n
}
else
{
    // Mostrar detalles de los errores en modo desarrollo
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Activar autenticaci�n y autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

// Configurar las rutas de la aplicaci�n
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// Ejecutar la aplicaci�n
app.Run();