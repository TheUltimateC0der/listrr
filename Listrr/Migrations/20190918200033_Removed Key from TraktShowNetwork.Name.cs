using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class RemovedKeyfromTraktShowNetworkName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TraktShowNetworks",
                table: "TraktShowNetworks");

            migrationBuilder.DropIndex(
                name: "IX_TraktShowNetworks_Name",
                table: "TraktShowNetworks");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TraktShowNetworks",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TraktShowNetworks",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TraktShowNetworks",
                table: "TraktShowNetworks",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TraktShowNetworks",
                table: "TraktShowNetworks");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TraktShowNetworks",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TraktShowNetworks",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TraktShowNetworks",
                table: "TraktShowNetworks",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TraktShowNetworks_Name",
                table: "TraktShowNetworks",
                column: "Name",
                unique: true);
        }
    }
}
