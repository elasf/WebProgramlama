using System.ComponentModel.DataAnnotations;

namespace odev1.Models
{

    public class Service
    {

        public int id { get; set; }
        public string name { get; set; }
        public int duraiton { get; set; }

        public decimal price { get; set; }

        public ICollection<TrainerService> trainerService { get; set; }




    }
}