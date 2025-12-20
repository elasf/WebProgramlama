using odev1.Models;

namespace odev1.Services.Interfaces
{
    public interface IAIRecommendationService
    {
        /// <summary>
        /// Kullanıcının gönderdiği bilgilere göre AI'dan egzersiz ve diyet önerileri alır
        /// </summary>
        Task<AIRecommendation> GetRecommendationsAsync(
            string userId,
            int? memberId,
            decimal? height,
            decimal? weight,
            string? bodyType,
            string? goal,
            string? photoPath);

        /// <summary>
        /// Kullanıcının tüm AI önerilerini getirir
        /// </summary>
        Task<List<AIRecommendation>> GetUserRecommendationsAsync(string userId);

        /// <summary>
        /// Belirli bir AI önerisini getirir
        /// </summary>
        Task<AIRecommendation?> GetRecommendationByIdAsync(int id, string userId);
    }
}

