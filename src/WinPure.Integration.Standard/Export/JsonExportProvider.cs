using Newtonsoft.Json;

namespace WinPure.Integration.Export;

internal class JsonExportProvider : FileExportProviderBase
{
    public JsonExportProvider() : base(ExternalSourceTypes.Json)
    {
        DisplayName = "JSON";
    }

    protected override string GetFileContent(DataTable data)
    {
        return JsonConvert.SerializeObject(data);
    }
}