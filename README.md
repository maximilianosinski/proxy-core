
# proxy-core

With this library you can check your proxies, query live latency, get location, retrieve Tor relays and use them as proxies and more.

## Examples

### Checking & Resolving Proxy:

```csharp
var proxy = await Core.ResolveProxyAsync(ProxyScheme.Http, "85.214.66.137", 8080);
```
 And with credentials: 

```csharp
var proxy = await Core.ResolveProxyAsync(ProxyScheme.Http, "85.214.66.137", 8080, "admin", "securepassword");
```

### Query average ping

```csharp
var latency = proxy.AveragePing;
```
Note: `proxy.StartPingAnalyses();` must be called to update the ping. It is stopped with: `proxy.StopPingAnalyses();`

Query ping:

```csharp
proxy.Ping += (sender, result) =>  
{  
    var latency = result.Latency;  
};
```
   
### Tor Relays
```csharp
var relay = await Core.GetTorRelayAsync();  
var proxy = await Core.ResolveProxyAsync(ProxyScheme.Socks5, relay.Host, relay.Port);
```
    
And with Exit Nodes:

```csharp
var relay = await Core.GetTorRelayAsync("nl"); // Netherlands
var proxy = await Core.ResolveProxyAsync(ProxyScheme.Socks5, relay.Host, relay.Port);
```

## Issues and Contributions

If you encounter any issues with, please open an issue in the repository.
