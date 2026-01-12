
using Application;


using Infrastructure.ExtetnionMethods;
using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using Prometheus;

using Serilog;

using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
SerilogSetup.Configure(builder.Configuration);
builder.Host.UseSerilog();


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
    options.UseNpgsql((formattedConnectionString)));

builder.Services.AddOpenApi();

builder.Services.AddApiServices();


var redisUrl = Environment.GetEnvironmentVariable("Redis_Local")?? Environment.GetEnvironmentVariable("REDIS_URL");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisUrl;
    options.InstanceName = "Invento:";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    configuration.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDevPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
    options.AddPolicy("ProdNetlifyPolicy", policy =>
    {
        policy.WithOrigins("https://invento-front.netlify.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseCors("LocalDevPolicy");
}
else
{
    app.UseCors("ProdNetlifyPolicy");
}
    app.UseHttpsRedirection();

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
