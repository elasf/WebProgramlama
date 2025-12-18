using System.ComponentModel.DataAnnotations;

namespace odev1.Models
{

    public class Trainer
    {

        public int id { get; set; } 
        public string fullName { get; set; }

        public string userId { get; set; }

        public ICollection<TrainerExpertise> trainerExpertises { get; set; }
        public ICollection<TrainerService> trainerServices { get; set; }
        public ICollection<Availability> availabilities { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

    }








}