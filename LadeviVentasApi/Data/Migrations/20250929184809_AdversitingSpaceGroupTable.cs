using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdversitingSpaceGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AdversitingSpaceGroupId",
                table: "ProductAdvertisingSpaces",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdversitingSpaceGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdversitingSpaceGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdversitingSpaceGroups_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdvertisingSpaces_AdversitingSpaceGroupId",
                table: "ProductAdvertisingSpaces",
                column: "AdversitingSpaceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AdversitingSpaceGroups_ProductId",
                table: "AdversitingSpaceGroups",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAdvertisingSpaces_AdversitingSpaceGroups_Adversiting~",
                table: "ProductAdvertisingSpaces",
                column: "AdversitingSpaceGroupId",
                principalTable: "AdversitingSpaceGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAdvertisingSpaces_AdversitingSpaceGroups_Adversiting~",
                table: "ProductAdvertisingSpaces");

            migrationBuilder.DropTable(
                name: "AdversitingSpaceGroups");

            migrationBuilder.DropIndex(
                name: "IX_ProductAdvertisingSpaces_AdversitingSpaceGroupId",
                table: "ProductAdvertisingSpaces");

            migrationBuilder.DropColumn(
                name: "AdversitingSpaceGroupId",
                table: "ProductAdvertisingSpaces");
        }
    }
}
