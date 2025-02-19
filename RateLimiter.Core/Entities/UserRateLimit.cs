using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Core.Entities;

public class UserRateLimit
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BaseLimit { get; set; }  // Original limit based on role
    public int BonusRequests { get; set; }  // Additional requests from dynamic increase
    public int RolloverRequests { get; set; }  // Unused requests from previous period
    public DateTime LastResetTime { get; set; }
    public User User { get; set; } = null!;
}
