using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class changeClientTaxesToClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientTaxes_Clients_ClientId",
                table: "ClientTaxes");

            migrationBuilder.DropIndex(
                name: "IX_ClientTaxes_ClientId",
                table: "ClientTaxes");

            migrationBuilder.AlterColumn<double>(
                name: "CurrencyParity",
                table: "Contracts",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<long>(
                name: "ClientTaxesId",
                table: "Clients",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "IdentificationTypeId",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificationValue",
                table: "Clients",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "TaxPercentage",
                table: "Clients",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_IdentificationTypeId",
                table: "Clients",
                column: "IdentificationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_ClientTaxes_IdentificationTypeId",
                table: "Clients",
                column: "IdentificationTypeId",
                principalTable: "ClientTaxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_ClientTaxes_IdentificationTypeId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_IdentificationTypeId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ClientTaxesId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IdentificationTypeId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IdentificationValue",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "Clients");

            migrationBuilder.AlterColumn<double>(
                name: "CurrencyParity",
                table: "Contracts",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientTaxes_ClientId",
                table: "ClientTaxes",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientTaxes_Clients_ClientId",
                table: "ClientTaxes",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
