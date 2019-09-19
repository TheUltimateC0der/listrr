using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class Replacedcomplexobjectwithsinglecolsindb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Filter_SearchField",
                table: "TraktLists");

            migrationBuilder.AddColumn<bool>(
                name: "SearchByAlias",
                table: "TraktLists",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SearchByBiography",
                table: "TraktLists",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SearchByDescription",
                table: "TraktLists",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SearchByName",
                table: "TraktLists",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SearchByOverview",
                table: "TraktLists",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SearchByPeople",
                table: "TraktLists",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SearchByTagline",
                table: "TraktLists",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SearchByTitle",
                table: "TraktLists",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SearchByTranslations",
                table: "TraktLists",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchByAlias",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "SearchByBiography",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "SearchByDescription",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "SearchByName",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "SearchByOverview",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "SearchByPeople",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "SearchByTagline",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "SearchByTitle",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "SearchByTranslations",
                table: "TraktLists");

            migrationBuilder.AddColumn<string>(
                name: "Filter_SearchField",
                table: "TraktLists",
                nullable: true);
        }
    }
}
