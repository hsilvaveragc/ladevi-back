using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class addLogicDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "TaxType",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "TaxType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "TaxType",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "State",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "State",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "State",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "SoldSpaces",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "SoldSpaces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "SoldSpaces",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "RefreshTokens",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "RefreshTokens",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "RefreshTokens",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "PublishingOrders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "PublishingOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "PublishingOrders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ProductVolumeDiscount",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ProductVolumeDiscount",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "ProductVolumeDiscount",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ProductTypes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ProductTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "ProductTypes",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ProductLocationDiscount",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ProductLocationDiscount",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "ProductLocationDiscount",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ProductEditions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ProductEditions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "ProductEditions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ProductCurrencyParity",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ProductCurrencyParity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "ProductCurrencyParity",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ProductAdvertisingSpaces",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ProductAdvertisingSpaces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "ProductAdvertisingSpaces",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "PaymentMethods",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "PaymentMethods",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "District",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "District",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "District",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Currency",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "Currency",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Country",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Country",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "Country",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Contracts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Contracts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "Contracts",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "City",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "City",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "City",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "CheckPayments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "CheckPayments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "CheckPayments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "BillingConditions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "BillingConditions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ApplicationUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ApplicationUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "ApplicationUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ApplicationRole",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ApplicationRole",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "ApplicationRole",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "AdvertisingSpaceLocationTypes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "AdvertisingSpaceLocationTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedUser",
                table: "AdvertisingSpaceLocationTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "TaxType");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "TaxType");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "TaxType");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "State");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "State");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "State");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "SoldSpaces");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "PublishingOrders");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "PublishingOrders");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "PublishingOrders");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ProductVolumeDiscount");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ProductVolumeDiscount");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "ProductVolumeDiscount");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ProductLocationDiscount");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ProductLocationDiscount");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "ProductLocationDiscount");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ProductEditions");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ProductEditions");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "ProductEditions");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ProductCurrencyParity");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ProductCurrencyParity");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "ProductCurrencyParity");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ProductAdvertisingSpaces");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ProductAdvertisingSpaces");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "ProductAdvertisingSpaces");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "District");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "District");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "District");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "City");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "City");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "City");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "CheckPayments");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "CheckPayments");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "CheckPayments");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "BillingConditions");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "BillingConditions");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ApplicationRole");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ApplicationRole");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "ApplicationRole");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "AdvertisingSpaceLocationTypes");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "AdvertisingSpaceLocationTypes");

            migrationBuilder.DropColumn(
                name: "DeletedUser",
                table: "AdvertisingSpaceLocationTypes");
        }
    }
}
