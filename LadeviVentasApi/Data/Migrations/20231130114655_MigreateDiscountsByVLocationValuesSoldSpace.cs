using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class MigreateDiscountsByVLocationValuesSoldSpace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO ""ProductAdvertisingSpaceLocationDiscount""
	                                    (""Deleted"", ""DeletedDate"", ""DeletedUser"", ""Discount"", ""ProductAdvertisingSpaceId"", ""AdvertisingSpaceLocationTypeId"")  
                                    SELECT ""ProductLocationDiscount"".""Deleted"",
	                                    ""ProductLocationDiscount"".""DeletedDate"",
	                                    ""ProductLocationDiscount"".""DeletedUser"",
	                                    ""ProductLocationDiscount"".""Discount"",	
	                                    ""ProductAdvertisingSpaces"".""Id"",
	                                    ""ProductLocationDiscount"".""AdvertisingSpaceLocationTypeId""
	                                    FROM ""ProductLocationDiscount""
		                                    JOIN ""Products"" ON ""Products"".""Id"" = ""ProductLocationDiscount"".""ProductId""
		                                    JOIN ""ProductAdvertisingSpaces"" ON ""Products"".""Id"" = ""ProductAdvertisingSpaces"".""ProductId""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
