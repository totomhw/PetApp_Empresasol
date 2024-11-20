using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PetApp_Empresa.Models;

public partial class PettappPruebaContext : DbContext
{
    public PettappPruebaContext()
    {
    }

    public PettappPruebaContext(DbContextOptions<PettappPruebaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Accesorio> Accesorios { get; set; }
    public virtual DbSet<Adopcione> Adopciones { get; set; }
    public virtual DbSet<CarritoAccesorio> CarritoAccesorios { get; set; }
    public virtual DbSet<CarritoDeCompra> CarritoDeCompras { get; set; }
    public virtual DbSet<Donacione> Donaciones { get; set; }
    public virtual DbSet<Mascota> Mascotas { get; set; }
    public virtual DbSet<Refugio> Refugios { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<UsuarioRol> UsuarioRols { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=SERG\\SQLEXPRESS;Database=PettappPrueba;Trusted_connection=true;TrustServerCertificate=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Accesorio>(entity =>
        {
            entity.HasKey(e => e.AccesorioId).HasName("PK__Accesori__4BCD4E498BF95DEF");

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            // Configuración para la propiedad CantidadDisponible con valor predeterminado de 0
            entity.Property(e => e.CantidadDisponible)
                  .HasDefaultValue(0);

            entity.HasOne(d => d.Vendedor).WithMany(p => p.Accesorios)
                .HasForeignKey(d => d.VendedorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Accesorio__Vende__4E88ABD4");
        });

        modelBuilder.Entity<Adopcione>(entity =>
        {
            entity.HasKey(e => e.AdopcionId).HasName("PK__Adopcion__AAEE3F47EBDDAE63");

            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.FechaAprobacion)
                .HasColumnType("datetime");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente");

            entity.Property(e => e.NombreCompleto).HasMaxLength(100);
            entity.Property(e => e.CorreoElectronico).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Direccion).HasMaxLength(255);

            entity.HasOne(d => d.Mascota).WithMany(p => p.Adopciones)
                .HasForeignKey(d => d.MascotaId)
                .HasConstraintName("FK__Adopcione__Masco__4AB81AF0");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Adopciones)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Adopcione__Usuar__4BAC3F29");
        });

        modelBuilder.Entity<CarritoAccesorio>(entity =>
        {
            entity.HasKey(e => e.CarritoAccesorioId).HasName("PK__CarritoA__ABF0DCE15F7E6066");

            entity.Property(e => e.Cantidad)
                  .HasDefaultValue(1);  // Puedes establecer un valor predeterminado de 1 para la cantidad

            entity.HasOne(d => d.Accesorio).WithMany(p => p.CarritoAccesorios)
                .HasForeignKey(d => d.AccesorioId)
                .HasConstraintName("FK__CarritoAc__Acces__5629CD9C");

            entity.HasOne(d => d.Carrito).WithMany(p => p.CarritoAccesorios)
                .HasForeignKey(d => d.CarritoId)
                .HasConstraintName("FK__CarritoAc__Carri__5535A963");
        });


        modelBuilder.Entity<CarritoDeCompra>(entity =>
        {
            entity.HasKey(e => e.CarritoId).HasName("PK__CarritoD__778D586BBD48FB94");

            entity.Property(e => e.Total)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Usuario).WithMany(p => p.CarritoDeCompras)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__CarritoDe__Usuar__52593CB8");
        });

        modelBuilder.Entity<Donacione>(entity =>
        {
            entity.HasKey(e => e.DonacionId).HasName("PK__Donacion__9F5DEE875969A9BC");

            entity.Property(e => e.FechaDonacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Refugio).WithMany(p => p.Donaciones)
                .HasForeignKey(d => d.RefugioId)
                .HasConstraintName("FK__Donacione__Refug__59FA5E80");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Donaciones)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Donacione__Usuar__5AEE82B9");
        });

        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.HasKey(e => e.MascotaId).HasName("PK__Mascotas__8DBC413C68F2B033");

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.EstadoAdopcion)
                .HasMaxLength(20)
                .HasDefaultValue("Disponible");
            entity.Property(e => e.ImagenUrl).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Raza).HasMaxLength(50);
            entity.Property(e => e.Sexo).HasMaxLength(10);
            entity.Property(e => e.Edad).HasDefaultValue(0);

            entity.HasOne(d => d.Refugio).WithMany(p => p.Mascota)
                .HasForeignKey(d => d.RefugioId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Mascotas__Refugi__46E78A0C");
        });

        modelBuilder.Entity<Refugio>(entity =>
        {
            entity.HasKey(e => e.RefugioId).HasName("PK__Refugios__AD4F9CA2861619D7");

            entity.Property(e => e.Direccion).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);

            entity.HasOne(r => r.Usuario)
                .WithMany(u => u.Refugios)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Roles__F92302F12633B530");

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE7B8FB44ABF2");

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D105345CB187E6").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);

            entity.HasMany(e => e.Refugios).WithOne(r => r.Usuario)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            entity.HasKey(e => e.UsuarioRolId).HasName("PK__UsuarioR__C869CDCA96DA8FAB");

            entity.ToTable("UsuarioRol");

            entity.HasIndex(e => new { e.UsuarioId, e.RolId }, "UQ_Usuario_Rol").IsUnique();

            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Rol).WithMany(p => p.UsuarioRols)
                .HasForeignKey(d => d.RolId)
                .HasConstraintName("FK__UsuarioRo__RolId__412EB0B6");

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioRols)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__UsuarioRo__Usuar__403A8C7D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
