using odev1.Models;

namespace odev1.Services
{
    // LINQ tabanlı raporlama/filtreleme (REST API için kullanılacak)
    public interface ISchedulingQueryService
    {
        Task<List<Trainer>> GetAllTrainersAsync();
        Task<List<Trainer>> GetAvailableTrainersByDateAsync(DateTime date, int serviceId);
        Task<List<Appointment>> GetUserAppointmentsAsync(string userId);
    }
}

