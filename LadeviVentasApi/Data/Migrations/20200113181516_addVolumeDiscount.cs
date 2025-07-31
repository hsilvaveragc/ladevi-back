using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class addVolumeDiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SoldSpaceId",
                table: "PublishingOrders",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<double>(
                name: "DiscountForVolume",
                table: "Contracts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountForVolume",
                table: "Contracts");

            migrationBuilder.AlterColumn<long>(
                name: "SoldSpaceId",
                table: "PublishingOrders",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
