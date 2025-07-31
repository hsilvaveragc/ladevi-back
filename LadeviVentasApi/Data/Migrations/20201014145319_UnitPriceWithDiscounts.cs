using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class UnitPriceWithDiscounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "UnitPriceWithDiscounts",
                table: "SoldSpaces",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitPriceWithDiscounts",
                table: "SoldSpaces");
        }
    }
}
