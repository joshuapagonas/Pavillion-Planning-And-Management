using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using PPM.Models.DTOs;
using PPM.Models.Interfaces;
using PPM.Services;

namespace PPM.Models.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly PPMDBContext _db;
        private readonly ILogger<RegistrationRepository> _logger;
        public RegistrationRepository(PPMDBContext db, ILogger<RegistrationRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<Registration> CreateRegistrationAsync(Registration registration)
        {
            var hasConflict = await _db.Registration.AnyAsync(r => 
            r.user_id == registration.user_id && 
            r.park_id == registration.park_id && 
            r.registration_date == registration.registration_date && 
            r.start_time == registration.start_time);

            if (hasConflict)
            {
                throw new ApplicationException("You already have a booking at this time.");
            }

            _db.Registration.Add(registration);
            await _db.SaveChangesAsync();
            _logger.LogInformation("✅ Created Registration ID: {Id}", registration.registration_id);
            return registration;
        }
        public async Task<Registration> GetRegistrationByIdAsync(int registration_id)
        {
            var registration = await _db.Registration.FindAsync(registration_id);
            if (registration == null)
            {
                throw new KeyNotFoundException($"Registration with ID {registration_id} was not found.");
            }
            return registration;
        }
        public async Task<IEnumerable<Registration>> GetAllRegistrationsAsync(int user_id)
        {
            var registration_inquiries = await _db.Registration.Include(r=>r.Park).Where(u => u.user_id == user_id).ToListAsync();
            if (registration_inquiries == null)
            {
                throw new KeyNotFoundException($"No Registrations with ID {user_id} was not found.");
            }
            return registration_inquiries;
        }
    }
}
