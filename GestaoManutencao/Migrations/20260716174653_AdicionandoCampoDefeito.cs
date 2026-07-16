using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoManutencao.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoCampoDefeito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Defeito",
                table: "OrdensDeServico",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Defeito",
                table: "OrdensDeServico");
        }
    }
}
