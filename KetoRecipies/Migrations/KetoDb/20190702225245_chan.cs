using Microsoft.EntityFrameworkCore.Migrations;

namespace KetoRecipies.Migrations.KetoDb
{
    public partial class chan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_favorites_recipes_RecipeID",
                table: "favorites");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeID",
                table: "favorites",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_favorites_recipes_RecipeID",
                table: "favorites",
                column: "RecipeID",
                principalTable: "recipes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_favorites_recipes_RecipeID",
                table: "favorites");

            migrationBuilder.AlterColumn<int>(
                name: "RecipeID",
                table: "favorites",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_favorites_recipes_RecipeID",
                table: "favorites",
                column: "RecipeID",
                principalTable: "recipes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
