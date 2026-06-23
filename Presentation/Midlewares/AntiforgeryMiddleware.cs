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
            // الحصول على الخدمة الخاصة بالـ Antiforgery
            var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();

            // توليد التوكنات وحفظها في الكوكيز الخاصة بالـ .NET
            var tokens = antiforgery.GetAndStoreTokens(context);

            // إرسال التوكن لـ Angular في كوكي اسمها XSRF-TOKEN
            // ملاحظة: HttpOnly = false ضرورية لكي يتمكن Angular من قراءة القيمة
            context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
                new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true, // يضمن إرسالها عبر HTTPS فقط
                    SameSite = SameSiteMode.Lax
                });

            await next(context);
        });
    }
}