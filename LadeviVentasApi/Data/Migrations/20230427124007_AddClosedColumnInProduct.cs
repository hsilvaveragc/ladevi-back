using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddClosedColumnInProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Closed",
                table: "ProductEditions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"UPDATE ""ProductEditions""
	                                  SET ""Closed"" = true
                                    WHERE ""Start"" <  NOW()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Closed",
                table: "ProductEditions");
        }
    }
}
