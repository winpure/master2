using Microsoft.EntityFrameworkCore.Migrations;

namespace WinPure.Configuration.Migrations
{
    public partial class AddEntityResolutionMappingSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityResolutionMapping",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataColumnName = table.Column<string>(nullable: false),
                    EntityType = table.Column<string>(nullable: true),
                    ExactMatch = table.Column<bool>(nullable: false),
                    UsageGroup = table.Column<string>(nullable: true),
                    ConflictEntityTypes = table.Column<string>(nullable: true),
                    PrerequisiteEntityTypes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityResolutionMapping", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityResolutionMapping");
        }
    }
}
