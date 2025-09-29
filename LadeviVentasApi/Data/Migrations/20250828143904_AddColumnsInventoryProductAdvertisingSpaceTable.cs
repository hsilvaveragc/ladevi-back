using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsInventoryProductAdvertisingSpaceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductAdvertisingSpaceId",
                table: "InventoryProductAdvertisingSpaces",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryProductAdvertisingSpaces_ProductAdvertisingSpaceId",
                table: "InventoryProductAdvertisingSpaces",
                column: "ProductAdvertisingSpaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryProductAdvertisingSpaces_ProductAdvertisingSpaces_~",
                table: "InventoryProductAdvertisingSpaces",
                column: "ProductAdvertisingSpaceId",
                principalTable: "ProductAdvertisingSpaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryProductAdvertisingSpaces_ProductAdvertisingSpaces_~",
                table: "InventoryProductAdvertisingSpaces");

            migrationBuilder.DropIndex(
                name: "IX_InventoryProductAdvertisingSpaces_ProductAdvertisingSpaceId",
                table: "InventoryProductAdvertisingSpaces");

            migrationBuilder.DropColumn(
                name: "ProductAdvertisingSpaceId",
                table: "InventoryProductAdvertisingSpaces");
        }
    }
}
