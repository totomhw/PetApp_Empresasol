using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetApp_Empresa.Migrations
{
    /// <inheritdoc />
    public partial class crrito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cantidad",
                table: "CarritoAccesorios",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CantidadDisponible",
                table: "Accesorios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cantidad",
                table: "CarritoAccesorios");

            migrationBuilder.DropColumn(
                name: "CantidadDisponible",
                table: "Accesorios");
        }
    }
}
