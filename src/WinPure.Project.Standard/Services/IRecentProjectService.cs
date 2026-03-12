using System.Collections.Generic;
using System.Threading.Tasks;
using WinPure.Project.Models;

namespace WinPure.Project.Services;

internal interface IRecentProjectService
{
    event Action OnRecentListChanged;

    Task AddOrUpdateProjectAsync(ProjectSettings project);
    bool CheckProjectIfExist(RecentProject project);
    Task<RecentProject> GetRecentProjectByPath(string projectPath);
    Task<List<RecentProject>> GetRecentProjectsAsync();
    Task RemoveProjectAsync(RecentProject project);
    void RemoveProject(RecentProject project);
    Task RenameProjectAsync(RecentProject project);
    void RenameProject(RecentProject project);
}