using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddedModificationsToAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CredentialsUserId",
                table: "ApplicationUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_CredentialsUserId",
                table: "ApplicationUsers",
                column: "CredentialsUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_AspNetUsers_CredentialsUserId",
                table: "ApplicationUsers",
                column: "CredentialsUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_AspNetUsers_CredentialsUserId",
                table: "ApplicationUsers");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_CredentialsUserId",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "CredentialsUserId",
                table: "ApplicationUsers");
        }
    }
}
