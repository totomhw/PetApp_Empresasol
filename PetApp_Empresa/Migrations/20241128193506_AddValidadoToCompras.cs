using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetApp_Empresa.Migrations
{
    /// <inheritdoc />
    public partial class AddValidadoToCompras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Validado",
                table: "Compras",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Validado",
                table: "Compras");
        }
    }
}
