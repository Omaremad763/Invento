using System.Text;
using System.Threading.RateLimiting;

using Application;
using Application.Contracts;
using Application.Internal_Services_implementation;

using AspNetCore.ReCaptcha;

using Domain;
using Domain.Entites;

using FluentEmail.MailKitSmtp;

using FluentValidation;

using Infrastructure.Contracts_Implemintaion;
using Infrastructure.External_Services;
using Infrastructure.Persistence;
using Infrastructure.Repos;

using MassTransit;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Extentions
{
    public static class DependenciesCollector
    {

        public static IServiceCollection AddApiServices(this IServiceCollection services , IConfiguration config)
        {
            var host = config["EmailSettings:Host"];
            var assembly = typeof(IApplicationHandlerMarker).Assembly;
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IInventoServices, InventoService>();
            services.AddHttpClient<IExternalApisService, ExternalApisService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();

            #region  auth
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.AllowedForNewUsers = true;
    })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
            services.AddReCaptcha(configuration: config.GetSection("ReCaptcha"));

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
           services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN"; 
            });
            #endregion


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
                    RequiresAuthentication = host != "maildev"
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
            services.AddMassTransit(x =>
            {
                // 1. تعريف الـ Bus للـ RabbitMQ
                x.AddConsumer<UserRegisteredConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h => {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });

                // 2. تعريف الـ Rider للكافكا (هنا سر الحل)
                x.AddRider(rider =>
                {
                    rider.AddProducer<UserRegisteredEvent>("user-registered-topic");

                    rider.AddConsumer<UserRegisteredConsumer>();

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<UserRegisteredEvent>("user-registered-topic", "invento-group", e =>
                        {
                            e.ConfigureConsumer<UserRegisteredConsumer>(context);
                        });
                    });
                });
            }); 
            return services;
        }
    }
}