using RateLimiter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Core.Interfaces
{
    public interface IRateLimitService
    {
        Task<(bool IsAllowed, int RemainingRequests, int TotalLimit)> CheckRateLimitAsync(int userId, UserRole role);
        Task HandleRateLimitExceededAsync(int userId, UserRole role);
        Task HandleSuccessfulRequestAsync(int userId, UserRole role);
    }
}
