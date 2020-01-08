using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class Addedreversefilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReverseFilter_Certifications_Movie",
                table: "TraktLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReverseFilter_Certifications_Show",
                table: "TraktLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReverseFilter_Countries",
                table: "TraktLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReverseFilter_Genres",
                table: "TraktLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReverseFilter_Languages",
                table: "TraktLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReverseFilter_Networks",
                table: "TraktLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReverseFilter_Status",
                table: "TraktLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReverseFilter_Translations",
                table: "TraktLists",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReverseFilter_Certifications_Movie",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "ReverseFilter_Certifications_Show",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "ReverseFilter_Countries",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "ReverseFilter_Genres",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "ReverseFilter_Languages",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "ReverseFilter_Networks",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "ReverseFilter_Status",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "ReverseFilter_Translations",
                table: "TraktLists");
        }
    }
}
