using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetApp_Empresa.Migrations
{
    /// <inheritdoc />
    public partial class juan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__F92302F12633B530", x => x.RolId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuarios__2B3DE7B8FB44ABF2", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "Accesorios",
                columns: table => new
                {
                    AccesorioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    VendedorId = table.Column<int>(type: "int", nullable: true),
                    CantidadDisponible = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Accesori__4BCD4E498BF95DEF", x => x.AccesorioId);
                    table.ForeignKey(
                        name: "FK__Accesorio__Vende__4E88ABD4",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CarritoDeCompras",
                columns: table => new
                {
                    CarritoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CarritoD__778D586BBD48FB94", x => x.CarritoId);
                    table.ForeignKey(
                        name: "FK__CarritoDe__Usuar__52593CB8",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Refugios",
                columns: table => new
                {
                    RefugioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Refugios__AD4F9CA2861619D7", x => x.RefugioId);
                    table.ForeignKey(
                        name: "FK_Refugios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateTable(
                name: "Tarjetas",
                columns: table => new
                {
                    TarjetaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    FechaVencimiento = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CVV = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarjetas", x => x.TarjetaId);
                    table.ForeignKey(
                        name: "FK_Tarjetas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRol",
                columns: table => new
                {
                    UsuarioRolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UsuarioR__C869CDCA96DA8FAB", x => x.UsuarioRolId);
                    table.ForeignKey(
                        name: "FK__UsuarioRo__RolId__412EB0B6",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__UsuarioRo__Usuar__403A8C7D",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarritoAccesorios",
                columns: table => new
                {
                    CarritoAccesorioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarritoId = table.Column<int>(type: "int", nullable: false),
                    AccesorioId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CarritoA__ABF0DCE15F7E6066", x => x.CarritoAccesorioId);
                    table.ForeignKey(
                        name: "FK__CarritoAc__Acces__5629CD9C",
                        column: x => x.AccesorioId,
                        principalTable: "Accesorios",
                        principalColumn: "AccesorioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__CarritoAc__Carri__5535A963",
                        column: x => x.CarritoId,
                        principalTable: "CarritoDeCompras",
                        principalColumn: "CarritoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Donaciones",
                columns: table => new
                {
                    DonacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefugioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FechaDonacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Donacion__9F5DEE875969A9BC", x => x.DonacionId);
                    table.ForeignKey(
                        name: "FK__Donacione__Refug__59FA5E80",
                        column: x => x.RefugioId,
                        principalTable: "Refugios",
                        principalColumn: "RefugioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Donacione__Usuar__5AEE82B9",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mascotas",
                columns: table => new
                {
                    MascotaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Raza = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sexo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Edad = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EstadoAdopcion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Disponible"),
                    RefugioId = table.Column<int>(type: "int", nullable: true),
                    ImagenUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Mascotas__8DBC413C68F2B033", x => x.MascotaId);
                    table.ForeignKey(
                        name: "FK__Mascotas__Refugi__46E78A0C",
                        column: x => x.RefugioId,
                        principalTable: "Refugios",
                        principalColumn: "RefugioId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    CompraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TarjetaId = table.Column<int>(type: "int", nullable: true),
                    FechaCompra = table.Column<DateTime>(type: "datetime", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BancoDestino = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroTransaccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Validado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.CompraId);
                    table.ForeignKey(
                        name: "FK_Compras_Tarjetas_TarjetaId",
                        column: x => x.TarjetaId,
                        principalTable: "Tarjetas",
                        principalColumn: "TarjetaId");
                    table.ForeignKey(
                        name: "FK_Compras_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateTable(
                name: "Adopciones",
                columns: table => new
                {
                    AdopcionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MascotaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Adopcion__AAEE3F47EBDDAE63", x => x.AdopcionId);
                    table.ForeignKey(
                        name: "FK__Adopcione__Masco__4AB81AF0",
                        column: x => x.MascotaId,
                        principalTable: "Mascotas",
                        principalColumn: "MascotaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Adopcione__Usuar__4BAC3F29",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesCompra",
                columns: table => new
                {
                    DetalleCompraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    AccesorioId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesCompra", x => x.DetalleCompraId);
                    table.ForeignKey(
                        name: "FK_DetallesCompra_Accesorios_AccesorioId",
                        column: x => x.AccesorioId,
                        principalTable: "Accesorios",
                        principalColumn: "AccesorioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesCompra_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "CompraId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accesorios_VendedorId",
                table: "Accesorios",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Adopciones_MascotaId",
                table: "Adopciones",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Adopciones_UsuarioId",
                table: "Adopciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoAccesorios_AccesorioId",
                table: "CarritoAccesorios",
                column: "AccesorioId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoAccesorios_CarritoId",
                table: "CarritoAccesorios",
                column: "CarritoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoDeCompras_UsuarioId",
                table: "CarritoDeCompras",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_TarjetaId",
                table: "Compras",
                column: "TarjetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_UsuarioId",
                table: "Compras",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompra_AccesorioId",
                table: "DetallesCompra",
                column: "AccesorioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompra_CompraId",
                table: "DetallesCompra",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Donaciones_RefugioId",
                table: "Donaciones",
                column: "RefugioId");

            migrationBuilder.CreateIndex(
                name: "IX_Donaciones_UsuarioId",
                table: "Donaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_RefugioId",
                table: "Mascotas",
                column: "RefugioId");

            migrationBuilder.CreateIndex(
                name: "IX_Refugios_UsuarioId",
                table: "Refugios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarjetas_UsuarioId",
                table: "Tarjetas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_RolId",
                table: "UsuarioRol",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "UQ_Usuario_Rol",
                table: "UsuarioRol",
                columns: new[] { "UsuarioId", "RolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Usuarios__A9D105345CB187E6",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adopciones");

            migrationBuilder.DropTable(
                name: "CarritoAccesorios");

            migrationBuilder.DropTable(
                name: "DetallesCompra");

            migrationBuilder.DropTable(
                name: "Donaciones");

            migrationBuilder.DropTable(
                name: "UsuarioRol");

            migrationBuilder.DropTable(
                name: "Mascotas");

            migrationBuilder.DropTable(
                name: "CarritoDeCompras");

            migrationBuilder.DropTable(
                name: "Accesorios");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Refugios");

            migrationBuilder.DropTable(
                name: "Tarjetas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
