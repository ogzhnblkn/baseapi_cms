namespace BaseApi.Infrastructure.Middleware
{
    public class SimpleExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SimpleExceptionMiddleware> _logger;

        public SimpleExceptionMiddleware(RequestDelegate next, ILogger<SimpleExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal Server Error - Check logs");
            }
        }
    }
}