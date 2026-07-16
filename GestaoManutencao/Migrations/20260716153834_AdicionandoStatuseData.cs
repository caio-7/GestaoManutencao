using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoManutencao.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoStatuseData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OrdensDeServico",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrdensDeServico");
        }
    }
}
