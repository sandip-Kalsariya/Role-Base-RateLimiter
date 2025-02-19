using Microsoft.EntityFrameworkCore;
using RateLimiter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
        public DbSet<UserRateLimit> UserRateLimits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Seed Users
            modelBuilder.Entity<User>().HasData(
                  new User
                  {
                      UserId = 1,
                      Name = "Admin User",
                      Username = "admin",
                      Password = "admin123",
                      Role = UserRole.Admin
                  },
                  new User
                  {
                      UserId = 2,
                      Name = "Regular User",
                      Username = "user",
                      Password = "user123",
                      Role = UserRole.User
                  },
                  new User
                  {
                      UserId = 3,
                      Name = "Guest User",
                      Username = "guest",
                      Password = "guest123",
                      Role = UserRole.Guest
                  }
              );
            #endregion

            #region Seed UserRateLimits
            modelBuilder.Entity<UserRateLimit>().HasData(
                new UserRateLimit
                {
                    Id = 1,
                    UserId = 1, // Admin
                    BaseLimit = 1000, // Admin limit
                    BonusRequests = 0,
                    RolloverRequests = 0,
                    LastResetTime = DateTime.UtcNow
                },
                new UserRateLimit
                {
                    Id = 2,
                    UserId = 2, // Regular User
                    BaseLimit = 100, // User limit
                    BonusRequests = 0,
                    RolloverRequests = 0,
                    LastResetTime = DateTime.UtcNow
                },
                new UserRateLimit
                {
                    Id = 3,
                    UserId = 3, // Guest
                    BaseLimit = 10, // Guest limit
                    BonusRequests = 0,
                    RolloverRequests = 0,
                    LastResetTime = DateTime.UtcNow
                }
            );
            #endregion
        }
    }
}
