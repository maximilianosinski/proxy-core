using ProxyCore;

namespace ProxyCore_CLI
{
    internal abstract class Program
    {
        private static async Task Main(string[] args)
        {
            var proxies = new List<ResolvedProxy>();
            var path = args.Length == 1 ? args[0] : "proxies.txt";
            if (!File.Exists(path)) throw new FileNotFoundException("Proxy file not found: proxies.txt is missing or no argument with path given.");

            var combos = await File.ReadAllLinesAsync(path);
            foreach (var rawCombo in combos)
            {
                try
                {
                    var combo = rawCombo.Contains('@') ? rawCombo.Replace("@", ":").Split(":") : rawCombo.Split(":");
                    string host;
                    int port;
                    string? username = null;
                    string? password = null;
                    if (combo.Length == 4)
                    {
                        host = combo[2];
                        port = Convert.ToInt32(combo[3]);
                        username = combo[0];
                        password = combo[1];
                    }
                    else
                    {
                        host = combo[0];
                        port = Convert.ToInt32(combo[1]);
                    }
                
                    var proxy = await Core.ResolveProxy(ProxyScheme.Http, host, port, username, password);
                    proxies.Add(proxy);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{proxies.Count}]: {proxy.Host} | {proxy.Location.City}, {proxy.Location.Country}. ({proxy.AveragePing}ms)");
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(exception.Message);
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Select proxy by ID: ");
            var proxyIndex = Convert.ToInt32(Console.ReadLine()) - 1;
            var selectedProxy = proxies[proxyIndex];
            selectedProxy.StartPingAnalyses();
            selectedProxy.Ping += (sender, result) =>
            {
                Console.Clear();
                Console.WriteLine($"Ping: {result.Latency}ms");
            };

            Console.ReadKey();
        }
    }
}