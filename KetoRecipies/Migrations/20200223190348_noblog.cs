using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KetoRecipies.Migrations
{
    public partial class noblog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mainComments_BlogPosts_BlogPostID",
                table: "mainComments");

            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_mainComments_BlogPostID",
                table: "mainComments");

            migrationBuilder.DropColumn(
                name: "BlogPostID",
                table: "mainComments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlogPostID",
                table: "mainComments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImageUrl = table.Column<string>(nullable: true),
                    Post = table.Column<string>(nullable: true),
                    VideoUrl = table.Column<string>(nullable: true),
                    userId = table.Column<string>(nullable: true),
                    userName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mainComments_BlogPostID",
                table: "mainComments",
                column: "BlogPostID");

            migrationBuilder.AddForeignKey(
                name: "FK_mainComments_BlogPosts_BlogPostID",
                table: "mainComments",
                column: "BlogPostID",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
