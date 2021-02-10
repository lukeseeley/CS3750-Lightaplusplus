using Microsoft.EntityFrameworkCore.Migrations;

namespace Lightaplusplus.Migrations
{
    public partial class AddedUserType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "usertype",
                table: "Users",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "usertype",
                table: "Users");
        }
    }
}
