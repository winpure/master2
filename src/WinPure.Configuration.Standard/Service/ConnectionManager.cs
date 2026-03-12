using System.Data;
using System.Data.SQLite;
using System.IO;
using WinPure.Configuration.Enums;

namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    internal static IConnectionManager CreateConnectionManager() => new ConnectionManager();

    private class ConnectionManager : IConnectionManager
    {
        internal DatabaseKind DbKind;

        private SQLiteConnection _dbConnection;
        private string _dbPath;
        
        public string DbPath => _dbPath;

        public ConnectionManager()
        {
            DbKind = DatabaseKind.Project;
        }

        [ActivatorUtilitiesConstructor]
        public ConnectionManager(IServiceProvider serviceProvider)
        {
            DbKind = DatabaseKind.Project;
            Configure(serviceProvider);
        }

        public void Initialize(string dbPath)
        {
            if (string.IsNullOrWhiteSpace(dbPath))
            {
                throw new ArgumentNullException(nameof(dbPath));
            }
            _dbPath = dbPath;
            CheckDb();
        }

        public SQLiteConnection Connection => _dbConnection;

        public void CloseConnection()
        {
            if (_dbConnection?.State == ConnectionState.Open)
            {
                _dbConnection?.Close();
            }
        }

        public void CheckConnectionState()
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
        }

        public string GetDbSize()
        {
            return FileHelper.GetDatabaseSize(_dbPath);
        }

        internal void Configure(IServiceProvider serviceProvider)
        {
            var configurationService = serviceProvider.GetRequiredService<IConfigurationService>();

            var dbConfiguration = DbKind == DatabaseKind.Project
                ? configurationService.Configuration.ProjectDatabase
                : configurationService.Configuration.LogDatabase;

            var dbPath = SystemDatabaseConnectionHelper.GetDatabasePath(dbConfiguration.UseRelativePath, dbConfiguration.Path, dbConfiguration.Name);
            Initialize(dbPath);
        }

        private void CheckDb()
        {
            if (!File.Exists(_dbPath))
            {
                var folderPath = Path.GetDirectoryName(_dbPath);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                FileStream fs = File.Create(_dbPath);
                fs.Close();
            }

            if (_dbConnection == null)
            {
                var connectionString = SystemDatabaseConnectionHelper.GetConnectionString(_dbPath);
                _dbConnection = new SQLiteConnection(connectionString);
            }
        }

        public void Dispose()
        {
            try
            {
                CloseConnection();
                _dbConnection?.Dispose();
            }
            catch
            {
                // ignored
            }
        }
    }
}