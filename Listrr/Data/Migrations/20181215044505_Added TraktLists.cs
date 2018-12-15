using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Data.Migrations
{
    public partial class AddedTraktLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TraktLists",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Slug = table.Column<string>(nullable: true),
                    Process = table.Column<bool>(nullable: false),
                    LastProcessed = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<string>(nullable: true),
                    Filter_Title = table.Column<string>(nullable: true),
                    Filter_Tagline = table.Column<string>(nullable: true),
                    Filter_Overview = table.Column<string>(nullable: true),
                    Filter_People = table.Column<string>(nullable: true),
                    Filter_Translations = table.Column<string>(nullable: true),
                    Filter_Aliases = table.Column<string>(nullable: true),
                    Filter_Years = table.Column<string>(nullable: true),
                    Filter_Runtimes = table.Column<string>(nullable: true),
                    Filter_Ratings = table.Column<string>(nullable: true),
                    Filter_Languages = table.Column<string>(nullable: true),
                    Filter_Genres = table.Column<string>(nullable: true),
                    Filter_Countries = table.Column<string>(nullable: true),
                    Filter_Certifications = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraktLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraktLists_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TraktLists_OwnerId",
                table: "TraktLists",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TraktLists");
        }
    }
}
