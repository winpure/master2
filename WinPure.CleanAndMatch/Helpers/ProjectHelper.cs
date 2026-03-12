namespace WinPure.CleanAndMatch.Helpers;

internal static class ProjectHelper
{
    internal static void SaveProject(bool saveAs)
    {
        var service = WinPureUiDependencyResolver.Resolve<IDataManagerService>();
        var projectService = WinPureUiDependencyResolver.Resolve<IProjectService>();
        var projectName = string.IsNullOrEmpty(service.ProjectName) ? Resources.CAPTION_DEFAULT_PROJECT_NAME : service.ProjectName;
        if (saveAs || projectService.IsNewProject)
        {
            var projPath = GetProjectFileName(service, saveAs);
            if (saveAs || projectName == Resources.CAPTION_DEFAULT_PROJECT_NAME)
            {
                projectName = Path.GetFileNameWithoutExtension(projPath);
            }
            if (!string.IsNullOrEmpty(projPath))
            {
                projectService.SaveProjectAsync(projectName, saveAs, projPath);
            }
        }
        else
        {
            projectService.SaveProjectAsync(projectName, saveAs);
        }
    }

    private static string GetProjectFileName(IDataManagerService service, bool saveAs)
    {
        var sv = new SaveFileDialog
        {
            Filter = Resources.DIALOG_PROJECT_FORMAT,
            FileName = service.ProjectName,
            ValidateNames = true,
            OverwritePrompt = true,
            Title = saveAs ? Resources.DIALOG_PROJECT_SAVEAS_CAPTION : Resources.DIALOG_PROJECT_SAVE_CAPTION
        };

        return sv.ShowDialog() == DialogResult.OK ? sv.FileName : null;
    }
}