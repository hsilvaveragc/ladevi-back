using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class DeleteContractInconsistent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete from public.""PublishingOrders""  where ""ContractId"" = 9175");
            migrationBuilder.Sql(@"delete from ""SoldSpaces"" where ""ContractId"" = 9175");
            migrationBuilder.Sql(@"delete from ""Contracts"" where ""Id"" = 9175");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
