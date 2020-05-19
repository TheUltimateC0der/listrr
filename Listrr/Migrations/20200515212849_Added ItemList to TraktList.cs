using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class AddedItemListtoTraktList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "TraktLists");

            migrationBuilder.AddColumn<string>(
                name: "ItemList",
                table: "TraktLists",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemList",
                table: "TraktLists");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "TraktLists",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
