using System.ComponentModel.DataAnnotations;

namespace odev1.ViewModels
{
    public class AppointmentRescheduleViewModel
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime? AppointmentDate { get; set; }

        [Required, DataType(DataType.Time)]
        public TimeSpan? StartTime { get; set; }

        // Görüntüleme amaçlı
        public string ServiceName { get; set; }
        public int ServiceDuration { get; set; }
        public string TrainerName { get; set; }
    }
}

