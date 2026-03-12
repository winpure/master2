using System.Threading;
using WinPure.Common.Abstractions;
using WinPure.Common.Enums;
using WinPure.DataService.Senzing.Models;

namespace WinPure.DataService.Senzing;

internal interface ISenzingService : IWinPureNotification
{
    List<FieldType> GetFieldTypes();
    List<MapError> VerifyRowConfigurations(string tableName, ExternalSourceTypes sourceType, List<EntityResolutionRowConfiguration> rows);
    EntityResolutionReport RunAnalyze(string dbPath, List<EntityResolutionConfiguration> configurations, CancellationToken cToken);
    void MapColumns(List<EntityResolutionRowConfiguration> rows);
    void ExportErSettings(List<EntityResolutionRowConfiguration> settings, string fileName);
    List<EntityResolutionRowConfiguration> ImportErSettings(string destinationFile);
}