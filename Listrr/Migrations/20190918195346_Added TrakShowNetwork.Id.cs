using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class AddedTrakShowNetworkId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "TraktShowNetworks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "TraktShowNetworks");
        }
    }
}
