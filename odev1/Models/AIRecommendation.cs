using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace odev1.Models
{
    public class AIRecommendation
    {
        public int Id { get; set; }

        // Kullanıcı bağlantısı
        [Required]
        public string UserId { get; set; }
        
        [ForeignKey("UserId")]
        public UserDetails User { get; set; }

        // Member bağlantısı (opsiyonel - her user bir member olabilir)
        public int? MemberId { get; set; }
        
        [ForeignKey("MemberId")]
        public Member? Member { get; set; }

        // İstek verileri (kullanıcının gönderdiği bilgiler)
        public decimal? Height { get; set; } // cm cinsinden
        public decimal? Weight { get; set; } // kg cinsinden
        public string? BodyType { get; set; } // "ectomorph", "mesomorph", "endomorph" vb.
        public string? Goal { get; set; } // "weight_loss", "muscle_gain", "general_fitness" vb.
        public string? PhotoPath { get; set; } // Yüklenen fotoğraf yolu (opsiyonel)

        // AI'dan gelen öneriler
        [Column(TypeName = "text")]
        public string? ExerciseRecommendations { get; set; } // Egzersiz önerileri

        [Column(TypeName = "text")]
        public string? DietRecommendations { get; set; } // Diyet önerileri

        [Column(TypeName = "text")]
        public string? GeneralAdvice { get; set; } // Genel tavsiyeler

        // Meta bilgiler
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}

