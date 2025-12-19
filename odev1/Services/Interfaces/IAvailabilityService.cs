using odev1.Models;

namespace odev1.Services
{
    public interface IAvailabilityService
    {
        Task<List<Availability>> GetTrainerAvailabilitiesAsync(int trainerId);
        Task<Availability?> GetByIdAsync(int id);
        Task<Availability> CreateAsync(Availability availability);
        Task<Availability> UpdateAsync(Availability availability);
        Task DeleteAsync(int id);
    }
}

