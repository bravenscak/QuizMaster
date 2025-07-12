using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizMaster.Repositories;
using System.Security.Claims;

namespace QuizMaster.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionController(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        [HttpGet("organizer/{organizerId}/status")]
        public async Task<ActionResult<bool>> GetSubscriptionStatus(int organizerId)
        {
            var userId = GetCurrentUserId();
            var isSubscribed = await _subscriptionRepository.IsSubscribedAsync(userId, organizerId);
            return Ok(isSubscribed);
        }

        [HttpPost("organizer/{organizerId}/toggle")]
        public async Task<ActionResult<bool>> ToggleSubscription(int organizerId)
        {
            var userId = GetCurrentUserId();

            if (userId == organizerId)
                return BadRequest("Cannot subscribe to yourself");

            var isSubscribed = await _subscriptionRepository.ToggleSubscriptionAsync(userId, organizerId);
            return Ok(new { isSubscribed, message = isSubscribed ? "Subscribed" : "Unsubscribed" });
        }

        [HttpGet("my")]
        public async Task<ActionResult> GetMySubscriptions()
        {
            var userId = GetCurrentUserId();
            var subscriptions = await _subscriptionRepository.GetBySubscriberIdAsync(userId);

            var result = subscriptions.Where(s => s.IsActive).Select(s => new
            {
                s.Id,
                OrganizerId = s.OrganizerId,
                OrganizerName = $"{s.Organizer.FirstName} {s.Organizer.LastName}",
                OrganizationName = s.Organizer.OrganizationName
            });

            return Ok(result);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid user token");

            return userId;
        }
    }
}