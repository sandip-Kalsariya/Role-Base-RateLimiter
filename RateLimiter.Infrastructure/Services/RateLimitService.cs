using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Entities;
using RateLimiter.Core.Interfaces;
using RateLimiter.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Infrastructure.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRequestLogRepository _requestLogRepository;
        private readonly RateLimitSettings _settings;
        private readonly ILogger<RateLimitService> _logger;

        public RateLimitService(
            ApplicationDbContext context,
            IRequestLogRepository requestLogRepository,
            IOptions<RateLimitSettings> settings,
            ILogger<RateLimitService> logger)
        {
            _context = context;
            _requestLogRepository = requestLogRepository;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<(bool IsAllowed, int RemainingRequests, int TotalLimit)> CheckRateLimitAsync(int userId, UserRole role)
        {
            var userLimit = await GetOrCreateUserLimitAsync(userId, role);
            await UpdatePeriodIfNeededAsync(userLimit, role);

            var windowStart = DateTime.UtcNow.AddHours(-1);
            var requestsUsed = await _requestLogRepository.GetRequestCountInWindowAsync(userId, windowStart);

            // Calculate total available requests
            var totalLimit = userLimit.BaseLimit + userLimit.BonusRequests + userLimit.RolloverRequests;
            var remaining = totalLimit - requestsUsed;

            return (remaining > 0, remaining, totalLimit);
        }

        public async Task HandleSuccessfulRequestAsync(int userId, UserRole role)
        {
            var userLimit = await GetOrCreateUserLimitAsync(userId, role);
            await _requestLogRepository.LogRequestAsync(userId);
        }

        public async Task HandleRateLimitExceededAsync(int userId, UserRole role)
        {
            var userLimit = await GetOrCreateUserLimitAsync(userId, role);

            // Increase limit for next period
            userLimit.BonusRequests += _settings.DynamicIncreaseAmount;
            userLimit.RolloverRequests = 0; // Reset rollover when limit is exceeded

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Rate limit exceeded for user {UserId}. Increased limit by {Increase} for next period",
                userId,
                _settings.DynamicIncreaseAmount);
        }

        private async Task UpdatePeriodIfNeededAsync(UserRateLimit userLimit, UserRole role)
        {
            var hoursSinceReset = (DateTime.UtcNow - userLimit.LastResetTime).TotalHours;

            if (hoursSinceReset >= 1)
            {
                // Calculate unused requests from previous period
                var windowStart = DateTime.UtcNow.AddHours(-1);
                var requestsUsed = await _requestLogRepository.GetRequestCountInWindowAsync(userLimit.UserId, windowStart);
                var totalPreviousLimit = userLimit.BaseLimit + userLimit.BonusRequests + userLimit.RolloverRequests;
                var unusedRequests = Math.Max(0, totalPreviousLimit - requestsUsed);

                // Update for new period
                userLimit.LastResetTime = DateTime.UtcNow;
                userLimit.RolloverRequests = unusedRequests;
                // Keep bonus requests from previous period

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Reset period for user {UserId}. Rolled over {Rollover} unused requests",
                    userLimit.UserId,
                    unusedRequests);
            }
        }

        private async Task<UserRateLimit> GetOrCreateUserLimitAsync(int userId, UserRole role)
        {
            var userLimit = await _context.UserRateLimits.FirstOrDefaultAsync(u => u.UserId == userId);

            if (userLimit == null)
            {
                var baseLimit = GetBaseLimit(role);

                userLimit = new UserRateLimit
                {
                    UserId = userId,
                    BaseLimit = baseLimit,
                    BonusRequests = 0,
                    RolloverRequests = 0,
                    LastResetTime = DateTime.UtcNow
                };
                
                _context.UserRateLimits.Add(userLimit);
                await _context.SaveChangesAsync();
            }

            return userLimit;
        }

        private int GetBaseLimit(UserRole role) => role switch
        {
            UserRole.Admin => _settings.AdminRequestsPerHour,
            UserRole.User => _settings.UserRequestsPerHour,
            _ => _settings.GuestRequestsPerHour
        };
    }
}
