using Application;

using Infrastructure.Extentions;
using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using Presentation;

using Prometheus;

using Scalar.AspNetCore;

using Serilog;

using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
SerilogSetup.Configure(builder.Configuration);
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("VercelPolicy", policy =>
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
 {
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.MapScalarApiReference(options=>
    {
        options.WithTitle("Invento APIS")
               .WithTheme(ScalarTheme.Mars)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseRouting();
app.UseSerilogRequestLogging();
app.UseCors("VercelPolicy");
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    DatabaseSeeder.Seed(context);
}

app.UseMetricServer();
app.MapMetrics();
app.UseHttpMetrics();
app.Run();
