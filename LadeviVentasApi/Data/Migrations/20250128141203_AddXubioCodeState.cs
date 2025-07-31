using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddXubioCodeState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "XubioCode",
                table: "State",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE  ""State""
                                    SET ""XubioCode""=REPLACE(""Name"", ' ', '_')
                                    WHERE ""CountryId"" = 4");

            migrationBuilder.Sql(@"UPDATE  ""State""
                                    SET ""XubioCode""='CIUDAD_AUTONOMA_DE_BUENOS_AIRES'
                                    WHERE ""Name"" = 'CAPITAL FEDERAL'");

            migrationBuilder.Sql(@"UPDATE  ""State""
                                    SET ""XubioCode""='SANTA_CRUZ'
                                    WHERE ""Name"" = 'SANTA CRUZ - ARG'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XubioCode",
                table: "State");
        }
    }
}
