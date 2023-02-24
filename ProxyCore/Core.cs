using System.Net;
using Newtonsoft.Json.Linq;

namespace ProxyCore;

public class Core
{
    public static async Task<ResolvedProxy> ResolveProxy(ProxyScheme proxyScheme, string host, int port, string? username = null, string? password = null)
    {
        var useAuthentication = username != null && password != null;
        NetworkCredential? credentials = null;
        if (useAuthentication)
        {
            credentials = new NetworkCredential(username, password);
        }

        var uri = new Uri($"{proxyScheme.ToString().ToLower()}://{host}:{port}");
        
        var proxy = new WebProxy
        {
            Address = uri,
            BypassProxyOnLocal = false,
            UseDefaultCredentials = false
        };
        
        var httpClientHandler = new HttpClientHandler
        {
            Proxy = proxy,
            UseProxy = true,
            PreAuthenticate = useAuthentication,
            UseDefaultCredentials = false
        };

        if (useAuthentication)
        {
            httpClientHandler.Proxy.Credentials = credentials;
            httpClientHandler.Credentials = credentials;
        }

        var client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
        var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var json = JObject.Parse(await client.GetStringAsync("http://ip-api.com/json"));
        var latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start;

        if (json["city"] == null || json["country"] == null) throw new MissingFieldException("Resolve failed: City or Country field not found in Geo API response.");

        return new ResolvedProxy(uri, (string)json["city"], (string)json["country"], credentials)
        {
            AveragePing = latency
        };
    }
}