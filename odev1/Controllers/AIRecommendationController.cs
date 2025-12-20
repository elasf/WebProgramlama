using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev1.Data;
using odev1.Models;
using odev1.Services.Interfaces;
using odev1.ViewModels;

namespace odev1.Controllers
{
    [Authorize(Roles = "user")]
    public class AIRecommendationController : Controller
    {
        private readonly IAIRecommendationService _aiService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserDetails> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AIRecommendationController> _logger;

        public AIRecommendationController(
            IAIRecommendationService aiService,
            ApplicationDbContext context,
            UserManager<UserDetails> userManager,
            IWebHostEnvironment environment,
            ILogger<AIRecommendationController> logger)
        {
            _aiService = aiService;
            _context = context;
            _userManager = userManager;
            _environment = environment;
            _logger = logger;
        }

        // AI önerisi formu
        [HttpGet]
        public IActionResult Create()
        {
            return View(new AIRecommendationViewModel());
        }

        // AI önerisi gönder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AIRecommendationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Member'ı bul
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            // Fotoğraf yükleme
            string? photoPath = null;
            if (model.Photo != null && model.Photo.Length > 0)
            {
                photoPath = await SavePhotoAsync(model.Photo);
            }

            try
            {
                // AI önerilerini al
                var recommendation = await _aiService.GetRecommendationsAsync(
                    userId: user.Id,
                    memberId: member?.Id,
                    height: model.Height,
                    weight: model.Weight,
                    bodyType: model.BodyType,
                    goal: model.Goal,
                    photoPath: photoPath
                );

                TempData["success"] = "AI önerileriniz hazırlandı!";
                return RedirectToAction(nameof(Details), new { id = recommendation.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI önerisi oluşturulurken hata oluştu");
                ModelState.AddModelError("", "AI önerileri oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
                return View(model);
            }
        }

        // AI önerisi detayı
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var recommendation = await _aiService.GetRecommendationByIdAsync(id, user.Id);
            if (recommendation == null)
            {
                return NotFound();
            }

            var viewModel = new AIRecommendationResultViewModel
            {
                Id = recommendation.Id,
                Height = recommendation.Height,
                Weight = recommendation.Weight,
                BodyType = recommendation.BodyType,
                Goal = recommendation.Goal,
                PhotoPath = recommendation.PhotoPath,
                ExerciseRecommendations = recommendation.ExerciseRecommendations,
                DietRecommendations = recommendation.DietRecommendations,
                GeneralAdvice = recommendation.GeneralAdvice,
                CreatedAt = recommendation.CreatedAt
            };

            return View(viewModel);
        }

        // Kullanıcının tüm AI önerileri
        [HttpGet]
        public async Task<IActionResult> MyRecommendations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var recommendations = await _aiService.GetUserRecommendationsAsync(user.Id);

            var viewModels = recommendations.Select(r => new AIRecommendationResultViewModel
            {
                Id = r.Id,
                Height = r.Height,
                Weight = r.Weight,
                BodyType = r.BodyType,
                Goal = r.Goal,
                PhotoPath = r.PhotoPath,
                ExerciseRecommendations = r.ExerciseRecommendations,
                DietRecommendations = r.DietRecommendations,
                GeneralAdvice = r.GeneralAdvice,
                CreatedAt = r.CreatedAt
            }).ToList();

            return View(viewModels);
        }

        // Fotoğraf kaydetme yardımcı metodu
        private async Task<string> SavePhotoAsync(IFormFile photo)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "ai-photos");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{photo.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            return $"/uploads/ai-photos/{uniqueFileName}";
        }
    }
}

