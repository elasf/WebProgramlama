using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using odev1.Data;
using odev1.Models;
using odev1.Services.Interfaces;
using System.Text;
using System.Text.Json;
using System.Net;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace odev1.Services
{
    public class AIRecommendationService : IAIRecommendationService
    {
        private static readonly SemaphoreSlim ApiConcurrency = new SemaphoreSlim(2); // limit concurrent external calls
        private static readonly ConcurrentDictionary<string, (string Content, DateTime ExpiresAt)> PromptCache = new();

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
            // Tek bir API çağrısında üç bölümü birlikte al
            var (exerciseRecommendations, dietRecommendations, generalAdvice) =
                await GetCombinedRecommendationsAsync(height, weight, bodyType, goal);

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
            return await CallAIAPIAsync(prompt);
        }

        private async Task<string> GetDietRecommendationsAsync(
            decimal? height, decimal? weight, string? bodyType, string? goal)
        {
            var prompt = BuildDietPrompt(height, weight, bodyType, goal);
            return await CallAIAPIAsync(prompt);
        }

        private async Task<string> GetGeneralAdviceAsync(
            decimal? height, decimal? weight, string? bodyType, string? goal)
        {
            var prompt = BuildGeneralAdvicePrompt(height, weight, bodyType, goal);
            return await CallAIAPIAsync(prompt);
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

        private string BuildCombinedPrompt(decimal? height, decimal? weight, string? bodyType, string? goal)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Sen fitnes ve beslenme alanında uzmansın. Türkçe yanıt ver.");
            sb.AppendLine("Aşağıdaki bilgiler ışığında üç ayrı bölüm üret ve her bölümü işaretlerle ayır:");
            sb.AppendLine("[EXERCISE] ... [/EXERCISE]");
            sb.AppendLine("[DIET] ... [/DIET]");
            sb.AppendLine("[GENERAL] ... [/GENERAL]");
            sb.AppendLine();
            if (height.HasValue) sb.AppendLine($"Boy: {height} cm");
            if (weight.HasValue) sb.AppendLine($"Kilo: {weight} kg");
            if (!string.IsNullOrEmpty(bodyType)) sb.AppendLine($"Vücut Tipi: {bodyType}");
            if (!string.IsNullOrEmpty(goal)) sb.AppendLine($"Hedef: {goal}");
            sb.AppendLine();
            sb.AppendLine("EXERCISE bölümünde: Haftalık program, set/tekrar, ilerleme önerileri, dikkat noktaları.");
            sb.AppendLine("DIET bölümünde: Kalori hedefi, makro dağılımı, örnek öğün planları, önerilen/kaçınılacaklar.");
            sb.AppendLine("GENERAL bölümünde: Sağlıklı yaşam, motivasyon, uyku/dinlenme, genel sağlık tavsiyeleri.");
            return sb.ToString();
        }

        private static (string Exercise, string Diet, string General) ParseCombinedResponse(string content)
        {
            static string Extract(string text, string tag)
            {
                var start = text.IndexOf($"[{tag}]", StringComparison.OrdinalIgnoreCase);
                var end = text.IndexOf($"[/{tag}]", StringComparison.OrdinalIgnoreCase);
                if (start >= 0 && end > start)
                {
                    var s = start + tag.Length + 2; // [TAG]
                    return text.Substring(s, end - s).Trim();
                }
                return string.Empty;
            }

            var exercise = Extract(content, "EXERCISE");
            var diet = Extract(content, "DIET");
            var general = Extract(content, "GENERAL");

            // Boş kalanları tüm yanıtla doldur (en azından içerik sağlansın)
            if (string.IsNullOrWhiteSpace(exercise)) exercise = content;
            if (string.IsNullOrWhiteSpace(diet)) diet = content;
            if (string.IsNullOrWhiteSpace(general)) general = content;
            return (exercise, diet, general);
        }

        private async Task<(string Exercise, string Diet, string General)> GetCombinedRecommendationsAsync(
            decimal? height, decimal? weight, string? bodyType, string? goal)
        {
            var prompt = BuildCombinedPrompt(height, weight, bodyType, goal);
            var response = await CallAIAPIAsync(prompt);
            return ParseCombinedResponse(response);
        }

        private static string ComputeCacheKey(string model, string prompt)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(model + "|" + prompt);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        private async Task<string> CallAIAPIAsync(string prompt)
        {
            var groqKey = _configuration["Groq:ApiKey"];
            var groqModel = _configuration["Groq:Model"];
            var maxTokensStr = _configuration["Groq:MaxOutputTokens"];
            int maxTokens = 1024;
            if (!string.IsNullOrWhiteSpace(maxTokensStr) && int.TryParse(maxTokensStr, out var parsed) && parsed > 0)
            {
                maxTokens = parsed;
            }

            try
            {
                // Sistem prompt'u kullanıcı prompt'una ekle
                var fullPrompt = "Sen bir fitness ve beslenme uzmanısın. Türkçe yanıt ver.\n\n" + prompt;

                if (string.IsNullOrWhiteSpace(groqKey))
                {
                    _logger.LogWarning("Groq API key bulunamadı.");
                    return "Groq API anahtarı yapılandırılmamış. Lütfen appsettings.json dosyasına Groq:ApiKey ekleyin. Ücretsiz API key için: https://console.groq.com/keys";
                }
                var resolvedModel = string.IsNullOrWhiteSpace(groqModel) ? "llama-3.1-70b-versatile" : groqModel;
                var groqBody = new
                {
                    model = resolvedModel,
                    messages = new object[]
                    {
                        new { role = "system", content = "Sen bir fitness ve beslenme uzmanısın. Türkçe yanıt ver." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.7,
                    max_tokens = maxTokens
                };
                var groqJson = JsonSerializer.Serialize(groqBody);
                var content = new StringContent(groqJson, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {groqKey}");
                var url = "https://api.groq.com/openai/v1/chat/completions";

                // Basit retry: 429/503 durumlarında exponential backoff ile tekrar dene
                var attempt = 0;
                var maxAttempts = 3;
                Exception? lastException = null;
                var cacheKey = ComputeCacheKey(resolvedModel, fullPrompt);

                // Cache kontrolü (10 dk)
                if (PromptCache.TryGetValue(cacheKey, out var cached) && cached.ExpiresAt > DateTime.UtcNow)
                {
                    return cached.Content;
                }

                await ApiConcurrency.WaitAsync();
                while (attempt < maxAttempts)
                {
                    attempt++;
                    HttpResponseMessage? response = null;
                    try
                    {
                        response = await _httpClient.PostAsync(url, content);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var jsonDoc = JsonDocument.Parse(responseContent);
                            // OpenAI/Groq tarzı yanıt: choices
                            if (jsonDoc.RootElement.TryGetProperty("choices", out var choices))
                            {
                                if (choices.GetArrayLength() > 0)
                                {
                                    var choice = choices[0];
                                    if (choice.TryGetProperty("message", out var msg) && msg.TryGetProperty("content", out var msgContent))
                                    {
                                        var result = msgContent.GetString() ?? "Yanıt alınamadı.";
                                        PromptCache[cacheKey] = (result, DateTime.UtcNow.AddMinutes(10));
                                        return result;
                                    }
                                    // bazı sağlayıcılarda 'text' alanı olabilir
                                    if (choice.TryGetProperty("text", out var textNode))
                                    {
                                        var result = textNode.GetString() ?? "Yanıt alınamadı.";
                                        PromptCache[cacheKey] = (result, DateTime.UtcNow.AddMinutes(10));
                                        return result;
                                    }
                                }
                            }
                            _logger.LogWarning("AI API yanıt formatı beklenmedik: {Response}", responseContent);
                            return "Yanıt formatı beklenmedik.";
                        }
                        else if (response.StatusCode == HttpStatusCode.TooManyRequests || response.StatusCode == HttpStatusCode.ServiceUnavailable)
                        {
                            // Retry After başlığı varsa ona uy
                            TimeSpan delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)); // 2,4,8
                            if (response.Headers.RetryAfter != null)
                            {
                                if (response.Headers.RetryAfter.Delta.HasValue)
                                {
                                    delay = response.Headers.RetryAfter.Delta.Value;
                                }
                                else if (response.Headers.RetryAfter.Date.HasValue)
                                {
                                    var until = response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
                                    if (until > TimeSpan.Zero) delay = until;
                                }
                            }
                            // küçük jitter
                            var jitterMs = Random.Shared.Next(100, 400);
                            delay += TimeSpan.FromMilliseconds(jitterMs);
                            _logger.LogWarning("AI API oran limitine takıldı ({Status}). {Delay} sonra tekrar denenecek (attempt {Attempt}/{Max}).", response.StatusCode, delay, attempt, maxAttempts);
                            await Task.Delay(delay);
                            continue;
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            _logger.LogError("AI API hatası: {StatusCode} - {Error}", response.StatusCode, errorContent);
                            return $"API hatası: {response.StatusCode}. Lütfen daha sonra tekrar deneyin.";
                        }
                    }
                    catch (Exception exLoop)
                    {
                        lastException = exLoop;
                        _logger.LogWarning(exLoop, "AI API çağrısı denemesi başarısız (attempt {Attempt}/{Max})", attempt, maxAttempts);
                        // kısa bekleme ile yeniden dene
                        var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(100, 300));
                        await Task.Delay(delay);
                        continue;
                    }
                    finally
                    {
                        response?.Dispose();
                    }
                }
                ApiConcurrency.Release();
                
                // Tüm denemeler tükendi
                if (lastException != null)
                {
                    _logger.LogError(lastException, "AI API çağrısı tekrar denemelerine rağmen başarısız oldu");
                }
                return "Şu anda AI servisi yoğun. Lütfen kısa bir süre sonra yeniden deneyin.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI API çağrısı sırasında hata oluştu");
                return $"Bir hata oluştu: {ex.Message}. Lütfen daha sonra tekrar deneyin.";
            }
        }
    }
}

