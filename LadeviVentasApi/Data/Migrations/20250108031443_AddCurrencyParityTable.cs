using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddCurrencyParityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCurrencyParity_Products_ProductId",
                table: "ProductCurrencyParity");

            migrationBuilder.DropIndex(
                name: "IX_ProductCurrencyParity_ProductId",
                table: "ProductCurrencyParity");

            migrationBuilder.CreateTable(
                name: "CurrencyParities",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Deleted = table.Column<bool>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    DeletedUser = table.Column<string>(nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    LocalCurrencyToDollarExchangeRate = table.Column<double>(nullable: false),
                    CurrencyId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyParities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyParities_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyParities_CurrencyId",
                table: "CurrencyParities",
                column: "CurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyParities");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCurrencyParity_ProductId",
                table: "ProductCurrencyParity",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCurrencyParity_Products_ProductId",
                table: "ProductCurrencyParity",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
