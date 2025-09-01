using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrasactionIdContractAndOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "XubioTransactionId",
                table: "PublishingOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "XubioTransactionId",
                table: "Contracts",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XubioTransactionId",
                table: "PublishingOrders");

            migrationBuilder.DropColumn(
                name: "XubioTransactionId",
                table: "Contracts");
        }
    }
}
