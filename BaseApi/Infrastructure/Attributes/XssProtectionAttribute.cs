using BaseApi.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BaseApi.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class XssProtectionAttribute : ActionFilterAttribute
    {
        public bool Enabled { get; set; } = true;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!Enabled)
            {
                base.OnActionExecuting(context);
                return;
            }

            var xssProtectionService = context.HttpContext.RequestServices.GetService<IXssProtectionService>();
            if (xssProtectionService == null)
            {
                base.OnActionExecuting(context);
                return;
            }

            foreach (var argument in context.ActionArguments.ToList())
            {
                if (argument.Value is string stringValue && !string.IsNullOrEmpty(stringValue))
                {
                    if (xssProtectionService.ContainsXss(stringValue))
                    {
                        context.Result = new BadRequestObjectResult(new
                        {
                            error = "XSS content detected",
                            parameter = argument.Key
                        });
                        return;
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}