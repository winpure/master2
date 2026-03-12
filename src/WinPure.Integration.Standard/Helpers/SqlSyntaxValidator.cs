using System.Data.Common;

public static class SqlSyntaxValidator
{
    /// <summary>
    /// Validates SQL syntax for the given DbConnection (SQL Server, PostgreSQL, MySQL/MariaDB, Oracle).
    /// Works in both .NET Framework 4.8 and .NET 8.
    /// Returns (isValid, errorMessage).
    /// </summary>
    public static (bool isValid, string error) Validate(DbConnection connection, string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return (false, "Empty SQL string.");

        string provider = connection.GetType().FullName ?? string.Empty;
        string commandText;

        // Provider-specific validation logic
        if (provider.IndexOf("SqlClient", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            // SQL Server: compile but don't execute
            commandText = $"SET NOEXEC ON; {sql}; SET NOEXEC OFF;";
        }
        else if (provider.IndexOf("Npgsql", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            // PostgreSQL: prepare/deallocate to test parsing
            commandText = $"PREPARE _v_stmt AS {sql}; DEALLOCATE _v_stmt;";
        }
        else if (provider.IndexOf("MySql", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 provider.IndexOf("MariaDb", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            // MySQL / MariaDB: EXPLAIN for DML, otherwise PREPARE
            string trimmed = sql.TrimStart();
            if (trimmed.StartsWith("select", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("insert", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("update", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("delete", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("replace", StringComparison.OrdinalIgnoreCase))
            {
                commandText = $"EXPLAIN {sql}";
            }
            else
            {
                string safe = sql.Replace("'", "''");
                commandText = $"PREPARE _v_stmt FROM '{safe}'; DEALLOCATE PREPARE _v_stmt;";
            }
        }
        else if (provider.IndexOf("Oracle", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            // Oracle: DBMS_SQL.PARSE to validate syntax
            commandText = $@"DECLARE c INTEGER;
                                BEGIN
                                  c := DBMS_SQL.OPEN_CURSOR;
                                  DBMS_SQL.PARSE(c, q'[{sql}]', DBMS_SQL.NATIVE);
                                  DBMS_SQL.CLOSE_CURSOR(c);
                                END;";
        }
        else
        {
            return (false, $"Unsupported provider: {provider}");
        }

        try
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.ExecuteNonQuery();
            }
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
