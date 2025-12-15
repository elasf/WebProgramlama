using System.ComponentModel.DataAnnotations;

namespace odev1.Models
{

    public class Availability
    {

        public int id { get; set; }
        public int tarinerId { get; set; }
        public Trainer trainer { get; set; }

        public DateTime date { get; set; }

        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }




    }
}