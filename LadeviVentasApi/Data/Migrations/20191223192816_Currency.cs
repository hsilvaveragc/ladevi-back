using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class Currency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /* migrationBuilder.DropColumn(
                name: "ApplicationUserSellerId",
                table: "PublishingOrders"); */

            migrationBuilder.AddColumn<long>(
                name: "CountryId",
                table: "Currency",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<double>(
                name: "DiscountForSameCountry",
                table: "Contracts",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<double>(
                name: "DiscountForOtherCountry",
                table: "Contracts",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<double>(
                name: "DiscountForLoyalty",
                table: "Contracts",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<double>(
                name: "DiscountForCheck",
                table: "Contracts",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<double>(
                name: "DiscountForAgency",
                table: "Contracts",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "DaysToFirstPayment",
                table: "Contracts",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "DaysBetweenChecks",
                table: "Contracts",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CheckQuantity",
                table: "Contracts",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Currency");

            migrationBuilder.AddColumn<long>(
                name: "ApplicationUserSellerId",
                table: "PublishingOrders",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<double>(
                name: "DiscountForSameCountry",
                table: "Contracts",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DiscountForOtherCountry",
                table: "Contracts",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DiscountForLoyalty",
                table: "Contracts",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DiscountForCheck",
                table: "Contracts",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DiscountForAgency",
                table: "Contracts",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DaysToFirstPayment",
                table: "Contracts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DaysBetweenChecks",
                table: "Contracts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CheckQuantity",
                table: "Contracts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
