using BaseApi.Application.Common.Security;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace BaseApi.Infrastructure.Filters
{
    public class GlobalXssProtectionFilter : IActionFilter
    {
        private readonly IXssProtectionService _xssProtectionService;
        private readonly ILogger<GlobalXssProtectionFilter> _logger;

        public GlobalXssProtectionFilter(IXssProtectionService xssProtectionService, ILogger<GlobalXssProtectionFilter> logger)
        {
            _xssProtectionService = xssProtectionService;
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value != null)
                {
                    var argumentValue = argument.Value;
                    var argumentType = argumentValue.GetType();

                    if (ShouldSkipSanitization(argumentType))
                        continue;

                    try
                    {
                        // Sadece gerçek tehlikeli content'i logla
                        if (ContainsDangerousXss(argumentValue))
                        {
                            var userAgent = context.HttpContext.Request.Headers.UserAgent.ToString();
                            var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

                            _logger.LogWarning("Dangerous XSS attempt detected from IP: {IP}, Controller: {Controller}, Action: {Action}",
                                ipAddress, context.Controller.GetType().Name, context.ActionDescriptor.DisplayName);
                        }

                        // Hafif sanitization yap - encoding deðil!
                        var sanitizedObject = _xssProtectionService.SanitizeObject(argumentValue);
                        context.ActionArguments[argument.Key] = sanitizedObject;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sanitizing object of type {Type}", argumentType.Name);
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Response sanitization if needed
        }

        private bool ShouldSkipSanitization(Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTime?) ||
                   type == typeof(decimal) ||
                   type == typeof(decimal?) ||
                   type.IsEnum ||
                   type.Namespace?.StartsWith("System") == true ||
                   type.Namespace?.StartsWith("Microsoft") == true;
        }

        private bool ContainsDangerousXss(object obj)
        {
            if (obj == null) return false;

            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.CanRead && p.PropertyType == typeof(string));

            foreach (var property in properties)
            {
                var value = property.GetValue(obj) as string;
                if (!string.IsNullOrEmpty(value))
                {
                    // Sadece gerçekten tehlikeli script pattern'leri kontrol et
                    var dangerousPatterns = new[]
                    {
                        "<script",
                        "javascript:",
                        "onclick=",
                        "onload=",
                        "alert(",
                        "eval("
                    };

                    if (dangerousPatterns.Any(pattern => value.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}