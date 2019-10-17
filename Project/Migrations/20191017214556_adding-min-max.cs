using Microsoft.EntityFrameworkCore.Migrations;

namespace TestProject.Migrations
{
    public partial class addingminmax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResponseTime",
                table: "Pages",
                newName: "MinResponseTime");

            migrationBuilder.AddColumn<double>(
                name: "MaxResponseTime",
                table: "Pages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxResponseTime",
                table: "Pages");

            migrationBuilder.RenameColumn(
                name: "MinResponseTime",
                table: "Pages",
                newName: "ResponseTime");
        }
    }
}
