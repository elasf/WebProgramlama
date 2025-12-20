using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace odev1.ViewModels
{
    // Kullanıcının randevu oluştururken dolduracağı form modeli
    public class AppointmentCreateViewModel
    {
        [Required]
        public int? ServiceId { get; set; }

        [Required]
        public int? TrainerId { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime? AppointmentDate { get; set; }

        [Required, DataType(DataType.Time)]
        public TimeSpan? StartTime { get; set; }

        public decimal? Price { get; set; }

        // Dropdown listeleri için
        public IEnumerable<SelectListItem> Services { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Trainers { get; set; } = Enumerable.Empty<SelectListItem>();

        public List<SlotItemViewModel> AvailableSlots { get; set; } = new List<SlotItemViewModel>();
    }
}

