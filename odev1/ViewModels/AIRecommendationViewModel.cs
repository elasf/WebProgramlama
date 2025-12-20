using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace odev1.ViewModels
{
    public class AIRecommendationViewModel
    {
        [Display(Name = "Boy (cm)")]
        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasında olmalıdır.")]
        public decimal? Height { get; set; }

        [Display(Name = "Kilo (kg)")]
        [Range(30, 300, ErrorMessage = "Kilo 30-300 kg arasında olmalıdır.")]
        public decimal? Weight { get; set; }

        [Display(Name = "Vücut Tipi")]
        public string? BodyType { get; set; }

        [Display(Name = "Hedef")]
        public string? Goal { get; set; }

        [Display(Name = "Fotoğraf (Opsiyonel)")]
        public IFormFile? Photo { get; set; }
    }

    public class AIRecommendationResultViewModel
    {
        public int Id { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public string? BodyType { get; set; }
        public string? Goal { get; set; }
        public string? PhotoPath { get; set; }
        public string? ExerciseRecommendations { get; set; }
        public string? DietRecommendations { get; set; }
        public string? GeneralAdvice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

