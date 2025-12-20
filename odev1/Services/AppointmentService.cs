using odev1.Data;
using odev1.Models;

namespace odev1.Services
{
    public class AppointmentService : IAppointmentService
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


        public Appointment create(Appointment appointment)
        {
            if (!canCreateAppointment(appointment))
                throw new Exception("Trainer is not available.");

            appointment.Status = odev1.Models.AppointmentStatus.Pending;

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return appointment;
        }


        public Appointment update(Appointment updated)
        {
            var existing = _context.Appointments
                .FirstOrDefault(a => a.id == updated.id);

            if (existing == null)
                throw new KeyNotFoundException("Appointment not found.");

            if (!canCreateAppointment(updated))
                throw new InvalidOperationException("Updated appointment is not valid.");

            existing.trainerId = updated.trainerId;
            existing.serviceId = updated.serviceId;
            existing.AppointmentDate = updated.AppointmentDate;
            existing.StartTime = updated.StartTime;
            existing.EndTime = updated.EndTime;

            _context.SaveChanges();
            return existing;
        }

        public Appointment delete(int id)
        {
            var appointment = _context.Appointments
                .FirstOrDefault(a => a.id == id);

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            appointment.Status = odev1.Models.AppointmentStatus.Cancelled;
            _context.SaveChanges();

            return appointment;

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
        private bool isWithinTrainerAvailability(int trainerId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var trainer = _context.Trainers.Find(trainerId);
            if (trainer == null) return false;

            bool isWeekend = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);

            // Hafta sonu veya hafta içi için sınırları belirle
            TimeSpan startLimit = isWeekend ? trainer.WeekendStart : trainer.WeekdayStart;
            TimeSpan endLimit = isWeekend ? trainer.WeekendEnd : trainer.WeekdayEnd;

            // 1. Randevu hocanın mesai saatleri içinde mi?
            // 2. Geçmiş bir saate randevu alınmaya çalışılıyor mu? (Bugün için kontrol)
            if (date.Date == DateTime.Today && startTime < DateTime.Now.TimeOfDay)
                return false;

            return startTime >= startLimit && endTime <= endLimit;
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
                ap.Status != odev1.Models.AppointmentStatus.Cancelled &&
                startTime < ap.EndTime &&
                endTime > ap.StartTime
            );
        }


    }
}