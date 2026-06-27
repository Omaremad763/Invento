namespace Presentation.Midlewares;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public static class AntiforgeryMiddleware
{
    public static IApplicationBuilder UseAntiforgeryTokenMiddleware(this IApplicationBuilder app)
    {
        return app.Use(next => async context =>
        {
            var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();

            var tokens = antiforgery.GetAndStoreTokens(context);

            context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
                new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,      
                    SameSite = SameSiteMode.Lax
                });

            await next(context);
        });
    }
}