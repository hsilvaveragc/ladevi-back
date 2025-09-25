using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionTemplateAndSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientTaxes");

            migrationBuilder.DropTable(
                name: "ProductionItems");

            migrationBuilder.DropTable(
                name: "InventoryProductAdvertisingSpaces");

            migrationBuilder.CreateTable(
                name: "InventoryAdvertisingSpaces",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductEditionId = table.Column<long>(type: "bigint", nullable: false),
                    ProductAdvertisingSpaceId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAdvertisingSpaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryAdvertisingSpaces_ProductAdvertisingSpaces_Product~",
                        column: x => x.ProductAdvertisingSpaceId,
                        principalTable: "ProductAdvertisingSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryAdvertisingSpaces_ProductEditions_ProductEditionId",
                        column: x => x.ProductEditionId,
                        principalTable: "ProductEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductEditionId = table.Column<long>(type: "bigint", nullable: false),
                    PageNumber = table.Column<int>(type: "integer", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionTemplates_ProductEditions_ProductEditionId",
                        column: x => x.ProductEditionId,
                        principalTable: "ProductEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionSlots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductionTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    SlotNumber = table.Column<int>(type: "integer", nullable: false),
                    InventoryAdvertisingSpaceId = table.Column<long>(type: "bigint", nullable: false),
                    PublishingOrderId = table.Column<long>(type: "bigint", nullable: false),
                    Observations = table.Column<string>(type: "text", nullable: true),
                    IsEditorial = table.Column<bool>(type: "boolean", nullable: false),
                    IsCA = table.Column<bool>(type: "boolean", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionSlots_InventoryAdvertisingSpaces_InventoryAdverti~",
                        column: x => x.InventoryAdvertisingSpaceId,
                        principalTable: "InventoryAdvertisingSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionSlots_ProductionTemplates_ProductionTemplateId",
                        column: x => x.ProductionTemplateId,
                        principalTable: "ProductionTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionSlots_PublishingOrders_PublishingOrderId",
                        column: x => x.PublishingOrderId,
                        principalTable: "PublishingOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdvertisingSpaces_ProductAdvertisingSpaceId",
                table: "InventoryAdvertisingSpaces",
                column: "ProductAdvertisingSpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdvertisingSpaces_ProductEditionId",
                table: "InventoryAdvertisingSpaces",
                column: "ProductEditionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionSlots_InventoryAdvertisingSpaceId",
                table: "ProductionSlots",
                column: "InventoryAdvertisingSpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionSlots_ProductionTemplateId",
                table: "ProductionSlots",
                column: "ProductionTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionSlots_PublishingOrderId",
                table: "ProductionSlots",
                column: "PublishingOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionTemplates_ProductEditionId",
                table: "ProductionTemplates",
                column: "ProductEditionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionSlots");

            migrationBuilder.DropTable(
                name: "InventoryAdvertisingSpaces");

            migrationBuilder.DropTable(
                name: "ProductionTemplates");

            migrationBuilder.CreateTable(
                name: "InventoryProductAdvertisingSpaces",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductAdvertisingSpaceId = table.Column<long>(type: "bigint", nullable: false),
                    ProductEditionId = table.Column<long>(type: "bigint", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryProductAdvertisingSpaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryProductAdvertisingSpaces_ProductAdvertisingSpaces_~",
                        column: x => x.ProductAdvertisingSpaceId,
                        principalTable: "ProductAdvertisingSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryProductAdvertisingSpaces_ProductEditions_ProductEd~",
                        column: x => x.ProductEditionId,
                        principalTable: "ProductEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InventoryProductAdvertisingSpaceId = table.Column<long>(type: "bigint", nullable: false),
                    PublishingOrderId = table.Column<long>(type: "bigint", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    IsCA = table.Column<bool>(type: "boolean", nullable: false),
                    IsEditorial = table.Column<bool>(type: "boolean", nullable: false),
                    Observations = table.Column<string>(type: "text", nullable: false),
                    PageNumber = table.Column<int>(type: "integer", nullable: false),
                    ProductEditionId = table.Column<long>(type: "bigint", nullable: false),
                    Slot = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_InventoryProductAdvertisingSpaces_ProductAdvertisingSpaceId",
                table: "InventoryProductAdvertisingSpaces",
                column: "ProductAdvertisingSpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryProductAdvertisingSpaces_ProductEditionId",
                table: "InventoryProductAdvertisingSpaces",
                column: "ProductEditionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionItems_InventoryProductAdvertisingSpaceId",
                table: "ProductionItems",
                column: "InventoryProductAdvertisingSpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionItems_PublishingOrderId",
                table: "ProductionItems",
                column: "PublishingOrderId");
        }
    }
}
