using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class addContractHistorical : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublishingOrders_ApplicationUsers_SellerId",
                table: "PublishingOrders");

            migrationBuilder.AlterColumn<long>(
                name: "SellerId",
                table: "PublishingOrders",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ContractHistoricals",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Deleted = table.Column<bool>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    DeletedUser = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    User = table.Column<string>(nullable: false),
                    Changes = table.Column<string>(nullable: false),
                    ContractId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractHistoricals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractHistoricals_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractHistoricals_ContractId",
                table: "ContractHistoricals",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_PublishingOrders_ApplicationUsers_SellerId",
                table: "PublishingOrders",
                column: "SellerId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublishingOrders_ApplicationUsers_SellerId",
                table: "PublishingOrders");

            migrationBuilder.DropTable(
                name: "ContractHistoricals");

            migrationBuilder.AlterColumn<long>(
                name: "SellerId",
                table: "PublishingOrders",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_PublishingOrders_ApplicationUsers_SellerId",
                table: "PublishingOrders",
                column: "SellerId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
