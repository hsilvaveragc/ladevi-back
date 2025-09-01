using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToContractNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar constraint unique al campo Number
            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Number",
                table: "Contracts",
                column: "Number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Quitar el constraint unique
            migrationBuilder.DropIndex(
                name: "IX_Contracts_Number",
                table: "Contracts");
        }
    }
}
