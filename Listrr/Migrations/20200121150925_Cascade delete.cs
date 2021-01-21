using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class Cascadedelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TraktLists_AspNetUsers_OwnerId",
                table: "TraktLists");

            migrationBuilder.AddForeignKey(
                name: "FK_TraktLists_AspNetUsers_OwnerId",
                table: "TraktLists",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TraktLists_AspNetUsers_OwnerId",
                table: "TraktLists");

            migrationBuilder.AddForeignKey(
                name: "FK_TraktLists_AspNetUsers_OwnerId",
                table: "TraktLists",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
