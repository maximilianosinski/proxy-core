using Knapcode.TorSharp;

namespace ProxyCore;

public class TorRelay
{
    public TorSharpProxy TorProxy { get; }
    public string Host { get; }
    public int Port { get; }
    public TorRelay(TorSharpProxy torProxy, string host, int port)
    {
        TorProxy = torProxy;
        Host = host;
        Port = port;
    }
}