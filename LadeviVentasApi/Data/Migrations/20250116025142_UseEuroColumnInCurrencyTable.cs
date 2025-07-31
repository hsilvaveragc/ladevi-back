using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class UseEuroColumnInCurrencyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseEruro",
                table: "Currency",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseEruro",
                table: "Currency");
        }
    }
}
