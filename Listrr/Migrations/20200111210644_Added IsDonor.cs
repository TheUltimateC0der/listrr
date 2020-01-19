using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class AddedIsDonor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDonor",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDonor",
                table: "AspNetUsers");
        }
    }
}
