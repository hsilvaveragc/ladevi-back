using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class MigreateDiscountsByVolumeValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO ""ProductAdvertisingSpaceVolumeDiscount""
	                                    (""Deleted"", ""DeletedDate"", ""DeletedUser"",""RangeStart"", ""RangeEnd"", ""Discount"", ""ProductAdvertisingSpaceId"")  
                                    SELECT ""ProductVolumeDiscount"".""Deleted"",
	                                    ""ProductVolumeDiscount"".""DeletedDate"",
	                                    ""ProductVolumeDiscount"".""DeletedUser"",
	                                    ""ProductVolumeDiscount"".""RangeStart"",
	                                    ""ProductVolumeDiscount"".""RangeEnd"",
	                                    ""ProductVolumeDiscount"".""Discount"",
	                                    ""ProductAdvertisingSpaces"".""Id""
	                                    FROM ""ProductVolumeDiscount""
		                                    JOIN ""Products"" ON ""Products"".""Id"" = ""ProductVolumeDiscount"".""ProductId""
		                                    JOIN ""ProductAdvertisingSpaces"" ON ""Products"".""Id"" = ""ProductAdvertisingSpaces"".""ProductId""");
            }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
