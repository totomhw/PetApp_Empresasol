using PetApp_Empresa.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PettappPruebaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Middleware personalizado para proteger rutas
app.Use(async (context, next) =>
{
    // Si el usuario no está autenticado
    if (!context.User.Identity?.IsAuthenticated ?? false)
    {
        var path = context.Request.Path.Value?.ToLower();

        // Permitir acceso a ciertas rutas públicas como DashboardCliente o Login
        if (path != null && !(
            path.StartsWith("/auth") ||
            path.StartsWith("/home/dashboardcliente") ||
            path.Equals("/")
        ))
        {
            // Redirigir al login si intenta interactuar con una ruta protegida
            context.Response.Redirect($"/Auth/Login?returnUrl={context.Request.Path}");
            return;
        }
    }

    await next(); // Continuar con la siguiente acción si no es una ruta protegida
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=DashboardCliente}/{id?}");

app.Run();
