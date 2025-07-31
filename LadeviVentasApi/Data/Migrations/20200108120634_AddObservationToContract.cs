using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddObservationToContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PageNumber",
                table: "PublishingOrders",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Observations",
                table: "Contracts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observations",
                table: "Contracts");

            migrationBuilder.AlterColumn<int>(
                name: "PageNumber",
                table: "PublishingOrders",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
