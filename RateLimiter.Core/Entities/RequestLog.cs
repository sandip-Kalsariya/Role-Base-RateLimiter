namespace RateLimiter.Core.Entities
{
    public class RequestLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public User User { get; set; } = null!;
    }
}
