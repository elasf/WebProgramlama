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


       

        //adres sonra eklenebilir
        

        //şifre ve telefon kısmı silindi oto yapılıyor
        ///Password2


    }
}
