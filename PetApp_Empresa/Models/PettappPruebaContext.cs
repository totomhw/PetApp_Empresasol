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
    public virtual DbSet<Compra> Compras { get; set; }
    public virtual DbSet<DetalleCompra> DetallesCompra { get; set; }
    public virtual DbSet<Tarjeta> Tarjetas { get; set; }
    public virtual DbSet<QR> QRs { get; set; }
    public virtual DbSet<TransaccionBancaria> TransaccionesBancarias { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=SERG\\SQLEXPRESS;Database=PettappPrueba;Trusted_connection=true;TrustServerCertificate=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransaccionBancaria>(entity =>
        {
            entity.HasKey(e => e.TransaccionBancariaId);

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Apellido)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.NumeroTransaccion)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.BancoOrigen)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.ComprobantePath)
                .HasMaxLength(255);

            entity.Property(e => e.FechaTransaccion)
                .HasColumnType("datetime")
                .IsRequired();

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.TransaccionesBancarias)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.CompraId);

            entity.Property(e => e.FechaCompra)
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .IsRequired();

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.Compras)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Tarjeta)
                .WithMany(p => p.Compras)
                .HasForeignKey(d => d.TarjetaId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.HasKey(e => e.DetalleCompraId);

            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)");

            entity.HasOne(e => e.Compra)
                .WithMany(c => c.DetallesCompra)
                .HasForeignKey(e => e.CompraId);

            entity.HasOne(e => e.Accesorio)
                .WithMany(a => a.DetallesCompra)
                .HasForeignKey(e => e.AccesorioId);
        });

        modelBuilder.Entity<Tarjeta>(entity =>
        {
            entity.HasKey(e => e.TarjetaId);

            entity.Property(e => e.Numero)
                .IsRequired()
                .HasMaxLength(16);

            entity.Property(e => e.FechaVencimiento)
                .IsRequired()
                .HasMaxLength(5);

            entity.Property(e => e.CVV)
                .IsRequired()
                .HasMaxLength(3);

            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Tarjetas)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Accesorio>(entity =>
        {
            entity.HasKey(e => e.AccesorioId);

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.Property(e => e.CantidadDisponible)
                .HasDefaultValue(0);

            entity.HasOne(d => d.Vendedor)
                .WithMany(p => p.Accesorios)
                .HasForeignKey(d => d.VendedorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Adopcione>(entity =>
        {
            entity.HasKey(e => e.AdopcionId);

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
                .HasForeignKey(d => d.MascotaId);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Adopciones)
                .HasForeignKey(d => d.UsuarioId);
        });

        modelBuilder.Entity<CarritoAccesorio>(entity =>
        {
            entity.HasKey(e => e.CarritoAccesorioId);

            entity.Property(e => e.Cantidad)
                  .HasDefaultValue(1);

            entity.HasOne(d => d.Accesorio).WithMany(p => p.CarritoAccesorios)
                .HasForeignKey(d => d.AccesorioId);

            entity.HasOne(d => d.Carrito).WithMany(p => p.CarritoAccesorios)
                .HasForeignKey(d => d.CarritoId);
        });

        modelBuilder.Entity<CarritoDeCompra>(entity =>
        {
            entity.HasKey(e => e.CarritoId);

            entity.Property(e => e.Total)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Usuario).WithMany(p => p.CarritoDeCompras)
                .HasForeignKey(d => d.UsuarioId);
        });

        modelBuilder.Entity<Donacione>(entity =>
        {
            entity.HasKey(e => e.DonacionId);

            entity.Property(e => e.FechaDonacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Refugio).WithMany(p => p.Donaciones)
                .HasForeignKey(d => d.RefugioId);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Donaciones)
                .HasForeignKey(d => d.UsuarioId);
        });

        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.HasKey(e => e.MascotaId);

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
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Refugio>(entity =>
        {
            entity.HasKey(e => e.RefugioId);

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
            entity.HasKey(e => e.RolId);

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId);

            entity.HasIndex(e => e.Email).IsUnique();

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
            entity.HasKey(e => e.UsuarioRolId);

            entity.ToTable("UsuarioRol");

            entity.HasIndex(e => new { e.UsuarioId, e.RolId }).IsUnique();

            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Rol).WithMany(p => p.UsuarioRols)
                .HasForeignKey(d => d.RolId);

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioRols)
                .HasForeignKey(d => d.UsuarioId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
