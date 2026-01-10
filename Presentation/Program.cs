
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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddOpenApi();

builder.Services.AddApiServices();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "Invento:";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    configuration.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(configuration);
});

var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();       // تطبّق أي migrations
    DatabaseSeeder.Seed(context);     // تنفيذ seed data
}

app.UseMetricServer();
app.MapMetrics();
app.UseHttpMetrics();
app.Run();
