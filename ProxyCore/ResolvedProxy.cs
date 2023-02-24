using System.Net;

namespace ProxyCore;

public class ResolvedProxy
{
    public Uri Host { get; }
    public NetworkCredential? Credentials { get; }
    public long AveragePing;
    private long _totalPingCount;
    private int _attemptedPingAnalyses;
    private Thread? _pingAnalysesThread;
    public GeoLocation Location { get; }

    public ResolvedProxy(Uri host, string city, string country, NetworkCredential? credentials = null)
    {
        Host = host;
        Credentials = credentials;
        Location = new GeoLocation(city, country);
    }

    public class PingResult : EventArgs
    {
        public int Latency { get; }

        public PingResult(int latency)
        {
            Latency = latency;
        }
    }

    public event EventHandler<PingResult> Ping; 

    public void StartPingAnalyses(int pollingRate = 5000)
    {
        if (_pingAnalysesThread != null) throw new Exception("Analyses already running.");
        _pingAnalysesThread = new Thread(async () =>
        {
            while (true)
            {
                try
                {
                    var proxy = new WebProxy
                    {
                        Address = Host,
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false
                    };

                    var httpClientHandler = new HttpClientHandler
                    {
                        Proxy = proxy,
                        UseProxy = true,
                        PreAuthenticate = Credentials != null,
                        UseDefaultCredentials = false
                    };

                    if (Credentials != null)
                    {
                        httpClientHandler.Proxy.Credentials = Credentials;
                        httpClientHandler.Credentials = Credentials;
                    }

                    var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
                    await client.GetStringAsync("http://httpbin.org/ip");
                    var latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start;
                    _attemptedPingAnalyses++;
                    _totalPingCount += latency;
                    AveragePing = _totalPingCount / _attemptedPingAnalyses;
                    Ping(null, new PingResult((int)latency));
                    await Task.Delay(pollingRate);
                }
                catch
                {
                    // ignored
                }
            }
        });
        _pingAnalysesThread.Start();
    }

    public void StopPingAnalyses()
    {
        if (_pingAnalysesThread == null) throw new Exception("No current analyses running.");
        _pingAnalysesThread = null;
    }
}