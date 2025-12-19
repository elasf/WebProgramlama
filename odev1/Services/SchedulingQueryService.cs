using Microsoft.EntityFrameworkCore;
using odev1.Data;
using odev1.Models;

namespace odev1.Services
{
    // LINQ filtreleri: tüm antrenörler, tarihe göre uygun olanlar, kullanıcının randevuları
    public class SchedulingQueryService : ISchedulingQueryService
    {
        private readonly ApplicationDbContext _context;
        public SchedulingQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Trainer>> GetAllTrainersAsync()
        {
            return await _context.Trainers
                .AsNoTracking()
                .OrderBy(t => t.fullName)
                .ToListAsync();
        }

        public async Task<List<Trainer>> GetAvailableTrainersByDateAsync(DateTime date, int serviceId)
        {
            // Şart: belirtilen hizmeti verebilen ve o tarihte en az bir uygunluk kaydı olan eğitmenler
            var trainerIdsForService = await _context.TrainerServices
                .Where(ts => ts.serviceId == serviceId)
                .Select(ts => ts.trainerId)
                .ToListAsync();

            var trainers = await _context.Trainers
                .Where(t => trainerIdsForService.Contains(t.id))
                .Where(t => _context.Availabilities
                    .Any(a => a.trainerId == t.id && a.date.Date == date.Date))
                .AsNoTracking()
                .OrderBy(t => t.fullName)
                .ToListAsync();

            return trainers;
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(string userId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.service)
                .Include(a => a.trainer)
                .Where(a => a.userId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.StartTime)
                .ToListAsync();
        }
    }
}

