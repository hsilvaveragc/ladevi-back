using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class TaxCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TaxCategoryId",
                table: "Clients",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaxCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_TaxCategoryId",
                table: "Clients",
                column: "TaxCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_TaxCategories_TaxCategoryId",
                table: "Clients",
                column: "TaxCategoryId",
                principalTable: "TaxCategories",
                principalColumn: "Id");

            migrationBuilder.Sql(@"INSERT INTO ""TaxCategories"" (""Code"", ""Name"", ""Deleted"", ""DeletedDate"", ""DeletedUser"")
                                    VALUES ('CF', 'Consumidor Final', null, null, null)");

            migrationBuilder.Sql(@"INSERT INTO ""TaxCategories"" (""Code"", ""Name"",  ""Deleted"", ""DeletedDate"", ""DeletedUser"")
                                    VALUES ('EX', 'Exento', null, null, null)");

            migrationBuilder.Sql(@"INSERT INTO ""TaxCategories"" (""Code"", ""Name"", ""Deleted"", ""DeletedDate"", ""DeletedUser"")
                                    VALUES ('CE', 'Exterior', null, null, null)");

            migrationBuilder.Sql(@"INSERT INTO ""TaxCategories"" (""Code"", ""Name"", ""Deleted"", ""DeletedDate"", ""DeletedUser"")
                                    VALUES ('NA', 'IVA No Alcanzado', null, null, null)");

            migrationBuilder.Sql(@"INSERT INTO ""TaxCategories"" (""Code"", ""Name"", ""Deleted"", ""DeletedDate"", ""DeletedUser"")
                                    VALUES ('MT', 'Monotributista', null, null, null)");

            migrationBuilder.Sql(@"INSERT INTO ""TaxCategories"" (""Code"", ""Name"", ""Deleted"", ""DeletedDate"", ""DeletedUser"")
                                    VALUES ('RI', 'Responsable Inscripto', null, null, null)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_TaxCategories_TaxCategoryId",
                table: "Clients");

            migrationBuilder.DropTable(
                name: "TaxCategories");

            migrationBuilder.DropIndex(
                name: "IX_Clients_TaxCategoryId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TaxCategoryId",
                table: "Clients");
        }
    }
}
