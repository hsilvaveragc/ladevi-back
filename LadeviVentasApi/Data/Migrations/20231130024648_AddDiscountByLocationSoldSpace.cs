using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddDiscountByLocationSoldSpace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductAdvertisingSpaceLocationDiscount",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Deleted = table.Column<bool>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    DeletedUser = table.Column<string>(nullable: true),
                    Discount = table.Column<double>(nullable: false),
                    ProductAdvertisingSpaceId = table.Column<long>(nullable: false),
                    AdvertisingSpaceLocationTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAdvertisingSpaceLocationDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAdvertisingSpaceLocationDiscount_ProductAdvertisingS~",
                        column: x => x.ProductAdvertisingSpaceId,
                        principalTable: "ProductAdvertisingSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdvertisingSpaceLocationDiscount_ProductAdvertisingS~",
                table: "ProductAdvertisingSpaceLocationDiscount",
                column: "ProductAdvertisingSpaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAdvertisingSpaceLocationDiscount");
        }
    }
}
