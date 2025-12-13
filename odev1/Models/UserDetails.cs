using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace odev1.Models
{
    public class UserDetails :IdentityUser
    {
        [Required]
        [MaxLength(30, ErrorMessage ="İsminiz 30 karakterden uzun olamaz.")]
        [MinLength(3, ErrorMessage ="İsminiz 3 karakterden kısa olamaz.")]
        public string userAd { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "Soyisminiz 30 karakterden uzun olamaz.")]
        [MinLength(2, ErrorMessage = "Soyisminiz 2 karakterden kısa olamaz.")]
        public string userSoyad { get; set; }


        [Required]
        [Phone] //???? salladım
        public string userTel { get; set; }

        public string userAdres { get; set; }
        public string password { get; set; }
        

        //PASSWORD 
        ///Password2


    }
}
