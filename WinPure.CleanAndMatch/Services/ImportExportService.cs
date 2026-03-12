using System.Diagnostics;

namespace WinPure.CleanAndMatch.Services;

internal class ImportExportService : IImportExportService
{
    public void Import(ExternalSourceTypes srcType)
    {
        var service = WinPureUiDependencyResolver.Resolve<IDataManagerService>();
        if (service.IsImportAllowed)
        {
            var frm = ImportExportWinFormFactory.GetImportForm(srcType);
            var importProvider = ImportExportDataFactory.GetImportDataInstance(srcType);
            if (frm.ShowImportForm(importProvider))
            {
                service.ImportData(importProvider);
            }
        }
        else
        {
            throw new WinPureLicenseException(Resources.EXCEPTION_IMPORT_DATA_NOT_ALLOWED);
        }
    }

    public void Export(ExternalSourceTypes srcType, DataTable data = null, string criteria = "", bool removeSystemFields = false)
    {
        if (WinPureUiDependencyResolver.Resolve<ILicenseService>().IsDemo)
        {

            if (XtraMessageBox.Show(Resources.EXCEPTION_EXPORT_100RECORDS_ON_DEMO, Resources.CAPTION_DEMO_LIMITATION, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start("https://winpure.com/request-custom-quote/");
            }

        }

        var service = WinPureUiDependencyResolver.Resolve<IDataManagerService>();

        var frmExp = ImportExportWinFormFactory.GetExportForm(srcType);
        var exportProvider = ImportExportDataFactory.GetExportDataInstance(srcType);
        if (frmExp.ShowExportForm(exportProvider))
        {
            service.ExportData(frmExp.ExportProvider, data, criteria, removeSystemFields);
        }
    }

    public static string GetExportParameter(ExternalSourceTypes externalSource)
    {
        var frmExp = ImportExportWinFormFactory.GetExportForm(externalSource);
        var exportService = ImportExportDataFactory.GetExportDataInstance(externalSource);
        if (frmExp.ShowExportForm(exportService))
        {
            return frmExp.ExportProvider.ExportParameters;
        }
        return "";
    }
}