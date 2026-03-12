using WinPure.DataService.Senzing.Models;

namespace WinPure.DataService.Senzing;

public static class SenzingHelper
{
    public static string GetSenzingFieldName(EntityResolutionRowConfiguration configuration)
    {
        if (configuration.IsIgnore)
        {
            return String.Empty;
        }

        if (configuration.IsInclude)
        {
            return configuration.ColumnName;
        }

        return string.IsNullOrWhiteSpace(configuration.Label)
             ? configuration.FieldType
             : $"{configuration.Label}_{configuration.FieldType}";
    }

    public static string GetPossibleRelatedView()
    {
        return $"SELECT * from [{NameHelper.EntityResolutionRelatedView}] ";
    }

    public static string GetPossibleDuplicatedView()
    {
        return $"SELECT  * from [{NameHelper.EntityResolutionDuplicatedView}]";
    }
    public static string GetPossibleRelatedCreateQuery()
    {
        return $"CREATE VIEW IF NOT EXISTS [{NameHelper.EntityResolutionRelatedView}] AS SELECT p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID}] AS [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID}], p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYKEY}] AS [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID_KEY}], 0 AS [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID}], '' AS [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID_KEY}], er.* FROM [{NameHelper.EntityResolutionTable}] AS er INNER JOIN [{NameHelper.EntityResolutionRelatedTable}] AS p ON p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYENTITYID}] = er.[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] AND p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}] = {(int)PossibilityType.Related} ";
    }

    public static string GetPossibleDuplicatedCreateQuery()
    {
        return $"CREATE VIEW IF NOT EXISTS [{NameHelper.EntityResolutionDuplicatedView}] AS SELECT 0 AS [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID}], '' AS [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID_KEY}], p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID}] AS [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID}], p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYKEY}] AS [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID_KEY}], er.* FROM [{NameHelper.EntityResolutionTable}] AS er INNER JOIN [{NameHelper.EntityResolutionRelatedTable}] AS p ON p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYENTITYID}] = er.[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] AND p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}] = {(int)PossibilityType.Duplicated} ";
    }
}