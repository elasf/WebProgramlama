using System.ComponentModel.DataAnnotations;

namespace odev1.ViewModels
{
    public class ProfileViewModel
    {
        [Required, StringLength(30, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required, StringLength(30, MinimumLength = 2)]
        public string LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}

