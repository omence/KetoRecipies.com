using Microsoft.EntityFrameworkCore.Migrations;

namespace KetoRecipies.Migrations.KetoDb
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecipeID",
                table: "mainComments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mainComments_RecipeID",
                table: "mainComments",
                column: "RecipeID");

            migrationBuilder.AddForeignKey(
                name: "FK_mainComments_recipes_RecipeID",
                table: "mainComments",
                column: "RecipeID",
                principalTable: "recipes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mainComments_recipes_RecipeID",
                table: "mainComments");

            migrationBuilder.DropIndex(
                name: "IX_mainComments_RecipeID",
                table: "mainComments");

            migrationBuilder.DropColumn(
                name: "RecipeID",
                table: "mainComments");
        }
    }
}
