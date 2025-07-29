// EventService.cs
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PPM.Interfaces;
using PPM.Models.DTOs;
using PPM.Models.Interfaces;
using PPM.Services;

namespace PPM.Models.Services
{
    public class EventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IParkRepository _parkRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RegistrationService> _logger;
        public EventService(IEventRepository eventRepository, IUserRepository userRepository, IParkRepository parkRepository, IRegistrationRepository registrationRepository, UserService userService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RegistrationService> logger)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _parkRepository = parkRepository;
            _registrationRepository = registrationRepository;
            _userService = userService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public async Task<EventDTO> CreateEventAsync(EventDTO eventDTO)
        {
            var user_id = _userService.GetLoggedInUserId();
            if (user_id == null)
            {
                throw new ApplicationException("User is not Logged In.");
            }
            var user = await _userRepository.GetUserByIdAsync(user_id.Value);
            if (user == null)
            {
                throw new ApplicationException("User does not exist.");
            }
            var park = await _parkRepository.GetParkByIdAsync(eventDTO.park_id);
            if (park == null)
            {
                throw new ApplicationException("Park is currently Null.");
            }
            var registration = await _registrationRepository.GetRegistrationByIdAsync(eventDTO.registration_id);
            if (park == null)
            {
                throw new ApplicationException("Park is currently Null.");
            }
            var ev = new Event
            {
                user_id = user.user_id,
                park_id = park.park_id,
                event_name = eventDTO.event_name,
                event_description = eventDTO.event_description,
                event_start_date = eventDTO.event_start_date,
                event_end_date = eventDTO.event_end_date,
                event_start_time = eventDTO.event_start_time,
                event_end_time = eventDTO.event_end_time,
                User = user,
                Park = park,
            };
            var createdEvent = await _eventRepository.CreateEventAsync(ev);
            return new EventDTO
            {
                user_id = createdEvent.user_id,
                park_id = createdEvent.park_id,
                event_name = createdEvent.event_name,
                event_description = createdEvent.event_description,
                event_start_date = createdEvent.event_start_date,
                event_end_date = createdEvent.event_end_date,
                event_start_time = createdEvent.event_start_time,
                event_end_time = createdEvent.event_end_time,
                User = new UserDTO
                {
                    first_name = user.first_name,
                    last_name = user.last_name,

                },
                Park = new ParkDTO
                {
                    park_name = park.park_name,
                    street_address = park.street_address,
                    city = park.city,
                    state = park.state,
                    zipcode = park.zipcode,
                }
            };
        }
        /*
        if (registration.is_approved)
        {
            Save for another time
        }
        else
        {
            Save for another time
        }
        */
        public async Task<EventDTO> GetEventDetailsAsync(int event_id)
        {
            var user_id = _userService.GetLoggedInUserId();
            if (user_id == null)
            {
                throw new ApplicationException("User is not Logged In.");
            }
            var user = await _userRepository.GetUserByIdAsync(user_id.Value);
            if (user == null)
            {
                throw new ApplicationException("User does not exist.");
            }
            var ev = await _eventRepository.GetEventById(event_id);
            if (ev == null || event_id != user_id.Value)
            {
                throw new ApplicationException("Event does not exist.");
            }
            var park = await _parkRepository.GetParkByIdAsync(ev.park_id);
            if (park == null)
            {
                throw new ApplicationException("Park is currently Null.");
            }
            return new EventDTO
            {
                user_id = ev.user_id,
                park_id = ev.park_id,
                event_name = ev.event_name,
                event_description = ev.event_description,
                event_start_date = ev.event_start_date,
                event_end_date = ev.event_end_date,
                event_start_time = ev.event_start_time,
                event_end_time = ev.event_end_time,
                User = new UserDTO
                {
                    first_name = user.first_name,
                    last_name = user.last_name,

                },
                Park = new ParkDTO
                {
                    park_name = park.park_name,
                    street_address = park.street_address,
                    city = park.city,
                    state = park.state,
                    zipcode = park.zipcode,
                }
            };
        }
        public async Task<IEnumerable<EventDTO>> GetAllEventsAsync(int user_id)
        {
            var logged_in_user_id = _userService.GetLoggedInUserId();
            if (logged_in_user_id == null)
            {
                throw new ApplicationException("User is not Logged In.");
            }
            var user = await _userRepository.GetUserByIdAsync(logged_in_user_id.Value);
            if (user == null)
            {
                throw new ApplicationException("User does not exist.");
            }
            var events = await _eventRepository.GetAllEventsAsync(logged_in_user_id.Value);
            if (events == null)
            {
                return Enumerable.Empty<EventDTO>();
            }
            var eventDTOs = events.Select(ev => new EventDTO
            {
                event_id = ev.event_id,
                registration_id = ev.registration_id,
                user_id = ev.user_id,
                park_id = ev.park_id,
                event_name = ev.event_name,
                event_description = ev.event_description,
                event_start_date = ev.event_start_date,
                event_end_date = ev.event_end_date,
                event_start_time = ev.event_start_time,
                event_end_time = ev.event_end_time,
                User = new UserDTO
                {
                    first_name = user.first_name,
                    last_name = user.last_name,
                },
                Park = new ParkDTO
                {
                    park_name = ev.Park.park_name,
                    street_address = ev.Park.street_address,
                    city = ev.Park.city,
                    state = ev.Park.state,
                    zipcode = ev.Park.zipcode,
                }
            });
            return eventDTOs;
        }
    }
}
