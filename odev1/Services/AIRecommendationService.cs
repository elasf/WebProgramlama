using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using odev1.Data;
using odev1.Models;
using odev1.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace odev1.Services
{
    public class AIRecommendationService : IAIRecommendationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIRecommendationService> _logger;

        public AIRecommendationService(
            ApplicationDbContext context,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<AIRecommendationService> logger)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<AIRecommendation> GetRecommendationsAsync(
            string userId,
            int? memberId,
            decimal? height,
            decimal? weight,
            string? bodyType,
            string? goal,
            string? photoPath)
        {
            // AI önerilerini oluştur
            var exerciseRecommendations = await GetExerciseRecommendationsAsync(height, weight, bodyType, goal);
            var dietRecommendations = await GetDietRecommendationsAsync(height, weight, bodyType, goal);
            var generalAdvice = await GetGeneralAdviceAsync(height, weight, bodyType, goal);

            // Veritabanına kaydet
            var recommendation = new AIRecommendation
            {
                UserId = userId,
                MemberId = memberId,
                Height = height,
                Weight = weight,
                BodyType = bodyType,
                Goal = goal,
                PhotoPath = photoPath,
                ExerciseRecommendations = exerciseRecommendations,
                DietRecommendations = dietRecommendations,
                GeneralAdvice = generalAdvice,
                CreatedAt = DateTime.UtcNow
            };

            _context.AIRecommendations.Add(recommendation);
            await _context.SaveChangesAsync();

            return recommendation;
        }

        public async Task<List<AIRecommendation>> GetUserRecommendationsAsync(string userId)
        {
            return await _context.AIRecommendations
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<AIRecommendation?> GetRecommendationByIdAsync(int id, string userId)
        {
            return await _context.AIRecommendations
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        }

        private async Task<string> GetExerciseRecommendationsAsync(
            decimal? height, decimal? weight, string? bodyType, string? goal)
        {
            var prompt = BuildExercisePrompt(height, weight, bodyType, goal);
            return await CallGeminiAPIAsync(prompt);
        }

        private async Task<string> GetDietRecommendationsAsync(
            decimal? height, decimal? weight, string? bodyType, string? goal)
        {
            var prompt = BuildDietPrompt(height, weight, bodyType, goal);
            return await CallGeminiAPIAsync(prompt);
        }

        private async Task<string> GetGeneralAdviceAsync(
            decimal? height, decimal? weight, string? bodyType, string? goal)
        {
            var prompt = BuildGeneralAdvicePrompt(height, weight, bodyType, goal);
            return await CallGeminiAPIAsync(prompt);
        }

        private string BuildExercisePrompt(decimal? height, decimal? weight, string? bodyType, string? goal)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Sen bir fitness uzmanısın. Aşağıdaki bilgilere göre kişiselleştirilmiş egzersiz önerileri sun:");
            sb.AppendLine();
            
            if (height.HasValue) sb.AppendLine($"Boy: {height} cm");
            if (weight.HasValue) sb.AppendLine($"Kilo: {weight} kg");
            if (!string.IsNullOrEmpty(bodyType)) sb.AppendLine($"Vücut Tipi: {bodyType}");
            if (!string.IsNullOrEmpty(goal)) sb.AppendLine($"Hedef: {goal}");
            
            sb.AppendLine();
            sb.AppendLine("Lütfen şunları içeren detaylı bir egzersiz planı oluştur:");
            sb.AppendLine("- Haftalık egzersiz programı");
            sb.AppendLine("- Her egzersiz için set/tekrar sayıları");
            sb.AppendLine("- İlerleme önerileri");
            sb.AppendLine("- Dikkat edilmesi gerekenler");
            sb.AppendLine();
            sb.AppendLine("Yanıtını Türkçe olarak ver.");

            return sb.ToString();
        }

        private string BuildDietPrompt(decimal? height, decimal? weight, string? bodyType, string? goal)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Sen bir beslenme uzmanısın. Aşağıdaki bilgilere göre kişiselleştirilmiş diyet önerileri sun:");
            sb.AppendLine();
            
            if (height.HasValue) sb.AppendLine($"Boy: {height} cm");
            if (weight.HasValue) sb.AppendLine($"Kilo: {weight} kg");
            if (!string.IsNullOrEmpty(bodyType)) sb.AppendLine($"Vücut Tipi: {bodyType}");
            if (!string.IsNullOrEmpty(goal)) sb.AppendLine($"Hedef: {goal}");
            
            sb.AppendLine();
            sb.AppendLine("Lütfen şunları içeren detaylı bir beslenme planı oluştur:");
            sb.AppendLine("- Günlük kalori hedefi");
            sb.AppendLine("- Makro besin dağılımı (protein, karbonhidrat, yağ)");
            sb.AppendLine("- Örnek öğün planları");
            sb.AppendLine("- Önerilen besinler");
            sb.AppendLine("- Kaçınılması gerekenler");
            sb.AppendLine();
            sb.AppendLine("Yanıtını Türkçe olarak ver.");

            return sb.ToString();
        }

        private string BuildGeneralAdvicePrompt(decimal? height, decimal? weight, string? bodyType, string? goal)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Sen bir fitness ve sağlık koçusun. Aşağıdaki bilgilere göre genel tavsiyeler sun:");
            sb.AppendLine();
            
            if (height.HasValue && weight.HasValue)
            {
                var bmi = (double)weight / Math.Pow((double)height / 100, 2);
                sb.AppendLine($"BMI: {bmi:F2}");
            }
            if (height.HasValue) sb.AppendLine($"Boy: {height} cm");
            if (weight.HasValue) sb.AppendLine($"Kilo: {weight} kg");
            if (!string.IsNullOrEmpty(bodyType)) sb.AppendLine($"Vücut Tipi: {bodyType}");
            if (!string.IsNullOrEmpty(goal)) sb.AppendLine($"Hedef: {goal}");
            
            sb.AppendLine();
            sb.AppendLine("Lütfen şunları içeren genel tavsiyeler ver:");
            sb.AppendLine("- Sağlıklı yaşam önerileri");
            sb.AppendLine("- Motivasyon ipuçları");
            sb.AppendLine("- Uyku ve dinlenme önerileri");
            sb.AppendLine("- Genel sağlık tavsiyeleri");
            sb.AppendLine();
            sb.AppendLine("Yanıtını Türkçe olarak ver.");

            return sb.ToString();
        }

        private async Task<string> CallGeminiAPIAsync(string prompt)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Gemini API key bulunamadı. Varsayılan mesaj döndürülüyor.");
                return "Gemini API anahtarı yapılandırılmamış. Lütfen appsettings.json dosyasına Gemini:ApiKey ekleyin. Ücretsiz API key için: https://aistudio.google.com/apikey";
            }

            try
            {
                // Gemini API için sistem prompt'u kullanıcı prompt'una ekle
                var fullPrompt = "Sen bir fitness ve beslenme uzmanısın. Türkçe yanıt ver.\n\n" + prompt;

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = fullPrompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 2000
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();

                // Gemini API endpoint - gemini-pro modeli kullanıyoruz
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";
                
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    
                    // Gemini API response yapısı
                    if (jsonDoc.RootElement.TryGetProperty("candidates", out var candidates))
                    {
                        if (candidates.GetArrayLength() > 0)
                        {
                            var candidate = candidates[0];
                            if (candidate.TryGetProperty("content", out var contentElement))
                            {
                                if (contentElement.TryGetProperty("parts", out var parts))
                                {
                                    if (parts.GetArrayLength() > 0)
                                    {
                                        var text = parts[0].GetProperty("text").GetString();
                                        return text ?? "Yanıt alınamadı.";
                                    }
                                }
                            }
                        }
                    }
                    
                    _logger.LogWarning("Gemini API yanıt formatı beklenmedik: {Response}", responseContent);
                    return "Yanıt formatı beklenmedik.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API hatası: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    return $"API hatası: {response.StatusCode}. Lütfen daha sonra tekrar deneyin.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini API çağrısı sırasında hata oluştu");
                return $"Bir hata oluştu: {ex.Message}. Lütfen daha sonra tekrar deneyin.";
            }
        }
    }
}

