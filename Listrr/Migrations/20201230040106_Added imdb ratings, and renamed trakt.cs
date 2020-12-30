using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class Addedimdbratingsandrenamedtrakt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Filter_Ratings",
                table: "TraktLists",
                newName: "Filter_Ratings_Trakt");

            migrationBuilder.AddColumn<string>(
                name: "Filter_Ratings_IMDb",
                table: "TraktLists",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Filter_Ratings_IMDb",
                table: "TraktLists");

            migrationBuilder.RenameColumn(
                name: "Filter_Ratings_Trakt",
                table: "TraktLists",
                newName: "Filter_Ratings");
        }
    }
}
