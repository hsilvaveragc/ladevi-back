using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class FixSoldSpaceBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE public.""SoldSpaces""
	                                  SET ""Balance"" = ""sq"".""RealBalance""
                                     FROM (select ""po"".""SoldSpaceId"", ""ss"".""Quantity"", ""ss"".""Balance"", 
				                                  (""ss"".""Quantity"" - count(""po"".""Id"")) as ""RealBalance""
		                                     from public.""PublishingOrders"" as ""po""
			                                    join public.""SoldSpaces"" as ""ss"" on ""ss"".""Id"" = ""po"".""SoldSpaceId""
		                                    where (""po"".""Deleted"" is null or ""po"".""Deleted"" = false)
		                                    group by ""po"".""SoldSpaceId"", ""ss"".""Quantity"", ""ss"".""Balance""
		                                   having ""ss"".""Balance"" <> (""ss"".""Quantity"" - count(""po"".""Id""))) as ""sq""
                                    WHERE ""SoldSpaces"".""Id"" = ""sq"".""SoldSpaceId""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
