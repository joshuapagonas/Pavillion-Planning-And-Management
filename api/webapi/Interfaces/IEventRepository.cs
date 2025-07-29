// IEventRepository.cs
namespace PPM.Models.Interfaces
{
    public interface IEventRepository
    {
        Task<Event> CreateEventAsync(Event ev);
        Task<Event> GetEventById(int event_id);
        Task<IEnumerable<Event>> GetAllEventsAsync(int user_id);
    }
}
