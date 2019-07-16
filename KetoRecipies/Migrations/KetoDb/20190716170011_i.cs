using Microsoft.EntityFrameworkCore.Migrations;

namespace KetoRecipies.Migrations.KetoDb
{
    public partial class i : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mainComments_recipes_RecipeID",
                table: "mainComments");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeID",
                table: "mainComments",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_mainComments_recipes_RecipeID",
                table: "mainComments",
                column: "RecipeID",
                principalTable: "recipes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mainComments_recipes_RecipeID",
                table: "mainComments");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeID",
                table: "mainComments",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_mainComments_recipes_RecipeID",
                table: "mainComments",
                column: "RecipeID",
                principalTable: "recipes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
