using System.ComponentModel.DataAnnotations;

namespace odev1.Models
{

    public class Service
    {

        public int id { get; set; }

        [Required]
        [StringLength(60)]
        public string name { get; set; }

        // dakika cinsinden
        [Range(1, 480)]
        public int duration { get; set; } // fixed typo

        [DataType(DataType.Currency)]
        [Range(0.0, double.MaxValue)]
        public decimal price { get; set; }

        public ICollection<TrainerService> trainerService { get; set; }




    }
}