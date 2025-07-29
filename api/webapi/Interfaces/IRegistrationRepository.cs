namespace PPM.Models.Interfaces
{
    public interface IRegistrationRepository
    {
        Task<Registration> CreateRegistrationAsync(Registration registration);
        Task<Registration> GetRegistrationByIdAsync(int registration_id);
        Task<IEnumerable<Registration>> GetAllRegistrationsAsync(int user_id);
    }
}

