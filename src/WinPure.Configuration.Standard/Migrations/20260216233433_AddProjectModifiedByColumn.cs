using Microsoft.EntityFrameworkCore.Migrations;

namespace WinPure.Configuration.Migrations
{
    public partial class AddProjectModifiedByColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "RecentProjects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "RecentProjects");
        }
    }
}
