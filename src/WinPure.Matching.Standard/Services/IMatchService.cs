using System.Data;
using System.Threading;
using WinPure.Matching.Models;

namespace WinPure.Matching.Services;

internal interface IMatchService
{
    DataTable MatchData(List<TableParameter> tables, MatchParameter parameter, List<FieldMapping> fieldMap, CancellationToken cToken, Action<string, int> raiseOnProgressUpdate, int recordsToProcess = -1);
}