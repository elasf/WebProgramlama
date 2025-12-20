using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace odev1.Models
{
    public class Member
    {
        
       
        public int Id { get; set; }

        //identity ile bağlantı
        public string UserId { get; set; }

        public UserDetails User { get; set; }

        // --- Müşteriye Özel Alanlar ---
        public string FullName { get; set; } 

        public DateTime KayitTarihi { get; set; }

        //boy kilo eklenebilir idk


        // randevular eklenecek
        // public ICollection<Appointment> Appointments { get; set; }
        
        // AI önerileri
        public ICollection<AIRecommendation> AIRecommendations { get; set; } 
    }
}