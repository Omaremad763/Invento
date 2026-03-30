using System.Text;
using System.Threading.RateLimiting;

using Application;
using Application.Contracts;
using Application.Internal_Services_implementation;

using Domain.Entites;

using FluentEmail.MailKitSmtp;

using FluentValidation;

using Infrastructure.Contracts_Implemintaion;
using Infrastructure.External_Services;
using Infrastructure.Persistence;
using Infrastructure.Repos;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Extentions
{
    public static class DependenciesCollector
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration config)
        {
            var assembly = typeof(IApplicationHandlerMarker).Assembly;
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IInventoServices, InventoService>();
            services.AddHttpClient<IExternalApisService, ExternalApisService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddValidatorsFromAssembly(assembly);

            services.AddFluentEmail(config["EmailSettings:FromEmail"])
                .AddRazorRenderer()
                .AddMailKitSender(new SmtpClientOptions
                {
                    Server = config["EmailSettings:Host"],
                    Port = int.Parse(config["EmailSettings:Port"]),
                    User = config["EmailSettings:Username"],
                    Password = config["EmailSettings:Password"],
                    UseSsl = false,
                    RequiresAuthentication = true
                });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
                };
            });
            services.AddMemoryCache();
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(policyName: "auth_policy", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.PermitLimit = 3;
                    opt.QueueLimit = 0;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
            return services;
        }
    }
}