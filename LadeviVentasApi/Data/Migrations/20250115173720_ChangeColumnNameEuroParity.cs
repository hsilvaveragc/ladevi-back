using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class ChangeColumnNameEuroParity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocalCurrencyToDollarExchangeRate",
                table: "EuroParities",
                newName: "EuroToDollarExchangeRate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EuroToDollarExchangeRate",
                table: "EuroParities",
                newName: "LocalCurrencyToDollarExchangeRate");
        }
    }
}
