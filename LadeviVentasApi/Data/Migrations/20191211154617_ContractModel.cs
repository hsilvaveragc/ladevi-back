using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class ContractModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ApplyDiscountForCheck",
                table: "Contracts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyDiscountForLoyalty",
                table: "Contracts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyDiscountForOtherCountry",
                table: "Contracts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyDiscountForSameCountry",
                table: "Contracts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyDiscountForVolume",
                table: "Contracts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AppyDiscountForAgency",
                table: "Contracts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "BillingConditionId",
                table: "Contracts",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BillingCountryId",
                table: "Contracts",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "CheckQuantity",
                table: "Contracts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractDate",
                table: "Contracts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "Contracts",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "DaysBetweenChecks",
                table: "Contracts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DaysToFirstPayment",
                table: "Contracts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForAgency",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForCheck",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForLoyalty",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForOtherCountry",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountForSameCountry",
                table: "Contracts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Contracts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Contracts",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Number",
                table: "Contracts",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "PaidOut",
                table: "Contracts",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PaymentMethodId",
                table: "Contracts",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "BillingConditions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoldSpaceLocations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldSpaceLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublishingOrders",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ProductEditionId = table.Column<long>(nullable: false),
                    Latent = table.Column<bool>(nullable: false),
                    ClientId = table.Column<long>(nullable: false),
                    ContractId = table.Column<long>(nullable: false),
                    AdvertisingSpaceLocationTypeId = table.Column<long>(nullable: false),
                    PageNumber = table.Column<int>(nullable: false),
                    SoldSpaceLocationId = table.Column<long>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    PaidOut = table.Column<bool>(nullable: true),
                    Quantity = table.Column<double>(nullable: false),
                    Observations = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishingOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublishingOrders_AdvertisingSpaceLocationTypes_AdvertisingS~",
                        column: x => x.AdvertisingSpaceLocationTypeId,
                        principalTable: "AdvertisingSpaceLocationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishingOrders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishingOrders_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishingOrders_ProductEditions_ProductEditionId",
                        column: x => x.ProductEditionId,
                        principalTable: "ProductEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishingOrders_SoldSpaceLocations_SoldSpaceLocationId",
                        column: x => x.SoldSpaceLocationId,
                        principalTable: "SoldSpaceLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoldSpaces",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AdvertisingSpaceLocationTypeId = table.Column<long>(nullable: false),
                    SoldSpaceLocationId = table.Column<long>(nullable: false),
                    ContractId = table.Column<long>(nullable: false),
                    TypeSpecialDiscount = table.Column<short>(nullable: true),
                    DescriptionSpecialDiscount = table.Column<string>(nullable: true),
                    SpecialDiscount = table.Column<double>(nullable: false),
                    TypeGerentialDiscount = table.Column<short>(nullable: false),
                    DescriptionGerentialDiscount = table.Column<string>(nullable: true),
                    GerentialDiscount = table.Column<double>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldSpaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoldSpaces_AdvertisingSpaceLocationTypes_AdvertisingSpaceLo~",
                        column: x => x.AdvertisingSpaceLocationTypeId,
                        principalTable: "AdvertisingSpaceLocationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SoldSpaces_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SoldSpaces_SoldSpaceLocations_SoldSpaceLocationId",
                        column: x => x.SoldSpaceLocationId,
                        principalTable: "SoldSpaceLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_BillingConditionId",
                table: "Contracts",
                column: "BillingConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_BillingCountryId",
                table: "Contracts",
                column: "BillingCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_CurrencyId",
                table: "Contracts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_PaymentMethodId",
                table: "Contracts",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingOrders_AdvertisingSpaceLocationTypeId",
                table: "PublishingOrders",
                column: "AdvertisingSpaceLocationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingOrders_ClientId",
                table: "PublishingOrders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingOrders_ContractId",
                table: "PublishingOrders",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingOrders_ProductEditionId",
                table: "PublishingOrders",
                column: "ProductEditionId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingOrders_SoldSpaceLocationId",
                table: "PublishingOrders",
                column: "SoldSpaceLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldSpaces_AdvertisingSpaceLocationTypeId",
                table: "SoldSpaces",
                column: "AdvertisingSpaceLocationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldSpaces_ContractId",
                table: "SoldSpaces",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldSpaces_SoldSpaceLocationId",
                table: "SoldSpaces",
                column: "SoldSpaceLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_BillingConditions_BillingConditionId",
                table: "Contracts",
                column: "BillingConditionId",
                principalTable: "BillingConditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Country_BillingCountryId",
                table: "Contracts",
                column: "BillingCountryId",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Currency_CurrencyId",
                table: "Contracts",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_PaymentMethods_PaymentMethodId",
                table: "Contracts",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_BillingConditions_BillingConditionId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Country_BillingCountryId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Currency_CurrencyId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_PaymentMethods_PaymentMethodId",
                table: "Contracts");

            migrationBuilder.DropTable(
                name: "BillingConditions");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "PublishingOrders");

            migrationBuilder.DropTable(
                name: "SoldSpaces");

            migrationBuilder.DropTable(
                name: "SoldSpaceLocations");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_BillingConditionId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_BillingCountryId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_CurrencyId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_PaymentMethodId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ApplyDiscountForCheck",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ApplyDiscountForLoyalty",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ApplyDiscountForOtherCountry",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ApplyDiscountForSameCountry",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ApplyDiscountForVolume",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "AppyDiscountForAgency",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "BillingConditionId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "BillingCountryId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "CheckQuantity",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ContractDate",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DaysBetweenChecks",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DaysToFirstPayment",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DiscountForAgency",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DiscountForCheck",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DiscountForLoyalty",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DiscountForOtherCountry",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DiscountForSameCountry",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "PaidOut",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Contracts");
        }
    }
}
