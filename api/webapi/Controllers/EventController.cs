using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PPM.Models;
using PPM.Models.DTOs;
using PPM.Models.Services;
using PPM.Services;

namespace PPM.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;
        private readonly UserService _userService;
        private readonly ILogger<EventController> _logger;

        public EventController(EventService eventService, UserService userService, ILogger<EventController> logger)
        {
            _eventService = eventService;
            _userService = userService;
            _logger = logger;
        }

        // POST /api/Event
        [HttpPost("")]
        public async Task<ActionResult<EventDTO>> CreateEvent([FromBody] EventDTO eventDTO)
        {
            try
            {
                // 🔍 Sanity check for authentication and claims
                var identity = User?.Identity;
                bool isAuthenticated = identity?.IsAuthenticated ?? false;
                int claimCount = User?.Claims?.Count() ?? 0;

                _logger.LogInformation("🔒 Is Authenticated: {Auth}", isAuthenticated);
                _logger.LogInformation("🔎 Claims Count: {Count}", claimCount);

                if (User?.Claims != null)
                {
                    foreach (var claim in User.Claims)
                    {
                        _logger.LogInformation("➡️ Incoming Claim: {Type} = {Value}", claim.Type, claim.Value);
                    }
                }
                else
                {
                    _logger.LogWarning("⚠️ No claims found — User context might be missing or invalid.");
                }

                _logger.LogInformation("Starting booking creation…");
                var ev = await _eventService.CreateEventAsync(eventDTO);
                _logger.LogInformation("Booking successfully created: Event ID {Id}", ev.event_id);
                return Ok(ev);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning("Failed to Create Event: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Event Creation.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("{event_id}")]
        public async Task<ActionResult<EventDTO>> GetEventDetailsAsync(int event_id)
        {
            var ev = await _eventService.GetEventDetailsAsync(event_id);
            if (ev == null)
            {
                _logger.LogWarning("User with ID {user_id} not found.", event_id);
                return NotFound();
            }
            return Ok(ev);
        }

        [HttpGet("user/{user_id}")]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetAllEventsAsync(int user_id)
        {
            var logged_in_user_id = _userService.GetLoggedInUserId();
            if (logged_in_user_id == null || logged_in_user_id != user_id)
            {
                _logger.LogWarning("Unauthorized access attempt by user {logged_in_user_id} to user {user_id}", logged_in_user_id, user_id);
                return Unauthorized();
            }
            var user_events = await _eventService.GetAllEventsAsync(user_id);
            if (user_events == null || !user_events.Any())
            {
                return NotFound();
            }
            return Ok(user_events);
        }
    }
}
