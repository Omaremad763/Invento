using System.Reflection;

using Application;
using Application.Contracts;
using Application.Internal_Services_implementation;

using FluentValidation;

using Infrastructure.Contracts_Implemintaion;
using Infrastructure.External_Services;
using Infrastructure.Repos;

using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extentions
{
    public  static class DependenciesCollector
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            var assembly = typeof(IApplicationHandlerMarker).Assembly;
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IInventoServices,InventoService >();
            services.AddHttpClient<IExternalApisService, ExternalApisService>();
            services.AddScoped<IRedisCacheService,RedisCacheService>();
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddValidatorsFromAssembly(assembly);
            return services;
        }
    }
}
