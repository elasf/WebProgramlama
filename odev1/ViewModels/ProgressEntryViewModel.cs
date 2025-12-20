using System.ComponentModel.DataAnnotations;

namespace odev1.ViewModels
{
    public class ProgressEntryViewModel
    {
        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required, Range(1, 500)]
        public decimal WeightKg { get; set; }

        [Range(1, 100)]
        public decimal? BodyFatPercent { get; set; }
    }
}

