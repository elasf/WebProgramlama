using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev1.Data; // AppDbContext'in olduğu namespace
using System;
using System.Security.Claims;

namespace odev1.Controllers
{
    [Authorize(Roles = "trainer")] // Sadece eğitmenler girebilir
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(MyAppointments));
        }
        //takvim kısmı
        public async Task<IActionResult> MyAppointments()
        {
            var userEmail = User.Identity.Name;

            var appointments = await _context.Appointments
                .Include(a => a.user)       
                .Include(a => a.service)    
                .Include(a => a.trainer)   
                    .ThenInclude(t => t.User) 
                .Where(a => a.trainer.User.Email == userEmail)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }
        //onaylama
        //trainerlarımız özenle seçildiği için onaylama işlemlerini kendileri yapıyor öyle de bir kurumuz
        [HttpPost]
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            
            appointment.Status = odev1.Models.AppointmentStatus.Approved;

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyAppointments));
        }

    }
}
