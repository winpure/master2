using System.Threading;
using System.Threading.Tasks;
using WinPure.Common.Enums;
using WinPure.Matching.Models;

namespace WinPure.Project.Services;

internal interface IProjectService
{
    bool IsNewProject { get; }

    event Action OnAfterProjectLoad;
    event Action OnBeforeProjectLoad;
    event Action<string> OnProjectNameChanged;
    event Action OnCleanedProject;

    event Action<string, Task, bool, CancellationTokenSource> OnProgressShow;
    event Action<string, string, MessagesType, Exception> OnException;

    void LoadProjectAsync(string fileName);
    bool CloseCurrentAndCreateNewProject();
    void SaveProjectAsync(string projectName, bool saveAs, string projectPath = "");
    void SetProjectName(string projectName);
    void PreLoadProject(IConnectionManager connManager, ProjectSettings settings, MatchSettingsViewModel matchSettings);
    void LoadProjectData(string projectPath);
}