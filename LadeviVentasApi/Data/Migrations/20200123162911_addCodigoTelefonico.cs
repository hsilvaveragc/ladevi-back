using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class addCodigoTelefonico : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoTelefonico",
                table: "Country",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BillingPointOfSale",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 30);

            migrationBuilder.AddColumn<string>(
                name: "CodigoTelefonico",
                table: "City",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoTelefonico",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "CodigoTelefonico",
                table: "City");

            migrationBuilder.AlterColumn<string>(
                name: "BillingPointOfSale",
                table: "Clients",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
