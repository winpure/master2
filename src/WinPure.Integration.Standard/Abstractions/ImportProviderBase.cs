using WinPure.Common.Constants;
using WinPure.Common.Models;
using WinPure.Configuration.DependencyInjection;
using WinPure.Integration.Enums;

namespace WinPure.Integration.Abstractions;

internal abstract class ImportProviderBase : ImportExportProviderBase, IImportProvider
{
    protected const int _MAX_ROW_TO_PREVIEW = 20;
    protected readonly int _MAX_ROW_TO_IMPORT;
    protected DataTable _previewTable;
    protected ImportProviderBase(ExternalSourceTypes sourceType) : base(OperationType.Import, sourceType)
    {
        ImportedInfo = new ImportedDataInfo { SourceType = sourceType };
        var licenseService = WinPureConfigurationDependency.Resolve<ILicenseService>();
        _MAX_ROW_TO_IMPORT = GlobalConstants.ImportRowLimitForProgram(licenseService.ProgramType, licenseService.IsDemo);
        _previewTable = null;
    }

    public bool LimitedRecords { get; internal set; }
    public ImportedDataInfo ImportedInfo { get; internal set; }

    public virtual ImportedDataInfo SelectFields()
    {
        try
        {
            var table = _previewTable ?? GetPreview(_MAX_ROW_TO_PREVIEW);
            ImportedInfo.Fields = ColumnHelper.GetTableFields(table);
            return ImportedInfo;
        }
        catch (Exception ex)
        {
            _logger.Debug($"Cannot get fields from {DisplayName}.", ex);
            NotifyException(Resources.EXCEPTION_IO_ERROR_ON_IMPORT, ex);
        }

        return null;
    }

    protected abstract DataTable GetPreview(int rowToPreview);

    [STAThread]
    public virtual DataTable GetPreview()
    {
        try
        {
            _previewTable = GetPreview(_MAX_ROW_TO_PREVIEW);
            return _previewTable;
        }
        catch (Exception ex)
        {
            _logger.Debug($"PREVIEW FROM {DisplayName} ERROR", ex);
            NotifyException(Resources.EXCEPTION_IO_ERROR_ON_IMPORT, ex);
        }
        return null;
    }

    protected abstract DataTable GetData();

    public abstract DataTable ImportData();

    protected abstract DataTable GetReimportData(string parameterJson);

    public virtual DataTable ReimportData(string parametersJson)
    {
        try
        {
            return GetReimportData(parametersJson);
        }
        catch (Exception ex)
        {
            _logger.Debug($"REIMPORT FROM {DisplayName} ERROR", ex);
            NotifyException(Resources.EXCEPTION_IO_ERROR_ON_IMPORT, ex);
        }

        return null;
    }

    protected List<int> RemoveSkippedField(DataTable dt, IList<DataField> fields)
    {
        var res = new List<int>();
        var colToDelete = new List<DataColumn>();
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            if (fields.All(x => x.DatabaseName != dt.Columns[i].ColumnName))
            {
                res.Add(i);
                colToDelete.Add(dt.Columns[i]);
            }
        }
        if (colToDelete.Any())
        {
            foreach (DataColumn col in colToDelete)
            {
                dt.Columns.Remove(col);
            }
        }
        return res;
    }
}