using System.ComponentModel.DataAnnotations;

namespace odev1.Models
{

    public class TrainerExpertise
    {

        public int trainerId { get; set; }
        public Trainer trainer { get; set; }

        public int expertiseId { get; set; }
        public Expertise expertise { get; set; }


    }
}