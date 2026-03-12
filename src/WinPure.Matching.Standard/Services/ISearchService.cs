using System.Data;
using System.Threading;
using WinPure.Matching.Models;

namespace WinPure.Matching.Services;

internal interface ISearchService
{
    DataTable SearchData(TableParameter table, SearchParameter parameter, CancellationToken cToken, Action<string, int> raiseOnProgressUpdate, int recordsToProcess = -1);
}