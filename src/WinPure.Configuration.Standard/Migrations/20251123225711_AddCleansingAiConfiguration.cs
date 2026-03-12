using Microsoft.EntityFrameworkCore.Migrations;

namespace WinPure.Configuration.Migrations
{
    public partial class AddCleansingAiConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CleansingAiConfigurations",
                columns: table => new
                {
                    AiType = table.Column<string>(nullable: false),
                    MappedFields = table.Column<string>(type: "TEXT", nullable: false),
                    Options = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleansingAiConfiguration", x => x.AiType);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CleansingAiConfigurations");
        }
    }
}
