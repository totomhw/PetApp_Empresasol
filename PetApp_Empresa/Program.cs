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

// Middleware para redirigir al dashboard adecuado según el rol del usuario
app.Use(async (context, next) =>
{
    // Verificar si el usuario está autenticado
    if (context.User.Identity?.IsAuthenticated ?? false)
    {
        var path = context.Request.Path.Value?.ToLower();

        // Evitar redirecciones repetitivas desde cualquier Dashboard
        if (path != null &&
            (path.Equals("/") || path.StartsWith("/home/dashboard")))
        {
            var roles = context.User.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (roles.Contains("Admin") && !path.EndsWith("dashboardadmin"))
            {
                context.Response.Redirect("/Home/DashboardAdmin");
                return;
            }
            if (roles.Contains("Refugio") && !path.EndsWith("dashboardrefugio"))
            {
                context.Response.Redirect("/Home/DashboardRefugio");
                return;
            }
            if (roles.Contains("Vendedor") && !path.EndsWith("dashboardvendedor"))
            {
                context.Response.Redirect("/Home/DashboardVendedor");
                return;
            }
            if (roles.Contains("Cliente") && !path.EndsWith("dashboardcliente"))
            {
                context.Response.Redirect("/Home/DashboardCliente");
                return;
            }
        }
    }

    await next(); // Continuar con la siguiente acción
});
app.UseStaticFiles();
// Configurar la ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=DashboardCliente}/{id?}");

app.Run();
