using System.Threading;

namespace WinPure.Cleansing.Services;

internal interface ICleansingService
{
    /// <summary>
    /// In-memory data cleansing
    /// </summary>
    /// <param name="settings">WinPure clean settings</param>
    /// <param name="data">DataTable with shource data for cleansing.</param>
    /// <param name="cancellationToken">Cancellation token for interruption of the cleansing process</param>
    void CleanTable(DataTable data, WinPureCleanSettings settings, CancellationToken cancellationToken);

    /// <summary>
    /// In-memory calculation of statistic for given data
    /// </summary>
    /// <param name="data">DataTable with source data</param>
    /// <param name="cancellationToken">Cancellation token for interruption of the cleansing process</param>
    /// <returns>DataTable with WinPure statistic</returns>
    DataTable CalculateStatistic(DataTable data, List<DataField> dataFields, CancellationToken cancellationToken);
}