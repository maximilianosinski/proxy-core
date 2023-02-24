using Knapcode.TorSharp;

namespace ProxyCore;

public class TorRelay
{
    public TorSharpProxy TorProxy { get; }
    public Uri Host { get; }

    public TorRelay(TorSharpProxy torProxy, Uri host)
    {
        TorProxy = torProxy;
        Host = host;
    }
}