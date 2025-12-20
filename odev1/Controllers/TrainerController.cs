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

        public async Task<IActionResult> MyAppointments()
        {
            var userEmail = User.Identity.Name;

            var appointments = await _context.Appointments
                .Include(a => a.user)       // Randevu içindeki UserDetails (Üye Bilgisi)
                .Include(a => a.service)    // Randevu içindeki Hizmet
                .Include(a => a.trainer)    // Randevu içindeki Eğitmen
                    .ThenInclude(t => t.User) // Eğitmenin UserDetails bilgisi (Email kontrolü için)
                .Where(a => a.trainer.User.Email == userEmail)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

    }
}
