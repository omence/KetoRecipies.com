using Microsoft.EntityFrameworkCore.Migrations;

namespace KetoRecipies.Migrations.KetoDb
{
    public partial class @in : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "subComments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "mainComments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User",
                table: "subComments");

            migrationBuilder.DropColumn(
                name: "User",
                table: "mainComments");
        }
    }
}
