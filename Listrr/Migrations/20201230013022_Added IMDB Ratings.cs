using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class AddedIMDBRatings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImDbRatings",
                columns: table => new
                {
                    IMDbId = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    Rating = table.Column<float>(nullable: false),
                    Votes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImDbRatings", x => x.IMDbId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImDbRatings_IMDbId",
                table: "ImDbRatings",
                column: "IMDbId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImDbRatings");
        }
    }
}
