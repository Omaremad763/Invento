using Application;

using Infrastructure.External_Services;
using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Prometheus;

using Serilog;

var builder = WebApplication.CreateBuilder(args);
SerilogSetup.Configure(builder.Configuration);
builder.Host.UseSerilog();


builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddOpenApi();

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
app.UseHttpMetrics();
app.Run();
