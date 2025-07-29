// EventRepository.cs
using PPM;
using PPM.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PPM.Models.Services
{
    public class EventRepository : IEventRepository
    {
        private readonly PPMDBContext _db;
        public EventRepository(PPMDBContext db)
        {
            _db = db;
        }
        public async Task<Event> CreateEventAsync(Event ev)
        {
            _db.Event.Add(ev);
            await _db.SaveChangesAsync();
            return ev;
        }

        public async Task<Event> GetEventById(int event_id)
        {
            var ev = await _db.Event.FindAsync(event_id);
            if (ev == null)
            {
                throw new KeyNotFoundException($"Event with ID {event_id} was not found.");
            }
            return ev;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync(int user_id)
        {
            var events = await _db.Event.Where(u => u.user_id == user_id).ToListAsync();
            if (events == null)
            {
                throw new KeyNotFoundException($"No Events with ID {user_id} was not found.");
            }
            return events;
        }
    }
}
