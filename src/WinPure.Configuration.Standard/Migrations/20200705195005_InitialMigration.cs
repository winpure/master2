using Microsoft.EntityFrameworkCore.Migrations;

namespace WinPure.Configuration.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutomationHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConfigurationName = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutomationStepsDictionary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationStepsDictionary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DictionaryName",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DictionaryName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryName", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecentProjects",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ProjectName = table.Column<string>(nullable: false),
                    ProjectPath = table.Column<string>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false, defaultValueSql: "datetime('now', 'localtime')"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false, defaultValueSql: "datetime('now', 'localtime')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutomationSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduleName = table.Column<string>(nullable: false),
                    ConfigurationId = table.Column<int>(nullable: false),
                    ScheduleType = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Frequency = table.Column<int>(nullable: true),
                    DayOfWeek = table.Column<string>(nullable: true),
                    DayOfMonth = table.Column<short>(nullable: true),
                    WeeklyTime = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutomationSchedules_AutomationHeaders_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalTable: "AutomationHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutomationConfigurationSteps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConfigurationId = table.Column<int>(nullable: false),
                    StepId = table.Column<int>(nullable: false),
                    SourceName = table.Column<string>(nullable: false),
                    Parameter1 = table.Column<string>(nullable: true),
                    Parameter2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationConfigurationSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutomationConfigurationSteps_AutomationHeaders_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalTable: "AutomationHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutomationConfigurationSteps_AutomationStepsDictionary_StepId",
                        column: x => x.StepId,
                        principalTable: "AutomationStepsDictionary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DictionaryData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DictionaryId = table.Column<int>(nullable: false),
                    SearchValue = table.Column<string>(nullable: false),
                    ReplaceValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DictionaryData_DictionaryName_DictionaryId",
                        column: x => x.DictionaryId,
                        principalTable: "DictionaryName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutomationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduleId = table.Column<int>(nullable: false),
                    DateOfRun = table.Column<DateTime>(nullable: false),
                    Successful = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    ExecutionTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutomationLogs_AutomationSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "AutomationSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutomationConfigurationSteps_ConfigurationId",
                table: "AutomationConfigurationSteps",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomationConfigurationSteps_StepId",
                table: "AutomationConfigurationSteps",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomationLogs_ScheduleId",
                table: "AutomationLogs",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomationSchedules_ConfigurationId",
                table: "AutomationSchedules",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryData_DictionaryId",
                table: "DictionaryData",
                column: "DictionaryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutomationConfigurationSteps");

            migrationBuilder.DropTable(
                name: "AutomationLogs");

            migrationBuilder.DropTable(
                name: "DictionaryData");

            migrationBuilder.DropTable(
                name: "RecentProjects");

            migrationBuilder.DropTable(
                name: "AutomationStepsDictionary");

            migrationBuilder.DropTable(
                name: "AutomationSchedules");

            migrationBuilder.DropTable(
                name: "DictionaryName");

            migrationBuilder.DropTable(
                name: "AutomationHeaders");
        }
    }
}
