using System.ComponentModel.DataAnnotations;

namespace odev1.Models
{

    public class TrainerService
    {

        public int trainerId { get; set; }
        public Trainer trainer { get; set; }

        public int serviceId { get; set; }  
        public Service service { get; set; }



    }
}