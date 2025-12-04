using BaseApi.Application.Common.Security;
using BaseApi.Infrastructure.Filters;
using BaseApi.Infrastructure.Middleware;
using BaseApi.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.Infrastructure.Extensions
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddGlobalXssProtection(this IServiceCollection services)
        {
            // Register XSS Protection Service
            services.AddScoped<IXssProtectionService, XssProtectionService>();

            // Register Global XSS Filter
            services.AddScoped<GlobalXssProtectionFilter>();

            return services;
        }

        public static IApplicationBuilder UseGlobalXssProtection(this IApplicationBuilder app)
        {
            return app.UseMiddleware<XssProtectionMiddleware>();
        }

        public static IMvcBuilder AddGlobalXssFilter(this IMvcBuilder builder)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add<GlobalXssProtectionFilter>();
            });

            return builder;
        }
    }
}