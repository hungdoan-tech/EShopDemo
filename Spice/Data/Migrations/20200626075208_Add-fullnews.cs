using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class Addfullnews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "News",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "News",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DatePublished",
                table: "News",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Header",
                table: "News",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NewsCategoryId",
                table: "News",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NewsCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Alias = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsCategoryId",
                table: "News",
                column: "NewsCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_NewsCategory_NewsCategoryId",
                table: "News",
                column: "NewsCategoryId",
                principalTable: "NewsCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_NewsCategory_NewsCategoryId",
                table: "News");

            migrationBuilder.DropTable(
                name: "NewsCategory");

            migrationBuilder.DropIndex(
                name: "IX_News_NewsCategoryId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Alias",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "News");

            migrationBuilder.DropColumn(
                name: "DatePublished",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Header",
                table: "News");

            migrationBuilder.DropColumn(
                name: "NewsCategoryId",
                table: "News");
        }
    }
}
