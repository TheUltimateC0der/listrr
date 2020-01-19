using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class AddedDonors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDonor",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsDonor",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
