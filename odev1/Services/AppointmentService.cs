using odev1.Data;
using odev1.Models;

namespace odev1.Services
{
    public class AppointmentService
    {
        private readonly ApplicationDbContext _context;
        public AppointmentService(ApplicationDbContext context) 
        {
            _context = context;
        }
        //randevuOlusturabilirMi() ingilizcen yetmezse diye türkçeleri yazıyorum hadi iyisn 🤪
        public bool canCreateAppointment(Appointment appointment)
        {
            return isTrainerAvailable(
                appointment.trainerId,
                appointment.AppointmentDate,
                appointment.StartTime,
                appointment.EndTime
            );
        }


        public bool CreateAppointment(Appointment appointment)
        {
            if (!canCreateAppointment(appointment))
                return false;

            appointment.Status = AppointmentStatus.Pending;

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return true;
        }




        //--------------------------------------------------
        //Bunlar daha çok yardımcı fonksiyonlar 

        private bool isTrainerAvailable(
                  int trainerId,
                  DateTime date,
                  TimeSpan startTime,
                  TimeSpan endTime)
        {

            if (!isWithinTrainerAvailability(trainerId, date, startTime, endTime))
                return false;

            if (hasAppointmentConflict(trainerId, date, startTime, endTime))
                return false;

            return true;
        }

        //trainer müsait  mi 
        private bool isWithinTrainerAvailability(
            int trainerId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            //BUnlardan biri bile yanlış çıkarsa trainer müsait değil abla.
            //Şimdilik baya basit ileride canımız sıkılırsa geliştiririz.
            return _context.Availabilities.Any(a =>
            a.tarinerId == trainerId &&
            a.date.Date == date.Date &&
            startTime >= a.startTime &&
            endTime <= a.endTime);

        }

        private bool hasAppointmentConflict(

            int trainerId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            //biraz açıklama gereği duydum ben de yeni yeni öğreniyorum şahsen.
            //en başta bir tane nesne ürettik, ürettiğimiz nesne(_context) db nesnesi.
            //onunn sayesinde içindeki tablolara, tabloların içinde ki parametlere ulaşabiliyoruz ve sonrasında metotun parametleriyle db deki dataları karşılaştırıyoruz.
            //fark ettiğin gibi sql sorgusu atıyor(any)ve en başta bizim yazdığımız "ap" ile db deki verileri alıyor sonra karılaştırıyor birisi bile yanlış ise false dönecek yukarıda metotları etkileyecek.
            //İnş anlatabilmişimdir. Yukarıda da aynısı yapılıyoe zaten anlamışsındır 
            return _context.Appointments.Any(ap =>
                ap.trainerId == trainerId &&
                ap.AppointmentDate.Date == date.Date &&
                ap.Status != AppointmentStatus.Cancelled &&
                startTime < ap.EndTime &&
                endTime > ap.StartTime
            );
        }


    }
}