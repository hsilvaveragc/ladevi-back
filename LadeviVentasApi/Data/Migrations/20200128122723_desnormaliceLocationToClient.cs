using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class desnormaliceLocationToClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_City_CityId",
                table: "Clients");

            migrationBuilder.AlterColumn<long>(
                name: "CityId",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "CountryId",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DistrictId",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StateId",
                table: "Clients",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CountryId",
                table: "Clients",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_DistrictId",
                table: "Clients",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_StateId",
                table: "Clients",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_City_CityId",
                table: "Clients",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Country_CountryId",
                table: "Clients",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_District_DistrictId",
                table: "Clients",
                column: "DistrictId",
                principalTable: "District",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_State_StateId",
                table: "Clients",
                column: "StateId",
                principalTable: "State",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_City_CityId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Country_CountryId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_District_DistrictId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_State_StateId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_CountryId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_DistrictId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_StateId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Clients");

            migrationBuilder.AlterColumn<long>(
                name: "CityId",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_City_CityId",
                table: "Clients",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
