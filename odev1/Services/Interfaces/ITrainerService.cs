using odev1.Models;

namespace odev1.Services
{
    public interface ITrainerService
    {
        Task<List<Trainer>> GetAllAsync();
        Task<Trainer?> GetByIdAsync(int id, bool includeRelations = false);
        Task<Trainer> UpdateNameAsync(int id, string fullName);
        // İleri aşamada: uzmanlık/hizmet eşleştirme metodları eklenebilir
    }
}

