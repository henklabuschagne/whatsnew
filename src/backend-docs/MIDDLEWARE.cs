// ============================================
// What's New API - Middleware
// ============================================

using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using WhatsNewAPI.Models.DTOs.Common;

namespace WhatsNewAPI.Middleware
{
    // ============================================
    // EXCEPTION HANDLING MIDDLEWARE
    // ============================================
    
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var errorResponse = new ErrorResponse
            {
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case ArgumentNullException:
                case ArgumentException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = "Invalid request";
                    errorResponse.Details = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = "Unauthorized access";
                    errorResponse.Details = exception.Message;
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = "Resource not found";
                    errorResponse.Details = exception.Message;
                    break;

                case InvalidOperationException:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse.Message = "Operation conflict";
                    errorResponse.Details = exception.Message;
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An internal server error occurred";
                    errorResponse.Details = "Please contact support if the problem persists";
                    break;
            }

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    // ============================================
    // AUDIT LOGGING MIDDLEWARE
    // ============================================
    
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLoggingMiddleware> _logger;

        public AuditLoggingMiddleware(
            RequestDelegate next,
            ILogger<AuditLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            
            // Log request
            _logger.LogInformation(
                "HTTP {Method} {Path} started at {StartTime}",
                context.Request.Method,
                context.Request.Path,
                startTime
            );

            // Read and store the original response body
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                var duration = DateTime.UtcNow - startTime;
                
                // Log response
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    duration.TotalMilliseconds
                );

                // Copy the response back to the original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }

    // ============================================
    // JWT MIDDLEWARE (Optional - for additional JWT handling)
    // ============================================
    
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(
            RequestDelegate next,
            ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                // Additional JWT processing can be done here if needed
                _logger.LogDebug("JWT token present in request");
            }

            await _next(context);
        }
    }

    // ============================================
    // RATE LIMITING MIDDLEWARE (Basic Implementation)
    // ============================================
    
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly Dictionary<string, (DateTime timestamp, int count)> _requests = new();
        private readonly int _maxRequestsPerMinute = 100;

        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;

            lock (_requests)
            {
                // Clean up old entries (older than 1 minute)
                var keysToRemove = _requests
                    .Where(kvp => (now - kvp.Value.timestamp).TotalMinutes > 1)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    _requests.Remove(key);
                }

                // Check rate limit
                if (_requests.TryGetValue(clientIp, out var requestInfo))
                {
                    if ((now - requestInfo.timestamp).TotalMinutes < 1)
                    {
                        if (requestInfo.count >= _maxRequestsPerMinute)
                        {
                            _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}", clientIp);
                            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                            context.Response.ContentType = "application/json";
                            
                            var errorResponse = new
                            {
                                message = "Rate limit exceeded",
                                details = $"Maximum {_maxRequestsPerMinute} requests per minute allowed"
                            };

                            await context.Response.WriteAsJsonAsync(errorResponse);
                            return;
                        }

                        _requests[clientIp] = (requestInfo.timestamp, requestInfo.count + 1);
                    }
                    else
                    {
                        _requests[clientIp] = (now, 1);
                    }
                }
                else
                {
                    _requests[clientIp] = (now, 1);
                }
            }

            await _next(context);
        }
    }

    // ============================================
    // REQUEST VALIDATION MIDDLEWARE
    // ============================================
    
    public class RequestValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestValidationMiddleware> _logger;

        public RequestValidationMiddleware(
            RequestDelegate next,
            ILogger<RequestValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Validate Content-Type for POST/PUT requests
            if ((context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
                && !context.Request.Path.Value.Contains("import/excel"))
            {
                var contentType = context.Request.ContentType;
                
                if (string.IsNullOrEmpty(contentType) || !contentType.Contains("application/json"))
                {
                    _logger.LogWarning("Invalid Content-Type: {ContentType}", contentType);
                    context.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                    context.Response.ContentType = "application/json";
                    
                    var errorResponse = new
                    {
                        message = "Invalid Content-Type",
                        details = "Content-Type must be application/json"
                    };

                    await context.Response.WriteAsJsonAsync(errorResponse);
                    return;
                }
            }

            await _next(context);
        }
    }

    // ============================================
    // SECURITY HEADERS MIDDLEWARE
    // ============================================
    
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
            
            // Remove server header
            context.Response.Headers.Remove("Server");

            await _next(context);
        }
    }
}
