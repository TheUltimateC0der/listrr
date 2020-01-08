using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class Addedstuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Filter_Certifications",
                table: "TraktLists");

            migrationBuilder.AddColumn<string>(
                name: "Filter_Certifications_Movie",
                table: "TraktLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Filter_Certifications_Show",
                table: "TraktLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Filter_Networks",
                table: "TraktLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Filter_Status",
                table: "TraktLists",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TraktShowNetworks",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraktShowNetworks", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "TraktShowStatuses",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraktShowStatuses", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TraktShowNetworks_Name",
                table: "TraktShowNetworks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TraktShowStatuses_Name",
                table: "TraktShowStatuses",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TraktShowNetworks");

            migrationBuilder.DropTable(
                name: "TraktShowStatuses");

            migrationBuilder.DropColumn(
                name: "Filter_Certifications_Movie",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "Filter_Certifications_Show",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "Filter_Networks",
                table: "TraktLists");

            migrationBuilder.DropColumn(
                name: "Filter_Status",
                table: "TraktLists");

            migrationBuilder.AddColumn<string>(
                name: "Filter_Certifications",
                table: "TraktLists",
                nullable: true);
        }
    }
}
