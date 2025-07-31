using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class FixSellersPublishingOrders2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""PublishingOrders""
                                            SET  ""SellerId""=""subquery"".""ApplicationUserSellerId""
                                            FROM (SELECT  distinct ""Clients"".""Id"", ""Clients"".""ApplicationUserSellerId"" 
	  	                                            FROM ""PublishingOrders"" AS po
			                                            INNER JOIN ""Clients"" ON ""po"".""ClientId"" = ""Clients"".""Id""
	 	                                            WHERE ""po"".""CreationDate"" >= '20231116' OR ""po"".""LastUpdate"" >= '20231116'
	 	                                            ORDER BY ""Clients"".""Id"") AS subquery
                                            WHERE (""PublishingOrders"".""CreationDate"" >= '20231116' OR ""PublishingOrders"".""LastUpdate"" >= '20231116') 
	                                            AND ""PublishingOrders"".""ClientId"" = ""subquery"".""Id""");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
