using Microsoft.EntityFrameworkCore;
using RateLimiter.Core.Entities;
using RateLimiter.Core.Interfaces;
using RateLimiter.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Infrastructure.Repositories;

public class RequestLogRepository : IRequestLogRepository
{
    private readonly ApplicationDbContext _context;

    public RequestLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogRequestAsync(int userId)
    {
        var log = new RequestLog
        {
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };
        _context.RequestLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetRequestCountInWindowAsync(int userId, DateTime windowStart)
    {
        return await _context.RequestLogs.CountAsync(r => r.UserId == userId && r.Timestamp >= windowStart);
    }
}
