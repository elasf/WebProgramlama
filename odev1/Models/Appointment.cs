using System.ComponentModel.DataAnnotations;

namespace odev1.Models
{

    public class Appointment
    {
        public int id { get; set; }

        public string userId { get; set; }
        public UserDetails user { get; set; }

        public int trainerId { get; set; }
        public Trainer trainer { get; set; }

        public int serviceId { get; set; }
        public Service service { get; set; }


        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        
        public AppointmentStatus Status { get; set; }

        
        public decimal Price { get; set; }


    }
}