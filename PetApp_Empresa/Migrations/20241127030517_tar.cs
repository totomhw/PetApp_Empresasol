using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetApp_Empresa.Migrations
{
    /// <inheritdoc />
    public partial class tar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsVisible",
                table: "Tarjetas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsVisible",
                table: "Tarjetas");
        }
    }
}
