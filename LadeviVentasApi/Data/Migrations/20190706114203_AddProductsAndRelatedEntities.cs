using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddProductsAndRelatedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvertisingSpaceLocationTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisingSpaceLocationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ProductTypeId = table.Column<long>(nullable: false),
                    CountryId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    DiscountForCheck = table.Column<double>(nullable: false),
                    DiscountForLoyalty = table.Column<double>(nullable: false),
                    DiscountForAgency = table.Column<double>(nullable: false),
                    DiscountForSameCountry = table.Column<double>(nullable: false),
                    DiscountForOtherCountry = table.Column<double>(nullable: false),
                    DiscountSpecialBySeller = table.Column<double>(nullable: false),
                    DiscountByManager = table.Column<double>(nullable: false),
                    MaxAplicableDiscount = table.Column<double>(nullable: false),
                    AliquotForSalesCommission = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAdvertisingSpaces",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    DollarPrice = table.Column<double>(nullable: false),
                    Height = table.Column<double>(nullable: false),
                    Width = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAdvertisingSpaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAdvertisingSpaces_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCurrencyParity",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    LocalCurrencyToDollarExchangeRate = table.Column<double>(nullable: false),
                    ProductId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCurrencyParity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCurrencyParity_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductEditions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Code = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductEditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductEditions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductLocationDiscount",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Discount = table.Column<double>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    AdvertisingSpaceLocationTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLocationDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLocationDiscount_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVolumeDiscount",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RangeStart = table.Column<long>(nullable: false),
                    RangeEnd = table.Column<long>(nullable: false),
                    Discount = table.Column<double>(nullable: false),
                    ProductId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVolumeDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVolumeDiscount_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdvertisingSpaces_ProductId",
                table: "ProductAdvertisingSpaces",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCurrencyParity_ProductId",
                table: "ProductCurrencyParity",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductEditions_ProductId",
                table: "ProductEditions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationDiscount_ProductId",
                table: "ProductLocationDiscount",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CountryId",
                table: "Products",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductTypeId",
                table: "Products",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVolumeDiscount_ProductId",
                table: "ProductVolumeDiscount",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertisingSpaceLocationTypes");

            migrationBuilder.DropTable(
                name: "ProductAdvertisingSpaces");

            migrationBuilder.DropTable(
                name: "ProductCurrencyParity");

            migrationBuilder.DropTable(
                name: "ProductEditions");

            migrationBuilder.DropTable(
                name: "ProductLocationDiscount");

            migrationBuilder.DropTable(
                name: "ProductVolumeDiscount");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductTypes");
        }
    }
}
