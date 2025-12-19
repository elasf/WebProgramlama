using System.ComponentModel.DataAnnotations;

namespace odev1.Models.ViewModels
{
    public class TrainerRegisterViewModel
    {
        // admin kaydederken bunları girecek

        [Required, EmailAddress]
        public string email { get; set; }

        [Required, MinLength(6), MaxLength(100)]
        public string password { get; set; }

        [Required, StringLength(30, MinimumLength = 2)]
        public string firstName { get; set; }

        [Required, StringLength(30, MinimumLength = 2)]
        public string lastName { get; set; }

        [Required, Phone]
        public string phoneNumber { get; set; }

        public string experties { get; set; }
        public string services { get; set; }


    }
}