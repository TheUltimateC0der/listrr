using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class Addedlikestolists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Items",
                table: "TraktLists",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "TraktLists",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Likes",
                table: "TraktLists");

            migrationBuilder.AlterColumn<int>(
                name: "Items",
                table: "TraktLists",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
