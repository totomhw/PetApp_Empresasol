﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PetApp_Empresa.Models;

#nullable disable

namespace PetApp_Empresa.Migrations
{
    [DbContext(typeof(PettappPruebaContext))]
    partial class PettappPruebaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PetApp_Empresa.Models.Accesorio", b =>
                {
                    b.Property<int>("AccesorioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AccesorioId"));

                    b.Property<int>("CantidadDisponible")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Descripcion")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("Precio")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int?>("VendedorId")
                        .HasColumnType("int");

                    b.HasKey("AccesorioId")
                        .HasName("PK__Accesori__4BCD4E498BF95DEF");

                    b.HasIndex("VendedorId");

                    b.ToTable("Accesorios");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Adopcione", b =>
                {
                    b.Property<int>("AdopcionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AdopcionId"));

                    b.Property<string>("CorreoElectronico")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasDefaultValue("Pendiente");

                    b.Property<DateTime?>("FechaAprobacion")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("FechaSolicitud")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("MascotaId")
                        .HasColumnType("int");

                    b.Property<string>("NombreCompleto")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("AdopcionId")
                        .HasName("PK__Adopcion__AAEE3F47EBDDAE63");

                    b.HasIndex("MascotaId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Adopciones");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.CarritoAccesorio", b =>
                {
                    b.Property<int>("CarritoAccesorioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CarritoAccesorioId"));

                    b.Property<int>("AccesorioId")
                        .HasColumnType("int");

                    b.Property<int>("Cantidad")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<int>("CarritoId")
                        .HasColumnType("int");

                    b.HasKey("CarritoAccesorioId")
                        .HasName("PK__CarritoA__ABF0DCE15F7E6066");

                    b.HasIndex("AccesorioId");

                    b.HasIndex("CarritoId");

                    b.ToTable("CarritoAccesorios");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.CarritoDeCompra", b =>
                {
                    b.Property<int>("CarritoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CarritoId"));

                    b.Property<decimal?>("Total")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10, 2)")
                        .HasDefaultValue(0m);

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("CarritoId")
                        .HasName("PK__CarritoD__778D586BBD48FB94");

                    b.HasIndex("UsuarioId");

                    b.ToTable("CarritoDeCompras");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Donacione", b =>
                {
                    b.Property<int>("DonacionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DonacionId"));

                    b.Property<DateTime?>("FechaDonacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<decimal>("Monto")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("RefugioId")
                        .HasColumnType("int");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("DonacionId")
                        .HasName("PK__Donacion__9F5DEE875969A9BC");

                    b.HasIndex("RefugioId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Donaciones");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Mascota", b =>
                {
                    b.Property<int>("MascotaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MascotaId"));

                    b.Property<string>("Descripcion")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("Edad")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("EstadoAdopcion")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasDefaultValue("Disponible");

                    b.Property<string>("ImagenUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Raza")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("RefugioId")
                        .HasColumnType("int");

                    b.Property<string>("Sexo")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("MascotaId")
                        .HasName("PK__Mascotas__8DBC413C68F2B033");

                    b.HasIndex("RefugioId");

                    b.ToTable("Mascotas");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Refugio", b =>
                {
                    b.Property<int>("RefugioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RefugioId"));

                    b.Property<string>("Direccion")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Telefono")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int?>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("RefugioId")
                        .HasName("PK__Refugios__AD4F9CA2861619D7");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Refugios");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Role", b =>
                {
                    b.Property<int>("RolId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RolId"));

                    b.Property<string>("Descripcion")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("RolId")
                        .HasName("PK__Roles__F92302F12633B530");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Usuario", b =>
                {
                    b.Property<int>("UsuarioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UsuarioId"));

                    b.Property<bool?>("Activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("FechaRegistro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("UsuarioId")
                        .HasName("PK__Usuarios__2B3DE7B8FB44ABF2");

                    b.HasIndex(new[] { "Email" }, "UQ__Usuarios__A9D105345CB187E6")
                        .IsUnique();

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.UsuarioRol", b =>
                {
                    b.Property<int>("UsuarioRolId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UsuarioRolId"));

                    b.Property<DateTime?>("FechaAsignacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("RolId")
                        .HasColumnType("int");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("UsuarioRolId")
                        .HasName("PK__UsuarioR__C869CDCA96DA8FAB");

                    b.HasIndex("RolId");

                    b.HasIndex(new[] { "UsuarioId", "RolId" }, "UQ_Usuario_Rol")
                        .IsUnique();

                    b.ToTable("UsuarioRol", (string)null);
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Accesorio", b =>
                {
                    b.HasOne("PetApp_Empresa.Models.Usuario", "Vendedor")
                        .WithMany("Accesorios")
                        .HasForeignKey("VendedorId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK__Accesorio__Vende__4E88ABD4");

                    b.Navigation("Vendedor");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Adopcione", b =>
                {
                    b.HasOne("PetApp_Empresa.Models.Mascota", "Mascota")
                        .WithMany("Adopciones")
                        .HasForeignKey("MascotaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Adopcione__Masco__4AB81AF0");

                    b.HasOne("PetApp_Empresa.Models.Usuario", "Usuario")
                        .WithMany("Adopciones")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Adopcione__Usuar__4BAC3F29");

                    b.Navigation("Mascota");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.CarritoAccesorio", b =>
                {
                    b.HasOne("PetApp_Empresa.Models.Accesorio", "Accesorio")
                        .WithMany("CarritoAccesorios")
                        .HasForeignKey("AccesorioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__CarritoAc__Acces__5629CD9C");

                    b.HasOne("PetApp_Empresa.Models.CarritoDeCompra", "Carrito")
                        .WithMany("CarritoAccesorios")
                        .HasForeignKey("CarritoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__CarritoAc__Carri__5535A963");

                    b.Navigation("Accesorio");

                    b.Navigation("Carrito");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.CarritoDeCompra", b =>
                {
                    b.HasOne("PetApp_Empresa.Models.Usuario", "Usuario")
                        .WithMany("CarritoDeCompras")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__CarritoDe__Usuar__52593CB8");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Donacione", b =>
                {
                    b.HasOne("PetApp_Empresa.Models.Refugio", "Refugio")
                        .WithMany("Donaciones")
                        .HasForeignKey("RefugioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Donacione__Refug__59FA5E80");

                    b.HasOne("PetApp_Empresa.Models.Usuario", "Usuario")
                        .WithMany("Donaciones")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Donacione__Usuar__5AEE82B9");

                    b.Navigation("Refugio");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Mascota", b =>
                {
                    b.HasOne("PetApp_Empresa.Models.Refugio", "Refugio")
                        .WithMany("Mascota")
                        .HasForeignKey("RefugioId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK__Mascotas__Refugi__46E78A0C");

                    b.Navigation("Refugio");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Refugio", b =>
                {
                    b.HasOne("PetApp_Empresa.Models.Usuario", "Usuario")
                        .WithMany("Refugios")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.UsuarioRol", b =>
                {
                    b.HasOne("PetApp_Empresa.Models.Role", "Rol")
                        .WithMany("UsuarioRols")
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__UsuarioRo__RolId__412EB0B6");

                    b.HasOne("PetApp_Empresa.Models.Usuario", "Usuario")
                        .WithMany("UsuarioRols")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__UsuarioRo__Usuar__403A8C7D");

                    b.Navigation("Rol");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Accesorio", b =>
                {
                    b.Navigation("CarritoAccesorios");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.CarritoDeCompra", b =>
                {
                    b.Navigation("CarritoAccesorios");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Mascota", b =>
                {
                    b.Navigation("Adopciones");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Refugio", b =>
                {
                    b.Navigation("Donaciones");

                    b.Navigation("Mascota");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Role", b =>
                {
                    b.Navigation("UsuarioRols");
                });

            modelBuilder.Entity("PetApp_Empresa.Models.Usuario", b =>
                {
                    b.Navigation("Accesorios");

                    b.Navigation("Adopciones");

                    b.Navigation("CarritoDeCompras");

                    b.Navigation("Donaciones");

                    b.Navigation("Refugios");

                    b.Navigation("UsuarioRols");
                });
#pragma warning restore 612, 618
        }
    }
}
