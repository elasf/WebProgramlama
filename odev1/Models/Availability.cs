using System.ComponentModel.DataAnnotations;

namespace odev1.Models
{

    public class Availability
    {

        public int id { get; set; }

        [Required]
        public int trainerId { get; set; }
        public Trainer trainer { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan startTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan endTime { get; set; }




    }
}