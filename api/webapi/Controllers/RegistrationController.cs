using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPM.Models.DTOs;
using PPM.Models.Services;
using PPM.Services;

namespace PPM.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController(UserService userService, RegistrationService registrationService, ILogger<RegistrationController> logger) : ControllerBase
    {
        private readonly UserService _userService = userService;
        private readonly RegistrationService _registrationService = registrationService;
        [HttpGet("{registration_id}")]
        public async Task<ActionResult<RegistrationDTO>> GetRegistration(int registration_id)
        {
            var registration = await _registrationService.GetRegistrationDetailsAsync(registration_id);
            if (registration == null)
            {
                logger.LogWarning("User with ID {user_id} not found.", registration_id);
                return NotFound();
            }
            return Ok(registration);
        }
        [Authorize]
        [HttpPost("")]
        public async Task<ActionResult<RegistrationDTO>> CreateRegistration([FromBody] RegistrationDTO registrationDTO)
        {
            try
            {
                // 🔍 Sanity check for authentication and claims
                var identity = User?.Identity;
                bool isAuthenticated = identity?.IsAuthenticated ?? false;
                int claimCount = User?.Claims?.Count() ?? 0;

                logger.LogInformation("🔒 Is Authenticated: {Auth}", isAuthenticated);
                logger.LogInformation("🔎 Claims Count: {Count}", claimCount);

                if (User?.Claims != null)
                {
                    foreach (var claim in User.Claims)
                    {
                        logger.LogInformation("➡️ Incoming Claim: {Type} = {Value}", claim.Type, claim.Value);
                    }
                }
                else
                {
                    logger.LogWarning("⚠️ No claims found — User context might be missing or invalid.");
                }

                logger.LogInformation("Starting booking creation…");
                var registration = await _registrationService.CreateRegistration(registrationDTO);
                logger.LogInformation("Booking successfully created: Registration ID {Id}", registration.registration_id);
                return Ok(registration);
            }
            catch (ApplicationException ex)
            {
                logger.LogWarning("Failed to create registration: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during registration.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpGet("user/{user_id}")]
        public async Task<ActionResult<IEnumerable<RegistrationDTO>>> GetAllUserRegistrations(int user_id)
        {
            var logged_in_user_id = _userService.GetLoggedInUserId();
            if(logged_in_user_id == null || logged_in_user_id != user_id)
            {
                logger.LogWarning("Unauthorized access attempt by user {logged_in_user_id} to user {user_id}", logged_in_user_id, user_id);
                return Unauthorized();
            }
            var user_registrations = await _registrationService.GetAllUserRegistrationsAsync(user_id);
            if (user_registrations == null || !user_registrations.Any())
            {
                return NotFound();
            }
            return Ok(user_registrations);
        }

    }
}
