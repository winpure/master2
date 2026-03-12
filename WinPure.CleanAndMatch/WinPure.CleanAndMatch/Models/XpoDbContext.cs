using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace WinPure.CleanAndMatch.Models;

internal sealed class XpoDbContext : IDisposable
{
    public ReflectionDictionary Dictionary { get; }
    public IDataLayer DataLayer { get; }
    public Session Session { get; }

    private XpoDbContext(
        ReflectionDictionary dictionary,
        IDataLayer dataLayer,
        Session session)
    {
        Dictionary = dictionary;
        DataLayer = dataLayer;
        Session = session;
    }

    /// <summary>
    /// Creates an XPO context by first allowing the caller to build/modify the dictionary,
    /// and only then constructing the DataLayer/Session. This order avoids T901732.
    /// </summary>
    public static XpoDbContext Create(string connectionString, Action<ReflectionDictionary> buildModel)
    {
        // 1) Build dictionary first
        var dict = new ReflectionDictionary();
        buildModel?.Invoke(dict); // ← AddClass calls happen here

        // 2) Create provider and DL AFTER dictionary is finalized
        var provider = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.SchemaAlreadyExists);
        var dataLayer = new SimpleDataLayer(dict, provider);
        var session = new Session(dataLayer);

        return new XpoDbContext(dict, dataLayer, session);
    }

    public void Dispose()
    {
        try
        {
            if (Session.IsConnected) Session.Disconnect();
            Session.Dispose();
            DataLayer.Connection?.Close();
            DataLayer.Dispose();
        }
        catch
        {
            // swallow or log
        }
    }
}