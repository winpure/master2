using System.Data;

namespace WinPure.DataService.Services;

internal interface IImportedDataManagementService
{
    DataTable GetPreview(ImportedDataInfo importedDataInfo);
    void AddTableColumn(ImportedDataInfo importedDataInfo, string columnName, string columnType);
    void RenameColumn(ImportedDataInfo importedDataInfo, List<DataField> databaseColumns);
    void CopyColumn(ImportedDataInfo importedDataInfo, List<DataField> databaseColumns);
    void RemoveColumn(ImportedDataInfo importedDataInfo, List<DataField> databaseColumns);
}