using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullPublishingOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionSlots_InventoryAdvertisingSpaces_InventoryAdverti~",
                table: "ProductionSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionSlots_PublishingOrders_PublishingOrderId",
                table: "ProductionSlots");

            migrationBuilder.AlterColumn<long>(
                name: "PublishingOrderId",
                table: "ProductionSlots",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "InventoryAdvertisingSpaceId",
                table: "ProductionSlots",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionSlots_InventoryAdvertisingSpaces_InventoryAdverti~",
                table: "ProductionSlots",
                column: "InventoryAdvertisingSpaceId",
                principalTable: "InventoryAdvertisingSpaces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionSlots_PublishingOrders_PublishingOrderId",
                table: "ProductionSlots",
                column: "PublishingOrderId",
                principalTable: "PublishingOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionSlots_InventoryAdvertisingSpaces_InventoryAdverti~",
                table: "ProductionSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionSlots_PublishingOrders_PublishingOrderId",
                table: "ProductionSlots");

            migrationBuilder.AlterColumn<long>(
                name: "PublishingOrderId",
                table: "ProductionSlots",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "InventoryAdvertisingSpaceId",
                table: "ProductionSlots",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionSlots_InventoryAdvertisingSpaces_InventoryAdverti~",
                table: "ProductionSlots",
                column: "InventoryAdvertisingSpaceId",
                principalTable: "InventoryAdvertisingSpaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionSlots_PublishingOrders_PublishingOrderId",
                table: "ProductionSlots",
                column: "PublishingOrderId",
                principalTable: "PublishingOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
