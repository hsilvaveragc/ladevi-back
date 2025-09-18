using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductEditionId = table.Column<long>(type: "bigint", nullable: false),
                    PageNumber = table.Column<int>(type: "integer", nullable: false),
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    InventoryProductAdvertisingSpaceId = table.Column<long>(type: "bigint", nullable: false),
                    PublishingOrderId = table.Column<long>(type: "bigint", nullable: true),
                    IsEditorial = table.Column<bool>(type: "boolean", nullable: false),
                    IsCA = table.Column<bool>(type: "boolean", nullable: false),
                    Observations = table.Column<string>(type: "text", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionItems_InventoryProductAdvertisingSpaces_Inventory~",
                        column: x => x.InventoryProductAdvertisingSpaceId,
                        principalTable: "InventoryProductAdvertisingSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionItems_PublishingOrders_PublishingOrderId",
                        column: x => x.PublishingOrderId,
                        principalTable: "PublishingOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionItems_InventoryProductAdvertisingSpaceId",
                table: "ProductionItems",
                column: "InventoryProductAdvertisingSpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionItems_PublishingOrderId",
                table: "ProductionItems",
                column: "PublishingOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionItems");
        }
    }
}
