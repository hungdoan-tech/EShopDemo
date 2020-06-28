using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class Addfullnews2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_NewsCategory_NewsCategoryId",
                table: "News");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsCategory",
                table: "NewsCategory");

            migrationBuilder.RenameTable(
                name: "NewsCategory",
                newName: "NewsCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsCategories",
                table: "NewsCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_News_NewsCategories_NewsCategoryId",
                table: "News",
                column: "NewsCategoryId",
                principalTable: "NewsCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_NewsCategories_NewsCategoryId",
                table: "News");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsCategories",
                table: "NewsCategories");

            migrationBuilder.RenameTable(
                name: "NewsCategories",
                newName: "NewsCategory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsCategory",
                table: "NewsCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_News_NewsCategory_NewsCategoryId",
                table: "News",
                column: "NewsCategoryId",
                principalTable: "NewsCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
