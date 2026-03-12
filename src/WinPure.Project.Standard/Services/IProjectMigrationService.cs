using WinPure.Matching.Models;

namespace WinPure.Project.Services;

internal interface IProjectMigrationService
{
    void Down(IConnectionManager connectionManager, ProjectSettings projectSettings, MatchSettingsViewModel matchSettings, MatchParameter matchParameter);
    void Up(IConnectionManager connectionManager, ProjectSettings projectSettings, MatchSettingsViewModel matchSettings, MatchParameter matchParameter);
}