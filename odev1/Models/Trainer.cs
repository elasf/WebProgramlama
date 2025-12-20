using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace odev1.Models
{

    public class Trainer
    {

        public int id { get; set; } 
        public string fullName { get; set; }

        public string userId { get; set; }

        
        
        [ForeignKey("userId")]
        public UserDetails User { get; set; }

        public ICollection<TrainerExpertise> trainerExpertises { get; set; }
        public ICollection<TrainerService> trainerServices { get; set; }
        public ICollection<Availability> availabilities { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

        public TimeSpan WeekdayStart { get; set; } = new TimeSpan(9, 0, 0);  // 09:00
        public TimeSpan WeekdayEnd { get; set; } = new TimeSpan(21, 0, 0);    // 21:00
        public TimeSpan WeekendStart { get; set; } = new TimeSpan(10, 0, 0); // 10:00
        public TimeSpan WeekendEnd { get; set; } = new TimeSpan(17, 0, 0);

    }








}