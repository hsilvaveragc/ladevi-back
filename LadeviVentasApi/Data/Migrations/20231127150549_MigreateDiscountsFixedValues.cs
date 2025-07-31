using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class MigreateDiscountsFixedValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""ProductAdvertisingSpaces""
									SET  ""DiscountForAgency""=""subquery"".""DiscountForAgency"",
										""DiscountForCheck"" =""subquery"".""DiscountForCheck"",
										""DiscountForLoyalty"" =""subquery"".""DiscountForLoyalty"",
										""DiscountForOtherCountry"" =""subquery"".""DiscountForOtherCountry"",
										""DiscountForSameCountry"" =""subquery"".""DiscountForSameCountry""
									FROM (SELECT ""ProductAdvertisingSpaces"".""Id"",
		  										""Products"".""DiscountForAgency"",
		  										""Products"".""DiscountForCheck"", 
		  										""Products"".""DiscountForLoyalty"", 
		  										""Products"".""DiscountForOtherCountry"",
		  										""Products"".""DiscountForSameCountry""
											FROM ""ProductAdvertisingSpaces""
												INNER JOIN ""Products""  ON ""Products"".""Id"" = ""ProductAdvertisingSpaces"".""ProductId"") AS subquery
									WHERE ""ProductAdvertisingSpaces"".""Id"" = ""subquery"".""Id""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
