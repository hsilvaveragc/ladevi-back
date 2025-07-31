using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddDiscountByVolumeSoldSpace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ApplyDiscountForVolume",
                table: "SoldSpaces",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForVolume",
                table: "SoldSpaces",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyDiscountForVolume",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "DiscountForVolume",
                table: "SoldSpaces");
        }
    }
}
