using Microsoft.EntityFrameworkCore;
using odev1.Data;
using odev1.Models;

namespace odev1.Services
{
    // Eğitmenlerin günlük çalışma aralıklarını (availability) yönetir
    public class AvailabilityService : IAvailabilityService
    {
        private readonly ApplicationDbContext _context;
        public AvailabilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Availability>> GetTrainerAvailabilitiesAsync(int trainerId)
        {
            return await _context.Availabilities
                .AsNoTracking()
                .Where(a => a.trainerId == trainerId)
                .OrderBy(a => a.date).ThenBy(a => a.startTime)
                .ToListAsync();
        }

        public async Task<Availability?> GetByIdAsync(int id)
        {
            return await _context.Availabilities.FindAsync(id);
        }

        public async Task<Availability> CreateAsync(Availability availability)
        {
            ValidateRange(availability);
            await EnsureNoOverlapAsync(availability);

            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();
            return availability;
        }

        public async Task<Availability> UpdateAsync(Availability availability)
        {
            ValidateRange(availability);
            await EnsureNoOverlapAsync(availability, availability.id);

            _context.Availabilities.Update(availability);
            await _context.SaveChangesAsync();
            return availability;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Availabilities.FirstOrDefaultAsync(a => a.id == id);
            if (existing == null) return;
            _context.Availabilities.Remove(existing);
            await _context.SaveChangesAsync();
        }

        private static void ValidateRange(Availability a)
        {
            if (a.endTime <= a.startTime)
                throw new InvalidOperationException("Bitiş saati başlangıçtan büyük olmalı.");
        }

        private async Task EnsureNoOverlapAsync(Availability candidate, int? ignoreId = null)
        {
            var overlaps = await _context.Availabilities
                .Where(a => a.trainerId == candidate.trainerId &&
                            a.date.Date == candidate.date.Date &&
                            (ignoreId == null || a.id != ignoreId.Value))
                .Where(a => candidate.startTime < a.endTime && candidate.endTime > a.startTime)
                .AnyAsync();

            if (overlaps)
                throw new InvalidOperationException("Bu saat aralığı mevcut bir uygunlukla çakışıyor.");
        }
    }
}

