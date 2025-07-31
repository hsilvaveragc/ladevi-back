using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddDiscountFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountForAgency",
                table: "ProductAdvertisingSpaces",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForCheck",
                table: "ProductAdvertisingSpaces",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForLoyalty",
                table: "ProductAdvertisingSpaces",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForOtherCountry",
                table: "ProductAdvertisingSpaces",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForSameCountry",
                table: "ProductAdvertisingSpaces",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountForAgency",
                table: "ProductAdvertisingSpaces");

            migrationBuilder.DropColumn(
                name: "DiscountForCheck",
                table: "ProductAdvertisingSpaces");

            migrationBuilder.DropColumn(
                name: "DiscountForLoyalty",
                table: "ProductAdvertisingSpaces");

            migrationBuilder.DropColumn(
                name: "DiscountForOtherCountry",
                table: "ProductAdvertisingSpaces");

            migrationBuilder.DropColumn(
                name: "DiscountForSameCountry",
                table: "ProductAdvertisingSpaces");
        }
    }
}
