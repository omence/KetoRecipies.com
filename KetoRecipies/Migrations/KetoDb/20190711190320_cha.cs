using Microsoft.EntityFrameworkCore.Migrations;

namespace KetoRecipies.Migrations.KetoDb
{
    public partial class cha : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_recipes_RecipeId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_RecipeId",
                table: "Likes");

            migrationBuilder.AddColumn<int>(
                name: "DisLikeCount",
                table: "recipes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "recipes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisLikeCount",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "recipes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_RecipeId",
                table: "Likes",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_recipes_RecipeId",
                table: "Likes",
                column: "RecipeId",
                principalTable: "recipes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
