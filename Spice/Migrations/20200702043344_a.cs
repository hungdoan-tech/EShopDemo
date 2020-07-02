using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Migrations
{
    public partial class a : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_MenuItem_MenuItemId",
                table: "News");

            migrationBuilder.AlterColumn<int>(
                name: "MenuItemId",
                table: "News",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_News_MenuItem_MenuItemId",
                table: "News",
                column: "MenuItemId",
                principalTable: "MenuItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_MenuItem_MenuItemId",
                table: "News");

            migrationBuilder.AlterColumn<int>(
                name: "MenuItemId",
                table: "News",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_News_MenuItem_MenuItemId",
                table: "News",
                column: "MenuItemId",
                principalTable: "MenuItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
