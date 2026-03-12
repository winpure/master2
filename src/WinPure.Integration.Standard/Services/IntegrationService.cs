using System.Threading;
using WinPure.Common.Abstractions;
using WinPure.Common.Models;
using WinPure.Configuration.Helper;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.DependencyInjection;

internal static partial class WinPureIntegrationDependencyExtension
{
    private class IntegrationService : WinPureNotification, IIntegrationService
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IWpLogger _logger;
        private readonly ICleansingService _cleansingService;
        private readonly ICleansingConfigurationService _cleansingConfigurationService;

        public IntegrationService(IConnectionManager connectionManager, IWpLogger logger, ICleansingService cleansingService, ICleansingConfigurationService cleansingConfigurationService)
        {
            _connectionManager = connectionManager;
            _logger = logger;
            _cleansingService = cleansingService;
            _cleansingConfigurationService = cleansingConfigurationService;
        }

        public ImportedDataInfo ImportData(IImportProvider importProvider, string tableName)
        {
            try
            {
                importProvider.OnProgressUpdate += NotifyProgress;
                importProvider.OnException += NotifyException;

                var importedData = importProvider.ImportData();
                importedData.TableName = tableName;
                var importedInfo = importProvider.ImportedInfo;
                if (importedInfo.RowCount > 0)
                {
                    NotifyProgress(Resources.CAPTION_IO_SAVING_DATA_TO_LOCAL_DB, 60);
                    SaveData(importedData, importedInfo, true);
                    if (importedInfo.RowCount <= 50000)
                    {
                        NotifyProgress(Resources.CAPTION_CALCULATION_OF_STATISTIC, 80);
                        var statistic = _cleansingService.CalculateStatistic(importedData, importedInfo.Fields, new CancellationToken());
                        var statisticTableName = NameHelper.GetStatisticTable(tableName);
                        SqLiteHelper.SaveDataToDb(_connectionManager.Connection, statistic, statisticTableName, _logger, false);
                        importedInfo.IsStatisticCalculated = true;
                    }
                }

                _logger.Information($"Import from {importProvider.DisplayName} row count {importedInfo.RowCount} to table {tableName}");
                return importedInfo;
            }
            finally
            {
                importProvider.OnProgressUpdate -= NotifyProgress;
                importProvider.OnException -= NotifyException;
            }
        }

        public void ReimportData(IImportProvider importProvider, ImportedDataInfo importedDataInfo)
        {
            try
            {
                importProvider.OnProgressUpdate += NotifyProgress;
                importProvider.OnException += NotifyException;

                var importedData = importProvider.ReimportData(importedDataInfo.ImportParameters);

                if (importedData == null)
                {
                    return;
                }

                importedData.TableName = importedDataInfo.TableName;
                var importedInfo = importProvider.ImportedInfo;
                if (importedInfo.RowCount > 0)
                {
                    importedInfo.IsStatisticCalculated = false;
                    NotifyProgress(Resources.CAPTION_IO_SAVING_DATA_TO_LOCAL_DB, 60);
                    
                    SaveData(importedData, importedInfo, true);
                    importedDataInfo.RowCount = importedInfo.RowCount;
                    if (importedInfo.RowCount <= 50000)
                    {
                        NotifyProgress(Resources.CAPTION_CALCULATION_OF_STATISTIC, 80);
                        var statistic = _cleansingService.CalculateStatistic(importedData, importedInfo.Fields, new CancellationToken());
                        var statisticTableName = NameHelper.GetStatisticTable(importedDataInfo.TableName);
                        SqLiteHelper.UpdateTableColumnList(_connectionManager.Connection, importedDataInfo, importedData, true);
                        SqLiteHelper.SaveDataToDb(_connectionManager.Connection, statistic, statisticTableName, _logger, false);
                        
                        importedInfo.IsStatisticCalculated = true;
                    }
                }

                _logger.Information($"Reimport from {importProvider.DisplayName} row count {importedInfo.RowCount} to table {importedDataInfo.TableName}");
            }
            finally
            {
                importProvider.OnProgressUpdate -= NotifyProgress;
                importProvider.OnException -= NotifyException;
            }
        }

        public void ExportData(IExportProvider exportProvider, DataTable dataForExport, bool removeSystemFields = false)
        {
            try
            {
                exportProvider.OnProgressUpdate += NotifyProgress;
                exportProvider.OnException += NotifyException;
                if (removeSystemFields)
                {
                    ColumnHelper.RemoveSystemFieldsFromTable(dataForExport);
                }
                else
                {
                    ColumnHelper.RemoveWinPurePrimaryKeyFieldFromTable(dataForExport);
                }
                exportProvider.Export(dataForExport);
            }
            finally
            {
                exportProvider.OnProgressUpdate -= NotifyProgress;
                exportProvider.OnException -= NotifyException;
            }

        }

        private string SaveData(DataTable table, ImportedDataInfo importedInfo, bool createAdditionalTable)
        {
            table = SqLiteHelper.ConvertGuidColumnsToString(table);
            for (int i = 0; i < importedInfo.Fields.Count; i++)
            {
                if (!table.Columns.Contains(importedInfo.Fields[i].DatabaseName))
                {
                    importedInfo.Fields.RemoveAt(i);
                    continue;
                }
                int r = 1, j = 0;
                var nm = importedInfo.Fields[i].DisplayName;
                while (j < i)
                {
                    if (importedInfo.Fields[j].DatabaseName == nm)
                    {
                        nm = importedInfo.Fields[i].DisplayName + r++;
                        j = 0;
                    }
                    else
                    {
                        j++;
                    }
                }

                importedInfo.Fields[i].DatabaseName = nm;
                table.Columns[i].ColumnName = importedInfo.Fields[i].DatabaseName;
            }

            if (importedInfo.Fields.Count != table.Columns.Count)
            {
                throw new Exception("Wrong table import");
            }

            CheckFieldsType(table, importedInfo);

            var filedList = SqLiteHelper.SaveDataToDb(_connectionManager.Connection, table, table.TableName, _logger);

            if (createAdditionalTable)
            {
                _cleansingConfigurationService.FillCleansingConfigurationTables(table);
            }

            importedInfo.TableName = table.TableName;
            return filedList;
        }

        private void CheckFieldsType(DataTable table, ImportedDataInfo importedInfo)
        {
            var columns = new List<DataColumn>();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (importedInfo.Fields[i].FieldType != table.Columns[i].DataType.ToString() &&
                    importedInfo.Fields[i].FieldType == "System.String")
                {
                    columns.Add(table.Columns[i]);
                }
            }
            columns.ForEach(x =>
            {
                table.Columns.Add(new DataColumn(x.ColumnName + "New20", typeof(string)));
                foreach (DataRow rw in table.Rows)
                {
                    rw[x.ColumnName + "New20"] = rw[x.ColumnName];
                }
                table.Columns[x.ColumnName + "New20"].SetOrdinal(x.Ordinal);
                table.Columns.Remove(x);
                table.Columns[x.ColumnName + "New20"].ColumnName = x.ColumnName;

            });
        }


    }
}