using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class UseEuroColumnRenameInCurrencyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UseEruro",
                table: "Currency",
                newName: "UseEuro");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Currency",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UseEuro",
                table: "Currency",
                newName: "UseEruro");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Currency",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
