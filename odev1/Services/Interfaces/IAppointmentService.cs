using odev1.Models;

namespace odev1.Services
{
    public interface IAppointmentService
    {
        bool canCreateAppointment(Appointment appointment);
        Appointment create(Appointment appointment);
        Appointment update(Appointment updated);
        Appointment delete(int id);
    }
}

