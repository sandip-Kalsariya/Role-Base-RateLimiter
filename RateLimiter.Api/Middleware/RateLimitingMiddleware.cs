using Microsoft.Extensions.Options;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Entities;
using RateLimiter.Core.Interfaces;
using System.Security.Claims;

namespace RateLimiter.Api.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RateLimitSettings _settings;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        public RateLimitingMiddleware(
            RequestDelegate next,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<RateLimitSettings> settings,
            ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                await _next(context);
                return;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var rateLimitService = scope.ServiceProvider.GetRequiredService<IRateLimitService>();

            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = Enum.Parse<UserRole>(context.User.FindFirst(ClaimTypes.Role)?.Value ?? "Guest");

            var (isAllowed, remaining, totalLimit) = await rateLimitService.CheckRateLimitAsync(userId, userRole);

            if (!isAllowed)
            {
                await rateLimitService.HandleRateLimitExceededAsync(userId, userRole);

                context.Response.StatusCode = 429;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Rate limit exceeded. Limit will be increased for next period.",
                    nextPeriodLimit = totalLimit + _settings.DynamicIncreaseAmount
                });
                return;
            }

            await rateLimitService.HandleSuccessfulRequestAsync(userId, userRole);

            // Add rate limit headers
            context.Response.Headers.Add("X-RateLimit-Limit", totalLimit.ToString());
            context.Response.Headers.Add("X-RateLimit-Remaining", remaining.ToString());
            context.Response.Headers.Add("X-RateLimit-Reset", DateTime.UtcNow.AddHours(1).ToString("O"));

            await _next(context);
        }
    }
}
