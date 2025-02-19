using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RateLimiter.Core.Entities;
using RateLimiter.Core.Interfaces;
using System.Security.Claims;

namespace RateLimiter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    //[Produces("application/json")]
    public class TestController : ControllerBase
    {
        private readonly IRateLimitService _rateLimitService;

        public TestController(IRateLimitService rateLimitService)
        {
            _rateLimitService = rateLimitService;
        }

        /// <summary>
        /// Test endpoint to check and test rate limit status
        /// </summary>
        /// <returns>A success message</returns>
        /// <response code="200">Returns the success message</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="429">If the rate limit is exceeded</response>
        [HttpGet("status")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> GetRateLimitStatus()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = Enum.Parse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value ?? "Guest");

            var (isAllowed, remaining, totalLimit) = await _rateLimitService.CheckRateLimitAsync(userId, userRole);

            return Ok(new
            {
                isAllowed,
                remaining,
                totalLimit,
                role = userRole.ToString(),
                nextReset = DateTime.UtcNow.AddHours(1)
            });
        }

    }
}
