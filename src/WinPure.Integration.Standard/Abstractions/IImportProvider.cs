using WinPure.Common.Models;

namespace WinPure.Integration.Abstractions;

internal interface IImportProvider : IImportExportProvider
{
    DataTable GetPreview();
    DataTable ImportData();

    bool LimitedRecords { get; }

    DataTable ReimportData(string parametersJson);
    ImportedDataInfo SelectFields();

    ImportedDataInfo ImportedInfo { get; }
}