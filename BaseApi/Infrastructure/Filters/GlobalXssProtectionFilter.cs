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

                    // Skip primitive types and some system types
                    if (ShouldSkipSanitization(argumentType))
                        continue;

                    try
                    {
                        // Check for XSS before sanitization for logging
                        if (ContainsXssInObject(argumentValue))
                        {
                            var userAgent = context.HttpContext.Request.Headers.UserAgent.ToString();
                            var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

                            _logger.LogWarning("XSS attempt detected from IP: {IP}, User-Agent: {UserAgent}, Controller: {Controller}, Action: {Action}",
                                ipAddress, userAgent, context.Controller.GetType().Name, context.ActionDescriptor.DisplayName);
                        }

                        // Sanitize the object
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
            // Optionally sanitize response data
        }

        private bool ShouldSkipSanitization(Type type)
        {
            // Skip primitive types, DateTime, enums, etc.
            return type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTime?) ||
                   type == typeof(decimal) ||
                   type == typeof(decimal?) ||
                   type.IsEnum ||
                   type.Namespace?.StartsWith("System") == true ||
                   type.Namespace?.StartsWith("Microsoft") == true;
        }

        private bool ContainsXssInObject(object obj)
        {
            if (obj == null) return false;

            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.CanRead && p.PropertyType == typeof(string));

            foreach (var property in properties)
            {
                var value = property.GetValue(obj) as string;
                if (!string.IsNullOrEmpty(value) && _xssProtectionService.ContainsXss(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}