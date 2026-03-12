namespace WinPure.CleanAndMatch.Services;

public interface IImportExportService
{
    void Import(ExternalSourceTypes srcType);
    void Export(ExternalSourceTypes srcType, DataTable data = null, string criteria = "", bool removeSystemFields = false);
}