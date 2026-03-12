using Microsoft.EntityFrameworkCore.Migrations;

namespace WinPure.Configuration.Migrations
{
    public partial class AddStatisticPattern : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatisticPatterns",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Pattern = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FieldType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticPatterns", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatisticPatterns_Pattern",
                table: "StatisticPatterns",
                column: "Pattern",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatisticPatterns");
        }
    }
}
