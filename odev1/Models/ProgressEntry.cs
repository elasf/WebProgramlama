using System.ComponentModel.DataAnnotations;

namespace odev1.Models
{
    public class ProgressEntry
    {
        public int id { get; set; }

        [Required]
        public string userId { get; set; }
        public UserDetails user { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime date { get; set; }

        [Range(1, 500)]
        public decimal weightKg { get; set; }

        [Range(1, 100)]
        public decimal? bodyFatPercent { get; set; }

        // Opsiyonel: bel/kalça/göğüs ölçüleri ileride eklenebilir
    }
}

