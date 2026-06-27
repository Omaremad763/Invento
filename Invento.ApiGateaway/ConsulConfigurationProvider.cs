using Consul;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

using Yarp.ReverseProxy.Configuration;

using DestinationConfig = Yarp.ReverseProxy.Configuration.DestinationConfig;
using RouteConfig = Yarp.ReverseProxy.Configuration.RouteConfig;

namespace Invento.Gateaway;

public sealed class ConsulConfigurationProvider : IProxyConfigProvider
{
    private readonly IConfiguration _configuration;
    private readonly IConsulClient _consul;

    private InMemoryConfig _config;

    private CancellationTokenSource _cts = new();

    public ConsulConfigurationProvider(
        IConfiguration configuration,
        IConsulClient consul)
    {
        _configuration = configuration;
        _consul = consul;

        _config = new InMemoryConfig(
            Array.Empty<RouteConfig>(),
            Array.Empty<ClusterConfig>(),
            new CancellationChangeToken(_cts.Token));
    }

    public IProxyConfig GetConfig() => _config;

    public async Task LoadAsync()
    {
        var routeEntries = _configuration
            .GetSection("ReverseProxy:Routes")
            .Get<Dictionary<string, RouteConfig>>()
            ?? new();

        var routes = routeEntries
            .Select(x => x.Value with
            {
                RouteId = x.Key
            })
            .ToList();

        var clusterEntries = _configuration
            .GetSection("ReverseProxy:Clusters")
            .Get<Dictionary<string, ClusterConfig>>()
            ?? new();

        var updatedClusters = new List<ClusterConfig>();

        foreach (var entry in clusterEntries)
        {
            var cluster = entry.Value with
            {
                ClusterId = entry.Key
            };

            if (cluster.Metadata != null &&
                cluster.Metadata.TryGetValue("ConsulServiceName", out var serviceName))
            {
                var services = await _consul.Health.Service(
                    serviceName,
                    tag: null,
                    passingOnly: true);

                var destinations = new Dictionary<string, DestinationConfig>();

                foreach (var service in services.Response)
                {
                    var host = "127.0.0.1";

                    destinations[service.Service.ID] = new DestinationConfig
                    {
                        Address = $"http://{host}:{service.Service.Port}"
                    };
                }

                updatedClusters.Add(cluster with
                {
                    Destinations = destinations
                });
            }
            else
            {
                updatedClusters.Add(cluster);
            }
        }

        var old = _cts;
        _cts = new CancellationTokenSource();

        _config = new InMemoryConfig(
            routes,
            updatedClusters,
            new CancellationChangeToken(_cts.Token));

        old.Cancel();
        old.Dispose();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[Consul] Configuration reloaded. Destinations: {updatedClusters.Sum(c => c.Destinations?.Count ?? 0)}");
        Console.ResetColor();
    }

}