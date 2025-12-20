using System.ComponentModel.DataAnnotations;

namespace odev1.Models.ViewModels
{
    public class TrainerManageViewModel
    {
       
        public int? TrainerId { get; set; } //nulluble çünkü kayıtta id girmeye gerek yok

        [Required(ErrorMessage = "Ad zorunludur.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur.")]
        public string LastName { get; set; }
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress]
        public string email { get; set; }

        //edit kısmı için nullable
        public string? Password { get; set; }

        public int[] SelectedServiceIds { get; set; }

        public int[] SelectedExpertiseIds { get; set; }
    }
}