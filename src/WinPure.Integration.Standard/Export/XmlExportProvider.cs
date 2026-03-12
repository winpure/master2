using System.IO;

namespace WinPure.Integration.Export;

internal class XmlExportProvider : FileExportProviderBase
{
    public XmlExportProvider() : base(ExternalSourceTypes.Xml)
    {
        DisplayName = "XML";
    }

    public override void Export(DataTable data)
    {
        try
        {
            if (!CheckExportData(data) || !CheckFilePath())
            {
                return;
            }

            FileHelper.SafeDeleteFile(Options.FilePath);

            using (var fileStream = new FileStream(Options.FilePath, FileMode.CreateNew))
            {
                using (var streamWriter = new StreamWriter(fileStream, FileHelper.GetEncoding(Options.CodePage)))
                {
                    data.WriteXml(streamWriter, XmlWriteMode.IgnoreSchema);
                }
                //fileStream.Flush();
            }

        }
        catch (Exception ex)
        {
            _logger.Error($"EXPORT TO {DisplayName} ERROR", ex);
            NotifyException(string.Format(Resources.EXCEPTION_IO_DATA_COULD_NOT_BE_EXPORTED_TO, "XML file"), ex);
        }
    }

    protected override string GetFileContent(DataTable data)
    {
        throw new NotImplementedException();
    }
}