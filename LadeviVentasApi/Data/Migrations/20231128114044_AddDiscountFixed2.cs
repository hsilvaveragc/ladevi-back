using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddDiscountFixed2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ApplyDiscountForCheck",
                table: "SoldSpaces",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyDiscountForLoyalty",
                table: "SoldSpaces",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyDiscountForOtherCountry",
                table: "SoldSpaces",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyDiscountForSameCountry",
                table: "SoldSpaces",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AppyDiscountForAgency",
                table: "SoldSpaces",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForAgency",
                table: "SoldSpaces",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForCheck",
                table: "SoldSpaces",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForLoyalty",
                table: "SoldSpaces",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForOtherCountry",
                table: "SoldSpaces",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForSameCountry",
                table: "SoldSpaces",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyDiscountForCheck",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "ApplyDiscountForLoyalty",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "ApplyDiscountForOtherCountry",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "ApplyDiscountForSameCountry",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "AppyDiscountForAgency",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "DiscountForAgency",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "DiscountForCheck",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "DiscountForLoyalty",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "DiscountForOtherCountry",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "DiscountForSameCountry",
                table: "SoldSpaces");
        }
    }
}
