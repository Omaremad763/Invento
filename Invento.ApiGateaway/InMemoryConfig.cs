using Microsoft.Extensions.Primitives;

using Yarp.ReverseProxy.Configuration;

namespace Invento.Gateaway;

public sealed class InMemoryConfig : IProxyConfig
{
    public InMemoryConfig(
        IReadOnlyList<RouteConfig> routes,
        IReadOnlyList<ClusterConfig> clusters,
        IChangeToken changeToken)
    {
        Routes = routes;
        Clusters = clusters;
        ChangeToken = changeToken;
    }

    public IReadOnlyList<RouteConfig> Routes { get; }

    public IReadOnlyList<ClusterConfig> Clusters { get; }

    public IChangeToken ChangeToken { get; }
}