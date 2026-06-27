using Application;

using Consul;

using Infrastructure.Extentions;
using Infrastructure.Persistence;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Presentation.Midlewares;

using Prometheus;

using Scalar.AspNetCore;

using Serilog;

using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);
SerilogSetup.Configure();
builder.Host.UseSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<AutoMapperProfile>();
}, typeof(AutoMapperProfile).Assembly);
var DBconnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
   ?? Environment.GetEnvironmentVariable("DATABASE_URL");
string formattedConnectionString;

if (DBconnectionString != null && DBconnectionString.StartsWith("postgresql://"))
{

    var uri = new Uri(DBconnectionString);
    var userInfo = uri.UserInfo.Split(':');

    formattedConnectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    formattedConnectionString = DBconnectionString;
}


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql((formattedConnectionString));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
builder.Services.AddApiServices(builder.Configuration);

string redisConfig;

var redisUrl = builder.Configuration.GetConnectionString("RedisConnection") ??
    Environment.GetEnvironmentVariable("REDIS_URL");
if (!string.IsNullOrWhiteSpace(redisUrl) && redisUrl.StartsWith("redis://"))
{
    var uri = new Uri(redisUrl);
    redisConfig = $"{uri.Host}:{uri.Port},password={uri.UserInfo.Split(':')[1]}";
}
else
{
    redisConfig = redisUrl;
}

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConfig;
    options.InstanceName = "Invento:";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConfig, true);
    configuration.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddSingleton<IConsulClient>(p => new ConsulClient(consulConfig =>
{
    var address = builder.Configuration["ConsulConfig:ConsulAddress"];
    consulConfig.Address = new Uri(address);
}));
builder.Services.AddControllers(options => {
    options.Filters.Add(new IgnoreAntiforgeryTokenAttribute());
});
var app = builder.Build();
var consulClient = app.Services.GetRequiredService<IConsulClient>();
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Invento APIS")
               .WithTheme(ScalarTheme.Mars)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseAntiforgeryTokenMiddleware();
app.UseSerilogRequestLogging();
app.UseHsts();
app.UseCookiePolicy();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    DatabaseSeeder.Seed(context);
}

app.UseMetricServer();
app.MapMetrics();
app.UseHttpMetrics();

#region Consul
var urls = builder.Configuration["urls"] ?? 
builder.Configuration["ASPNETCORE_URLS"]; 
var dynamicPort = new Uri(urls.Split(';')[0]).Port; 

var serviceId = $"invento-api-{dynamicPort}";

var healthCheckUrl = $"http://host.docker.internal:{dynamicPort}/health";

lifetime.ApplicationStarted.Register(async () =>
{
    var registration = new AgentServiceRegistration()
    {
        ID = serviceId,    
        Name = builder.Configuration["ConsulConfig:ServiceName"],
        Address = builder.Configuration["ConsulConfig:ServiceAddress"],
        Port = dynamicPort,
        Check = new AgentServiceCheck()
        {
            HTTP = healthCheckUrl,
            Interval = TimeSpan.FromSeconds(10),
            Timeout = TimeSpan.FromSeconds(5),
            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(20)
        }
    };;
    var result=  await consulClient.Agent.ServiceRegister(registration);
    if (result.StatusCode != System.Net.HttpStatusCode.OK)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] Consul Registration Failed: {result.StatusCode}");
        Console.ResetColor();
    }
});

lifetime.ApplicationStopped.Register(() =>
{
    consulClient.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
});
#endregion

await app.RunAsync();
