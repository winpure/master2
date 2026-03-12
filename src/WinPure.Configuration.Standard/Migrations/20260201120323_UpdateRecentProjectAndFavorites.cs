using Microsoft.EntityFrameworkCore.Migrations;

namespace WinPure.Configuration.Migrations
{
    public partial class UpdateRecentProjectAndFavorites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAddressVerification",
                table: "RecentProjects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAuditLog",
                table: "RecentProjects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutomation",
                table: "RecentProjects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCleansing",
                table: "RecentProjects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMatch",
                table: "RecentProjects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMatchAi",
                table: "RecentProjects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDataset",
                table: "RecentProjects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfRecords",
                table: "RecentProjects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FavouriteSources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Source = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteSources", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavouriteSources");

            migrationBuilder.DropColumn(
                name: "IsAddressVerification",
                table: "RecentProjects");

            migrationBuilder.DropColumn(
                name: "IsAuditLog",
                table: "RecentProjects");

            migrationBuilder.DropColumn(
                name: "IsAutomation",
                table: "RecentProjects");

            migrationBuilder.DropColumn(
                name: "IsCleansing",
                table: "RecentProjects");

            migrationBuilder.DropColumn(
                name: "IsMatch",
                table: "RecentProjects");

            migrationBuilder.DropColumn(
                name: "IsMatchAi",
                table: "RecentProjects");

            migrationBuilder.DropColumn(
                name: "NumberOfDataset",
                table: "RecentProjects");

            migrationBuilder.DropColumn(
                name: "NumberOfRecords",
                table: "RecentProjects");
        }
    }
}
