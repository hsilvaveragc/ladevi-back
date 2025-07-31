using LadeviVentasApi.Models.Domain;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class AddApplicationRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ApplicationRoleId",
                table: "ApplicationUsers",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ApplicationRole",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    ShouldHaveCommission = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRole", x => x.Id);
                });

            migrationBuilder.InsertData("ApplicationRole",
                new[] { "Name", "ShouldHaveCommission" },
                new object[] { ApplicationRole.SuperuserRole, false });

            migrationBuilder.Sql(@"
                update public.""ApplicationUsers""
                set ""ApplicationRoleId""=(select max(""Id"") from public.""ApplicationRole"")
            ");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_ApplicationRoleId",
                table: "ApplicationUsers",
                column: "ApplicationRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_ApplicationRole_ApplicationRoleId",
                table: "ApplicationUsers",
                column: "ApplicationRoleId",
                principalTable: "ApplicationRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_ApplicationRole_ApplicationRoleId",
                table: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "ApplicationRole");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_ApplicationRoleId",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationRoleId",
                table: "ApplicationUsers");
        }
    }
}
