using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddXubioCodeCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "XubioCode",
                table: "Country",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE  ""Country""
                                    SET ""XubioCode""=REPLACE(""Name"", ' ', '_')");

            migrationBuilder.Sql(@"UPDATE  ""Country""
                                    SET ""XubioCode""='OTROS_PAISES'
                                    WHERE ""Id""=83");

            migrationBuilder.Sql(@"UPDATE  ""Country""
                                    SET ""XubioCode""='OTROS_PAISES'
                                    WHERE ""Id""=88");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XubioCode",
                table: "Country");
        }
    }
}
