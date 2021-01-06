using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class Changednamingandaddedkeywords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReverseFilter_Translations",
                table: "TraktLists",
                newName: "ExclusionFilter_Translations");

            migrationBuilder.RenameColumn(
                name: "ReverseFilter_Status",
                table: "TraktLists",
                newName: "ExclusionFilter_Status");

            migrationBuilder.RenameColumn(
                name: "ReverseFilter_Networks",
                table: "TraktLists",
                newName: "ExclusionFilter_Networks");

            migrationBuilder.RenameColumn(
                name: "ReverseFilter_Languages",
                table: "TraktLists",
                newName: "ExclusionFilter_Languages");

            migrationBuilder.RenameColumn(
                name: "ReverseFilter_Genres",
                table: "TraktLists",
                newName: "ExclusionFilter_Keywords");

            migrationBuilder.RenameColumn(
                name: "ReverseFilter_Countries",
                table: "TraktLists",
                newName: "ExclusionFilter_Genres");

            migrationBuilder.RenameColumn(
                name: "ReverseFilter_Certifications_Show",
                table: "TraktLists",
                newName: "ExclusionFilter_Countries");

            migrationBuilder.RenameColumn(
                name: "ReverseFilter_Certifications_Movie",
                table: "TraktLists",
                newName: "ExclusionFilter_Certifications_Show");

            migrationBuilder.AddColumn<string>(
                name: "ExclusionFilter_Certifications_Movie",
                table: "TraktLists",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExclusionFilter_Certifications_Movie",
                table: "TraktLists");

            migrationBuilder.RenameColumn(
                name: "ExclusionFilter_Translations",
                table: "TraktLists",
                newName: "ReverseFilter_Translations");

            migrationBuilder.RenameColumn(
                name: "ExclusionFilter_Status",
                table: "TraktLists",
                newName: "ReverseFilter_Status");

            migrationBuilder.RenameColumn(
                name: "ExclusionFilter_Networks",
                table: "TraktLists",
                newName: "ReverseFilter_Networks");

            migrationBuilder.RenameColumn(
                name: "ExclusionFilter_Languages",
                table: "TraktLists",
                newName: "ReverseFilter_Languages");

            migrationBuilder.RenameColumn(
                name: "ExclusionFilter_Keywords",
                table: "TraktLists",
                newName: "ReverseFilter_Genres");

            migrationBuilder.RenameColumn(
                name: "ExclusionFilter_Genres",
                table: "TraktLists",
                newName: "ReverseFilter_Countries");

            migrationBuilder.RenameColumn(
                name: "ExclusionFilter_Countries",
                table: "TraktLists",
                newName: "ReverseFilter_Certifications_Show");

            migrationBuilder.RenameColumn(
                name: "ExclusionFilter_Certifications_Show",
                table: "TraktLists",
                newName: "ReverseFilter_Certifications_Movie");
        }
    }
}
