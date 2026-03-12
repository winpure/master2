using Newtonsoft.Json;

namespace WinPure.DataService.Models;

[Serializable]
internal class ProjectSettings
{
    public ProjectSettings(string projectVersion)
    {
        GenerateNewId();
        TableList = new List<ImportedDataInfo>();
        Version = projectVersion;
    }

    public void InitiateSettings(ProjectSettings sett)
    {
        TableList = sett.TableList;
        CurrentTable = sett.CurrentTable;
        ProjectName = sett.ProjectName;
        ProjectPath = sett.ProjectPath;
        Id = string.IsNullOrEmpty(sett.Id) ? Guid.NewGuid().ToString() : sett.Id;
        Version = sett.Version;
    }

    public void ResetSettings()
    {
        ProjectPath = "";
        ProjectName = "";
        Id = Guid.NewGuid().ToString();
    }

    public void GenerateNewId()
    {
        Id = Guid.NewGuid().ToString();
    }

    public string Id { get; set; }
    public string ProjectName { get; set; }
    public string ProjectPath { get; set; }
    public List<ImportedDataInfo> TableList { get; set; }
    public string CurrentTable { get; set; }
    public string Version { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}