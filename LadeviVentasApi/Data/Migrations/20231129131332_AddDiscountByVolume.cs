using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddDiscountByVolume : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductAdvertisingSpaceVolumeDiscount",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Deleted = table.Column<bool>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    DeletedUser = table.Column<string>(nullable: true),
                    RangeStart = table.Column<long>(nullable: false),
                    RangeEnd = table.Column<long>(nullable: false),
                    Discount = table.Column<double>(nullable: false),
                    ProductAdvertisingSpaceId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAdvertisingSpaceVolumeDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAdvertisingSpaceVolumeDiscount_ProductAdvertisingSpa~",
                        column: x => x.ProductAdvertisingSpaceId,
                        principalTable: "ProductAdvertisingSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdvertisingSpaceVolumeDiscount_ProductAdvertisingSpa~",
                table: "ProductAdvertisingSpaceVolumeDiscount",
                column: "ProductAdvertisingSpaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAdvertisingSpaceVolumeDiscount");
        }
    }
}
