using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class MigrateCurrenciesValuesFromProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO public.""CurrencyParities"" (
                    ""Start"",
                    ""End"",
                    ""LocalCurrencyToDollarExchangeRate"",
                    ""CurrencyId""
                )
                WITH LatestParity AS (
                    SELECT 
                        pcp.""ProductId"",
                        pcp.""LocalCurrencyToDollarExchangeRate"",
                        pcp.""Start"",
                        pcp.""End"",
                        p.""CountryId"",
                        ROW_NUMBER() OVER (PARTITION BY p.""CountryId"" ORDER BY pcp.""Start"" DESC) as rn
                    FROM public.""ProductCurrencyParity"" pcp
                    JOIN public.""Products"" p ON p.""Id"" = pcp.""ProductId""
                    WHERE 
                        pcp.""End"" > CURRENT_TIMESTAMP
                        AND (pcp.""Deleted"" IS NULL OR pcp.""Deleted"" IS NOT TRUE)
                        AND(p.""Deleted"" IS NULL OR p.""Deleted"" IS NOT TRUE)
                )
                SELECT 
                    lp.""Start"",
                    lp.""End"",
                    lp.""LocalCurrencyToDollarExchangeRate"",
                    c.""Id"" as ""CurrencyId""
                FROM LatestParity lp
                JOIN public.""Currency"" c ON c.""CountryId"" = lp.""CountryId""
                WHERE lp.rn = 1;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
