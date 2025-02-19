using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Core.Interfaces
{
    public interface IRequestLogRepository
    {
        Task LogRequestAsync(int userId);
        Task<int> GetRequestCountInWindowAsync(int userId, DateTime windowStart);
    }
}
