using System.Diagnostics;

namespace internLoanProjectAPI.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);

                stopwatch.Stop();

                var method = context.Request.Method;
                var path = context.Request.Path;
                var statusCode = context.Response.StatusCode;
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;


              
                if (statusCode >= 500)
                {
                    _logger.LogError(
                        "HTTP {Method} {Path} -> {StatusCode} | {ElapsedMilliseconds} ms",
                        method,
                        path,
                        statusCode,
                        elapsedMilliseconds
                    );
                }

                else if (statusCode >= 400)
                {
                    _logger.LogWarning(
                        "HTTP {Method} {Path} -> {StatusCode} | {ElapsedMilliseconds} ms",
                        method,
                        path,
                        statusCode,
                        elapsedMilliseconds
                    );
                }

      
                else
                {
                    _logger.LogInformation(
                        "HTTP {Method} {Path} -> {StatusCode} | {ElapsedMilliseconds} ms",
                        method,
                        path,
                        statusCode,
                        elapsedMilliseconds
                    );
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "HTTP {Method} {Path} sırasında hata oluştu | {ElapsedMilliseconds} ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds
                );

                throw;
            }
        }
    }
    
}
