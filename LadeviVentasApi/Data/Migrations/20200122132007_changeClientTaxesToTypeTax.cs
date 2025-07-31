using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class changeClientTaxesToTypeTax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_ClientTaxes_IdentificationTypeId",
                table: "Clients");

            /*migrationBuilder.DropTable(
                name: "ClientTaxes"); */

            migrationBuilder.DropIndex(
                name: "IX_Clients_IdentificationTypeId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IdentificationTypeId",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "ClientTaxesId",
                table: "Clients",
                newName: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_TaxTypeId",
                table: "Clients",
                column: "TaxTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_TaxType_TaxTypeId",
                table: "Clients",
                column: "TaxTypeId",
                principalTable: "TaxType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_TaxType_TaxTypeId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_TaxTypeId",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "TaxTypeId",
                table: "Clients",
                newName: "ClientTaxesId");

            migrationBuilder.AddColumn<long>(
                name: "IdentificationTypeId",
                table: "Clients",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientTaxes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientId = table.Column<long>(nullable: false),
                    TaxTypeId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientTaxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientTaxes_TaxType_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_IdentificationTypeId",
                table: "Clients",
                column: "IdentificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientTaxes_TaxTypeId",
                table: "ClientTaxes",
                column: "TaxTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_ClientTaxes_IdentificationTypeId",
                table: "Clients",
                column: "IdentificationTypeId",
                principalTable: "ClientTaxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
