using WinPure.Common.Abstractions;
using WinPure.Common.Models;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.Services;

internal interface IIntegrationService : IWinPureNotification
{
    ImportedDataInfo ImportData(IImportProvider importProvider, string tableName);
    void ReimportData(IImportProvider importProvider, ImportedDataInfo importedDataInfo);
    void ExportData(IExportProvider exportProvider, DataTable dataForExport, bool removeSystemFields = false);
}