using Consul;

using Invento.Gateaway;

using Microsoft.Extensions.DependencyInjection;

using Yarp.ReverseProxy.Configuration;

using RouteConfig = Yarp.ReverseProxy.Configuration.RouteConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayCorsPolicy", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            return string.IsNullOrEmpty(origin) ||
                   origin.EndsWith(".vercel.app") ||
                   origin.Contains("localhost");
        })
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

});

builder.Services.AddSingleton<IConsulClient>(_ =>
{
    return new ConsulClient(c =>
    {
        c.Address = new Uri(
            builder.Configuration["ConsulConfig:ConsulAddress"]!);
    });
});

builder.Services.AddSingleton<ConsulConfigurationProvider>();

builder.Services.AddSingleton<IProxyConfigProvider>(sp =>
{
    var provider = sp.GetRequiredService<ConsulConfigurationProvider>();

    provider.LoadAsync().GetAwaiter().GetResult();

    return provider;
});

builder.Services.AddHostedService<ConsulWatcherService>();
builder.Services.AddReverseProxy();

var app = builder.Build();
app.UseRouting();
app.MapReverseProxy();
app.UseCors("GatewayCorsPolicy");
app.Use(async (context, next) =>
{
    Console.WriteLine($"[GATEWAY] Forwarding request to: {context.Request.Path}");
    foreach (var header in context.Request.Headers)
    {
        Console.WriteLine($"[GATEWAY] Header: {header.Key} = {header.Value}");
    }
    await next();
});
await app.RunAsync();