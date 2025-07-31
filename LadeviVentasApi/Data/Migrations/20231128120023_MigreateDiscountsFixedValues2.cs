using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class MigreateDiscountsFixedValues2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""SoldSpaces""
									SET ""DiscountForAgency""=""subquery"".""DiscountForAgency"",
										""DiscountForCheck"" =""subquery"".""DiscountForCheck"",
										""DiscountForLoyalty"" =""subquery"".""DiscountForLoyalty"",
										""DiscountForOtherCountry"" =""subquery"".""DiscountForOtherCountry"",
										""DiscountForSameCountry"" =""subquery"".""DiscountForSameCountry"",
										""ApplyDiscountForCheck""=""subquery"".""ApplyDiscountForCheck"",
										""ApplyDiscountForLoyalty""=""subquery"".""ApplyDiscountForLoyalty"",
	 									""ApplyDiscountForSameCountry""=""subquery"".""ApplyDiscountForSameCountry"", 
										""ApplyDiscountForOtherCountry""=""subquery"".""ApplyDiscountForOtherCountry"",
    									""AppyDiscountForAgency""=""subquery"".""AppyDiscountForAgency""
									FROM (SELECT ""SoldSpaces"".""Id"",
		  										""Contracts"".""ApplyDiscountForCheck"", ""Contracts"".""ApplyDiscountForLoyalty"", ""Contracts"".""ApplyDiscountForSameCountry"", 	  		
		  										""Contracts"".""ApplyDiscountForOtherCountry"", ""Contracts"".""AppyDiscountForAgency"", 
		  										""Contracts"".""DiscountForCheck"", ""Contracts"".""DiscountForLoyalty"", ""Contracts"".""DiscountForSameCountry"", 	  		
		  										""Contracts"".""DiscountForOtherCountry"", ""Contracts"".""DiscountForAgency""		  		
											FROM ""SoldSpaces""
		  										INNER JOIN ""Contracts"" ON ""Contracts"".""Id"" = ""SoldSpaces"".""ContractId"") AS subquery
									WHERE ""SoldSpaces"".""Id"" = ""subquery"".""Id""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
