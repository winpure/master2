using WinPure.Common.Models;

namespace WinPure.Integration.Models.ImportExportOptions;

[Serializable]
public abstract class BaseImportExportOptions
{
    public List<DataField> Fields { get; set; }
}