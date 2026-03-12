using System.Data.SQLite;

namespace WinPure.Configuration.Service
{
    public interface IConnectionManager : IDisposable
    {
        string DbPath { get; }
        void Initialize(string dbPath);
        SQLiteConnection Connection { get; }
        void CloseConnection();
        void CheckConnectionState();
        string GetDbSize();
    }
}