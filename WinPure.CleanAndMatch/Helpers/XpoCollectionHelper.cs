using System.Runtime.CompilerServices;

namespace WinPure.CleanAndMatch.Helpers;

internal static class XpoCollectionHelper
{
    private static readonly ConditionalWeakTable<Session, XpoDbContext> Owners = new();

    public static void Register(Session session, XpoDbContext owner) => Owners.Add(session, owner);

    public static void DisposeWithOwner(object dataSource)
    {
        if (dataSource is not XPServerCollectionSource ds) 
            return;

        try
        {
            if (Owners.TryGetValue(ds.Session, out var owner))
            {
                ds.Dispose();
                owner.Dispose();
                Owners.Remove(ds.Session);
                return;
            }

            ds.Dispose();
            if (ds.Session.IsConnected) ds.Session.Disconnect();
            ds.Session.Dispose();
            ds.Session.DataLayer?.Connection?.Close();
            ds.Session.DataLayer?.Connection?.Dispose();
        }
        catch (Exception ex) 
        {
            // ignore
        }
    }
}