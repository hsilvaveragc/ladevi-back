using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class Cambioscontratosyordenesdepublicacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublishingOrders_Contracts_ContractId",
                table: "PublishingOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PublishingOrders_SoldSpaceLocations_SoldSpaceLocationId",
                table: "PublishingOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SoldSpaces_SoldSpaceLocations_SoldSpaceLocationId",
                table: "SoldSpaces");

            migrationBuilder.DropTable(
                name: "SoldSpaceLocations");

            migrationBuilder.RenameColumn(
                name: "SoldSpaceLocationId",
                table: "SoldSpaces",
                newName: "ProductAdvertisingSpaceId");

            migrationBuilder.RenameIndex(
                name: "IX_SoldSpaces_SoldSpaceLocationId",
                table: "SoldSpaces",
                newName: "IX_SoldSpaces_ProductAdvertisingSpaceId");

            migrationBuilder.RenameColumn(
                name: "SoldSpaceLocationId",
                table: "PublishingOrders",
                newName: "ProductAdvertisingSpaceId");

            migrationBuilder.RenameIndex(
                name: "IX_PublishingOrders_SoldSpaceLocationId",
                table: "PublishingOrders",
                newName: "IX_PublishingOrders_ProductAdvertisingSpaceId");

            migrationBuilder.AddColumn<double>(
                name: "SubTotal",
                table: "SoldSpaces",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalDiscounts",
                table: "SoldSpaces",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalTaxes",
                table: "SoldSpaces",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<long>(
                name: "ContractId",
                table: "PublishingOrders",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<double>(
                name: "Total",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalDiscounts",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalTaxes",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "CheckPayments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Order = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    ContractId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckPayments_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckPayments_ContractId",
                table: "CheckPayments",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_PublishingOrders_Contracts_ContractId",
                table: "PublishingOrders",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PublishingOrders_ProductAdvertisingSpaces_ProductAdvertisin~",
                table: "PublishingOrders",
                column: "ProductAdvertisingSpaceId",
                principalTable: "ProductAdvertisingSpaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SoldSpaces_ProductAdvertisingSpaces_ProductAdvertisingSpace~",
                table: "SoldSpaces",
                column: "ProductAdvertisingSpaceId",
                principalTable: "ProductAdvertisingSpaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublishingOrders_Contracts_ContractId",
                table: "PublishingOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PublishingOrders_ProductAdvertisingSpaces_ProductAdvertisin~",
                table: "PublishingOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SoldSpaces_ProductAdvertisingSpaces_ProductAdvertisingSpace~",
                table: "SoldSpaces");

            migrationBuilder.DropTable(
                name: "CheckPayments");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "TotalDiscounts",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "TotalTaxes",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "TotalDiscounts",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "TotalTaxes",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "ProductAdvertisingSpaceId",
                table: "SoldSpaces",
                newName: "SoldSpaceLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_SoldSpaces_ProductAdvertisingSpaceId",
                table: "SoldSpaces",
                newName: "IX_SoldSpaces_SoldSpaceLocationId");

            migrationBuilder.RenameColumn(
                name: "ProductAdvertisingSpaceId",
                table: "PublishingOrders",
                newName: "SoldSpaceLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_PublishingOrders_ProductAdvertisingSpaceId",
                table: "PublishingOrders",
                newName: "IX_PublishingOrders_SoldSpaceLocationId");

            migrationBuilder.AlterColumn<long>(
                name: "ContractId",
                table: "PublishingOrders",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SoldSpaceLocations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Deleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldSpaceLocations", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_PublishingOrders_Contracts_ContractId",
                table: "PublishingOrders",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublishingOrders_SoldSpaceLocations_SoldSpaceLocationId",
                table: "PublishingOrders",
                column: "SoldSpaceLocationId",
                principalTable: "SoldSpaceLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SoldSpaces_SoldSpaceLocations_SoldSpaceLocationId",
                table: "SoldSpaces",
                column: "SoldSpaceLocationId",
                principalTable: "SoldSpaceLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
