using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class MigreateDiscountsByVolumeValuesSoldSpace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""SoldSpaces""
	                                SET ""DiscountForVolume""=""subquery"".""DiscountForVolume"",
		                                ""ApplyDiscountForVolume""=""subquery"".""ApplyDiscountForVolume""
	                                FROM (SELECT ""SoldSpaces"".""Id"",
		  		                                ""Contracts"".""DiscountForVolume"", ""Contracts"".""ApplyDiscountForVolume""		  		
			                                FROM ""SoldSpaces""
		  		                                INNER JOIN ""Contracts"" ON ""Contracts"".""Id"" = ""SoldSpaces"".""ContractId"") AS subquery
	                                WHERE ""SoldSpaces"".""Id"" = ""subquery"".""Id""");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
