using Microsoft.EntityFrameworkCore;
using odev1.Data;
using odev1.Models;

namespace odev1.Services
{
    // Eğitmen listeleme ve temel güncellemeler
    public class TrainerService : ITrainerService
    {
        private readonly ApplicationDbContext _context;
        public TrainerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Trainer>> GetAllAsync()
        {
            return await _context.Trainers
                .AsNoTracking()
                .OrderBy(t => t.fullName)
                .ToListAsync();
        }

        public async Task<Trainer?> GetByIdAsync(int id, bool includeRelations = false)
        {
            IQueryable<Trainer> q = _context.Trainers;
            if (includeRelations)
            {
                q = q
                    .Include(t => t.trainerExpertises)
                    .Include(t => t.trainerServices)
                    .Include(t => t.availabilities);
            }
            return await q.FirstOrDefaultAsync(t => t.id == id);
        }

        public async Task<Trainer> UpdateNameAsync(int id, string fullName)
        {
            var t = await _context.Trainers.FirstOrDefaultAsync(x => x.id == id);
            if (t == null) throw new KeyNotFoundException("Trainer bulunamadı.");
            t.fullName = fullName;
            await _context.SaveChangesAsync();
            return t;
        }
    }
}

