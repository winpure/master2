using Renci.SshNet;
using System.Reflection;

namespace WinPure.Integration.Helpers;

internal class SshTunnel : IDisposable
{
    private readonly SshClient _client;
    private readonly ForwardedPortLocal _port;
    private uint _localPort = 22;

    public SshTunnel(string sshServer, string sshLogin, string sshPassword, string mySqlServer, uint mySqlPort)
    {
        try
        {
            var ci = new PasswordConnectionInfo(sshServer, sshLogin, sshPassword);
            _client = new SshClient(ci);
            _port = new ForwardedPortLocal("127.0.0.1", _localPort, mySqlServer, mySqlPort);

            _client.ErrorOccurred += (s, args) => { Console.WriteLine("_client error: " + args.Exception.Message); };
            _port.Exception += (s, args) => { Console.WriteLine("_port error: " + args.Exception.Message); };
            _port.RequestReceived += (s, args) => { Console.WriteLine("_port request received. " + args.OriginatorHost); };

            _client.Connect();
            _client.AddForwardedPort(_port);
            _port.Start();

            // Hack to allow dynamic local ports, ForwardedPortLocal should expose _listener.LocalEndpoint
            var listener = typeof(ForwardedPortLocal).GetField("_listener", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(_port);
            if (listener != null)
            {
                //LocalPort = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            }
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public uint BoundPort => _port.BoundPort;
    public string BoundHost => _port.BoundHost;

    public void Dispose()
    {
        _port?.Dispose();
        _client?.Dispose();
    }
}