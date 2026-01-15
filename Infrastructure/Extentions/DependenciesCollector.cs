using Application;
using Application.Contracts;
using Application.Internal_Services_implementation;

using Infrastructure.External_Services;
using Infrastructure.Repos;

using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ExtetnionMethods
{
    public  static class DependenciesCollector
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IInventoServices,InventoService >();
            services.AddScoped<IRedisCacheService,RedisCacheService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(IApplicationHandlerMarker).Assembly);
            });
            return services;
        }
    }
}
