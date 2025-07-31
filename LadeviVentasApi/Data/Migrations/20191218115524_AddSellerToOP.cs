using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddSellerToOP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SellerId",
                table: "PublishingOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublishingOrders_SellerId",
                table: "PublishingOrders",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PublishingOrders_ApplicationUsers_SellerId",
                table: "PublishingOrders",
                column: "SellerId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublishingOrders_ApplicationUsers_SellerId",
                table: "PublishingOrders");

            migrationBuilder.DropIndex(
                name: "IX_PublishingOrders_SellerId",
                table: "PublishingOrders");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "PublishingOrders");
        }
    }
}
