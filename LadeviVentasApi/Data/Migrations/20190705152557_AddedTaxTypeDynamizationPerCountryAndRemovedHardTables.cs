using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddedTaxTypeDynamizationPerCountryAndRemovedHardTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_TaxIdentificationType_TaxIdentificationTypeId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_TaxIvaCategory_TaxIvaCategoryId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_TaxJuridisctionType_TaxJuridisctionTypeId",
                table: "Clients");

            migrationBuilder.DropTable(
                name: "TaxIdentificationType");

            migrationBuilder.DropTable(
                name: "TaxIvaCategory");

            migrationBuilder.DropTable(
                name: "TaxJuridisctionType");

            migrationBuilder.DropIndex(
                name: "IX_Clients_TaxIdentificationTypeId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_TaxIvaCategoryId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_TaxJuridisctionTypeId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "GrossIncomeTaxCode",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TaxIdentificationCode",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TaxIdentificationTypeId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TaxIvaCategoryId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TaxJuridisctionTypeId",
                table: "Clients");

            migrationBuilder.CreateTable(
                name: "TaxType",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Order = table.Column<long>(nullable: false),
                    OptionsInternal = table.Column<string>(nullable: true),
                    CountryId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxType_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxType_CountryId",
                table: "TaxType",
                column: "CountryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxType");

            migrationBuilder.AddColumn<string>(
                name: "GrossIncomeTaxCode",
                table: "Clients",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TaxIdentificationCode",
                table: "Clients",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "TaxIdentificationTypeId",
                table: "Clients",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TaxIvaCategoryId",
                table: "Clients",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TaxJuridisctionTypeId",
                table: "Clients",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "TaxIdentificationType",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CountryId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxIdentificationType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxIdentificationType_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxIvaCategory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CountryId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxIvaCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxIvaCategory_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxJuridisctionType",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CountryId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxJuridisctionType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxJuridisctionType_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_TaxIdentificationTypeId",
                table: "Clients",
                column: "TaxIdentificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_TaxIvaCategoryId",
                table: "Clients",
                column: "TaxIvaCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_TaxJuridisctionTypeId",
                table: "Clients",
                column: "TaxJuridisctionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxIdentificationType_CountryId",
                table: "TaxIdentificationType",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxIvaCategory_CountryId",
                table: "TaxIvaCategory",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxJuridisctionType_CountryId",
                table: "TaxJuridisctionType",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_TaxIdentificationType_TaxIdentificationTypeId",
                table: "Clients",
                column: "TaxIdentificationTypeId",
                principalTable: "TaxIdentificationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_TaxIvaCategory_TaxIvaCategoryId",
                table: "Clients",
                column: "TaxIvaCategoryId",
                principalTable: "TaxIvaCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_TaxJuridisctionType_TaxJuridisctionTypeId",
                table: "Clients",
                column: "TaxJuridisctionTypeId",
                principalTable: "TaxJuridisctionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
