using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Core.Configuration
{
    public class RateLimitSettings
    {
        public int AdminRequestsPerHour { get; set; } = 1000;
        public int UserRequestsPerHour { get; set; } = 100;
        public int GuestRequestsPerHour { get; set; } = 10;
        public int GlobalRequestsPerHour { get; set; } = 5000;
        public int DynamicIncreaseAmount { get; set; } = 10;
    }
}
