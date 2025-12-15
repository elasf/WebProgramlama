using System.ComponentModel.DataAnnotations;

namespace odev1.Models
{

    public class Expertise
    {

        public int id { get; set; }


        public string expertise {get;set;}

        public ICollection<TrainerExpertise> trainerExpertise { get; set; }




    }
}
